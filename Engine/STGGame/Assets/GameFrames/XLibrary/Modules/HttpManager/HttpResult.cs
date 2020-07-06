
using UnityEngine;
using UnityEngine.Networking;

namespace XLibGame
{
    public class HttpResult
    {
        UnityWebRequest _webRequest;
        public HttpResult(UnityWebRequest webRequest)
        {
            _webRequest = webRequest;
        }
        public override string ToString()
        {
            return _webRequest.downloadHandler.text;
        }
        public byte[] ToBytes()
        {
            return _webRequest.downloadHandler.data;

        }
    }
}
