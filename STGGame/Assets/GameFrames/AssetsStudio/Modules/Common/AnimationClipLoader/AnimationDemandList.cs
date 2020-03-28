using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace ASGame
{
    public class AnimationDemandList : ScriptableObject
    {
        public string abPath;
        public string animCtrlPath;
        public string otherInfo;
        public List<string> animsList = new List<string>();

    }
 
}
