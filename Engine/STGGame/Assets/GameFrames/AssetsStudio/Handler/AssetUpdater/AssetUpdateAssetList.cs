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
                return string.Format("{0},{1},{2}", filePath, fileMd5, fileSize);
            }

            public void Deserialization(string content)
            {
                string[] strArray = content.Split(',');
                filePath = strArray[0];
                fileMd5 = strArray[1];
                fileSize = long.Parse(strArray[4]);
            }
        }

        public int version;
        public long date;

        public int fileCount;
        public Item[] fileItems;

        public void Create(string assetFolder)
        {
            if (string.IsNullOrEmpty(assetFolder))
                return;

            var fileList = new List<Item>();
            XFolderTools.TraverseFiles(assetFolder, (assetPath) =>
            {
                var relaPath = XPathTools.GetRelativePath(assetPath);
                var updateItem = new Item();
                updateItem.filePath = relaPath.ToLower();
                updateItem.fileMd5 = XFileTools.GetMD5(relaPath);
                updateItem.fileSize = XFileTools.GetLength(relaPath);

                fileList.Add(updateItem);

            },true);

            fileCount = fileList.Count;
            fileItems = fileList.ToArray();

        }

        //生成某个目录的文件列表
        public void Export(string savePath)
        {
            FileStream fileStream = new FileStream(savePath, FileMode.Create, FileAccess.Write);
            StreamWriter streamWriter = new StreamWriter(fileStream);

            date = XTimeTools.NowTimeStampMs();
            streamWriter.WriteLine(string.Format("{0},{1}", version, date));    //第一行
            streamWriter.WriteLine(string.Format("{0}", fileCount));

            for(int i = 0; i < fileItems.Length;i++)
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
            version = int.Parse(headSections[0]);
            date = long.Parse(headSections[1]);

            line = streamReader.ReadLine().Trim();
            string[] exSections = line.Split(',');
            fileCount = int.Parse(exSections[0]);

            var fileList = new List<Item>();
            for (int i = 0; i < fileCount; i++)
            {
                line = streamReader.ReadLine().Trim();
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
    }

}
