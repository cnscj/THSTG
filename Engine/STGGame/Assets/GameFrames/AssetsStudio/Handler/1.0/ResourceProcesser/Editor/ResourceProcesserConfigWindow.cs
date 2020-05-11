using UnityEditor;

namespace ASEditor
{
    public class ResourceProcesserConfigWindow : BaseResourceConfigWindow<ResourceProcesserConfigWindow>
    {
        [MenuItem("AssetsStudio/资源后处理配置",false,2)]
        static void ShowWnd()
        {
            ShowWindow("资源后处理配置");
        }

        protected override void OnInit()
        {
            AddObject(ResourceProcesserConfig.GetInstance());
            AddProperty("isUseADL", "是否动画按需加载", "Normal");

            AddProperty("effectisUseNodeLevel", "是否使用特效节点分级", "Effect");
            AddProperty("effectIsUseCalculateFxLength", "是否计算特效的时长", "Effect");

        }

        protected override void OnShow()
        {
            ShowPropertys("Normal");
            ShowPropertys("Effect");
        }
    }
}
