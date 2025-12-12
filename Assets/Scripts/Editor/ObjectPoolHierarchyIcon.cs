using GameSystem.ObjectPool;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class ObjectPoolHierarchyIcon
{
    static Texture2D icon;

    static ObjectPoolHierarchyIcon()
    {
        icon = AssetDatabase.LoadAssetAtPath<Texture2D>(
            "Assets/Resources/logistics.png"
        );

        EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
    }

    static void OnHierarchyGUI(int instanceID, Rect selectionRect)
    {
        var obj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
        if (obj == null) return;
        
        if (obj.GetComponent<GameObjectPool>() == null) return;

        Rect r = new Rect(selectionRect.xMax - 80f,
            selectionRect.y,
            16f,16f);
        GUI.DrawTexture(r, icon);
    }
}
