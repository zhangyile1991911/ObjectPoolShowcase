using System;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace GameSystem.ObjectPool
{
    public class ObjectHandler: IDisposable
    {
        private readonly GameObjectPool BelongsTo;
       
        public readonly string name;
        // public GameObject gameObject => Instance;
        // public Transform transform => Instance.transform;
        
        public GameObject Instance
        {
            get
            {
    #if UNITY_EDITOR
                StackTrace st = new StackTrace();
                var frame = st.GetFrame(1);
                var methodBase = frame.GetMethod();
                LastCallStack = methodBase?.DeclaringType?.FullName + "." + methodBase?.Name;
                LastUsedDateTime = DateTime.Now;
    #endif
                return _instance;
            }
        }
        
        
    #if UNITY_EDITOR
        public readonly DateTime CreateDateTime;
        public DateTime LastUsedDateTime { get;private set; }
        public String LastCallStack { get; private set; }
            
        public void PrintDebug()
        {
            Debug.Log($"LastCallStack: {LastCallStack}\n" +
                      $"LastUsedDateTime: {LastUsedDateTime}\n" +
                      $"CreateDateTime: {CreateDateTime}");
        }
    #endif
        
        public bool isReleased { get; private set; }
        private GameObject _instance;
        
        
        public ObjectHandler(GameObjectPool pool,GameObject inst)
        {
            _instance = inst;
            BelongsTo = pool;
            isReleased = false;
            name = inst.name;
    #if UNITY_EDITOR
            CreateDateTime = DateTime.Now;
    #endif
        }
        
        public void Release()
        {
            if (isReleased) return;
            
            _instance.transform.position = Vector3.zero;
            _instance.transform.localScale = Vector3.one;
            _instance.transform.rotation = Quaternion.identity;
            
            BelongsTo.Release(this);
            
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
            BelongsTo.HandlerBeDestroyedIllegal(this);
            //破棄されたら 手遅れでしたので　何もできない
            //警告を出すしかない
            //コードをチェックして書き直せ
            Debug.LogError($"{name} is destroyed. It is worry way!!!\n" +
                           "Please Fix Code!!!\n");
#if UNITY_EDITOR
            PrintDebug();
#endif
        }
    }
}
