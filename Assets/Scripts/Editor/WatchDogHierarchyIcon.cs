using GameSystem.ObjectPool;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [InitializeOnLoad]
    public static class WatchDogHierarchyIcon
    {
        private static readonly Texture2D Icon;

        static WatchDogHierarchyIcon()
        {
            Icon = AssetDatabase.LoadAssetAtPath<Texture2D>(
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
            GUI.DrawTexture(r, Icon);
        }
    }
}
