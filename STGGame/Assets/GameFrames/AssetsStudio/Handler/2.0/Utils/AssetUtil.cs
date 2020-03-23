using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASGame
{
    public static class AssetUtil
    {
        public enum AssetPathType
        {
            Unknow,
            Local,
            Url,
            AssetBundle,
        }

        public static AssetPathType GetAssetPathType(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                //Bundle路径
                if (path.IndexOf("|") > 0)
                {
                    return AssetPathType.AssetBundle;
                }
                else
                {
                    //第一个冒号之前的,如果是http,https,ftp等,则为网络协议
                    int index = path.IndexOf(":");
                    if (index > 0)
                    {
                        string protocol = path.Substring(0, index).ToLower();
                        if (protocol == "http" || protocol == "https")
                        {
                            return AssetPathType.Url;
                        }
                        else
                        {
                            return AssetPathType.Local;
                        }
                    }
                    else
                    {
                        return AssetPathType.Local;
                    }
                }
            }
            return AssetPathType.Unknow;
        }
    }
}
