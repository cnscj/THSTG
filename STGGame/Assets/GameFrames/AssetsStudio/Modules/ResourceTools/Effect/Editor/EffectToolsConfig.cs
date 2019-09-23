
using UnityEngine;

namespace ASEditor
{
    public class EffectToolsConfig : BaseResourceConfig<EffectToolsConfig>
    {
        static string assetPath = ChangeAssetPath(string.Format("Assets/Resources/ASEffectToolsConfig.asset"));

        public Shader defaultShader;

        private void OnDisable()
        {
            defaultShader = (defaultShader != null) ? defaultShader : Shader.Find("Standard");
        }

    }
}