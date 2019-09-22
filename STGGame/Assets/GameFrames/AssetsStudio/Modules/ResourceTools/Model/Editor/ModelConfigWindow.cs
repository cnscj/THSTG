using UnityEditor;
using XLibraryEditor;

namespace ASEditor
{
    public class ModelConfigWindow : BaseResourceConfigWindow<ModelConfigWindow>
    {
        [MenuItem("AssetsStudio/资源工具/资源配置/模型配置")]
        static void ShowWnd()
        {
            ShowWindow("模型配置");
        }
        protected override void OnObjs()
        {
            AddObject(ModelConfig.GetInstance());
        }

        protected override void OnProps()
        {
            AddProperty("defaultShader", "Normal", "默认Shader");
            AddProperty("defaultStateList", "Normal", "默认动作列表");
            AddProperty("loopStateList", "Normal", "循环动作列表");
        }

        protected override void OnShow()
        {
            ShowPropertys("Normal");
        }
    }
}
