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

        public AssetLoadCallback onCallback;

        public void Reset()
        {
            id = 0;
            status = 0;
            loader = null;
            path = null;

            onCallback = null;
        }
    }
}
