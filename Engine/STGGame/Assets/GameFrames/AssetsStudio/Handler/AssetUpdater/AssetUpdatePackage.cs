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

        public class UpdateItem
        {
            public string srcPath;
            public string destPath;
            public string md5;                                  //MD5
        }

        public int version;                                     //版本号
        public int flag;                                        //额外的flag
        public long size;                                       //更新包大小

        public AssetUpdatePackage[] dependences;                //依赖的资源更新包
        public UpdateItem[] items;                              //更新项

        public static AssetUpdatePackage XML2Class(XmlDocument xml)
        {

            return default;
        }

        public static XmlDocument Class2XML(AssetUpdatePackage package)
        {
            //https://www.jianshu.com/p/f98c3f18bf18
            XmlDocument doc = new XmlDocument();
            XmlNode xmlHead = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            doc.AppendChild(xmlHead);



            return doc;
        }
    }

}