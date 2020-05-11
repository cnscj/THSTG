using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASGame
{
    public class ChangeTexture : MonoBehaviour
    {
        [System.Serializable]
        public class TextureInfo
        {
            public string name;
            public Texture[] main;
            public Texture flow;
        }

        public Material[] materials;
        //TODO:
    }


}
