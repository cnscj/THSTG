using System;
using FairyGUI;

namespace THGame.UI
{
    public static class LuaMethodHelper
    {
       
        public static UIPackage LoadPackageInPcCustom(byte[] descBytes, string assetNamePrefix,Func<string, string, System.Type, object> call)
        {
            return UIPackage.AddPackage(descBytes, assetNamePrefix, (string name, string extension, System.Type type, out DestroyMethod destroyMethod) =>
            {
                destroyMethod = DestroyMethod.Unload;
                return call(name, extension, type);
            });
        }
    }

}
