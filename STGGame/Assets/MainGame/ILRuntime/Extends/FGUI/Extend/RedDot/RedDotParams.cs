using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace STGService.UI
{
    public class RedDotParams
    {
        public string key1;
        public string key2;
        public string key3;
        public string key4;

        public RedDotParams(string key1, string key2 = null, string key3 = null, string key4 = null)
        {
            this.key1 = key1;
            this.key2 = key2;
            this.key3 = key3;
            this.key4 = key4;
        }
    }
}

