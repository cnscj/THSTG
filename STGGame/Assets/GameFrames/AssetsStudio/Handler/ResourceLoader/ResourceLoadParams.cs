using System;
using UnityEngine.Events;

namespace ASGame
{
    public class ResourceLoadParams
    {
        public string resPath;
        public byte[] resData;

        public string assetName;

        public float stayTime = 30f;

        public ResourceLoadParams(string path, Type type = null)
        {
            ResourceLoaderUtil.SplitBundlePath(path, out resPath, out assetName, type);
        }

        public ResourceLoadParams(string path, string asset)
        {
            resPath = path;
            assetName = asset;
        }

        public ResourceLoadParams(byte[] data, string asset = null)
        {
            resData = data;
            assetName = asset;
        }

        public string GetUID()
        {
            if (resPath != null)
            {
                return ResourceLoaderUtil.CombineBundlePath(resPath, assetName);
            }
            else if(resData != null)
            {
                return ResourceLoaderUtil.CombineBundlePath(resData.GetHashCode().ToString(), assetName);
            }
            return null;
        }
    }

}
