using System.Collections;
using System.Collections.Generic;
using THGame;
using UnityEditor;
using UnityEngine;

namespace THEditor
{
    public class EffectConfig : BaseResourceConfig<EffectConfig>
    {
        static string assetPath = ChangeAssetPath(string.Format("Assets/Resources/THEffectConfig.asset"));

        public Shader defaultShader;

        private void OnDisable()
        {
            defaultShader = (defaultShader != null) ? defaultShader : Shader.Find("Standard");
        }

    }
}