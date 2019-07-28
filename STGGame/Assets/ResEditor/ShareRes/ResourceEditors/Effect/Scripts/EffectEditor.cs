using System.Collections;
using System.Collections.Generic;
using THGame.Package;
using UnityEngine;

namespace STGGame
{
    public class EffectEditor : MonoSingleton<ModelFxEditor>
    {
        public GameObject skillFxNode;
        public GameObject modelFxNode;
        public GameObject publicFxNode;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
        private void OnValidate()
        {
            skillFxNode = GameObject.Find("SkillFx");
            modelFxNode = GameObject.Find("ModelFx");
            publicFxNode = GameObject.Find("PublicFx");

        }
    }

}
