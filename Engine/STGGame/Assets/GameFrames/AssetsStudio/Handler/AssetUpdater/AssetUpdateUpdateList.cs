using System;
using System.Collections.Generic;
using System.IO;

namespace ASGame
{
    //文件差异列表
    public class AssetUpdateUpdateList
    {
        public class Item
        {
            public string filePath;
            public string fileMd5;
            public int flag;

            public Item(AssetUpdateDifferenceList.Item itemSrc, AssetUpdateConfigList.Item itemCfg)
            {
                if (itemSrc != null)
                {
                    filePath = itemSrc.filePath;
                    fileMd5 = itemSrc.fileMd5;
                }

                if (itemCfg != null)
                {
                    flag = itemCfg.flag;
                }
            }
        }

        public class Package
        {
            public long size;
            
            public List<string> urlPaths;
            public List<string> savePaths;

            public List<string> GetUrlList()
            {
                urlPaths = urlPaths ?? new List<string>();
                return urlPaths;
            }
            public List<string> GetSaveList()
            {
                savePaths = savePaths ?? new List<string>();
                return savePaths;
            }
        }

        private string _baseDownloadUrl;
        private string _baseSaveFolder;
        public Dictionary<string, Item> _items;
        private Dictionary<int, Package> _dict;


        public AssetUpdateUpdateList(string baseDownloadUrl,string baseSaveFolder)
        {
            _baseDownloadUrl = baseDownloadUrl;
            _baseSaveFolder = baseSaveFolder;
        }

        public void Convert(AssetUpdateDifferenceList differenceList, AssetUpdateConfigList configList)
        {
            if (string.IsNullOrEmpty(_baseDownloadUrl) || string.IsNullOrEmpty(_baseSaveFolder))
                return;

            if (differenceList == null)
                return;
            
            var packageDict = configList?.GetDict();
            if (differenceList.adds != null && differenceList.adds.dict != null)
            {
                foreach (var vPair in differenceList.adds.dict)
                {
                    ConvertItem(vPair, packageDict);
                }
            }

            if (differenceList.modifys != null && differenceList.modifys.dict != null)
            {
                foreach (var vPair in differenceList.modifys.dict)
                {
                    ConvertItem(vPair, packageDict);
                }
            }
        }

        private void ConvertItem(KeyValuePair<string,AssetUpdateDifferenceList.Item> pair,Dictionary<string, AssetUpdateConfigList.Item> dict)
        {
            var filePath = pair.Key;

            int packageindex = 0;
            AssetUpdateConfigList.Item itemCfg = null;
            AssetUpdateDifferenceList.Item itemSrc = pair.Value;
            if (dict != null && dict.TryGetValue(filePath, out itemCfg))
            {
                packageindex = itemCfg.packageId;
            }

            var item = new Item(itemSrc, itemCfg);
            string urlPath = Path.Combine(_baseDownloadUrl, filePath);
            string savePaths = Path.Combine(_baseSaveFolder, filePath);
            var package = GetOrCreatePackage(packageindex);

            package.size += pair.Value.fileSize;

            GetOrCreateItemDict().Add(filePath,item);
            package.GetUrlList().Add(urlPath);
            package.GetSaveList().Add(savePaths);
        }

        private Package GetOrCreatePackage(int index)
        {
            var dict = GetOrCreateDict();
            if (!dict.TryGetValue(index, out var package))
            {
                package = new Package();
                dict.Add(index, package);
            }
            return dict[index];
        }

        public Dictionary<string, Item> GetOrCreateItemDict()
        {
            _items = _items ?? new Dictionary<string, Item>();
            return _items;
        }

        private Dictionary<int, Package> GetOrCreateDict()
        {
            _dict = _dict ?? new Dictionary<int, Package>();
            return _dict;
        }


        public string[] GetUrlList(int packageIndex = 0)
        {
            var package = GetOrCreatePackage(packageIndex);
            return package.GetUrlList().ToArray();
        }


        public string[] GetSaveList(int packageIndex = 0)
        {
            var package = GetOrCreatePackage(packageIndex);
            return package.GetSaveList().ToArray();
        }
    }

}
