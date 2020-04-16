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
        public AssetLoadHandler[] children;

        public AssetLoadCallback onCallback;

        public bool IsCompleted()
        {
            bool isCompleted = (status == AssetLoadStatus.LOAD_SUCCESS);
            if (children != null)
            {
                foreach(var loader in children)
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
            children = null;

            onCallback = null;
        }
    }
}
