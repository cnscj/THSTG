using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using XLibrary;

namespace ASEditor
{
    public class AssembliesBuilderManager
    {
        public static void BuildAssemblySync()
        {
            BuildAssembly(true);
        }

        static void BuildAssembly(bool wait)
        {
            for (int i = 0; i < AssembliesToolsConfig.GetInstance().buildPathList.Count; i++)
            {
                var buildInfo = AssembliesToolsConfig.GetInstance().buildPathList[i];

                if (!string.IsNullOrEmpty(buildInfo.srcPath) && !string.IsNullOrEmpty(buildInfo.buildPath) && !string.IsNullOrEmpty(buildInfo.projectPath))
                {
                    bool[] editorFlags = { true, true, true, true };

                    List<string> csFilePaths = new List<string>();

                    XFolderTools.TraverseFiles(buildInfo.srcPath, (string fullPath) =>
                    {
                        if (Path.GetExtension(fullPath) == ".cs")
                        {
                            csFilePaths.Add(fullPath);
                        }
                    }, true);


                    string srcPathName = Path.GetFileNameWithoutExtension(buildInfo.srcPath);
                    string outputAssembly = PathUtil.Combine(buildInfo.buildPath, string.Format("{0}.dll", srcPathName));
                    string assemblyProjectPath = PathUtil.Combine(buildInfo.projectPath, string.Format("{0}.dll", srcPathName));
                    if (csFilePaths.Count > 0)
                    {
                        if (!XFolderTools.Exists(buildInfo.buildPath))
                        {
                            XFolderTools.CreateDirectory(buildInfo.buildPath);
                        }
                        if (!XFolderTools.Exists(buildInfo.projectPath))
                        {
                            XFolderTools.CreateDirectory(buildInfo.projectPath);
                        }

                        bool editorFlag = editorFlags[i];

                        var assemblyBuilder = new AssemblyBuilder(outputAssembly, csFilePaths.ToArray());

                        // Exclude a reference to the copy of the assembly in the Assets folder, if any.
                        assemblyBuilder.excludeReferences = new string[] { assemblyProjectPath };

                        if (editorFlag)
                        {
                            assemblyBuilder.flags = AssemblyBuilderFlags.EditorAssembly;
                        }

                        // Called on main thread
                        assemblyBuilder.buildStarted += delegate (string assemblyPath)
                        {
                            Debug.LogFormat("Assembly build started for {0}", assemblyPath);
                        };

                        // Called on main thread
                        assemblyBuilder.buildFinished += delegate (string assemblyPath, CompilerMessage[] compilerMessages)
                        {
                            foreach (var v in compilerMessages)
                            {
                                if (v.type == CompilerMessageType.Error)
                                    Debug.LogError(v.message);
                                else
                                    Debug.LogWarning(v.message);
                            }

                            var errorCount = compilerMessages.Count(m => m.type == CompilerMessageType.Error);
                            var warningCount = compilerMessages.Count(m => m.type == CompilerMessageType.Warning);

                            Debug.LogFormat("Assembly build finished for {0}", assemblyPath);
                            Debug.LogFormat("Warnings: {0} - Errors: {0}", errorCount, warningCount);

                            if (errorCount == 0)
                            {
                                File.Copy(outputAssembly, assemblyProjectPath, true);
                                AssetDatabase.ImportAsset(assemblyProjectPath);
                            }
                        };

                        // Start build of assembly
                        if (!assemblyBuilder.Build())
                        {
                            Debug.LogErrorFormat("Failed to start build of assembly {0}!", assemblyBuilder.assemblyPath);
                            return;
                        }

                        if (wait)
                        {
                            while (assemblyBuilder.status != AssemblyBuilderStatus.Finished)
                                System.Threading.Thread.Sleep(10);
                        }
                    }
                    else
                    {
                        XFileTools.Delete(assemblyProjectPath);
                        continue;
                    }

                }
            }
        }
    }
}