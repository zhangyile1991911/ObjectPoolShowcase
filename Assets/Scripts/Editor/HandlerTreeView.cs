using System;
using System.Collections.Generic;
using GameSystem.ObjectPool;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Editor
{
    public class HandlerTreeView : TreeView
    {
        List<HandlerTreeViewItem> _items = new List<HandlerTreeViewItem>();
        public HandlerTreeView(TreeViewState treeViewState) : base(treeViewState,CreateHeader())
        {
            Reload();
        }

        private static MultiColumnHeader CreateHeader()
        {
            var columns = new[] {
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("ノード名")
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("生成時間")
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("利用時間")
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("呼び関数")
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("返却済み")
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("親ノード")
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("逸脱した")
                },
            
            };
        
            var state = new MultiColumnHeaderState(columns);
            return new MultiColumnHeader(state);
        }
    
        protected override TreeViewItem BuildRoot()
        {
            var root = new TreeViewItem {id = 0, depth = -1, displayName = "Root"};
            if (root.children == null)
            {
                root.children = new List<TreeViewItem>();
            }

            foreach (var item in _items)
            {
                root.AddChild(item);    
            }
            
            
            return root;
        }

        public void RefreshPoolDetail(GameObjectPool pool)
        {
            _items.Clear();
            if (pool == null)
            {
                Reload();
                return;
            }
            
            var list = pool.HandlerList;
            if (list == null || list.Count < 0)
            {
                Reload();
                return;
            }

            foreach (var item in list)
            {
                var handlerItem = new HandlerTreeViewItem(
                    item.InstanceID,
                    1,
                    item.Name)
                {
                    CreateDate = item.CreateDateTime,
                    lastUsedDate = item.lastUsedDateTime,
                    lastCallStack = item.lastCallStack,
                    isReleased = item.isReleased,
                    parentName = item.hasParent ? item.ParentNode.name : ""
                };
                _items.Add(handlerItem);
            }
            Reload();
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            HandlerTreeViewItem item = args.item as HandlerTreeViewItem;
            if(item == null) return;
            
            for (int i = 0; i < args.GetNumVisibleColumns(); i++)
            {
                var rect = args.GetCellRect(i);
                switch (args.GetColumn(i))
                {
                    case 0:
                        EditorGUI.LabelField(rect,item.displayName);
                        break;
                    case 1:
                        DateTime created = DateTimeOffset.FromUnixTimeSeconds(item.CreateDate).ToLocalTime().DateTime;
                        EditorGUI.LabelField(rect,created.ToString("hh:mm:ss"));        
                        break;
                    case 2:
                        DateTime lastUsed = DateTimeOffset.FromUnixTimeSeconds(item.lastUsedDate).ToLocalTime().DateTime;
                        EditorGUI.LabelField(rect,lastUsed.ToString("hh:mm:ss"));        
                        break;
                    case 3:
                        EditorGUI.LabelField(rect,item.lastCallStack);        
                        break;
                    case 4:
                        EditorGUI.LabelField(rect,item.isReleased.ToString());        
                        break;
                    case 5:
                        EditorGUI.LabelField(rect,item.parentName);        
                        break;
                    case 6:
                        EditorGUI.LabelField(rect,item.parentName == "" ? "True" : "False");        
                        break;
                }
            }
        }
        
        
    }
}
