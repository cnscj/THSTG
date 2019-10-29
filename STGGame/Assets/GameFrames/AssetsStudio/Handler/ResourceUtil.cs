using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace ASEditor
{
	public static class ResourceUtil
	{
		//全局常量
		public static readonly string[] textureFileSuffixs = { "tga", "png", "jpg" };                           //常用图像文件后缀

		/// <summary>
		/// 判断是否为图片文件
		/// </summary>
		/// <param name="assetPath">文件路径</param>
		/// <returns>是否</returns>
		public static bool IsImageFile(string assetPath)
		{
			string fileExt = Path.GetExtension(assetPath);
			foreach (var imgExt in textureFileSuffixs)
			{
				if (fileExt.Contains(imgExt))
				{
					return true;
				}
			}
			return false;
		}

		public static string GetFolderId(string assetPath)
		{
			assetPath = assetPath.Replace("\\", "/");
			string fileName = Path.GetFileNameWithoutExtension(assetPath);
			int indexOf_ = fileName.IndexOf('_');
			string modelId = (indexOf_ == -1) ? fileName : fileName.Remove(indexOf_);
			int iModelId;
			return !int.TryParse(modelId, out iModelId) ? "" : modelId;
		}

        /// <summary>
        /// 取得关联的依赖文件路径,包括自己
        /// </summary>
        /// <param name="filePath">资源路径</param>
        /// <param name="excludeEx">排除文件</param>
        /// <returns></returns>
        public static string[] GetDependFiles(string filePath, string[] excludeEx = null)
        {
            Dictionary<string, bool> excludeMap = null;
            if (excludeEx != null)
            {
                excludeMap = new Dictionary<string, bool>();
                foreach (var ex in excludeEx)
                {
                    string exLower = ex.ToLower();
                    if (!excludeMap.ContainsKey(exLower))
                    {
                        excludeMap.Add(exLower, true);
                    }
                }

            }

            List<string> filesPath = new List<string>();
            string[] oriDepends = AssetDatabase.GetDependencies(filePath, true);
            foreach (var path in oriDepends)
            {
                string extension = Path.GetExtension(path).ToLower();

                if (excludeMap != null && excludeMap.ContainsKey(extension))
                {
                    continue;
                }

                filesPath.Add(path);
            }
            return filesPath.ToArray();
        }
    }
}
