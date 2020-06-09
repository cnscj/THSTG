using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLibrary.Package;

namespace ASEditor
{
    [System.Serializable]
    public class AssetSyncItem
    {
        public enum SearchMode
        {
            Forward,    //正向:从配置同步文件
            Reverse,    //方向:资源与配置比对,有则同步
        }
        public enum SearchKey
        {
            AssetName,      //匹配全名
            AssetPrefix,    //匹配前缀
        }
        public string name;
        public string srcPath;
        public string realSearcePath;
        public string realSyncPath;
        public SearchMode searchMode;
        public SearchKey searchKey;

    }
}

