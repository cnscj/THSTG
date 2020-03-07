using System.Collections.Generic;
using UnityEngine;

namespace ASGame
{
    public class AssetDebugBaseObject : MonoBehaviour
    {
        [Header("引用次数")]
        public int refCount;

        [Header("加载耗时")]
        public float usedTime;
    }
}
