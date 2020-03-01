using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
    [System.Serializable]
    public class SoundCommand
    {
        public int id;
        public SoundData data;
        public SoundArgs args;

        public override string ToString()
        {

            //TODO:只检查循环
            return string.Format("{0}|{1}", data?.clip?.GetHashCode(), args?.GetHashCode());
        }
    }


}
