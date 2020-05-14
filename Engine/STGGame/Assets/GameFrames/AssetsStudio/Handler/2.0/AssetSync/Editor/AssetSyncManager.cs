using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLibrary.Package;

namespace ASEditor
{
    //资源同步:一般会有多个版本的资源,需要对应到客户端该版本所使用的资源
    //要考虑资源冻结(一般是开除版本后,不在自动同步,如需自动同步,需要手动添加某些文件到分支版本,0kb文件
    public class AssetSyncManager : Singleton<AssetSyncManager>
    {

    }

}

