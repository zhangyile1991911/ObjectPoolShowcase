using System;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace GameSystem.ObjectPool
{
    public class ObjectHandler: IDisposable
    {
        private readonly GameObjectPool _belongsTo;
       
        public readonly string Name;
        // public GameObject gameObject => Instance;
        // public Transform transform => Instance.transform;

        public Transform ParentNode => _instance.transform;
        public GameObject instance
        {
            get
            {
    #if UNITY_EDITOR
                StackTrace st = new StackTrace();
                var frame = st.GetFrame(1);
                var methodBase = frame.GetMethod();
                lastCallStack = methodBase?.DeclaringType?.FullName + "." + methodBase?.Name;
                lastUsedDateTime = DateTime.UtcNow;
    #endif
                return _instance;
            }
        }
        
        
    #if UNITY_EDITOR
        public readonly DateTime CreateDateTime;
        public DateTime lastUsedDateTime { get;private set; }
        public String lastCallStack { get; private set; }
            
        public void PrintDebug()
        {
            Debug.Log($"LastCallStack: {lastCallStack}\n" +
                      $"LastUsedDateTime: {lastUsedDateTime}\n" +
                      $"CreateDateTime: {CreateDateTime}");
        }
    #endif
        
        public bool isReleased { get; private set; }
        private GameObject _instance;
        
        
        public ObjectHandler(GameObjectPool pool,GameObject inst)
        {
            _instance = inst;
            _belongsTo = pool;
            isReleased = false;
            Name = inst.name;
    #if UNITY_EDITOR
            CreateDateTime = DateTime.UtcNow;
    #endif
        }
        
        public void Release()
        {
            if (isReleased) return;
            
            _instance.transform.position = Vector3.zero;
            _instance.transform.localScale = Vector3.one;
            _instance.transform.rotation = Quaternion.identity;
            
            _belongsTo.Release(this);
            
            _instance.SetActive(false);
            _instance = null;
            isReleased = true;
        }
        public void Dispose()
        {
            Release();
        }

        public bool hasParent => _instance.transform.parent != null;
        internal void IllegalDestroy()
        {
            _belongsTo.HandlerBeDestroyedIllegal(this);
            //破棄されたら 手遅れでしたので　何もできない
            //警告を出すしかない
            //コードをチェックして書き直せ
            Debug.LogError($"{Name} is destroyed. It is worry way!!!\n" +
                           "Please Fix Code!!!\n");
#if UNITY_EDITOR
            PrintDebug();
#endif
        }
    }
}
