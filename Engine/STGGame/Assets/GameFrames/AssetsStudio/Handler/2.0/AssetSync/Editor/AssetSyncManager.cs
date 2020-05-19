using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLibrary.Package;

namespace ASEditor
{
    //资源同步:一般会有多个版本的资源,需要对应到客户端该版本所使用的资源
    //资源冻结:一般是开出版本后,不在自动同步(可通过移除同步文件实现),如需自动同步,需要手动添加某些文件到分支版本,0kb文件
    public class AssetSyncManager : Singleton<AssetSyncManager>
    {

    }

}

