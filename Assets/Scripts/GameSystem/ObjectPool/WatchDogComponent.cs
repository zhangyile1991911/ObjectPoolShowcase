using System;
using Unity.VisualScripting;
using UnityEngine;

namespace GameSystem.ObjectPool
{
    public class WatchDogComponent : MonoBehaviour
    {
        [DoNotSerialize]
        public ObjectHandler ObjectHandler;
        [DoNotSerialize]
        public bool IsBackToPool => ObjectHandler == null;
    
        private void OnDestroy()
        {
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
            {//Editor上でplaymodeから中止する時に何もしない
                return;
            }
#endif
            if (!Application.isPlaying)
            {//Exit
                return;
            }
            //予想外でオブジェクトが廃棄されたら
            ObjectHandler?.IllegalDestroy();
        }
    }
}
