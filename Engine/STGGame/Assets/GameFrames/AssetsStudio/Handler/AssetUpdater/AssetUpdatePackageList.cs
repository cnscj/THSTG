using System;
using System.Collections.Generic;

namespace ASGame
{
    //文件差异列表
    public class AssetUpdatePackageList
    {
        public class Item
        {
            public string filePath;     //文件路径
            public string fileMd5;      //文件MD5
            public long fileSize;       //文件长度
        }

        public string type = "assets";
        public int version;
        public long date;
    }
}
