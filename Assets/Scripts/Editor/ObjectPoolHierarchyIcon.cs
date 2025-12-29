using GameSystem.ObjectPool;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [InitializeOnLoad]
    public static class ObjectPoolHierarchyIcon
    {
        private static Texture2D Icon;

        static ObjectPoolHierarchyIcon()
        {
            reloadIcon();

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
            if(Icon == null)
            {
                reloadIcon();
            }
            GUI.DrawTexture(r, Icon);
        }
        
        static void reloadIcon()
        {
            Icon = AssetDatabase.LoadAssetAtPath<Texture2D>(
                "Assets/Resources/logistics.png"
            );
        }
    }
}
