using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class AdditiveSceneOpen
{
    [UnityEditor.Callbacks.OnOpenAsset]
    public static bool OnOpenAsset(int instanceID, int line)
    {
        Object obj = EditorUtility.InstanceIDToObject(instanceID);

        // Newer Unity: safer SceneAsset check via AssetDatabase
        string path = AssetDatabase.GetAssetPath(instanceID);
        if (string.IsNullOrEmpty(path))
            return false;

        if (AssetDatabase.GetMainAssetTypeAtPath(path) == typeof(SceneAsset))
        {
            EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
            return true; // cancel default Single open
        }

        return false;
    }
}
