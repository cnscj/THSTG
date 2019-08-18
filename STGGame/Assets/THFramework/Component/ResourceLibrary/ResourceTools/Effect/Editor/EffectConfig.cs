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


        void OnEnable()
        {
            defaultShader = defaultShader ? defaultShader : Shader.Find("Standard");
        }

    }
}