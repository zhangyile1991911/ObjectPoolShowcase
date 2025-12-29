using System.Collections.Generic;
using GameSystem.ObjectPool;
using UnityEngine;

public class ColliderWatchDog : MonoBehaviour
{
    public WatchDogComponent watchDogComponent;
    //所属するColliderを記録しておく
    public ColliderListener colliderParent;

    void OnTransformParentChanged()
    {//もしこのGameObjectはプールに管理されているなら、
    // プールに返却された時に、親オブジェクトが変更する
        if(watchDogComponent.IsBackToPool && colliderParent)
        {
            colliderParent.RemoveObjectFromOtherWay(this.transform);
        }
    }

    void OnDestroy()
    {
        if(colliderParent)
        {
            colliderParent.RemoveObjectFromOtherWay(this.transform);    
        }
    }
}
