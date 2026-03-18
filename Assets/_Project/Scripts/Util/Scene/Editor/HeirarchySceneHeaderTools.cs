using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Project.Scripts.Util.Scene.Editor
{
    [InitializeOnLoad]
    public static class HierarchySceneHeaderTools
    {
        static HierarchySceneHeaderTools()
        {
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
        }

        private static void OnHierarchyGUI(int instanceID, Rect selectionRect)
        {
            // Scene header row detection (same trick that worked for you)
            if (EditorUtility.InstanceIDToObject(instanceID) != null)
                return;

            UnityEngine.SceneManagement.Scene scene = default;
            bool found = false;

            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                var s = SceneManager.GetSceneAt(i);
                if (s.IsValid() && s.isLoaded && s.handle == instanceID)
                {
                    scene = s;
                    found = true;
                    break;
                }
            }

            if (!found)
                return;

            // Right-click anywhere on that header row
            if (Event.current.type == EventType.ContextClick &&
                selectionRect.Contains(Event.current.mousePosition))
            {
                ShowSceneHeaderMenu(scene);
                Event.current.Use();
            }

            // (Keep your red dot drawing + click behavior here if you already have it)
        }

        private static void ShowSceneHeaderMenu(UnityEngine.SceneManagement.Scene scene)
        {
            var menu = new GenericMenu();

            bool inBuild = IsInBuildSettings(scene.path);

            if (!inBuild)
                menu.AddItem(new GUIContent("Build Settings/Add Scene"), false, () => AddToBuildSettings(scene.path));
            else
                menu.AddItem(new GUIContent("Build Settings/Remove Scene"), false, () => RemoveFromBuildSettings(scene.path));

            menu.AddSeparator("Build Settings/");

            menu.AddItem(new GUIContent("Build Settings/Enable In Build"), false, () => SetBuildEnabled(scene.path, true));
            menu.AddItem(new GUIContent("Build Settings/Disable In Build"), false, () => SetBuildEnabled(scene.path, false));

            menu.ShowAsContext();
        }

        private static bool IsInBuildSettings(string scenePath)
        {
            if (string.IsNullOrEmpty(scenePath)) return false;

            foreach (var s in EditorBuildSettings.scenes)
            {
                if (string.Equals(s.path, scenePath, System.StringComparison.Ordinal))
                    return true;
            }
            return false;
        }

        private static void AddToBuildSettings(string scenePath)
        {
            if (string.IsNullOrEmpty(scenePath))
            {
                EditorUtility.DisplayDialog("Can't add to Build Settings",
                    "This scene has no path yet. Save the scene first.", "OK");
                return;
            }

            var scenes = EditorBuildSettings.scenes;
            // Don’t duplicate
            foreach (var s in scenes)
                if (s.path == scenePath) return;

            var newScenes = new EditorBuildSettingsScene[scenes.Length + 1];
            for (int i = 0; i < scenes.Length; i++) newScenes[i] = scenes[i];
            newScenes[^1] = new EditorBuildSettingsScene(scenePath, true);

            EditorBuildSettings.scenes = newScenes;
        }

        private static void RemoveFromBuildSettings(string scenePath)
        {
            var scenes = EditorBuildSettings.scenes;
            var list = new System.Collections.Generic.List<EditorBuildSettingsScene>(scenes.Length);
            foreach (var s in scenes)
                if (s.path != scenePath) list.Add(s);

            EditorBuildSettings.scenes = list.ToArray();
        }

        private static void SetBuildEnabled(string scenePath, bool enabled)
        {
            var scenes = EditorBuildSettings.scenes;
            for (int i = 0; i < scenes.Length; i++)
            {
                if (scenes[i].path == scenePath)
                {
                    scenes[i].enabled = enabled;
                    EditorBuildSettings.scenes = scenes;
                    return;
                }
            }

            // If not present, add it enabled/disabled
            var add = new EditorBuildSettingsScene(scenePath, enabled);
            var newScenes = new EditorBuildSettingsScene[scenes.Length + 1];
            for (int i = 0; i < scenes.Length; i++) newScenes[i] = scenes[i];
            newScenes[^1] = add;
            EditorBuildSettings.scenes = newScenes;
        }
    }
}
