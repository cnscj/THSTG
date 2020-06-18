﻿using System;
using System.Collections;
using System.IO;

namespace ASGame
{
    //异步加载二进制文件
    public class BinarylLoader : BaseLoadMethod
    {
        protected override IEnumerator OnLoadAssetAsync(AssetLoadHandler handler)
        {
            OnLoadAssetSync(handler);
            yield break;
        }

        protected override void OnLoadAssetSync(AssetLoadHandler handler)
        {
            string filePath = handler.path;

            try
            {
                FileInfo fi = new FileInfo(filePath);
                byte[] buff = new byte[fi.Length];

                FileStream fs = fi.OpenRead();
                fs.Read(buff, 0, Convert.ToInt32(fs.Length));
                fs.Close();


                handler.result = new AssetLoadResult(buff, true);
                handler.status = AssetLoadStatus.LOAD_FINISHED;
                handler.Callback();
            }
            catch
            {
                handler.result = AssetLoadResult.EMPTY_RESULT;
                handler.status = AssetLoadStatus.LOAD_ERROR;
                handler.Callback();
            }

        }
    }
}
