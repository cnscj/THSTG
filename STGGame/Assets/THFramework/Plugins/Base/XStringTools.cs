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

namespace THGame
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

		public static string GetAssetBundleExtName(string ext)
		{
			if (ext.Equals("jpg") || ext.Equals("png"))
			{
				return "_tex";
			}
			else if (ext.Equals("mat"))
			{
				return "_mat";
			}
			else if (ext.Equals("anim"))
			{
				return "_ani";
			}
			else if (ext.Equals("font"))
			{
				return "_fnt";
			}
			else
				return null;
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
			MD5 md5 = new MD5CryptoServiceProvider();
			byte[] retVal = md5.ComputeHash(file);
			file.Close();
			return ToMD5(retVal);
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
		/// 提取字符串包含的ID
		/// </summary>
		/// <param name="path">文件路径/文件名</param>
		/// <returns></returns>
		public static string SplitPathId(string path)
		{
			string fileId = "";
			int startPos = path.LastIndexOf("/", System.StringComparison.Ordinal);
			if (startPos != -1) //如果是路径,则取文件名
			{
				path = path.Substring(startPos + 1);
			}
			startPos = path.IndexOf("_", System.StringComparison.Ordinal);
			if (startPos != -1) 
			{
				fileId = path.Substring(0, startPos);//取得编号
			}
			
			return fileId;
		}

	}
}