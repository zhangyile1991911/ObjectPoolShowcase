using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace GameSystem.ObjectPool
{
    public class ObjectHandler: IDisposable
    {
        private readonly GameObjectPool _belongsTo;
       
        public readonly string Name;
        public Transform ParentNode => hasParent ?  _instance.transform.parent : null;
        
        public int InstanceID => _instance ? _instance.GetInstanceID() : 0;
        
        public GameObject instance
        {
            get
            {
                RecordGetInstance();
                return _instance;
            }
        }

        [Conditional("UNITY_EDITOR")]
        private void RecordGetInstance(
            [CallerFilePath] string file = "",
            [CallerMemberName] string methodName = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            lastCallStack = file + "." + methodName+":"+lineNumber;
            lastUsedDateTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }
        
        
        
    #if UNITY_EDITOR
        public readonly long CreateDateTime;
        public long lastUsedDateTime { get;private set; }
        public string lastCallStack { get; private set; }
    #endif
        [Conditional("UNITY_EDITOR")]
        public void PrintDebug()
        {
            Debug.Log($"LastCallStack: {lastCallStack}\n" +
                      $"LastUsedDateTime: {DateTimeOffset.FromUnixTimeSeconds(lastUsedDateTime).ToLocalTime().DateTime}\n" +
                      $"CreateDateTime: {DateTimeOffset.FromUnixTimeSeconds(CreateDateTime).ToLocalTime().DateTime}");
        }

        
        public bool isReleased { get; private set; }
        private GameObject _instance;
        
        
        public ObjectHandler(GameObjectPool pool,GameObject inst)
        {
            _instance = inst;
            _belongsTo = pool;
            isReleased = false;
            Name = inst.name;
    #if UNITY_EDITOR
            CreateDateTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    #endif
        }
        
        public void Release()
        {
            if (isReleased) return;
            
            ResetTransform();
            
            _belongsTo.Release(this);

            MarkAsReleased();
        }

        void ResetTransform()
        {
            _instance.transform.position = Vector3.zero;
            _instance.transform.localScale = Vector3.one;
            _instance.transform.rotation = Quaternion.identity;
            _instance.SetActive(false);
        }
        
        void MarkAsReleased()
        {
            _instance = null;
            isReleased = true;
        }
        
        public void Dispose()
        {
            Release();
        }

        public bool hasParent => _instance!=null && _instance.transform.parent != null;
        internal void IllegalDestroy()
        {
            _belongsTo.HandlerBeDestroyedIllegal(this);
            //破棄されたら 手遅れでしたので　何もできない
            //警告を出すしかない
            //コードをチェックして書き直せ
            Debug.LogError($"{Name} is destroyed. It is worry way!!!\n" +
                           "Please Fix Code!!!\n");

            PrintDebug();
        }
    }
}
