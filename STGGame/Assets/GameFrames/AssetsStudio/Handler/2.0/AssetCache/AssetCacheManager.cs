
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XLibrary.Package;
using Object = UnityEngine.Object;
namespace ASGame
{
    public abstract class AssetCacheManager<T> : MonoSingleton<T> where T: MonoBehaviour
    {
      
    }
}