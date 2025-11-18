using System.Collections.Generic;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace GameSystem.ObjectPool
{
    public class GameObjectPool : MonoBehaviour 
    {
        [SerializeField]
        private GameObject prefab;
        [SerializeField]
        private  int initCapacity = 0;
        [SerializeField]
        private float checkExpandFrequency = 300f;
        
        private float _lastCheckTime;
        private string _poolName;
        
        private int _currentBorrowed = 0;
        private int _maxBorrowed = 0;
        private float _smoothedPeak = 0f;
        [SerializeField]
        private float alpha = 0.25f;
        
        private Queue<GameObject> _pool = null;
        private HashSet<ObjectHandler> _objectHandlers = new ();
        private int _poolIndex = 0;
        void Awake()
        {
            _poolName = name;
            _pool = new Queue<GameObject>(initCapacity);
            MakeMore(initCapacity);
            DontDestroyOnLoad(gameObject);
        }
        
        private void MakeMore(int num)
        {
            for (int i = 0; i < num; i++,_poolIndex++)
            {
                GameObject inst = GameObject.Instantiate(prefab, transform, true);
                inst.AddComponent<WatchDogComponent>();
                inst.name = $"{_poolName}_{_poolIndex}";
                inst.SetActive(false);
                _pool.Enqueue(inst);
            }
            _previousExpandTime = Time.realtimeSinceStartup;
        }

        public ObjectHandler Get()
        {
            if (_pool.Count <= 0)
            {
                MakeMore(8);
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
            _currentBorrowed++;
            _maxBorrowed = Mathf.Max(_currentBorrowed, _maxBorrowed);
            return handler;
        }

        public void Release(ObjectHandler handler)
        {
            if (handler == null)
            {
                return;
            }

            WatchDogComponent watchDog = handler.instance.GetComponent<WatchDogComponent>();
            if (watchDog != null)
            {
                watchDog.ObjectHandler = null;
            }
            _objectHandlers.Remove(handler);
            
            handler.instance.transform.SetParent(transform);
            _pool.Enqueue(handler.instance);
            _currentBorrowed--;
            Debug.Log($"[{handler.Name}] has been released.");
        }

        public void PrintDebug()
        {
            Debug.Log($"{_poolName}: \n" +
                      $"count: {_pool.Count} \n" +
                      $"handler count: {_objectHandlers.Count} \n" +
                      $"total: {_objectHandlers.Count + _pool.Count}");
            
            foreach (var handler in _objectHandlers)
            {
                if (!handler.hasParent)
                {
                    Debug.LogWarning($"オブジェクト[{handler.Name}]が脱出した。貸し出したオブジェクトが親ノードを設定されてない");
                    handler.PrintDebug();
                }
            }
        }

        internal void HandlerBeDestroyedIllegal(ObjectHandler handler)
        {
            _objectHandlers.Remove(handler);
        }
        
        private float _previousExpandTime;
        public void LateUpdate()
        {
            float diff = Time.realtimeSinceStartup - _previousExpandTime;
            if (diff < checkExpandFrequency) return;
            _smoothedPeak = alpha * _maxBorrowed + (1.0f - alpha) * _smoothedPeak;
            int targetPoolNum = Mathf.CeilToInt(_smoothedPeak * 1.2f);
            ShrinkOrExpandTo(targetPoolNum);
            _maxBorrowed = _currentBorrowed;
            _previousExpandTime = Time.realtimeSinceStartup;
        }

        private void ShrinkOrExpandTo(int targetPoolNum)
        {
            int current = _pool.Count;
            
            if (current == targetPoolNum)
                return;
            
            float changePercent = Mathf.Abs(targetPoolNum - current) / (float)current;
            
            if (changePercent < 0.25f)
                return;

            if (targetPoolNum < current)
            {
                // shrink
                int toRemove = current - targetPoolNum;
                for (int i = 0; i < toRemove; i++)
                {
                    var obj = _pool.Dequeue();
                    Destroy(obj);
                }
                Debug.Log($"{name} shrink out of {toRemove}.");
            }
            else
            {
                // expand
                int toAdd = targetPoolNum - current;
                MakeMore(toAdd);
                Debug.Log($"{name} expand {toAdd}.");
            }
            Debug.Log($"{_poolName} After ShrinkOrExpandTo: \n" +
                      $"previous count = {current} \n" +
                      $"current count: {_pool.Count}");
        }


        private void OnDestroy()
        {
            Debug.Log($"{name} Destroy");
        }
    }
}
