using UnityEditor;

namespace ASEditor
{
    public class EffectToolsConfigWindow : BaseResourceConfigWindow<EffectToolsConfigWindow>
    {
        [MenuItem("AssetsStudio/资源工具/资源配置/特效配置")]
        static void ShowWnd()
        {
            ShowWindow("特效配置");
        }
        protected override void OnObjs()
        {
            AddObject(EffectToolsConfig.GetInstance());
        }

        protected override void OnProps()
        {
            AddProperty("defaultShader", "Normal", "默认Shader");
        }

        protected override void OnShow()
        {
            ShowPropertys("Normal");
        }
    }
}
