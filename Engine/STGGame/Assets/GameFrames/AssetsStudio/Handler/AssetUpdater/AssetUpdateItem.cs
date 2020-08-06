using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASGame
{
    public class AssetUpdateItem
    {
        public string fileName;     //文件路径
        public string fileMd5;      //文件MD5
        public string packageId;    //所属包
        public string resType;      //资源类型
        public long fileSize;       //文件长度

        public static AssetUpdateItem ParseContent(string content)
        {
            return default;
        }
    }

}
