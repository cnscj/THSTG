using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
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
