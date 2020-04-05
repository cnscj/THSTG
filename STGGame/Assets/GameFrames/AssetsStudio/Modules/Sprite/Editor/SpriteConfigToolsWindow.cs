using UnityEditor;
using XLibEditor;

namespace ASEditor
{
    public class SpriteToolsConfigWindow : BaseResourceConfigWindow<SpriteToolsConfigWindow>
    {

        [MenuItem("AssetsStudio/资源工具/资源配置/精灵配置")]
        static void ShowWnd()
        {
            ShowWindow("精灵配置");
        }

        protected override void OnInit()
        {
            AddObject(SpriteToolsConfig.GetInstance());

            AddProperty("defaultShader", "默认Shader");
            AddProperty("defaultFrameRate","默认动画帧率");
            AddProperty("defaultStateList", "默认动作列表");
            AddProperty("loopStateList", "循环动作列表");
        }
        
    }
}
