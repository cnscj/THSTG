using System;
using System.Collections.Generic;
using System.IO;
using XLibrary;

namespace ASGame
{
    //文件差异列表
    public class AssetUpdateUpdateList
    {
        public class Item
        {
            public AssetUpdateDifferenceList.Item itemSrc;
            public AssetUpdateConfigList.Item itemCfg;

            public Item(AssetUpdateDifferenceList.Item itemSrc, AssetUpdateConfigList.Item itemCfg)
            {
                this.itemSrc = itemSrc;
                this.itemCfg = itemCfg;
            }
        }

        public class Package
        {
            public int index;
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
        public Dictionary<string, Item> _itemsWhitMd5;
        public Dictionary<string, Item> _itemsWithPath;
        private Dictionary<int, Package> _dict;

        public string BaseDownloadUrl => _baseDownloadUrl;
        public string BaseSaveFolder => _baseSaveFolder;

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
            var fileMd5 = pair.Value.fileMd5;
            int packageindex = 0;
            AssetUpdateConfigList.Item itemCfg = null;
            AssetUpdateDifferenceList.Item itemSrc = pair.Value;
            if (dict != null && dict.TryGetValue(filePath, out itemCfg))
            {
                packageindex = itemCfg.packageId;
            }

            var item = new Item(itemSrc, itemCfg);
            string urlPath = GetFileDownloadUrl(item);
            string savePaths = GetSaveFilePath(item);
            var package = GetOrCreatePackage(packageindex);

            package.size += pair.Value.fileSize;

            GetOrCreateMd5ItemDict().Add(fileMd5, item);
            GetOrCreatePathItemDict().Add(filePath, item);
            package.GetUrlList().Add(urlPath);
            package.GetSaveList().Add(savePaths);
        }

        public string GetFileDownloadUrl(Item item)
        {
            return Path.Combine(_baseDownloadUrl, item.itemSrc.filePath);
        }

        public string GetSaveFilePath(Item item)
        {
            var packageIndex = 0;
            if (item.itemCfg != null) packageIndex = item.itemCfg.packageId;
            string folderPath = GetSaveFolderPath(packageIndex);
            return Path.Combine(folderPath, item.itemSrc.filePath);
        }

        public string GetSaveFolderPath(int packageIndex)
        {
            return Path.Combine(_baseSaveFolder, string.Format("S{0}", packageIndex));
        }

        //验证是否符合列表
        public bool Verify(int packageIndex = 0)
        {
            string folderPath = GetSaveFolderPath(packageIndex);
            if (string.IsNullOrEmpty(folderPath))
                return false;

            if (!Directory.Exists(folderPath))
                return false;

            bool isVerify = true;
            var dict = GetOrCreatePathItemDict();
            var record = new HashSet<string>();
            XFolderTools.TraverseFiles(folderPath, (fullPath) =>
            {
                if (isVerify == false)
                    return;

                var assetPath = XPathTools.GetRelativePath(fullPath);
                var relaPath = XPathTools.SubRelativePath(folderPath, assetPath);

                if (dict.TryGetValue(relaPath, out var item))
                {
                    var fileMd5 = XFileTools.GetMD5(relaPath);
                    if (string.Compare(item.itemSrc.fileMd5, fileMd5) != 0)
                    {
                        isVerify = false;
                    }
                    record.Add(relaPath);
                }

            }, true);

            if (isVerify)
            {
                if (dict.Count != record.Count)
                {
                    isVerify = false;
                }
            }
            return isVerify;
        }

        private Package GetOrCreatePackage(int index)
        {
            var dict = GetOrCreateDict();
            if (!dict.TryGetValue(index, out var package))
            {
                package = new Package();
                package.index = index;
                dict.Add(index, package);
            }
            return dict[index];
        }

        private Dictionary<string, Item> GetOrCreateMd5ItemDict()
        {
            _itemsWhitMd5 = _itemsWhitMd5 ?? new Dictionary<string, Item>();
            return _itemsWhitMd5;
        }

        private Dictionary<string, Item> GetOrCreatePathItemDict()
        {
            _itemsWithPath = _itemsWithPath ?? new Dictionary<string, Item>();
            return _itemsWithPath;
        }

        private Dictionary<int, Package> GetOrCreateDict()
        {
            _dict = _dict ?? new Dictionary<int, Package>();
            return _dict;
        }

        ////////////////
        public Item GetItemByMd5(string md5)
        {
            if (_itemsWhitMd5 == null)
                return null;

            if (_itemsWhitMd5.TryGetValue(md5, out var item))
            {
                return item;
            }

            return null;
        }

        public Item GetItemByPath(string path)
        {
            if (_itemsWithPath == null)
                return null;

            if (_itemsWithPath.TryGetValue(path, out var item))
            {
                return item;
            }

            return null;
        }

        public Package GetPackage(int packageIndex)
        {
            if (_dict != null && _dict.TryGetValue(packageIndex,out var package))
            {
                return package;
            }
            return null;
        }

        public Package[] GetPackageList()
        {
            List<Package> packages = new List<Package>();
            packages.Sort((a, b) =>
            {
                return a.index - b.index;
            });
            return packages.ToArray();
        }

        public Item[] GetItemList()
        {
            List<Item> items = new List<Item>();
            items.AddRange(GetOrCreatePathItemDict().Values);
            return items.ToArray();
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
