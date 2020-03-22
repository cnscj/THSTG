using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASGame
{
    public static class AssetUtil
    {
        public enum PathType
        {
            Local,
            URL,
            Bundle,
        }

        public static PathType GetPathType(string path)
        {
            //TODO:
            return PathType.Local;
        }
    }
}
