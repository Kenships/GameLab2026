using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Depth_Based_Pixelator.Editor
{
    public class PixelationCustomGUI : ShaderGUI
    {
        private const int MinDepthLevel = 2;
        private const int MaxDepthLevel = 8;

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            Material material = materialEditor.target as Material;

            // Get properties.
            MaterialProperty depthBased = FindProperty("_Depth_Based", properties);

            MaterialProperty uniformResolution = FindProperty("_Uniform_Resolution", properties);
            MaterialProperty resolutionMultiplier = FindProperty("_Resolution_Multiplier", properties);

            MaterialProperty depthCountProp = FindProperty("_Depth_Level_Count", properties);
            MaterialProperty showDepth = FindProperty("_Show_Depth", properties);

            // Modes.
            /*
            EditorGUILayout.LabelField("Modes", EditorStyles.boldLabel);
            materialEditor.ShaderProperty(depthBased, "Depth Based?");

            EditorGUILayout.Space();

            // Uniform resolution.
            if (depthBased.floatValue == 0)
            {
                EditorGUILayout.LabelField("Resolution", EditorStyles.boldLabel);
                var resolution = (int)uniformResolution.floatValue;
                EditorGUI.BeginChangeCheck();
                resolution = EditorGUILayout.IntSlider("Resolution", resolution, 0, 200);
                if (EditorGUI.EndChangeCheck())
                {
                    uniformResolution.floatValue = resolution;
                }
            }
            */

            EditorGUILayout.Space();

            // Depth-based mode.
            var depthLevelCount = Mathf.Clamp((int)depthCountProp.floatValue, MinDepthLevel, MaxDepthLevel);
            var thresholds = new float[depthLevelCount + 1];
            if (depthBased.floatValue != 0)
            {
                EditorGUILayout.LabelField("Depth & Resolution", EditorStyles.boldLabel);
                materialEditor.ShaderProperty(showDepth, "Show Depth?");

                EditorGUI.BeginChangeCheck();
                depthLevelCount =
                    EditorGUILayout.IntSlider("Depth Level Count", depthLevelCount, MinDepthLevel, MaxDepthLevel);
                if (EditorGUI.EndChangeCheck())
                {
                    depthCountProp.floatValue = depthLevelCount;
                }

                EditorGUILayout.Space(5f);

                // Draw visual depth bar
                EditorGUILayout.BeginVertical("box");
                GUILayout.Space(5f);
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(5f);
                EditorGUILayout.BeginVertical();

                #region Thresholds & Resolution

                float maxBarWidth = EditorGUIUtility.currentViewWidth - 50f;
                Rect barRect = GUILayoutUtility.GetRect(maxBarWidth, 30f);

                // Collect threshold values.
                var displayFarPlaneVal = 1750f;
                var actualFarPlaneVal = Camera.main ? Camera.main.farClipPlane : 2000;
                thresholds[0] = 0;
                for (int i = 1; i < depthLevelCount; i++)
                {
                    if (i >= thresholds.Length) return;
                    var threshold = FindProperty($"_Threshold_{i}", properties);
                    thresholds[i] = threshold.floatValue;
                }

                // Collect resolution values.
                var resolutions = new int[10];
                for (int i = 1; i < MaxDepthLevel + 1; i++)
                {
                    var resolution = FindProperty($"_Resolution_{i}", properties);
                    resolutions[i - 1] = (int)resolution.floatValue;
                }

                if (depthLevelCount >= thresholds.Length) return;
                thresholds[depthLevelCount] = displayFarPlaneVal;

                // Draw bar.
                GUIStyle eyeDepthLabelStyle = new GUIStyle(EditorStyles.miniLabel);
                eyeDepthLabelStyle.fontStyle = FontStyle.Italic;

                EditorGUILayout.Space(5f);
                string eyeDepthLabel = "Eye Depth (Distance to camera/near_plane)";
                Vector2 _labelSize = eyeDepthLabelStyle.CalcSize(new GUIContent(eyeDepthLabel));
                Rect eyeDepthLabelRect = new Rect(
                    barRect.x,
                    barRect.yMax + 17f,
                    _labelSize.x,
                    _labelSize.y
                );
                EditorGUI.LabelField(eyeDepthLabelRect, eyeDepthLabel, eyeDepthLabelStyle);

                List<Rect> segRects = new List<Rect>();
                var baseRes = uniformResolution.floatValue;
                for (int i = 0; i < depthLevelCount; i++)
                {
                    // Bar setup.
                    var t0 = thresholds[i];
                    var t1 = thresholds[i + 1];
                    var segmentWidth = ((t1 - t0) / thresholds[depthLevelCount]) * maxBarWidth;
                    Rect segRect = new Rect(barRect.x, barRect.y,
                        i == depthLevelCount - 1 ? segmentWidth : segmentWidth + 2f, barRect.height);
                    segRects.Add(segRect);
                    ColorUtility.TryParseHtmlString("#3A7C7C", out var farColor);
                    ColorUtility.TryParseHtmlString("#253B6E", out var nearColor);
                    EditorGUI.DrawRect(segRect, Color.Lerp(farColor, nearColor, i / (float)(depthLevelCount - 1)));

                    // Resolution.
                    var labelStyle = new GUIStyle(EditorStyles.miniBoldLabel) { alignment = TextAnchor.MiddleCenter };
                    EditorGUI.LabelField(segRect,
                        i == depthLevelCount - 1 ? $"{resolutions[i]} (Resolution)" : resolutions[i].ToString(),
                        labelStyle);

                    barRect.x += segmentWidth;
                }

                List<string> overlappingLabels = new List<string>();
                var previousX = float.MinValue;
                var previousLabelSize = 0f;
                var overlapThresholdOffset = 5f;
                var sameValCount = 0;
                for (int i = 0; i < thresholds.Length - 1; i++)
                {
                    var thresholdLabel = $"{thresholds[i]}";
                    Vector2 labelSize = EditorStyles.miniLabel.CalcSize(new GUIContent(thresholdLabel));

                    if (i >= segRects.Count) continue;
                    var labelX = segRects[i].x - labelSize.x / 2f;

                    string combined;
                    Vector2 combinedSize;
                    float combinedX;

                    if (Mathf.Abs(labelX - previousX) > (previousLabelSize + overlapThresholdOffset))
                    {
                        if (overlappingLabels.Count > 0)
                        {
                            combined = string.Join("/", overlappingLabels);
                            combinedSize = EditorStyles.miniLabel.CalcSize(new GUIContent(combined));
                            combinedX = overlappingLabels.Count == 1
                                ? segRects[i - 1].x - combinedSize.x / 2f
                                : (segRects[i - 1].x + segRects[i - overlappingLabels.Count - sameValCount].x) / 2f -
                                  combinedSize.x / 2f;
                            combinedX = Math.Max(combinedX, segRects[0].x);

                            GUI.Label(new Rect(combinedX, barRect.yMax + 2, combinedSize.x, 20), combined,
                                EditorStyles.miniLabel);
                            sameValCount = 0;
                            overlappingLabels.Clear();
                        }
                    }

                    if (overlappingLabels.Count == 0)
                    {
                        overlappingLabels.Add(thresholdLabel);
                    }
                    else if (overlappingLabels[^1] == thresholdLabel)
                    {
                        sameValCount++;
                    }
                    else
                    {
                        overlappingLabels.Add(thresholdLabel);
                    }

                    combined = string.Join("/", overlappingLabels);
                    combinedSize = EditorStyles.miniLabel.CalcSize(new GUIContent(combined));
                    combinedX = overlappingLabels.Count == 1
                        ? segRects[i].x - combinedSize.x / 2f
                        : (segRects[i].x + segRects[i - overlappingLabels.Count - sameValCount + 1].x) / 2f -
                          combinedSize.x / 2f;
                    ;
                    combinedX = Math.Max(combinedX, segRects[0].x);
                    previousX = combinedX;
                    previousLabelSize = combinedSize.x;
                }

                if (overlappingLabels.Count > 0)
                {
                    var combined = string.Join("/", overlappingLabels);
                    Vector2 combinedSize = EditorStyles.miniLabel.CalcSize(new GUIContent(combined));

                    if (thresholds.Length - overlappingLabels.Count - 1 - sameValCount >= segRects.Count ||
                        thresholds.Length - 2 >= segRects.Count) return;
                    var combinedX = overlappingLabels.Count == 1
                        ? previousX
                        : (segRects[thresholds.Length - 2].x +
                           segRects[thresholds.Length - overlappingLabels.Count - 1 - sameValCount].x) / 2f -
                          combinedSize.x / 2f;

                    GUI.Label(new Rect(combinedX, barRect.yMax + 2, combinedSize.x, 20), combined,
                        EditorStyles.miniLabel);
                    overlappingLabels.Clear();
                }

                // Final label (far plane value).
                var farPlaneLabel = $"{Mathf.Max(actualFarPlaneVal, 2000)}";
                _labelSize = EditorStyles.miniLabel.CalcSize(new GUIContent(farPlaneLabel));
                Rect _valueLabelRect = new Rect(
                    EditorGUIUtility.currentViewWidth - _labelSize.x - 7f,
                    barRect.y + 35f,
                    _labelSize.x,
                    _labelSize.y
                );
                EditorGUI.LabelField(_valueLabelRect, farPlaneLabel, EditorStyles.miniLabel);

                EditorGUILayout.Space(35f);

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Thresholds", EditorStyles.boldLabel, GUILayout.Width(100f));
                GUILayout.FlexibleSpace();
                GUILayout.Label("Resolutions", EditorStyles.boldLabel);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space(10f);

                // Threshold Settings.
                for (int i = 1; i < depthLevelCount; i++)
                {
                    var distanceProp = FindProperty($"_Threshold_{i}", properties);
                    var threshold = Mathf.Clamp((int)distanceProp.floatValue, 0, 1250);

                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label($"Depth {i} Threshold", GUILayout.Width(140));
                    threshold = EditorGUILayout.IntSlider(threshold, 0, 1250,
                        GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth - 290));
                    if (EditorGUI.EndChangeCheck())
                    {
                        if (threshold >= thresholds[i - 1] && threshold <= thresholds[i + 1])
                        {
                            distanceProp.floatValue = threshold;
                        }
                        else if (threshold < thresholds[i - 1])
                        {
                            distanceProp.floatValue = thresholds[i - 1];
                        }
                        else if (threshold > thresholds[i])
                        {
                            distanceProp.floatValue = thresholds[i + 1];
                        }
                    }

                    Rect resRect = new Rect(EditorGUIUtility.currentViewWidth - 75,
                        GUILayoutUtility.GetLastRect().y - 9, 50, 18);
                    var resolutionProp = FindProperty($"_Resolution_{i}", properties);
                    EditorGUI.BeginChangeCheck();
                    var newResolution = EditorGUI.FloatField(resRect, resolutionProp.floatValue);
                    if (EditorGUI.EndChangeCheck())
                    {
                        resolutionProp.floatValue = newResolution;
                    }

                    EditorGUILayout.EndHorizontal();
                }

                // Last res.
                Rect _resRect = new Rect(EditorGUIUtility.currentViewWidth - 50 - 25,
                    GUILayoutUtility.GetLastRect().y + 9, 50, 18);
                var _resolutionProp = FindProperty($"_Resolution_{depthLevelCount}", properties);
                EditorGUI.BeginChangeCheck();
                var _newResolution = EditorGUI.FloatField(_resRect, _resolutionProp.floatValue);
                if (EditorGUI.EndChangeCheck())
                {
                    _resolutionProp.floatValue = _newResolution;
                }

                #endregion
                
                EditorGUILayout.Space(12f);
                
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.BeginHorizontal();
                var resMultiplier = resolutionMultiplier.floatValue;
                GUILayout.Label("Resolution Multiplier", GUILayout.Width(140));
                resMultiplier = EditorGUILayout.Slider(resolutionMultiplier.floatValue, 0.1f, 3.0f,
                    GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth - 214));
                EditorGUILayout.EndHorizontal();

                if (EditorGUI.EndChangeCheck())
                {
                    resolutionMultiplier.floatValue = resMultiplier;
                }

                GUILayout.Space(10f);
                EditorGUILayout.EndVertical();
                GUILayout.Space(5f);
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(5f);
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
            }

            EditorGUILayout.Space(10);

            if (depthBased.floatValue == 0)
            {
                showDepth.floatValue = 0f;
            }

            // Reset Button.
            if (GUILayout.Button("Reset to Default"))
            {
                depthBased.floatValue = 1f;
                showDepth.floatValue = 0f;

                uniformResolution.floatValue = 40f;
                depthCountProp.floatValue = 7;
                thresholds = new[] { 2f, 8f, 15f, 25f, 45f, 80f, 1250f};
                var resolution = new[] { 60f, 80f, 100f, 120f, 160f, 200f, 240f, 1200f };
                for (int i = 0; i < depthLevelCount; i++)
                {
                    var resolutionProp = FindProperty($"_Resolution_{i + 1}", properties);
                    resolutionProp.floatValue = resolution[i];

                    if (i >= 9) break;
                    var threshold = FindProperty($"_Threshold_{i + 1}", properties);
                    threshold.floatValue = thresholds[i];
                }
                
                resolutionMultiplier.floatValue = 1f;

                EditorUtility.SetDirty(material);
            }
        }
    }
}