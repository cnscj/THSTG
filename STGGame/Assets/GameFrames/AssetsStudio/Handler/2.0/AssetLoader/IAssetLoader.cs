using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASGame
{
    public interface IAssetLoader 
    {
        /// <summary>
        /// 开始资源加载
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        AssetLoadHandler StartLoad(string path);


        /// <summary>
        /// 停止资源加载
        /// </summary>
        /// <param name="handler"></param>
        void StopLoad(AssetLoadHandler handler);
    }

}
