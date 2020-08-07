using System;
using System.Collections.Generic;
using System.Xml;

namespace ASGame
{
    //资源更新包,使用XML文件进行序列化和反序列化
    public class AssetUpdatePackage
    {
        public class PackageFlag
        {
            public static readonly int Force = 1 << 0;          //强制更新
        }
        public int id;                                          //包的编号
        public int version;                                     //版本号
        public int flag;                                        //额外的flag

        public AssetUpdatePackage[] dependences;                //依赖的资源更新包
        public AssetUpdateItem[] items;                         //更新项


        //对文件列表进行对比,生成资源资源更新包
        public static AssetUpdatePackage[] CreateUpdatePackages(AssetUpdateFilelList newResList, AssetUpdateFilelList oldResList)
        {
            return default;
        }

    }

}