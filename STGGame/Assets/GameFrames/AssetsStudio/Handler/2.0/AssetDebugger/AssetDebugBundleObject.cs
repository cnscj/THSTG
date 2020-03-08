using System.Collections.Generic;
using UnityEngine;

namespace ASGame
{
    public class AssetDebugBundleObject : AssetDebugBaseObject
    {
        [Header("引用次数")]
        public int refCount;

        [Header("加载耗时")]
        public float usedTime;

        public static AssetDebugBundleObject Create(string name, Transform parent)
        {
            GameObject newGObj = new GameObject(name);
            newGObj.transform.SetParent(parent);
            var debugObj = newGObj.AddComponent<AssetDebugBundleObject>();
            return debugObj;
        }
    }
}
