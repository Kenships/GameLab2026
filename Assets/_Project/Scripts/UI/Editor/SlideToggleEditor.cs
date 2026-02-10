using _Project.Scripts.UI.UIElements;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.UI;
using UnityEngine;

namespace _Project.Scripts.UI.Editor
{
    [CustomEditor(typeof(SlideToggle), true)]
    [CanEditMultipleObjects]
    public class SlideToggleEditor : SelectableEditor
    {
        private SerializedProperty _isOnProperty;
        private SerializedProperty _isMirroredProperty;
        private SerializedProperty _toggleBallProperty;
        private SerializedProperty _backgroundProperty;
        private SerializedProperty _onColorProperty;
        private SerializedProperty _offColorProperty;
        private SerializedProperty _onValueChangedProperty;
        private SerializedProperty _animationTimeProperty;
        
        
        
        
        protected override void OnEnable()
        {
            base.OnEnable();
            _isOnProperty = serializedObject.FindProperty("isOn");
            _isMirroredProperty = serializedObject.FindProperty("isMirrored");
            _toggleBallProperty = serializedObject.FindProperty("toggleBall");
            _backgroundProperty = serializedObject.FindProperty("background");
            _onColorProperty = serializedObject.FindProperty("onColor");
            _offColorProperty = serializedObject.FindProperty("offColor");
            _animationTimeProperty = serializedObject.FindProperty("animationTime");
            _onValueChangedProperty = serializedObject.FindProperty("onValueChanged");
            
            
        }
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();

            serializedObject.Update();
            SlideToggle toggle = serializedObject.targetObject as SlideToggle;
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_isOnProperty);
            if (EditorGUI.EndChangeCheck())
            {
                if (!Application.isPlaying)
                    EditorSceneManager.MarkSceneDirty(toggle.gameObject.scene);
                toggle.IsOn = _isOnProperty.boolValue;
            }
            EditorGUILayout.PropertyField(_isMirroredProperty);
            EditorGUILayout.PropertyField(_toggleBallProperty);
            EditorGUILayout.PropertyField(_backgroundProperty);
            
            EditorGUILayout.PropertyField(_onColorProperty);
            EditorGUILayout.PropertyField(_offColorProperty);
            EditorGUILayout.PropertyField(_animationTimeProperty);
            
            EditorGUILayout.Space();

            // Draw the event notification options
            EditorGUILayout.PropertyField(_onValueChangedProperty);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
