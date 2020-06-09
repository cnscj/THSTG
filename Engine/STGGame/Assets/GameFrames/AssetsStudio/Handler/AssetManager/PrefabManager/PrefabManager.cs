using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace ASGame
{
    /*
     * 引用计数记录prefab的来源
     * 
     * 
     */
    public class PrefabManager : BaseManager<PrefabManager>
    {
        public void Instance(string assetPath, Action<GameObject> action)
        {

        }

        public void Destroy(GameObject gameObject)
        {
            
        }
    }

}
