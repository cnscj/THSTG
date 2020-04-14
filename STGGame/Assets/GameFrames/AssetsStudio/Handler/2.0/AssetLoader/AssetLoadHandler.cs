using System;
using UnityEngine;

namespace ASGame
{
    public class AssetLoadHandler
    {
        public int id;
        public int status;
        public string path;
        public BaseLoader loader;
        public AssetLoadHandler[] dependencies;

        public AssetLoadCallback onCallback;

        public bool IsCompleted()
        {
            bool isCompleted = (status == AssetLoadStatus.LOAD_SUCCESS);
            if (dependencies != null)
            {
                foreach(var loader in dependencies)
                {
                    isCompleted &= loader.IsCompleted();
                }
            }
            return isCompleted;
        }

        public void Reset()
        {
            id = 0;
            status = 0;
            loader = null;
            path = null;
            dependencies = null;

            onCallback = null;
        }
    }
}
