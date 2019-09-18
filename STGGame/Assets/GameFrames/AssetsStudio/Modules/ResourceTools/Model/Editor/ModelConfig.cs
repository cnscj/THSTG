using System.Collections.Generic;
using UnityEngine;

namespace ASEditor
{
    public class ModelConfig : BaseResourceConfig<ModelConfig>
    {
        static string assetPath = ChangeAssetPath(string.Format("Assets/Resources/ASModelConfig.asset"));

        [SerializeField] public Shader defaultShader;
        [SerializeField] public List<string> defaultStateList = new List<string>()
        {
            "idle","stand",
        };
        [SerializeField] public List<string> loopStateList = new List<string>()
        {
            "idle","stand",
        };
        Dictionary<string, bool> defaultStateMap = new Dictionary<string, bool>();
        Dictionary<string, bool> loopStateMap = new Dictionary<string, bool>();


        private void OnEnable()
        {
            defaultShader = (defaultShader != null ) ? defaultShader : Shader.Find("Standard");
            foreach (var state in defaultStateList)
            {
                if (!defaultStateMap.ContainsKey(state))
                {
                    defaultStateMap.Add(state, true);
                }
            }

            foreach (var state in loopStateList)
            {
                if (!loopStateMap.ContainsKey(state))
                {
                    loopStateMap.Add(state, true);
                }
            }

        }

        public bool IsDefaultState(string stateName)
        {
            if (defaultStateMap.ContainsKey(stateName))
            {
                return true;
            }
            return false;
        }

        public bool IsNeedLoop(string stateName)
        {
            if (loopStateMap.ContainsKey(stateName))
            {
                return true;
            }
            return false;
        }

    }
}