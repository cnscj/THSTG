using System;
using System.Collections;
using System.IO;

namespace ASGame
{
    //负责对下载好文件,或其他二进制文件进行解码,加载
    public class BinarylLoader : BaseCoroutineLoader
    {
        protected override IEnumerator OnLoadAsset(AssetLoadHandler handler)
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

            yield break;
        }
    }
}

