
using System.Collections.Generic;
using ASGame;
using STGU3D;
using UnityEngine;
using XLibrary.Package;

namespace STGService
{
    //场景管理器
    public class UIManager : MonoSingleton<UIManager>
    {
        [System.Serializable]
        public class PackageSettingInfo
        {
            public string packageName;
            public float residentTimeS = -1;
            public bool isPlayLoad;
        }

        [System.Serializable]
        public class ViewSettingInfo
        {
            public string viewName;

            public bool isResident;      //常驻View
            public bool isPerpetual;     //不可Close
            public bool isPlayLoad;
        }


        [Header("Package设置")]
        public float residentTimeS = 15;
        public List<PackageSettingInfo> packageList = new List<PackageSettingInfo>();   
        

        [Header("View设置")]
        public List<ViewSettingInfo> viewList = new List<ViewSettingInfo>();
      
    }
}
