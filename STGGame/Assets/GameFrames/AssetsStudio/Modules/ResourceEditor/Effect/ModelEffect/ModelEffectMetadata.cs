using UnityEngine;

namespace ASGame
{
    [System.Serializable]
    public class ModelEffectMetadata
    {
        [SerializeField] 
        public GameObject effectGO;         //特效节点GO
        [SerializeField] 
        public string bonePath;             //骨骼路径
        [SerializeField]
        public int level;                   //等级
    }
}