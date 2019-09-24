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
        protected override void OnObjs()
        {
            AddObject(ResourceProcesserConfig.GetInstance());
        }

        protected override void OnProps()
        {

            AddProperty("isUseADL", "Normal", "是否动画按需加载");

            AddProperty("effectisUseNodeLevel", "Effect", "是否使用特效节点分级");
            AddProperty("effectIsUseCalculateFxLength", "Effect", "是否计算特效的时长");

        }

        protected override void OnShow()
        {
            ShowPropertys("Normal");
            ShowPropertys("Effect");
        }
    }
}
