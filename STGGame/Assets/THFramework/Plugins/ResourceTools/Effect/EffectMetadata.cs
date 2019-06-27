using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
    [System.Serializable]
    public class EffectMetadata
    {
      
        [SerializeField]
        public int level;                               //等级
        [SerializeField]
        public List<string> effectList;             //节点信息
    }
}