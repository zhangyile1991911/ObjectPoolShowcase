using System;
using UnityEditor.IMGUI.Controls;

namespace Editor
{
    public class HandlerTreeViewItem : TreeViewItem
    {
        public long CreateDate;
        public long lastUsedDate;
        public string lastCallStack;
        public bool isReleased;
        public string parentName;
        public HandlerTreeViewItem(int id,int depth,string displayName) : base(id,depth,displayName)
        {
            
        }
    }
}
