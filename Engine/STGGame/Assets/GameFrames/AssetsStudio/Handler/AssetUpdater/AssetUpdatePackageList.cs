using System.Collections.Generic;
using System.IO;
using XLibrary;

namespace ASGame
{
    //分包表
    public class AssetUpdatePackageList
    {
        public class Item
        {
            public string filePath;
            public int packageId;       //所分到的包
            public int resourceType;    //资源类型
            public int flag;

            public string Serialize()
            {
                return string.Format("{0}={1},{2},{3}", filePath, packageId, resourceType, flag);
            }

            public void Deserialization(string content)
            {
                int equalIndex = content.IndexOf('=');
                filePath = content.Substring(0, equalIndex);

                var argsStr = content.Substring(equalIndex + 1);
                var args = argsStr.Split(',');

                packageId = int.Parse(args[0]);
                resourceType = int.Parse(args[1]);
                flag = int.Parse(args[2]);
            }
        }

        public int version;
        public long date;

        public int fileCount;
        public Item[] fileItems;

        private Dictionary<string, Item> _packageDict;


        public Item[] GetItemList()
        {
            var fileList = new List<Item>();
            fileList.AddRange(_packageDict.Values);

            return fileList.ToArray();
        }

        //TODO:
        public void Remove(string assetPath)
        {

        }

        public Item Get(string assetPath)
        {
            return null;
        }


        public void Add(Item item)
        {
            return;
        }

        public void Create(string assetFolder)
        {
            if (string.IsNullOrEmpty(assetFolder))
                return;

            var fileList = new List<Item>();
            fileList.AddRange(_packageDict.Values);

            fileCount = fileList.Count;
            fileItems = fileList.ToArray();


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
