using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Project.Scripts.Util.Scene.Editor
{
    [CustomPropertyDrawer(typeof(SceneReference))]
    public class SceneReferenceDrawer : PropertyDrawer
    {
        private string[] _sceneNames;
        private int[] _sceneBuildIndices;

        private void RebuildCache()
        {
            var scenes = EditorBuildSettings.scenes;

            // Only enabled scenes
            var enabled = new System.Collections.Generic.List<EditorBuildSettingsScene>();
            foreach (var s in scenes)
                if (s.enabled) enabled.Add(s);

            _sceneNames = new string[enabled.Count];
            _sceneBuildIndices = new int[enabled.Count];

            for (int i = 0; i < enabled.Count; i++)
            {
                string path = enabled[i].path;
                _sceneNames[i] = Path.GetFileNameWithoutExtension(path);
                _sceneBuildIndices[i] = SceneUtility.GetBuildIndexByScenePath(path);
            }

            _sceneNames ??= System.Array.Empty<string>();
            _sceneBuildIndices ??= System.Array.Empty<int>();
        }

        private void EnsureCache()
        {
            if (_sceneNames != null && _sceneBuildIndices != null) return;
            RebuildCache();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            => EditorGUIUtility.singleLineHeight;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EnsureCache();
            
            var buildIndexProp = property.FindPropertyRelative("buildIndex");

            // --- Right click context menu on the property row ---
            HandleContextMenu(position, property);

            EditorGUI.BeginProperty(position, label, property);

            if (buildIndexProp == null)
            {
                EditorGUI.LabelField(position, label.text, "Missing field 'buildIndex' in SceneReference");
                EditorGUI.EndProperty();
                return;
            }

            if (_sceneNames.Length == 0 || _sceneBuildIndices.Length == 0)
            {
                EditorGUI.LabelField(position, label.text, "No enabled scenes in Build Settings");
                EditorGUI.EndProperty();
                return;
            }

            int current = System.Array.IndexOf(_sceneBuildIndices, buildIndexProp.intValue);
            if (current < 0)
            {
                // Scene removed/reordered: snap to first valid entry
                current = 0;
                buildIndexProp.intValue = _sceneBuildIndices[0];
            }

            int selected = EditorGUI.Popup(position, label.text, current, _sceneNames);
            buildIndexProp.intValue = _sceneBuildIndices[selected];

            EditorGUI.EndProperty();
        }

        private void HandleContextMenu(Rect position, SerializedProperty property)
        {
            Event e = Event.current;
            if (e.type != EventType.ContextClick) return;
            if (!position.Contains(e.mousePosition)) return;

            var menu = new GenericMenu();

            menu.AddItem(new GUIContent("Refresh Scene List"), false, () =>
                                                                      {
                                                                          RebuildCache();
                                                                          // Force inspectors to repaint so the new list shows immediately.
                                                                          UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
                                                                      });

            // Optional: quick utility item
            menu.AddItem(new GUIContent("Open Build Settings"), false, () =>
                                                                       {
                                                                           EditorWindow.GetWindow(System.Type.GetType("UnityEditor.BuildPlayerWindow,UnityEditor"));
                                                                       });

            menu.ShowAsContext();
            e.Use();
        }
    }
}
