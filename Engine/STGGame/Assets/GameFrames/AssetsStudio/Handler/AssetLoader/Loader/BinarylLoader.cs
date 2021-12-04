using System;
using System.Collections;
using System.IO;

namespace ASGame
{
    //异步加载二进制文件
    public class BinarylLoader : BaseLoadMethod
    {
        protected override IEnumerator OnLoadAssetAsync(AssetLoadHandler handler)
        {
            var filePath = handler.path;
            using (FileStream fsRead = new FileStream(filePath, FileMode.Open))
            {
                int fsLen = (int)fsRead.Length;
                int stLen = 1024 * 1024;
                byte[] heByte = new byte[fsLen];

                int hadReadedLen = 0;
                int needReadLen = hadReadedLen + stLen < fsLen ? stLen : fsLen - hadReadedLen;
                while (true)
                {
                    int r = fsRead.Read(heByte, hadReadedLen, needReadLen);
                    hadReadedLen = hadReadedLen + r;
                    if (hadReadedLen >= fsLen)
                    {
                        handler.result = new AssetLoadResult(heByte, true);
                        handler.status = AssetLoadStatus.LOAD_FINISHED;
                        handler.Callback();
                        yield break;
                    }
                    else
                    {
                        yield return null;
                    }
                }
            }
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

