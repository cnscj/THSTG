
using UnityEngine;

namespace ASEditor
{
    public class EffectConfig : BaseResourceConfig<EffectConfig>
    {
        static string assetPath = ChangeAssetPath(string.Format("Assets/Resources/ASEffectConfig.asset"));

        public Shader defaultShader;

        private void OnDisable()
        {
            defaultShader = (defaultShader != null) ? defaultShader : Shader.Find("Standard");
        }

    }
}