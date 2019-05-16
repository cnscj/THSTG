/*************************
 * 
 * 文件操作类
 * 
 * 
 **************************/
using System.Collections;

using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace THGame
{
	public static class XFileTools
	{

		public static bool Exists(string path)
		{
			if (string.IsNullOrEmpty(path))
				return false;
			return File.Exists(path);
		}

		public static string GetFileText(string fullPath)
		{
			return File.ReadAllText(fullPath);
		}

		public static void Move(string srcPath, string destPath, bool isMoveMeta = true)
		{
			if (Exists(srcPath))
			{
				File.Move(srcPath, destPath);

				// 移动meta文件，如果存在
				if (isMoveMeta)
				{
					string metaPath = srcPath + ".meta";
					if (Exists(metaPath))
					{
						string destMetaPath = destPath + ".meta";
						XFileTools.Move(metaPath, destMetaPath);
					}
				}
			}
		}

		public static void MoveEx(string srcPath, string destPath)
		{
			if (Exists(srcPath))
			{
				if (Exists(destPath))
				{
					File.Delete(destPath);
				}
				File.Move(srcPath, destPath);
			}
		}

		public static void Delete(string path)
		{
			if (Exists(path))
				File.Delete(path);
		}

		public static void Copy(string srcPath, string dstPath, bool overwrite = false)
		{
			if (Exists(srcPath))
			{
				File.Copy(srcPath, dstPath, overwrite);
			}
		}

		//没用
		public static string GetFileFullPath(string assetPath)
		{
			if (string.IsNullOrEmpty(assetPath))
				return null;
			int length = "Assets".Length;
			return Application.dataPath + assetPath.Substring(length);
		}

		// get相对路径
		public static string GetFileRelativePath(string fullPath)
		{
			if (string.IsNullOrEmpty(fullPath))
				return null;
			int pathLength = Application.dataPath.Length - "Assets".Length;
			return fullPath.Substring(pathLength);
		}

		//没用
		public static string GetFolderPath(string filePath)
		{
			string folder = "";
			if (Exists(filePath))
			{
				string standPath = filePath.ToLower().Replace('\\', '/');
				string fileName = standPath.Substring(standPath.LastIndexOf('/'));
				folder = standPath.Replace(fileName, "");
			}
			return folder;
		}

		public static string GetMD5(string fileName)
		{
			try
			{
				FileStream file = new FileStream(fileName, FileMode.Open);
				MD5 md5 = new MD5CryptoServiceProvider();
				byte[] retVal = md5.ComputeHash(file);
				file.Close();

				StringBuilder sb = new StringBuilder();
				for (int i = 0; i < retVal.Length; i++)
				{
					sb.Append(retVal[i].ToString("x2"));
				}
				return sb.ToString();
			}
			catch (System.Exception ex)
			{
				Debug.Log(ex);
				return "";
			}
		}
	}

}