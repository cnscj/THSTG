using UnityEngine.Events;
using System.Linq;
using Object = UnityEngine.Object;
namespace THGame
{
    public class ResourceLoadParams<T> where T : Object
    {
        string path;
        byte[] data;
        string assetName;
        UnityAction<T> onLoadComplate;
        UnityAction<float> onLoadProgress;
    }

}
