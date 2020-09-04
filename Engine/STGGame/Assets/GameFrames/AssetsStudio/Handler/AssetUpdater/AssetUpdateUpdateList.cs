using System;
using System.Collections.Generic;

namespace ASGame
{
    //文件差异列表
    public class AssetUpdateUpdateList
    {
        public class Item
        {
            public string filePath;     //文件路径
            public string fileMd5;      //文件MD5
            public long fileSize;
            public int flag;

            public Item(AssetUpdateAssetList.Item assetItem, AssetUpdatePackageList.Item packageItem)
            {
                if (assetItem != null)
                {
                    filePath = assetItem.filePath;
                    fileMd5 = assetItem.fileMd5;
                    fileSize = assetItem.fileSize;
                }

                if (packageItem!= null)
                {
                    flag = packageItem.flag;
                }
            }
        }

        public class Package
        {
            public int index;
            public List<Item> adds;
            public List<Item> modifys;
            public List<Item> removes;
        }

        private Dictionary<int, Package> _dict;

        public void Create(AssetUpdateAssetList oldAssetList, AssetUpdateAssetList newAssetList, AssetUpdatePackageList newPackageList = null)
        {
            if (oldAssetList == null || newAssetList == null)
                return;

            var oldDict = oldAssetList.GetDict();
            var newDict = newAssetList.GetDict();
            var packageDict = newPackageList?.GetDict();

            int maxCount = Math.Max(oldAssetList.fileItems.Length, newAssetList.fileItems.Length);
            for(int i = 0; i < maxCount; i++)
            {
                int package = 0;
                var oldItem = i < oldAssetList.fileItems.Length ? oldAssetList.fileItems[i] : null;
                var newItem = i < newAssetList.fileItems.Length ? newAssetList.fileItems[i] : null;

                if (newItem != null)
                {
                    AssetUpdatePackageList.Item packageItem = null;
                    if (packageDict != null)
                    {
                        if (packageDict.TryGetValue(newItem.filePath, out packageItem))
                        {
                            package = packageItem.packageId;
                        }
                    }

                    if (oldDict.TryGetValue(newItem.filePath,out var sideOldItem))
                    {
                        if (string.Compare(newItem.fileMd5, sideOldItem.fileMd5, true) != 0) //变更的
                        {
                            var item = new Item(newItem, packageItem);

                            var modifyList = GetOrAddDictPackageModifyList(package);
                            modifyList.Add(item);
                        }
                    }
                    else                                        //新增的
                    {
                        var item = new Item(newItem, packageItem);

                        var modifyList = GetOrAddDictPackageAddList(package);
                        modifyList.Add(item);
                    }
                }

                if (oldItem != null)
                {
                    package = 0;                                    //包0移除
                    if (!newDict.ContainsKey(oldItem.filePath))     //移除的
                    {
                        var item = new Item(oldItem, null);

                        var removeList = GetOrAddDictPackageRemoveList(package);
                        removeList.Add(item);
                    }
                }
            }
        }

        public Dictionary<int, Package> GetDict()
        {
            _dict = _dict ?? new Dictionary<int, Package>();
            return _dict;
        }

        private Package GetDictPackage(int pakage)
        {
            var dict = GetDict();
            var package = dict.ContainsKey(pakage) ? dict[pakage] : (dict[pakage] = new Package());
            return package;
        }

        private List<Item> GetOrAddDictPackageAddList(int pakage)
        {
            var package = GetDictPackage(pakage);
            package.adds = package.adds ?? new List<Item>();
            return package.adds;
        }

        private List<Item> GetOrAddDictPackageModifyList(int pakage)
        {
            var package = GetDictPackage(pakage);
            package.modifys = package.modifys ?? new List<Item>();
            return package.modifys;
        }

        private List<Item> GetOrAddDictPackageRemoveList(int pakage)
        {
            var package = GetDictPackage(pakage);
            package.removes = package.removes ?? new List<Item>();
            return package.removes;
        }

    }

}
