using System.Collections.Generic;
using System.IO;
using XLibrary;

namespace ASGame
{
    //分包配置表
    public class AssetUpdateConfigList
    {
        public static readonly string FILE_TYPE = "config";
        public static readonly int FILE_VERSION = 100;
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

        public string path;
        public string type = FILE_TYPE;
        public int version = FILE_VERSION;
        public long date;

        private Dictionary<string, Item> _data = new Dictionary<string, Item>();

        //生成某个目录的文件列表
        public void Export(string savePath)
        {
            var fileItems = GetItemList();
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
            while ((line = streamReader.ReadLine()) != null)
            {
                line = line.Trim();
                var updateItem = new Item();
                updateItem.Deserialization(line);

                fileList.Add(updateItem);
            }

            streamReader.Close();
            fileStream.Close();
            streamReader.Dispose();
            fileStream.Dispose();

            path = loadPath;

            List2Dict(fileList, _data);
        }

        private void List2Dict(List<Item> itemList, Dictionary<string,Item> itemDict)
        {
            itemDict.Clear();
            foreach(var item in itemList)
            {
                itemDict.Add(item.filePath, item);
            }
        }

        public Item[] GetItemList()
        {
            var fileList = new List<Item>();
            fileList.AddRange(_data.Values);

            return fileList.ToArray();
        }

        public Dictionary<string, Item> GetDict()
        {
            return _data;
        }
    }

}
