using UnityEditor;
using XLibEditor;

namespace ASEditor
{
    public class ModelToolsConfigWindow : BaseResourceConfigWindow<ModelToolsConfigWindow>
    {
        [MenuItem("AssetsStudio/资源工具/资源配置/模型配置")]
        static void ShowWnd()
        {
            ShowWindow("模型配置");
        }
        protected override void OnInit()
        {
            AddObject(ModelToolsConfig.GetInstance());

            AddProperty("defaultShader",  "默认Shader");
            AddProperty("defaultStateList", "默认动作列表");
            AddProperty("loopStateList", "循环动作列表");
        }

    }
}
