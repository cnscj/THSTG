using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASGame
{
    public class AssetUpdateInfo
    {
        public int version;                     //当前资源版本
        public AssetUpdateFilelList resList;    //资源总的列表


        public void Save(string savePath)
        {

        }

        public AssetUpdateInfo Load(string filePath)
        {
            return default;
        }


        public AssetUpdateInfo Parse(byte[] fileByte)
        {
            return default;
        }


        public AssetUpdateInfo Parse(string content)
        {
            return default;
        }
    }

}
