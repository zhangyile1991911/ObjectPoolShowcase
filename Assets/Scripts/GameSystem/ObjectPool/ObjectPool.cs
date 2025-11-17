using System;
using System.Collections.Generic;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace GameSystem.ObjectPool
{
    public class GameObjectPool : MonoBehaviour 
    {
        [SerializeField]
        private GameObject Prefab;
        [SerializeField]
        private  int _capacity = 0;
        [SerializeField]
        private bool needWarmup = false;
        
        private string _poolName;
        
        private Queue<GameObject> _pool = null;
        private HashSet<ObjectHandler> _objectHandlers = new ();

        void Awake()
        { 
            _poolName = name;
            _pool = new Queue<GameObject>(_capacity);
            if (needWarmup)
            {
                Warmup();
            }
            DontDestroyOnLoad(gameObject);
        }
        
        //一気に全部オブジェクトを生成するより、半分生成しておくのが早いです
        private void Warmup()
        {
            MakeMore(_capacity/2);
        }

        private void MakeMore(int num)
        {
            for (int i = 0; i < num; i++)
            {
                GameObject inst = GameObject.Instantiate(Prefab, transform, true);
                inst.AddComponent<WatchDogComponent>();
                inst.name = $"{_poolName}_{_pool.Count}";
                inst.SetActive(false);
                _pool.Enqueue(inst);
            }
        }

        public ObjectHandler Get()
        {
            if (_pool.Count <= 0)
            {
                MakeMore(_capacity/2);
            }

            GameObject inst = _pool.Dequeue();
            inst.transform.SetParent(null);
            //確認
            WatchDogComponent watchDog = inst.GetComponent<WatchDogComponent>();
            if (watchDog == null)
            {
                watchDog = inst.AddComponent<WatchDogComponent>();    
            }
            
            //ラップ
            ObjectHandler handler = new ObjectHandler(this,inst);
            watchDog.ObjectHandler = handler;
            
            //記録
            _objectHandlers.Add(handler);
            return handler;
        }

        public void Release(ObjectHandler handler)
        {
            if (handler == null)
            {
                return;
            }

            WatchDogComponent watchDog = handler.Instance.GetComponent<WatchDogComponent>();
            if (watchDog != null)
            {
                watchDog.ObjectHandler = null;
            }
            _objectHandlers.Remove(handler);
            
            handler.Instance.transform.SetParent(transform);
            _pool.Enqueue(handler.Instance);

            Debug.Log($"[{handler.name}] has been released.");
        }

        public void PrintDebug()
        {
            Debug.Log($"{_poolName}: \n" +
                      $"capacity: {_capacity} \n" +
                      $"count: {_pool.Count} \n" +
                      $"handler count: {_objectHandlers.Count} \n" +
                      $"total: {_objectHandlers.Count + _pool.Count}");
            
            foreach (var handler in _objectHandlers)
            {
                if (!handler.hasParent)
                {
                    Debug.LogWarning($"[{handler.name}]オブジェクトが脱出した。貸し出したオブジェクトが親ノードを設定されてない");
                    handler.PrintDebug();
                }
            }
        }

        internal void HandlerBeDestroyedIllegal(ObjectHandler handler)
        {
            _objectHandlers.Remove(handler);
        }

        private void OnDestroy()
        {
            Debug.Log($"{name} Destroy");
        }
    }
}
