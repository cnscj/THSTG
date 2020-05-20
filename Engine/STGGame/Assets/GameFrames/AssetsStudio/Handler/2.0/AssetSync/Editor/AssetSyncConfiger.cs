using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLibEditor;

namespace ASEditor
{
    public class AssetSyncConfiger : ConfigObject<AssetSyncConfiger>
    {

        public string repositoryRootPath;           //版本库路径
        public string checkRootPath;                //检查路径(相对路径
    }
}
