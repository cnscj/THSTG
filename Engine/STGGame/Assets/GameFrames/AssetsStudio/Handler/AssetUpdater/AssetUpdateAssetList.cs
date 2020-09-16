using System.Collections.Generic;
using System.IO;
using XLibrary;

namespace ASGame
{
    //资源列表
    public class AssetUpdateAssetList
    {
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

        public string type = "assets";
        public int version;
        public long date;

        public Item[] fileItems;

        public void Create(string assetFolder)
        {
            if (string.IsNullOrEmpty(assetFolder))
                return;

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

            fileItems = fileList.ToArray();

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
        }

        public Dictionary<string ,Item> GetDict()
        {
            var dict = new Dictionary<string, Item>();

            foreach(var item in fileItems)
            {
                dict.Add(item.filePath, item);
            }

            return dict;
        }
    }

}
