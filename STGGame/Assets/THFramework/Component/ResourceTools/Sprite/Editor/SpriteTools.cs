using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
namespace THEditor
{
    public static class SpriteTools
    {
        public static TextureImporter LoadImporterFromTextureFile(string assetPath)
        {
            TextureImporter textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (!textureImporter)
            {
                Debug.LogError(string.Format("{0}不是一个标准的FBX文件", Path.GetFileName(assetPath)));
                return null;
            }

            return textureImporter;
        }


        /// <summary>
        /// 生成动画及控制器
        /// sprite子项命名规则:注解_文件名_编号,没有编号的一律不生成Anima
        /// </summary>
        /// <param name="assetPath"></param>
        public static void GenerateAnimationClipFromTextureFile(string assetPath)
        {
            TextureImporter importer = LoadImporterFromTextureFile(assetPath);
            if (importer)
            {
                //获取所有精灵帧
                var sheetDatas = importer.spritesheet;
                Dictionary<string, SortedList<string, SpriteMetaData>> dict = new Dictionary<string, SortedList<string, SpriteMetaData>>();
                foreach (var data in sheetDatas)
                {
                    string sheetName = data.name;

                    string noteStr = "";
                    string outNameStr = "";
                    string idStr = "";

                    bool ret = GetSheetNameInfo(sheetName, ref noteStr, ref outNameStr, ref idStr);
                    if (ret)
                    {
                        string fileName = noteStr + "_" + outNameStr;
                        SortedList<string, SpriteMetaData> sortlist = null;
                        if (dict.ContainsKey(fileName))
                        {
                            sortlist = dict[fileName];
                        }
                        else
                        {
                            sortlist = new SortedList<string, SpriteMetaData>();
                            dict.Add(fileName, sortlist);
                        }
                        sortlist.Add(idStr, data);

                    }
                }
                foreach(var sheetPair in dict)
                {
                    //TODO:????
                    //AnimationClip clip = new AnimationClip();
                    foreach (var framePair in sheetPair.Value)
                    {
                        Debug.Log(string.Format("{0}:{1}", sheetPair.Key, framePair.Key));
                        //clip.SampleAnimation(sheetFrame, 12);
                    }
                }
            }
        }
        ////////
        private static bool GetSheetNameInfo(string ori, ref string note, ref string name, ref string idStr)
        {
            string[] segStrs = ori.Split(new char[] { '_' });
            if (segStrs.Length < 3)
                return false;

            note = segStrs[0];
            //
            StringBuilder stringbuilder = new StringBuilder();
            for (int i = 1; i < segStrs.Length - 1; i++)
            {
                stringbuilder.Append(segStrs[i]);
                stringbuilder.Append("_");
            }
            stringbuilder.Remove(stringbuilder.Length - 1,1);
            name = stringbuilder.ToString();
            //
            int id = -1;
            idStr = segStrs[segStrs.Length - 1];
            bool ret = int.TryParse(idStr, out id);
            idStr = ret ? idStr : "";

            if (!ret)
                return false;

            return true;
        }

        ////////
        public static void SetupSpriteSheetFromJsonFile(string assetPath)
        {

        }
    }

}
