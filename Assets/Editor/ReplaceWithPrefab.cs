using UnityEngine;
using UnityEditor;

public class ReplaceWithPrefab : EditorWindow
{
    [MenuItem("Tools/Replace With Prefab")]
    static void ShowWindow() => GetWindow<ReplaceWithPrefab>();

    public GameObject prefab;

    void OnGUI()
    {
        prefab = (GameObject)EditorGUILayout.ObjectField("Prefab Asset", prefab, typeof(GameObject), false);

        if (GUILayout.Button("Replace Selected Objects") && prefab != null)
        {
            foreach (GameObject go in Selection.gameObjects)
            {
                // Instantiate the prefab while maintaining the link
                GameObject newObj = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                
                // Match the transform of the old duplicate
                newObj.transform.SetParent(go.transform.parent);
                newObj.transform.position = go.transform.position;
                newObj.transform.rotation = go.transform.rotation;
                newObj.transform.localScale = go.transform.localScale;

                // Mark for Undo and delete the old one
                Undo.RegisterCreatedObjectUndo(newObj, "Replace With Prefab");
                Undo.DestroyObjectImmediate(go);
            }
        }
    }
}