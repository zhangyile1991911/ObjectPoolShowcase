using System;
using UnityEngine;

namespace GameSystem.ObjectPool
{
    public class WatchDogComponent : MonoBehaviour
    {
        public ObjectHandler ObjectHandler;
    
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
