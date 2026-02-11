using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class HierarchySceneHeaderCloseButton
{
    static HierarchySceneHeaderCloseButton()
    {
        EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
    }

    private static void OnHierarchyGUI(int instanceID, Rect selectionRect)
    {
        // Scene header rows often come through with an instanceID that is NOT a UnityEngine.Object,
        // so InstanceIDToObject returns null. We'll detect scene headers via Scene.handle instead.
        if (EditorUtility.InstanceIDToObject(instanceID) != null)
            return;

        // Match the instanceID to a loaded scene handle
        Scene scene = default;
        bool found = false;

        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            var s = SceneManager.GetSceneAt(i);
            if (!s.IsValid() || !s.isLoaded) continue;

            if (s.handle == instanceID)
            {
                scene = s;
                found = true;
                break;
            }
        }

        if (!found)
            return;

        // Unity won't close the last loaded scene
        if (SceneManager.sceneCount <= 1)
            return;

        DrawCloseButton(scene, selectionRect);
    }

    private static void DrawCloseButton(Scene scene, Rect rowRect)
    {
        const float size = 10f;

        Rect rect = new Rect(
            rowRect.xMax - size - 6f,
            rowRect.y + (rowRect.height - size) * 0.5f,
            size,
            size
        );

        // Optional: only show on hover
        if (!rowRect.Contains(Event.current.mousePosition))
            return;

        // Draw filled circle
        Handles.BeginGUI();
        Handles.color = new Color(0.85f, 0.2f, 0.2f, 0.95f);
        Handles.DrawSolidDisc(
            rect.center,
            Vector3.forward,
            size * 0.5f
        );
        Handles.EndGUI();

        // Click detection
        // Quick-save-close anywhere on the scene header with Alt+Click
        if (Event.current.type == EventType.MouseDown &&
            Event.current.button == 0 &&
            Event.current.alt &&
            rowRect.Contains(Event.current.mousePosition))
        {
            Event.current.Use();

            if (!QuickSaveThenClose(scene))
                return;

            GUIUtility.ExitGUI();
        }

// Normal close only when clicking the red dot
        if (Event.current.type == EventType.MouseDown &&
            Event.current.button == 0 &&
            rect.Contains(Event.current.mousePosition) &&
            !Event.current.alt)
        {
            Event.current.Use();

            // Prompt-based close (safe)
            if (!EditorSceneManager.SaveModifiedScenesIfUserWantsTo(new[] { scene }))
                return;

            EditorSceneManager.CloseScene(scene, true);
            GUIUtility.ExitGUI();
        }

    }

    private static bool QuickSaveThenClose(Scene scene)
    {
        // If the scene has never been saved, Unity needs a path -> you can’t avoid a dialog.
        if (string.IsNullOrEmpty(scene.path))
        {
            // Fall back to the normal prompt workflow
            if (!EditorSceneManager.SaveModifiedScenesIfUserWantsTo(new[] { scene }))
                return false;

            return EditorSceneManager.CloseScene(scene, true);
        }

        // Auto-save (no prompt) if there are changes
        if (scene.isDirty)
        {
            if (!EditorSceneManager.SaveScene(scene))
                return false; // save failed/cancelled
        }

        return EditorSceneManager.CloseScene(scene, true);
    }


}
