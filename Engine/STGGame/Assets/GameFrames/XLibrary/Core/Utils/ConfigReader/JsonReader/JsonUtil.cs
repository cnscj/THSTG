using System.Collections;
using System.Collections.Generic;
using LitJson;

namespace XLibrary
{
    public static class JsonUtil 
    {
        public static string Encode(object obj)
        {
            return JsonMapper.ToJson(obj);
        }

        public static T Decode<T>(string json)
        {
            return JsonMapper.ToObject<T>(json);
        }
    }

}
