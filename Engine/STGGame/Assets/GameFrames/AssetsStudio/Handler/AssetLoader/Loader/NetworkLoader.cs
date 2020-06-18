using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

namespace ASGame
{
    //不建议使用,请用AssetDownload进行资源加载
    public class NetworkLoader : BaseLoadMethod
    {
        protected override IEnumerator OnLoadAssetAsync(AssetLoadHandler handler)
        {
            UnityWebRequest webRequest = UnityWebRequest.Get(handler.path);
            webRequest.timeout = 30;
            handler.timeoutChecker.stayTime = webRequest.timeout;

            yield return webRequest.SendWebRequest();

            if (webRequest.isHttpError || webRequest.isNetworkError)
            {
                handler.result = AssetLoadResult.EMPTY_RESULT;
                handler.status = AssetLoadStatus.LOAD_ERROR;
                handler.Callback();
            }
            else
            {
                //因为第一次可能还没加载完，返回的是0没有数据，所以需要判断一下
                while (webRequest.responseCode != 200)
                {
                    yield return null;  //这里只能等自己超时了
                }

                if (webRequest.responseCode == 200)//200表示接受成功
                {
                    byte[] data = webRequest.downloadHandler.data;
                    handler.result = new AssetLoadResult(data, true);
                    handler.status = AssetLoadStatus.LOAD_FINISHED;
                    handler.Callback();
                }
            }
        }

        protected override void OnLoadAssetSync(AssetLoadHandler handler)
        {
            throw new NotImplementedException();
        }

    }
}

