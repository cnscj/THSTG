using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace ASGame
{
    //文件差异列表
    public class AssetUpdateDifferenceList
    {
        public class Item
        {
            public string filePath;     //文件路径
            public string fileMd5;      //文件MD5
            public long fileSize;

            public Item(AssetUpdateAssetList.Item item)
            {
                this.filePath = item.filePath;
                this.fileMd5 = item.fileMd5;
                this.fileSize = item.fileSize;
            }
        }

        public class Group
        {
            public long size;
            public Dictionary<string, Item> dict;

            public Dictionary<string, Item> GetDict()
            {
                dict = dict ?? new Dictionary<string, Item>();
                return dict;
            }
        }

        public Group adds;
        public Group modifys;
        public Group removes;

        public AssetUpdateDifferenceList Compare(AssetUpdateAssetList newAssetList, AssetUpdateAssetList oldAssetList)
        {
            if (oldAssetList == null || newAssetList == null)
                return this;

            var oldDict = oldAssetList.GetDictByPath();
            var newDict = newAssetList.GetDictByPath();

            int maxCount = Math.Max(oldAssetList.fileItems.Length, newAssetList.fileItems.Length);
            for (int i = 0; i < maxCount; i++)
            {
                var oldItem = i < oldAssetList.fileItems.Length ? oldAssetList.fileItems[i] : null;
                var newItem = i < newAssetList.fileItems.Length ? newAssetList.fileItems[i] : null;

                if (newItem != null)
                {
                    if (oldDict.TryGetValue(newItem.filePath, out var sideOldItem))
                    {
                        //变更的
                        if (string.Compare(newItem.fileMd5, sideOldItem.fileMd5, true) != 0)
                        {
                            var item = new Item(newItem);

                            var modifyGroup = GetModifyGroup();
                            var modifyList = modifyGroup.GetDict();
                            modifyList.Add(item.filePath, item);

                            modifyGroup.size += newItem.fileSize;
                        }
                    }
                    else //新增的
                    {
                        var item = new Item(newItem);

                        var addGroup = GetAddGroup();
                        var addList = addGroup.GetDict();
                        addList.Add(item.filePath, item);

                        addGroup.size += newItem.fileSize;
                    }
                }

                if (oldItem != null)
                {
                    if (!newDict.ContainsKey(oldItem.filePath))     //移除的
                    {
                        var item = new Item(oldItem);

                        var removeGroup = GetRemoveGroup();
                        var removeList = removeGroup.GetDict();
                        removeList.Add(item.filePath, item);

                        removeGroup.size += oldItem.fileSize;
                    }
                }
            }
            return this;
        }

        //完全一致
        public bool IsAccordance()
        {
            var hadAdds = adds != null && adds.dict != null && adds.dict.Count > 0;
            var hadModifys = modifys != null && modifys.dict != null && modifys.dict.Count > 0;
            var hadRemoves = removes != null && removes.dict != null && removes.dict.Count > 0;

            return !hadAdds && !hadModifys && !hadRemoves;
        }

        //通过,不包括新增的文件
        public bool IsVerify()
        {
            var hadModifys = modifys != null && modifys.dict != null && modifys.dict.Count > 0;
            var hadRemoves = removes != null && removes.dict != null && removes.dict.Count > 0;

            return !hadModifys && !hadRemoves;
        }

        //同步差异文件
        public void Synchronize(string srcFolderPath, string destFolderPath, bool isMove = false)
        {
            if (string.IsNullOrEmpty(srcFolderPath) || string.IsNullOrEmpty(destFolderPath))
                return;

            if (!Directory.Exists(srcFolderPath))
                return;

            if (!Directory.Exists(destFolderPath))
                Directory.CreateDirectory(destFolderPath);

            if (adds != null && adds.dict != null)
            {
                foreach (var item in adds.dict.Values)
                {
                    string srcFullPath = Path.Combine(srcFolderPath, item.filePath);
                    string destFullPath = Path.Combine(destFolderPath, item.filePath);

                    if (File.Exists(srcFullPath))
                    {
                        if (isMove) File.Move(srcFullPath, destFullPath);
                        else File.Copy(srcFullPath, destFullPath);
                    }
                }
            }

            if (modifys != null && modifys.dict != null)
            {
                foreach (var item in modifys.dict.Values)
                {
                    string srcFullPath = Path.Combine(srcFolderPath, item.filePath);
                    string destFullPath = Path.Combine(destFolderPath, item.filePath);

                    if (File.Exists(srcFullPath))
                    {
                        if (File.Exists(destFullPath))
                            File.Delete(destFullPath);

                        if (isMove) File.Move(srcFullPath, destFullPath);
                        else File.Copy(srcFullPath, destFullPath);
                    }
                }
            }

            if (removes != null && removes.dict != null)
            {
                foreach (var item in removes.dict.Values)
                {
                    string destFullPath = Path.Combine(destFolderPath, item.filePath);
                    File.Delete(destFullPath);
                }
            }
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (adds != null && adds.dict != null)
            {
                stringBuilder.AppendLine("Adds:");
                foreach (var item in adds.dict.Values)
                {
                    stringBuilder.AppendLine(item.filePath);
                }
            }

            if (modifys != null && modifys.dict != null)
            {
                stringBuilder.AppendLine("");
                stringBuilder.AppendLine("Modifys:");
                foreach (var item in modifys.dict.Values)
                {
                    stringBuilder.AppendLine(item.filePath);
                }
            }

            if (removes != null && removes.dict != null)
            {
                stringBuilder.AppendLine("");
                stringBuilder.AppendLine("Removes:");
                foreach (var item in removes.dict.Values)
                {
                    stringBuilder.AppendLine(item.filePath);
                }
            }

            return stringBuilder.ToString();
        }

        private Group GetModifyGroup()
        {
            modifys = modifys ?? new Group();
            return modifys;
        }

        private Group GetAddGroup()
        {
            adds = adds ?? new Group();
            return adds;
        }

        private Group GetRemoveGroup()
        {
            removes = removes ?? new Group();
            return removes;
        }
    }
}
