using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Object = UnityEngine.Object;

namespace ASGame
{
    //缓冲数据信息
    public interface IAssetRef
    {
        int RefCount();
        void Retain();
        void Release();
    }

}
