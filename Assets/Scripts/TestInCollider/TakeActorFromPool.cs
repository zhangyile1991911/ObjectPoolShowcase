using GameSystem.ObjectPool;
using UnityEngine;

public class TakeActorFromPool : MonoBehaviour
{
    private GameObjectPool ActorPool;

    void Start()
    {
        ActorPool = GetComponent<GameObjectPool>();
        previous = Time.realtimeSinceStartup;
    }

    float previous = 0;
    void Update()
    {
        float diff = Time.realtimeSinceStartup - previous;
        if(diff >= 5.0f)
        {
            previous = Time.realtimeSinceStartup;
            ObjectHandler handler = ActorPool.Rent();
            handler.instance.SetActive(true);
            int r = Random.Range(0,100);
            if(r > 50)
            {
                var padic = handler.instance.AddComponent<PoolActorDeadInCollider>();
                padic.OnFinish = ()=>{handler.Release();};
            }
            else
            {
                var pandic = handler.instance.AddComponent<PoolActorNotDeadInCollider>();
                pandic.OnFinish = ()=>{handler.Release();};
            }
        }
    }
}
