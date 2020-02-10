/*************************
 * 
 * 文件夹操作类
 * 
 * 
 **************************/
using System;
using System.IO;

namespace XLibrary
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
        public static void TraverseFiles(string[] dirs, Action<string> callBack, bool isTraverse = false, bool isLower = false)
        {
            if (dirs == null || callBack == null)
            {
                return;
            }

            foreach (var dir in dirs)
            {
                if (!Exists(dir))
                {
                    continue;
                }

                var fileList = isTraverse ? Directory.EnumerateFiles(dir, "*", SearchOption.AllDirectories) :
                                        Directory.EnumerateFiles(dir, "*", SearchOption.TopDirectoryOnly);

                foreach (string path in fileList)
                {
                    string fullPath = path.Replace('\\', '/');
                    if (isLower)
                    {
                        fullPath = fullPath.ToLower();
                    }
                    callBack(fullPath);
                }
            }
        }
        public static void TraverseFiles(string dir, Action<string> callBack, bool isTraverse = false, bool isLower = false)
        {
            TraverseFiles(new string[] { dir }, callBack, isTraverse, isLower);
        }

        /// <summary>
        /// 遍历文件夹
        /// </summary>
        /// <param name="dir">文件夹路径</param>
        /// <param name="callBack">找到文件夹回调</param>
        /// <param name="isTraverse">是否递归遍历</param>
        public static void TraverseFolder(string[] dirs, Action<string> callBack, bool isTraverse = false, bool isLower = false)
        {
            if (dirs == null || callBack == null)
            {
                return;
            }

            foreach (var dir in dirs)
            {
                if (!Exists(dir))
                {
                    continue;
                }
                var folderList = isTraverse ? Directory.EnumerateDirectories(dir, "*", SearchOption.AllDirectories) :
                                        Directory.EnumerateDirectories(dir, "*", SearchOption.TopDirectoryOnly);

                foreach (string path in folderList)
                {
                    string fullPath = path.Replace('\\', '/');
                    if (isLower)
                    {
                        fullPath = fullPath.ToLower();
                    }
                    callBack(fullPath);
                }
            }
        }
        public static void TraverseFolder(string dir, Action<string> callBack, bool isTraverse = false, bool isLower = false)
        {
            TraverseFolder(new string[] { dir }, callBack, isTraverse, isLower);
        }

        [Obsolete]
        public static void TraverseFilesEx(string[] dirs, Action<string> callBack, bool isTraverse = false)
        {
            if (dirs == null || callBack == null)
                return;

            foreach (var dir in dirs)
            {
                if (!Exists(dir))
                    continue;

                DirectoryInfo folder = new DirectoryInfo(dir);
                FileSystemInfo[] files = folder.GetFileSystemInfos();
                for (int i = 0, length = files.Length; i < length; i++)
                {
                    var file = files[i];
                    if (file is DirectoryInfo)
                    {
                        if (isTraverse)
                            TraverseFilesEx(file.FullName, callBack, isTraverse);
                    }
                    else
                    {
                        callBack(file.FullName.Replace('\\', '/'));
                    }
                }
            }
        }

        [Obsolete]
        public static void TraverseFilesEx(string dir, Action<string> callBack, bool isTraverse = false)
        {
            TraverseFilesEx(new string[] { dir }, callBack, isTraverse);
        }

        [Obsolete]
        public static void TraverseFolderEx(string[] dirs, Action<string> callBack, bool isTraverse = false)
        {
            if (dirs == null || callBack == null)
                return;

            foreach (var dir in dirs)
            {
                if (!Exists(dir))
                    continue;

                DirectoryInfo folder = new DirectoryInfo(dir);
                FileSystemInfo[] files = folder.GetFileSystemInfos();
                for (int i = 0, length = files.Length; i < length; i++)
                {
                    var file = files[i];
                    if (file is DirectoryInfo)
                    {
                        if (isTraverse)
                            TraverseFolderEx(file.FullName, callBack, isTraverse);
                        callBack(file.FullName.Replace('\\', '/').ToLower());
                    }
                }
            }
        }

        [Obsolete]
        public static void TraverseFolderEx(string dir, Action<string> callBack, bool isTraverse = false)
        {
            TraverseFolderEx(new string[] { dir }, callBack, isTraverse);
        }

    }
}
