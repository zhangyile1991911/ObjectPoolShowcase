using GameSystem.ObjectPool;
using UnityEngine;

public class TestPoolShrinkOrExpand : MonoBehaviour
{
    GameObjectPool _pool;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _pool = FindFirstObjectByType<GameObjectPool>();
        time = Time.realtimeSinceStartup;
    }

    private float time;

    private float wait = 1.0f;
    // Update is called once per frame パーワープレイ
    void Update()
    {
        float deltaTime = Time.realtimeSinceStartup - time;
        if (deltaTime > wait)
        {
            int borrowNum = Mathf.CeilToInt(Random.Range(1, 25));
            for (int i = 0; i < borrowNum; i++)
            {
                var handler = _pool.Get();
                handler.instance.transform.SetParent(transform);
                PrimeTween.Tween.Delay(this, duration: 1.0f+borrowNum*0.1f, onComplete: () => handler.Release());
            }

            wait = Random.Range(1.0f, 10.0f);
            time = Time.realtimeSinceStartup;
        }
        
    }
}
