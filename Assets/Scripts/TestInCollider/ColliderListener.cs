using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GameSystem.ObjectPool;
using UnityEngine;

public class ColliderListener : MonoBehaviour
{
    //key=InstanceId
    protected Dictionary<int,Transform> Actors;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Actors = new Dictionary<int,Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log($"Update===== {Actors.Count} =====");
        foreach(var one in Actors.Values)
        {
            Debug.Log($"{one.name}");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        HandleColliderEnter(other);   
    }

    void OnTriggerExit(Collider other)
    {
        HandleColliderExit(other);
    }

    void OnTriggerStay(Collider other)
    {
        
    }

    public virtual void HandleColliderEnter(Collider other)
    {
        ColliderWatchDog colliderWatchDog = other.gameObject.AddComponent<ColliderWatchDog>();
        WatchDogComponent poolWatchDog = other.gameObject.GetComponent<WatchDogComponent>();
        if(poolWatchDog != null)
        {//このオブジェクトはとあるプールに管理されている
            colliderWatchDog.watchDogComponent = poolWatchDog;
        }
        colliderWatchDog.colliderParent = this;
        Actors.Add(other.gameObject.GetInstanceID(),other.gameObject.transform);
        Debug.Log($"OnTriggerEnter {other.gameObject.name}");
    }

    public virtual void HandleColliderExit(Collider other)
    {
        ColliderWatchDog colliderWatchDog = other.gameObject.GetComponent<ColliderWatchDog>();
        if(colliderWatchDog != null)
        {//このオブジェクトはとあるプールに管理されている
            colliderWatchDog.colliderParent = null;
            Destroy(colliderWatchDog);
        }
        Actors.Remove(other.gameObject.GetInstanceID());
        Debug.Log($"OnTriggerExit {other.gameObject.name}");
    }

    public void RemoveObjectFromOtherWay(
        Transform other,
        [CallerMemberName] string methodName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        if(Actors.ContainsKey(other.gameObject.GetInstanceID()))
        {
            Actors.Remove(other.gameObject.GetInstanceID());    
        }
    }
}
