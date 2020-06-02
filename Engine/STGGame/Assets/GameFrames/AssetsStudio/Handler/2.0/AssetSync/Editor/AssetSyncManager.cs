using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XLibrary;
using XLibrary.Package;

namespace ASEditor
{
    //资源同步:一般会有多个版本的资源,需要对应到客户端该版本所使用的资源
    //资源冻结:一般是开出版本后,不在自动同步(可通过移除同步文件实现),如需自动同步,需要手动添加某些文件到分支版本,0kb文件
    public class AssetSyncManager : Singleton<AssetSyncManager>
    {
        public void Sync()
        {
            var syncItems = AssetSyncConfiger.GetInstance().syncItems;
            if (syncItems == null || syncItems.Count <= 0)
                return;

            //遍历版本库文件夹
            XFolderTools.TraverseFolder(AssetSyncConfiger.GetInstance().GetRepositoryRootPath(), (fullPath) =>
            {
                string repositoryPath = fullPath;
                string versionFolderName = Path.GetFileNameWithoutExtension(fullPath);
                int curVersion = AssetSyncConfiger.GetInstance().GetRepositoryVersion(versionFolderName);
                if (curVersion > 0 && curVersion >= AssetSyncConfiger.GetInstance().minVersion)
                {
                    foreach(var syncItem in syncItems)
                    {
                        string srcPath = syncItem.srcPath;
                        string searchPath = XPathTools.Combine(repositoryPath, syncItem.realSearcePath);
                        string syncPath = XPathTools.Combine(repositoryPath, syncItem.realSyncPath);
                        if (XFolderTools.Exists(srcPath) && XFolderTools.Exists(searchPath) && XFolderTools.Exists(syncPath))
                        {
                            List<string> syncFileList = new List<string>();
                            List<string> syncFolderList = new List<string>();
                            if (syncItem.searchMode == AssetSyncItem.SearchMode.Forward)     //根据搜索文件找资源
                            {
                                string srcEx = GetFolderFirstFileExtension(srcPath);
                                XFolderTools.TraverseFiles(searchPath, (filePath) =>
                                {
                                    string fileKey = null;
                                    if (syncItem.searchKey == AssetSyncItem.SearchKey.AssetName)
                                    {
                                        fileKey = Path.GetFileNameWithoutExtension(filePath);
                                    }
                                    else if (syncItem.searchKey == AssetSyncItem.SearchKey.AssetPrefix)
                                    {
                                        fileKey = XStringTools.SplitPathKey(filePath);
                                    }

                                    string srcFilePath = XPathTools.Combine(srcPath, string.Format("{0}{1}", fileKey, srcEx));
                                    if (XFileTools.Exists(srcFilePath))
                                    {
                                        syncFileList.Add(srcFilePath);
                                    }
                                });

                                XFolderTools.TraverseFolder(searchPath, (folderPath) =>
                                {
                                    string fileKey = null;
                                    if (syncItem.searchKey == AssetSyncItem.SearchKey.AssetName)
                                    {
                                        fileKey = Path.GetFileNameWithoutExtension(folderPath);
                                    }
                                    else if (syncItem.searchKey == AssetSyncItem.SearchKey.AssetPrefix)
                                    {
                                        fileKey = XStringTools.SplitPathKey(folderPath);
                                    }

                                    string srcFolderPath = XPathTools.Combine(srcPath, string.Format("{0}", fileKey));
                                    if (XFolderTools.Exists(srcFolderPath))
                                    {
                                        syncFolderList.Add(srcFolderPath);
                                    }
                                });
                            }
                            else if (syncItem.searchMode == AssetSyncItem.SearchMode.Reverse)   //根据资源匹对文件
                            {
                                XFolderTools.TraverseFiles(srcPath, (filePath) =>
                                {
                                    string fileKey = null;
                                    if (syncItem.searchKey == AssetSyncItem.SearchKey.AssetName)
                                    {
                                        fileKey = Path.GetFileNameWithoutExtension(filePath);
                                    }
                                    else if (syncItem.searchKey == AssetSyncItem.SearchKey.AssetPrefix)
                                    {
                                        fileKey = XStringTools.SplitPathKey(filePath);
                                    }

                                    string searchFilePath = XPathTools.Combine(searchPath, string.Format("{0}", fileKey));
                                    if (XFileTools.Exists(searchFilePath))
                                    {
                                        syncFileList.Add(filePath);
                                    }
                                });

                                XFolderTools.TraverseFolder(srcPath, (folderPath) =>
                                {
                                    string fileKey = null;
                                    if (syncItem.searchKey == AssetSyncItem.SearchKey.AssetName)
                                    {
                                        fileKey = Path.GetFileNameWithoutExtension(folderPath);
                                    }
                                    else if (syncItem.searchKey == AssetSyncItem.SearchKey.AssetPrefix)
                                    {
                                        fileKey = XStringTools.SplitPathKey(folderPath);
                                    }

                                    string searchFilePath = XPathTools.Combine(searchPath, string.Format("{0}", fileKey));
                                    if (XFileTools.Exists(searchFilePath))
                                    {
                                        syncFolderList.Add(folderPath);
                                    }
                                });
                            }

                            HashSet<string> syncFileDict = new HashSet<string>();
                            foreach(var syncSrcFile in syncFileList)
                            {
                                //把文件拷贝到同步目录
                                string syncFileName = Path.GetFileName(syncSrcFile);
                                string syncDestPath = XPathTools.Combine(syncPath, syncFileName);
                                XFileTools.Delete(syncDestPath);
                                XFileTools.Copy(syncSrcFile, syncDestPath);

                                if (!syncFileDict.Contains(syncDestPath))
                                    syncFileDict.Add(syncDestPath);
                            }

                            HashSet<string> syncFolderDict = new HashSet<string>();
                            foreach (var syncSrcFolder in syncFolderList)
                            {
                                //把文件拷贝到同步目录
                                string syncFileName = Path.GetFileName(syncSrcFolder);
                                string syncDestPath = XPathTools.Combine(syncPath, syncFileName);

                                XFolderTools.DeleteDirectory(syncDestPath,true);
                                XFolderTools.CopyDirectory(syncSrcFolder, syncDestPath);

                                if (!syncFolderDict.Contains(syncDestPath))
                                    syncFolderDict.Add(syncDestPath);
                            }

                            //移除不在同步的文件
                            XFolderTools.TraverseFiles(syncPath, (syncFullPath) =>
                            {
                                if (!syncFileDict.Contains(syncFullPath))
                                {
                                    XFileTools.Delete(syncFullPath);
                                }
                            });

                            XFolderTools.TraverseFolder(syncPath, (syncFullPath) =>
                            {
                                if (!syncFolderDict.Contains(syncFullPath))
                                {
                                    XFolderTools.DeleteDirectory(syncFullPath, true);
                                }
                            });
                        }
                    }
                }
            });
        }

        public void Freeze(int version)
        {
            if (version <= 0)
                return;

            var repositoryRoot = AssetSyncConfiger.GetInstance().GetRepositoryRootPath();
            if (string.IsNullOrEmpty(repositoryRoot))
                return;

            var syncItems = AssetSyncConfiger.GetInstance().syncItems;
            if (syncItems == null || syncItems.Count <= 0)
                return;

            string versionFolderName = string.Format("{0}", version);
            string repositoryPath = XPathTools.Combine(repositoryRoot, versionFolderName);

            foreach (var syncItem in syncItems)
            {
                string destPath = XPathTools.Combine(repositoryPath, syncItem.realSearcePath);
                if (XFolderTools.Exists(destPath))
                {
                    XFolderTools.DeleteDirectory(destPath,true);
                }
            }

        }


        //获取目录下第一个文件的后缀
        private string GetFolderFirstFileExtension(string folderPath)
        {
            string ex = "";
            XFolderTools.TraverseFiles(folderPath, (fullPath) =>
            {
                if (!string.IsNullOrEmpty(ex))
                    return;

                ex = Path.GetExtension(fullPath);
                if (ex.Contains("DS_Store")) ex = "";
            });
            return ex;
        }
    }
}

