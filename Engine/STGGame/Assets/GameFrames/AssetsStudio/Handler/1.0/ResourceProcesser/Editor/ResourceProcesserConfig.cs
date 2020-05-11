
using UnityEngine;

namespace ASEditor
{
    public class ResourceProcesserConfig : BaseResourceConfig<ResourceProcesserConfig>
    {
        static string assetPath = ChangeAssetPath(string.Format("Assets/Resources/ASResourceProcesserConfig.asset"));


        public bool isUseADL = true;

        public bool effectisUseNodeLevel = true;
        public bool effectIsUseCalculateFxLength = true;


    }
}