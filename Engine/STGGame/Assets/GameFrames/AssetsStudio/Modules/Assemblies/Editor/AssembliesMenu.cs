using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using XLibrary;

namespace ASEditor
{
    public static class AssembliesMenu
    {
        [MenuItem("AssetsStudio/资源工具/Build Assembly Sync")]
        public static void BuildAssemblySync()
        {
            AssembliesBuilderManager.BuildAssemblySync();
        }

    }

}
