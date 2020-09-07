using System;
using System.Collections.Generic;

namespace ASGame
{

    public class AssetUpdateUpdater
    {
        public class Item
        {
            public string filePath;     //文件路径
            public string fileMd5;      //文件MD5
            public long fileSize;
            public int flag;

            public Item(AssetUpdateAssetList.Item assetItem, AssetUpdateConfigList.Item packageItem)
            {
                if (assetItem != null)
                {
                    filePath = assetItem.filePath;
                    fileMd5 = assetItem.fileMd5;
                    fileSize = assetItem.fileSize;
                }

                if (packageItem != null)
                {
                    flag = packageItem.flag;
                }
            }
        }

    }
}
