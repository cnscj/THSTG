
using System.Collections.Generic;
using UnityEngine;

namespace ASEditor
{
    public class AssembliesToolsConfig : BaseResourceConfig<AssembliesToolsConfig>
    {
        static string assetPath = ChangeAssetPath(string.Format("Assets/Resources/ASAssembliesToolsConfig.asset"));

        [SerializeField] public List<AssembliesBuildInfos> buildPathList = new List<AssembliesBuildInfos>();
    }
}