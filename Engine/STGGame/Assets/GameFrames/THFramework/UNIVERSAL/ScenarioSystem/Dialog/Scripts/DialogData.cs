using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
    [System.Serializable]
    public class DialogData
    {
        public int version;
        public Dictionary<string,DialogStageData> datas;
    }

}


