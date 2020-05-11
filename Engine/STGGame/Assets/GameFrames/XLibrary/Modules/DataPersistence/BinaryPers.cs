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
    public static class BinaryPers
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
        /// 保存用户数据到二进制文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool SaveBinary(string fileName, Object obj)
        {
            if (obj == null) return false;
            string filePath = Path.Combine(GetUserFolder(), fileName);
            try
            {
                if (TryCreateFoler(filePath))
                {
                    FileStream fileStream = new FileStream(filePath, FileMode.Create);
                    BinaryFormatter bf = new BinaryFormatter();

                    bf.Serialize(fileStream, obj);
                    fileStream.Close();
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
        /// 加载二进制文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static T LoadBinary<T>(string fileName) where T : class
        {
            string filePath = Path.Combine(GetUserFolder(), fileName);
            try
            {
                FileStream fileStream = new FileStream(filePath, FileMode.Open);
                BinaryFormatter bf = new BinaryFormatter();

                Object obj = bf.Deserialize(fileStream);
                fileStream.Close();

                return obj as T;
            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log(e);
                return default;
            }
        }

    }
}
