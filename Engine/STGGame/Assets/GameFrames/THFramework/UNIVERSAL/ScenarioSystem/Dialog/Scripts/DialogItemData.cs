using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
    //TODO:需要考虑多语言的支持
    /// <summary>
    /// 对话项内容
    /// </summary>
    [System.Serializable]
    public class DialogItemData 
    {
        public string name;
        public string content;
        public long playArgs;
        public long exArgs;
    }
}
