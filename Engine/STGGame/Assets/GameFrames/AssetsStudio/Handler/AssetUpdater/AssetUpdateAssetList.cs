using System;
using System.Collections.Generic;
using System.IO;
using XLibrary;

namespace ASGame
{
    //资源列表
    public class AssetUpdateAssetList
    {
        public static readonly string FILE_TYPE = "assets";
        public static readonly int FILE_VERSION = 100;
        public class Item
        {
            public string filePath;     //文件路径
            public string fileMd5;      //文件MD5
            public long fileSize;       //文件长度

            public string Serialize()
            {
                return string.Format("{0}={1},{2}", filePath, fileMd5, fileSize);
            }

            public void Deserialization(string content)
            {
                int equalIndex = content.IndexOf('=');
                filePath = content.Substring(0, equalIndex);

                var argsStr = content.Substring(equalIndex + 1);
                var args = argsStr.Split(',');

                fileMd5 = args[0];
                fileSize = long.Parse(args[1]);
            }
        }

        public string path;
        public string type = FILE_TYPE;         //资源
        public int version = FILE_VERSION;      //版本号
        public long date;                       //日期

        public Item[] fileItems;

        public AssetUpdateAssetList Scan(string assetFolder)
        {
            if (string.IsNullOrEmpty(assetFolder))
                return this;

            var assetFolderLow = assetFolder.ToLower();
            var fileList = new List<Item>();
            XFolderTools.TraverseFiles(assetFolder, (fullPath) =>
            {
                var assetPath = XPathTools.GetRelativePath(fullPath);
                var relaPath = XPathTools.SubRelativePath(assetFolder, assetPath);

                var updateItem = new Item();
                updateItem.filePath = relaPath.ToLower();
                updateItem.fileMd5 = XFileTools.GetMD5(assetPath);
                updateItem.fileSize = XFileTools.GetLength(assetPath);

                fileList.Add(updateItem);

            },true);

            path = assetFolder;
            fileItems = fileList.ToArray();
            return this;
        }

        //生成某个目录的文件列表
        public void Export(string savePath)
        {
            FileStream fileStream = new FileStream(savePath, FileMode.Create, FileAccess.Write);
            StreamWriter streamWriter = new StreamWriter(fileStream);

            date = XTimeTools.NowTimeStampMs;
            streamWriter.WriteLine(string.Format("{0},{1},{2}",type, version, date));    //第一行

            for(int i = 0; i < fileItems.Length; i++)
            {
                var updateItem = fileItems[i];
                streamWriter.WriteLine(updateItem.Serialize());
            }

            streamWriter.Close();
            fileStream.Close();
            streamWriter.Dispose();
            fileStream.Dispose();

            path = savePath;
        }

        public void Import(string loadPath)
        {
            FileStream fileStream = new FileStream(loadPath, FileMode.Open, FileAccess.Read);
            StreamReader streamReader = new StreamReader(fileStream);

            string line = string.Empty;

            line = streamReader.ReadLine().Trim();  //第一行
            string[] headSections = line.Split(',');
            type = headSections[0];
            version = int.Parse(headSections[1]);
            date = long.Parse(headSections[2]);

            var fileList = new List<Item>();
            while((line = streamReader.ReadLine()) != null)
            {
                line = line.Trim();
                var updateItem = new Item();
                updateItem.Deserialization(line);

                fileList.Add(updateItem);
            }
            fileItems = fileList.ToArray();

            streamReader.Close();
            fileStream.Close();
            streamReader.Dispose();
            fileStream.Dispose();

            path = loadPath;
        }

        //验证某个文件夹的文件是否符合
        public bool Verify(string folderPath, List<Item> retList = null)
        {
            if (string.IsNullOrEmpty(folderPath))
                return false;

            if (!Directory.Exists(folderPath))
                return false;

            bool isVerify = true;
            var dict = GetDictByPath();

            foreach(var pair in dict)
            {
                bool isFailed = true;
                string path = Path.Combine(folderPath, pair.Value.filePath);
                if (File.Exists(path))
                {
                    string fileMd5 = XFileTools.GetMD5(path);
                    if(string.Compare(pair.Value.fileMd5, fileMd5) == 0)
                    {
                        isFailed = false;
                    }
                }

                if (isFailed)
                {
                    retList?.Add(pair.Value);
                    isVerify = false;
                    if (retList == null)
                        break;
                }

            }

            return isVerify;
        }


        public Dictionary<string ,Item> GetDictByPath()
        {
            var dict = new Dictionary<string, Item>();

            foreach(var item in fileItems)
            {
                dict.Add(item.filePath, item);
            }

            return dict;
        }

        public Dictionary<string, Item> GetDictByMd5()
        {
            var dict = new Dictionary<string, Item>();

            foreach (var item in fileItems)
            {
                dict.Add(item.fileMd5, item);
            }

            return dict;
        }   
    }
}
