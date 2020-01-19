/*************************
 * 
 * 字符串操作类
 * 
 **************************/
using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace XLibrary
{
	public static class XStringTools
	{

		/// <summary>
		/// 切分文件名，文件后缀
		/// </summary>
		/// <param name="path">全路径</param>
		/// <param name="fileName">文件名</param>
		/// <param name="ext">后缀名</param>
		/// <param name="trimExt">去除后缀文件名</param>
		public static void SplitFileName(string path, out string fileName, out string ext, out string trimExt)
		{
			fileName = "";
			ext = "";
			trimExt = "";
			int refSlash = path.LastIndexOf('/');
			if (refSlash >= 0)
				fileName = path.Substring(refSlash + 1).ToLower();
			int refPoint = path.LastIndexOf('.');
			if (refPoint >= 0)
				ext = path.Substring(refPoint + 1).ToLower();
			if (!string.IsNullOrEmpty(fileName))
				trimExt = fileName.Replace("." + ext, "");
		}

		public static string StringToMD5(string str)
		{
			if (string.IsNullOrEmpty(str))
				return "";

            return ToMD5(Encoding.UTF8.GetBytes(str));
		}

		public static string FileToMd5(string filePath)
		{
			if (string.IsNullOrEmpty(filePath))
				return "";

            FileStream file = new FileStream(filePath, FileMode.Open);
            byte[] bytes = new byte[file.Length];
            file.Read(bytes, 0, bytes.Length);
            file.Close();
            return ToMD5(bytes);
        }

		public static string ToMD5(byte[] data)
		{
            MD5 md5 = MD5.Create();
            data = md5.ComputeHash(data);

            StringBuilder sBuilder = new StringBuilder();
			for (int i = 0; i < data.Length; ++i)
			{
				sBuilder.Append(data[i].ToString("x2"));
			}

			return sBuilder.ToString();
		}


        /// <summary>
        /// 提取字符串包含的Key
        /// </summary>
        /// <param name="path">文件路径/文件名</param>
        /// <returns></returns>
        public static string SplitPathKey(string path)
        {
            string fileName = Path.GetFileNameWithoutExtension(path);
            int indexOf_ = fileName.IndexOf('_');
            string pathKey = (indexOf_ == -1) ? path : fileName.Remove(indexOf_);

            return pathKey;
        }

        /// <summary>
        /// 提取字符串包含的ID
        /// </summary>
        /// <param name="path">文件路径/文件名</param>
        /// <returns></returns>
        public static long SplitPathId(string path)
		{
            string fileName = Path.GetFileNameWithoutExtension(path);
            int indexOf_ = fileName.IndexOf('_');
            string pathId = (indexOf_ == -1) ? fileName : fileName.Remove(indexOf_);
            long iPathId;
            return !long.TryParse(pathId, out iPathId) ? -1 : iPathId;
        }


        /// <summary>
        /// 提取字符串包含的模块名
        /// </summary>
        /// <param name="path">文件路径/文件名</param>
        /// <returns></returns>
        public static string SplitPathModule(string path)
        {
            string fileName = Path.GetFileNameWithoutExtension(path);
            int indexOf_ = fileName.IndexOf('_');
            string pathModule = (indexOf_ == -1) ? fileName : fileName.Remove(indexOf_);
            
            return pathModule;
        }

        /// <summary>
        /// 把内容弄到剪贴板
        /// </summary>
        /// <param name="input"></param>
        public static void CopyToClipboard(string input)
        {
#if UNITY_EDITOR
            TextEditor t = new TextEditor();
            t.text = input;
            t.OnFocus();
            t.Copy();
#elif UNITY_IPHONE
        CopyTextToClipboard_iOS(input);  
#elif UNITY_ANDROID
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaClass tool = new AndroidJavaClass("com.my.ugcf.Tool");
        tool.CallStatic("CopyTextToClipboard", currentActivity, input);
#endif
        }
    }
}