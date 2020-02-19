
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XLibrary.Package;
using Object = UnityEngine.Object;
namespace ASGame
{
    public class AssetCaches : MonoSingleton<AssetCaches>
    {
        public bool Add(string key,Object obj)
        {
            return false;
        }
        public Object Get(string key)
        {
            return null;
        }
        public T Get<T>(string key)
        {
            return default;
        }
        public void Release(string key)
        {

        }
    }
}