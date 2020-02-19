
using System;
using XLibrary.Package;

namespace ASGame
{
    public class AssetTextureManager : AssetCacheBaseManager<AssetTextureManager>
    {
        public delegate void OnLoadSuccess(object texture);
        public delegate void OnLoadFailed(string error);

        public void Reset()
        {

        }

        public void Revive()
        {

        }

        public void Recycle()
        {

        }

        public void OnLoadCompleted(string error)
        {

        }

        public void ClearAll()
        {

        }

        public void LoadTexture(string url, OnLoadSuccess onLoadSuccess, OnLoadFailed onLoadFailed, bool isAsyncLoading)
        {

        }

        public void LoadUrl(string url, OnLoadSuccess onLoadSuccess, OnLoadFailed onLoadFailed, bool isAsyncLoading)
        {

        }

        public void UnloadTexture(string url, string name)
        {

        }




        void OnDestroy()
        {
            ClearAll();
        }
    }
}