using System;
using System.Collections.Generic;
using GameSystem.ObjectPool;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Editor
{
    public class ObjectPoolTreeView : TreeView
    {
        private List<ObjectPoolTreeViewItem> _roots = new List<ObjectPoolTreeViewItem>();
        public ObjectPoolTreeView(TreeViewState treeViewState) : base(treeViewState,CreateHeader())
        {
            Reload();
        }

        public void ReloadTree(List<GameObjectPool> poolInstList)
        {
            if (poolInstList == null || poolInstList.Count <= 0) return;
            _roots.Clear();
            foreach (var one in poolInstList)
            {
                var poolItemView = new ObjectPoolTreeViewItem(one.GetInstanceID(), 0, one.name);
                poolItemView.poolName = one.name;
                poolItemView.prefabName = one.PrefabName;
                poolItemView.remaining = one.CachedGameObjectCount;
                poolItemView.borrowed = one.BorrowedCount;
                poolItemView.createdIndex = one.HasCreatedObject;
                poolItemView.abnormal = one.AbnormalBorrowedCount;
                poolItemView.MaxBorrowedRecord = one.MaxBorrowedRecord;
                _roots.Add(poolItemView);
            }
            
            Reload();
        }

        public void RefreshTree(List<GameObjectPool> poolInstList)
        {
            if (poolInstList == null || poolInstList.Count <= 0)
            {
                _roots.Clear();
                Reload();
                return;
            }
                
            foreach (var item in rootItem.children)
            {
                var pool = poolInstList.Find(x => x.GetInstanceID() == item.id);
                var poolItemView = item as ObjectPoolTreeViewItem;
                poolItemView.poolName = pool.name;
                poolItemView.prefabName = pool.PrefabName;
                poolItemView.remaining = pool.CachedGameObjectCount;
                poolItemView.borrowed = pool.BorrowedCount;
                poolItemView.createdIndex = pool.HasCreatedObject;
                poolItemView.abnormal = pool.AbnormalBorrowedCount;
                poolItemView.MaxBorrowedRecord = pool.MaxBorrowedRecord;
            }
            Repaint();
        }

        private static MultiColumnHeader CreateHeader()
        {
            var columns = new[]
            {
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("プール名")
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("プレハブ名")
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("残り")
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("貸し出し")
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("逸脱")
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("生成Index")
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("最大貸出数")
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
            
            foreach (var item in _roots)
            {
                root.AddChild(item);
            }
            
            return root;
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            ObjectPoolTreeViewItem item = args.item as ObjectPoolTreeViewItem;
            if(item == null) return;
            for (int i = 0; i < args.GetNumVisibleColumns(); i++)
            {
                var rect = args.GetCellRect(i);
                switch (args.GetColumn(i))
                {
                    case 0:
                        EditorGUI.LabelField(rect,item.poolName);        
                        break;
                    case 1:
                        EditorGUI.LabelField(rect,item.prefabName);
                        break;
                    case 2:
                        EditorGUI.LabelField(rect,item.remaining.ToString());
                        break;
                    case 3:
                        EditorGUI.LabelField(rect,item.borrowed.ToString());
                        break;
                    case 4:
                        EditorGUI.LabelField(rect,item.abnormal.ToString());
                        break;
                    case 5:
                        EditorGUI.LabelField(rect,item.createdIndex.ToString());
                        break;
                    case 6:
                        EditorGUI.LabelField(rect,item.MaxBorrowedRecord.ToString());
                        break;
                }
                
            }
        }

        public Action<int> OnPoolSelected;
        protected override void SingleClickedItem(int id)
        {
            OnPoolSelected?.Invoke(id);
        }
    }
}
