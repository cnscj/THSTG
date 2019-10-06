using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using XLibrary;

namespace ASEditor
{
    public class ExcelMenu
    {
        [MenuItem("AssetsStudio/资源工具/Excel2Unity")]
        public static void ShowExcelTools()
        {
            ExcelWindow.ShowExcelTools();
        }
    }
}
