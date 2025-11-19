using System.Collections.Generic;
using GameSystem.ObjectPool;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Editor
{
    public class ObjectPoolDebugger : EditorWindow
    {
        [SerializeField] TreeViewState _PoolTreeViewState;
        [SerializeField] TreeViewState _HandlerTreeViewState;
    
        ObjectPoolTreeView _objectPoolTreeView;
        HandlerTreeView _handlerTreeView;
        private bool isPlaying = false;
        private float split = 0.5f; 
    
        [MenuItem("Window/Object Pool Debugger")]
        public static void ShowWindow()
        {
            var window = GetWindow(typeof(ObjectPoolDebugger));
            window.titleContent = new GUIContent("Object Pool Debugger");
            window.Show();
        }

        private void OnEnable()
        {
            _PoolTreeViewState ??= new TreeViewState();
            _objectPoolTreeView = new ObjectPoolTreeView(_PoolTreeViewState);
            
            _HandlerTreeViewState ??= new TreeViewState();
            _handlerTreeView = new HandlerTreeView(_HandlerTreeViewState);

            //プレイー中で
            isPlaying = EditorApplication.isPlaying;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }
        
        void OnGUI()
        {
            GUILayout.Space(5);
            if (GUILayout.Button("Refresh", GUILayout.Height(30)))
            {
                UpdateTreeView();
            }
            
            Rect total = new Rect(0, 30, position.width, position.height);

            float topHeight = total.height * split;
            float bottomHeight = total.height - topHeight;

            // 上部区域
            Rect topRect = new Rect(total.x, total.y, total.width, topHeight);
            _objectPoolTreeView.OnGUI(topRect);

            // 下部区域
            Rect bottomRect = new Rect(total.x, total.y + topHeight, total.width, bottomHeight);
            _handlerTreeView.OnGUI(bottomRect);

            _objectPoolTreeView.multiColumnHeader.ResizeToFit();
            _handlerTreeView.multiColumnHeader.ResizeToFit();
            
            // 拖动 splitter
            DrawSplitter(total);
        }
        
        bool dragging = false;
        void DrawSplitter(Rect total)
        {
            Rect bar = new Rect(0, total.height * split - 3, total.width, 6);

            EditorGUIUtility.AddCursorRect(bar, MouseCursor.ResizeVertical);

            if (Event.current.type == EventType.MouseDown && bar.Contains(Event.current.mousePosition))
                dragging = true;

            if (dragging)
            {
                split = Mathf.Clamp(Event.current.mousePosition.y / position.height, 0.1f, 0.9f);
                Repaint();

                if (Event.current.type == EventType.MouseUp)
                    dragging = false;
            }
        }

        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                poolInstList.Clear();
                _objectPoolTreeView.RefreshTree(poolInstList);
                _objectPoolTreeView.OnPoolSelected = null;
                _handlerTreeView.RefreshPoolDetail(null);
                
            }

            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                isPlaying = EditorApplication.isPlaying;
                //オブジェクトプールノードを検索
                FindObjectPoolInHierarchy();
                _objectPoolTreeView.ReloadTree(poolInstList);
                _objectPoolTreeView.OnPoolSelected = onPoolSelected;
            }
            
        }

        private List<GameObjectPool> poolInstList = new();
        private void FindObjectPoolInHierarchy()
        {
            poolInstList.Clear();
            GameObjectPool[] allObjects = GameObject.FindObjectsByType<GameObjectPool>(FindObjectsSortMode.InstanceID);
            foreach (var t in allObjects)
            {
                var pool = t.GetComponent<GameObjectPool>();
                if (pool != null)
                {
                    poolInstList.Add(pool);
                }
            }
        }
        

        private void UpdateTreeView()
        {
            if (poolInstList.Count == 0) return;
            if (!isPlaying) return;
            _objectPoolTreeView.RefreshTree(poolInstList);
        }

        private void onPoolSelected(int instanceID)
        {
            var poolItem = poolInstList.Find(x => x.GetInstanceID() == instanceID);
            _handlerTreeView.RefreshPoolDetail(poolItem);
        }
        
    }
}