﻿using Object = UnityEngine.Object;
namespace ASGame
{
    public class AssetLoadResult
    {
        public static readonly AssetLoadResult FAILED_RESULT = new AssetLoadResult(null,false);

        public readonly Object asset;
        public readonly bool isDone;

        public AssetLoadResult(Object asset, bool isDone)
        {
            this.asset = asset;
            this.isDone = isDone;
        }
        public T GetAsset<T>() where T : Object
        {
            return asset as T;
        }
    }
}
