using UnityEditor.IMGUI.Controls;
using UnityEngine.UIElements;

namespace Editor
{
    public class ObjectPoolTreeViewItem : TreeViewItem
    {
        public string poolName;
        public string prefabName;
        public int remaining;
        public int borrowed;
        public int abnormal;//逸脱
        public int createdIndex;
        public int MaxBorrowedRecord;
        public ObjectPoolTreeViewItem(int id,int depth,string displayName) : base(id,depth,displayName)
        {
            
        }
    }
}
