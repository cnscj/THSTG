using System.Collections;
using System.Collections.Generic;
using THGame.Package;
using UnityEngine;

namespace STGGame
{
    public class PublicFxEditor : MonoSingleton<PublicFxEditor>
    {
        public GameObject effectsNode = null;
        
        void Start()
        {
            effectsNode = gameObject.transform.Find("Effects").gameObject;
        }
        private void OnValidate()
        {
            effectsNode = gameObject.transform.Find("Effects").gameObject;
        }
    }

}
