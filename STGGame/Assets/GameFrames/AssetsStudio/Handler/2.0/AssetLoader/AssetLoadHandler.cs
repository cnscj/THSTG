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

    }
}
