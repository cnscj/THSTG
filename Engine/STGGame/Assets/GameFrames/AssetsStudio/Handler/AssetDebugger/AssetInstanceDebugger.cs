using System.Collections.Generic;
using UnityEngine;

namespace ASGame
{
    public class AssetInstanceDebugger : AssetBaseDebugger
    {
        Dictionary<string, AssetDebugBundleObject> m_resGameObj = new Dictionary<string, AssetDebugBundleObject>();
        public override void Add(string key)
        {
            AssetDebugBundleObject bundleObj = null;
            if (!m_resGameObj.TryGetValue(key,out bundleObj))
            {
                bundleObj = AssetDebugBundleObject.Create(key, transform);
                m_resGameObj.Add(key,bundleObj);
            }
            bundleObj.refCount++;
        }

        public override void Remove(string key)
        {
            AssetDebugBundleObject bundleObj = null;
            if (!m_resGameObj.TryGetValue(key, out bundleObj))
            {
                bundleObj.refCount--;
                if(bundleObj.refCount <= 0 )
                {
                    Object.Destroy(bundleObj.gameObject);
                    m_resGameObj.Remove(key);
                }
            }
        }

        public override void Clear()
        {
            foreach(var bundleObj in m_resGameObj.Values)
            {
                Object.Destroy(bundleObj.gameObject);
            }
            m_resGameObj.Clear();
        }

        public override void To(string key)
        {
            
        }
    }
}
