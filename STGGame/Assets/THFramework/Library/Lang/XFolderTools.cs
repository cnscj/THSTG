/*************************
 * 
 * 文件夹操作类
 * 
 * 
 **************************/
using System;
using System.IO;

namespace THGame
{
	public static class XFolderTools
	{

		public static bool Exists(string path)
		{
			if (string.IsNullOrEmpty(path))
				return false;
			return Directory.Exists(path);
		}

		public static void CreateDirectory(string path)
		{
			if (Exists(path))
				return;
			Directory.CreateDirectory(path);
		}

		public static void DeleteDirectory(string path)
		{
			if (!Exists(path))
				return;
			Directory.Delete(path);
		}

		public static void DeleteDirectory(string path, bool recursive)
		{
			if (!Exists(path))
				return;
			Directory.Delete(path, recursive);
		}

		public static bool CheckNullFolder(string path)
		{
			if (!Exists(path))
				return true;
			DirectoryInfo folder = new DirectoryInfo(path);
			FileSystemInfo[] files = folder.GetFileSystemInfos();

			return files.Length <= 0;
		}

		public static string GetFolderName(string path)
		{
			DirectoryInfo folder = new DirectoryInfo(path);
			return folder.Parent.Name;
		}

		/// <summary>
		/// 遍历文件
		/// </summary>
		/// <param name="dirs">文件夹们的路径</param>
		/// <param name="callBack">找到文件回调</param>
		/// <param name="isTraverse">是否递归遍历</param>
		public static void TraverseFiles(string[] dirs, Action<string> callBack, bool isTraverse = false)
		{
            if (dirs == null)
                return;

            foreach(var dir in dirs)
            {
                if (!Exists(dir) || callBack == null)
                    return;
                DirectoryInfo folder = new DirectoryInfo(dir);
                FileSystemInfo[] files = folder.GetFileSystemInfos();
                for (int i = 0, length = files.Length; i < length; i++)
                {
                    var file = files[i];
                    if (file is DirectoryInfo)
                    {
                        if (isTraverse)
                            TraverseFiles(file.FullName, callBack, isTraverse);
                    }
                    else
                    {
                        callBack(file.FullName.Replace('\\', '/'));
                    }
                }
            }
		}

		public static void TraverseFiles(string dir, Action<string> callBack, bool isTraverse = false)
        {
            TraverseFiles(new string[] { dir }, callBack, isTraverse);
        }



        /// <summary>
        /// 遍历文件夹
        /// </summary>
        /// <param name="dir">文件夹路径</param>
        /// <param name="callBack">找到文件夹回调</param>
        /// <param name="isTraverse">是否递归遍历</param>
        public static void TraverseFolder(string dir, Action<string> callBack, bool isTraverse = false)
		{
			if (!Exists(dir) || callBack == null)
				return;
			DirectoryInfo folder = new DirectoryInfo(dir);
			FileSystemInfo[] files = folder.GetFileSystemInfos();
			for (int i = 0, length = files.Length; i < length; i++)
			{
				var file = files[i];
				if (file is DirectoryInfo)
				{
					if (isTraverse)
						TraverseFolder(file.FullName, callBack, isTraverse);
					callBack(file.FullName.Replace('\\', '/').ToLower());
				}
			}
		}
	}
}
