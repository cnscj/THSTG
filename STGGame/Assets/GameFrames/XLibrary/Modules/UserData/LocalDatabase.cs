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
    public static class LocalDatabase
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
            catch(Exception e)
            {
                Debug.Log(e);
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
                Debug.Log(e);
                return default;
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
            catch(Exception e)
            {
                Debug.Log(e);
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
            catch(Exception e)
            {
                Debug.Log(e);
                return default;
            }
        }

        //
        /// <summary>
        /// 储存Bool
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public static void SetBool(string key, bool value)
        {
            PlayerPrefs.SetString(key + "Bool", value.ToString());
        }

        /// <summary>
        /// 取Bool
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static bool GetBool(string key, bool defaulValue = false)
        {
            try
            {
                return bool.Parse(PlayerPrefs.GetString(key + "Bool"));
            }
            catch (Exception)
            {
                return defaulValue;
            }

        }


        /// <summary>
        /// 储存String
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public static void SetString(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
        }

        /// <summary>
        /// 取String
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static string GetString(string key, string defaultValue = "")
        {
            return PlayerPrefs.GetString(key, defaultValue);
        }

        /// <summary>
        /// 储存Float
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public static void SetFloat(string key, float value)
        {
            PlayerPrefs.SetFloat(key, value);
        }

        /// <summary>
        /// 取Float
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static float GetFloat(string key,float defaultValue = 0f)
        {
            return PlayerPrefs.GetFloat(key, defaultValue);
        }

        /// <summary>
        /// 储存Int
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public static void SetInt(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
        }


        /// <summary>
        /// 取Int
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static int GetInt(string key, int defaultValue = 0)
        {
            return PlayerPrefs.GetInt(key, defaultValue);
        }



        /// <summary>
        /// 储存IntArray
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public static void SetIntArray(string key, int[] value)
        {

            for (int i = 0; i < value.Length; i++)
            {
                PlayerPrefs.SetInt(key + "IntArray" + i, value[i]);
            }
            PlayerPrefs.SetInt(key + "IntArray", value.Length);
        }

        /// <summary>
        /// 取IntArray
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static int[] GetIntArray(string key)
        {
            int[] intArr = new int[1];
            if (PlayerPrefs.GetInt(key + "IntArray") != 0)
            {
                intArr = new int[PlayerPrefs.GetInt(key + "IntArray")];
                for (int i = 0; i < intArr.Length; i++)
                {
                    intArr[i] = PlayerPrefs.GetInt(key + "IntArray" + i);
                }
            }
            return intArr;
        }

        /// <summary>
        /// 储存FloatArray
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public static void SetFloatArray(string key, float[] value)
        {

            for (int i = 0; i < value.Length; i++)
            {
                PlayerPrefs.SetFloat(key + "FloatArray" + i, value[i]);
            }
            PlayerPrefs.SetInt(key + "FloatArray", value.Length);
        }

        /// <summary>
        /// 取FloatArray
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static float[] GetFloatArray(string key)
        {
            float[] floatArr = new float[1];
            if (PlayerPrefs.GetInt(key + "FloatArray") != 0)
            {
                floatArr = new float[PlayerPrefs.GetInt(key + "FloatArray")];
                for (int i = 0; i < floatArr.Length; i++)
                {
                    floatArr[i] = PlayerPrefs.GetFloat(key + "FloatArray" + i);
                }
            }
            return floatArr;
        }


        /// <summary>
        /// 储存StringArray
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public static void SetStringArray(string key, string[] value)
        {

            for (int i = 0; i < value.Length; i++)
            {
                PlayerPrefs.SetString(key + "StringArray" + i, value[i]);
            }
            PlayerPrefs.SetInt(key + "StringArray", value.Length);
        }

        /// <summary>
        /// 取StringArray
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static string[] GetStringArray(string key)
        {
            string[] stringArr = new string[1];
            if (PlayerPrefs.GetInt(key + "StringArray") != 0)
            {
                stringArr = new string[PlayerPrefs.GetInt(key + "StringArray")];
                for (int i = 0; i < stringArr.Length; i++)
                {
                    stringArr[i] = PlayerPrefs.GetString(key + "StringArray" + i);
                }
            }
            return stringArr;
        }


        /// <summary>
        /// 储存Vector2
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public static void SetVector2(string key, Vector2 value)
        {
            PlayerPrefs.SetFloat(key + "Vector2X", value.x);
            PlayerPrefs.SetFloat(key + "Vector2Y", value.y);

        }

        /// <summary>
        /// 取Vector2
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static Vector2 GetVector2(string key)
        {
            Vector2 Vector2;
            Vector2.x = PlayerPrefs.GetFloat(key + "Vector2X");
            Vector2.y = PlayerPrefs.GetFloat(key + "Vector2Y");
            return Vector2;
        }


        /// <summary>
        /// 储存Vector3
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public static void SetVector3(string key, Vector3 value)
        {
            PlayerPrefs.SetFloat(key + "Vector3X", value.x);
            PlayerPrefs.SetFloat(key + "Vector3Y", value.y);
            PlayerPrefs.SetFloat(key + "Vector3Z", value.z);
        }

        /// <summary>
        /// 取Vector3
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static Vector3 GetVector3(string key)
        {
            Vector3 vector3;
            vector3.x = PlayerPrefs.GetFloat(key + "Vector3X");
            vector3.y = PlayerPrefs.GetFloat(key + "Vector3Y");
            vector3.z = PlayerPrefs.GetFloat(key + "Vector3Z");
            return vector3;
        }


        /// <summary>
        /// 储存Quaternion
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public static void SetQuaternion(string key, Quaternion value)
        {
            PlayerPrefs.SetFloat(key + "QuaternionX", value.x);
            PlayerPrefs.SetFloat(key + "QuaternionY", value.y);
            PlayerPrefs.SetFloat(key + "QuaternionZ", value.z);
            PlayerPrefs.SetFloat(key + "QuaternionW", value.w);
        }

        /// <summary>
        /// 取Quaternion
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static Quaternion GetQuaternion(string key)
        {
            Quaternion quaternion;
            quaternion.x = PlayerPrefs.GetFloat(key + "QuaternionX");
            quaternion.y = PlayerPrefs.GetFloat(key + "QuaternionY");
            quaternion.z = PlayerPrefs.GetFloat(key + "QuaternionZ");
            quaternion.w = PlayerPrefs.GetFloat(key + "QuaternionW");
            return quaternion;
        }
    }
}
