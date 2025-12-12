using GameSystem.ObjectPool;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class WatchDogHierarchyIcon
{
    static Texture2D icon;

    static WatchDogHierarchyIcon()
    {
        icon = AssetDatabase.LoadAssetAtPath<Texture2D>(
            "Assets/Resources/happy.png"
        );

        EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
    }

    static void OnHierarchyGUI(int instanceID, Rect selectionRect)
    {
        var obj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
        if (obj == null) return;
        
        if (obj.GetComponent<WatchDogComponent>() == null) return;

        Rect r = new Rect(selectionRect.xMax - 20f,
            selectionRect.y,
            16f,16f);
        GUI.DrawTexture(r, icon);
    }
}
