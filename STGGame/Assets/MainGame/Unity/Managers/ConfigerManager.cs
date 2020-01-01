
using System.IO;
using UnityEngine;
using XLibrary.Package;

namespace STGU3D
{
    public class ConfigerManager : MonoSingleton<ConfigerManager>
    {
        public string LoadConfig(string code)
        {
            return AssetManager.GetInstance().LoadConfig(code);
        }
    }
}
