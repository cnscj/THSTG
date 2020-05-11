using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;
using Object = System.Object;
namespace XLibGame
{
    /// <summary>
    /// 本地数据管理类 
    /// </summary>
    public static class JsonPers
    {
        public static readonly string SAVES_NAME = "Saves";

        //取得用户可写目录
        private static string GetUserFolder()
        {
            return Path.Combine(Application.persistentDataPath, SAVES_NAME);
        }

        private static bool TryCreateFoler(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return false;

            try
            {
                string folderPath = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                return true;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 保存用户数据到Json
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool SaveJson(string fileName, Object obj)
        {
            if (obj == null) return false;
            string filePath = Path.Combine(GetUserFolder(), fileName);
            try
            {
                string content = JsonUtility.ToJson(obj, true);
                if (TryCreateFoler(filePath))
                {
                    File.WriteAllText(filePath, content, Encoding.UTF8);
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log(e);
                return false;
            }

        }

        /// <summary>
        /// 加载用户数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static T LoadJson<T>(string fileName) where T : class
        {
            string filePath = Path.Combine(GetUserFolder(), fileName);
            try
            {
                string content = File.ReadAllText(filePath, Encoding.UTF8);
                return JsonUtility.FromJson<T>(content);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log(e);
                return default;
            }

        }
    }
}
