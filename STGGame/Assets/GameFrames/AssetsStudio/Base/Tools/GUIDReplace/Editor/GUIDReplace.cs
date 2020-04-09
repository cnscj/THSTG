using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace ASEditor
{
    public static class GUIDReplace
    {
        public static string[] SERIALIZABLE_FILE_SUFFIX = { ".prefab", ".unity", ".mat", ".asset", ".controller", ".playable" };

        public static void ReplaceGUID(string assetPath, string oldGUID, string newGUID)
        {
            var content = File.ReadAllText(assetPath);
            if (oldGUID != newGUID)
            {
                if (Regex.IsMatch(content, oldGUID))
                {
                    content = content.Replace(oldGUID, newGUID);
                }
            }
            
            File.WriteAllText(assetPath, content);
            AssetDatabase.Refresh();
        }

        public static void ReplaceGUIDs(Dictionary<string,string> guidMap, string []files)
        {
            if (guidMap == null || guidMap.Count <= 0)
                return;

            if (files != null && files.Length > 0)
            {
                foreach (var file in files)
                {
                    var content = File.ReadAllText(file);
                    foreach(var guidPair in guidMap)
                    {
                        var oldGUID = guidPair.Key;
                        var newGUID = guidPair.Value;
                        if (oldGUID != newGUID)
                        {
                            if (Regex.IsMatch(content, oldGUID))
                            {
                                content = content.Replace(oldGUID, newGUID);
                            }
                        }
                    }
                    File.WriteAllText(file, content);
                }
                AssetDatabase.Refresh();
            }
        }
        public static void ReplaceGUIDs(Dictionary<string, string> guidMap, string path, string[] suffix = null)
        {
            suffix = (suffix != null) ? suffix : SERIALIZABLE_FILE_SUFFIX;
            path = !string.IsNullOrEmpty(path) ? path : Application.dataPath;
            string[] files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories).Where(s => suffix.Contains(Path.GetExtension(s).ToLower())).ToArray();
            ReplaceGUIDs(guidMap, files);
        }

        public static void ReplaceGUIDs(string oldGUID, string newGUID, string[] files)
        {
            ReplaceGUIDs(new Dictionary<string, string> { { oldGUID, newGUID } }, files);
        }

        public static void ReplaceGUIDs(string oldGUID, string newGUID, string path, string []suffix = null)
        {
            suffix = (suffix != null) ? suffix : SERIALIZABLE_FILE_SUFFIX;
            path = !string.IsNullOrEmpty(path) ? path : Application.dataPath;

            string[] files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories).Where(s => suffix.Contains(Path.GetExtension(s).ToLower())).ToArray();
            ReplaceGUIDs(new Dictionary<string, string> { { oldGUID, newGUID } }, files);
        }

    }
}
