using System.Collections.Generic;
using UnityEngine;

namespace ASGame
{
    [System.Serializable]
    public class ModelEffectInfo : MonoBehaviour
    {
        public int skeletonId;                          //骨骼ID
        public int totalLevel;                          //总级别
        public List<ModelEffectMetadata> metadataList;  //记录着的信息

    }
}