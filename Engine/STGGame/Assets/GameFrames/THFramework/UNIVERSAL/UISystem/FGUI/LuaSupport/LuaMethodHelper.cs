using System;
using System.Collections.Generic;
using System.IO;
using FairyGUI;
using UnityEngine;
using UnityEngine.Networking;

namespace THGame.UI
{
    public static class LuaMethodHelper
    {
        //传assetPath需要修改UIPackage.AddPackage里的TextAsset为byte[]
        public static UIPackage LoadPackageSyncInPcCustom(byte[] descData, string assetNamePrefix, Func<string, string, System.Type, object> call)
        {
            return UIPackage.AddPackage(descData, assetNamePrefix ,(string name, string extension, System.Type type, out DestroyMethod destroyMethod) =>
            {
                destroyMethod = DestroyMethod.None;
                var retObj = call?.Invoke(name, extension, type);
                if (retObj == null)
                {
#if UNITY_EDITOR
                    retObj = UnityEditor.AssetDatabase.LoadAssetAtPath(name + extension, type);
#else
                    retObj = Resources.Load(name, type);
#endif
                    if (retObj != null) destroyMethod = DestroyMethod.Unload;
                    else destroyMethod = DestroyMethod.None;
                }
                return retObj;
            });
        }

        //传assetPath需要修改UIPackage.AddPackage里的TextAsset为byte[]
        public static UIPackage LoadPackageAsyncInPcCustom(byte[] descData, string assetNamePrefix, Action<string, string, System.Type, Action<object>> call)
        {
            return UIPackage.AddPackage(descData, assetNamePrefix, (string name, string extension, System.Type type, PackageItem item) =>
            {
                if (call != null)
                {
                    Action<object> call2 = (retObj) =>
                    {
                        var destroyMethod = DestroyMethod.None;
                        if (retObj == null)
                        {
#if UNITY_EDITOR
                            retObj = UnityEditor.AssetDatabase.LoadAssetAtPath(name + extension, type);
#else
                            retObj = Resources.Load(name, type);
#endif
                            if (retObj != null) destroyMethod = DestroyMethod.Unload;
                            else destroyMethod = DestroyMethod.None;
                        }
                        item.owner.SetItemAsset(item, retObj, destroyMethod);
                    };
                    call?.Invoke(name, extension, type, call2);
                }
            });
        }

        //获取包的依赖
        public static string[] GetPackageDependencies(UIPackage package)
        {
            if (package == null)
                return default;

            int count = package.dependencies != null ? package.dependencies.Length : 0;
            if (count > 0)
            {
                var depList = new string[count];
                int i = 0;
                foreach (var depDict in package.dependencies)
                {
                    depList[i] = depDict["name"];
                    i++;
                }

                return depList;
            }
            
            return default;
        }
    }

}
