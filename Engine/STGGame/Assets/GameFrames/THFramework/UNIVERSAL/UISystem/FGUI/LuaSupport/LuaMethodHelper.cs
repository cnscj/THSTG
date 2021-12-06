using System;
using System.Collections.Generic;
using System.IO;
using FairyGUI;
using UnityEngine;
using UnityEngine.Networking;

namespace THGame.UI
{
    public static class LuaMethodHelper
    {

        public static UIPackage LoadPackageInPcCustom(byte[] descBytes, string assetNamePrefix, Func<string, string, object> call)
        {

            return UIPackage.AddPackage(descBytes, assetNamePrefix, (string name, string extension, System.Type type, out DestroyMethod destroyMethod) =>
            {
                destroyMethod = DestroyMethod.Unload;
                byte[] bytes = (byte[])call?.Invoke(name, extension);
                if (bytes != null)
                {
                    if (type == typeof(Texture))
                    {
                        // texture
                        Texture2D texture = new Texture2D(16, 16, TextureFormat.ARGB32, false);
                        texture.LoadImage(bytes);
                        texture.name = System.IO.Path.GetFileNameWithoutExtension(name);
                        return texture;
                    }
                    else if (type == typeof(AudioClip))
                    {
                        if (extension == ".ogg")
                        {
                            string fullPath = "";
                            string url = "file://" + fullPath;
                            UnityWebRequest req = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.UNKNOWN);
                            var obj = req.SendWebRequest();
                            AudioClip audipClip = null;
                            obj.completed += (_) =>
                            {
                                if (string.IsNullOrEmpty(req.error))
                                {
                                    audipClip = audipClip = DownloadHandlerAudioClip.GetContent(req);
                                }
                            };
                            audipClip.name = System.IO.Path.GetFileNameWithoutExtension(name);
                            return audipClip;

                            //FIXME:OGG解码支持
                            //audioClip.name = System.IO.Path.GetFileNameWithoutExtension(name);
                            //return audioClip;
                        }
                        else if (extension == ".wav")
                        {
                            AudioClip audioClip = WavUtil.ToAudioClip(bytes);
                            audioClip.name = System.IO.Path.GetFileNameWithoutExtension(name);
                            return audioClip;
                        }
                    }

                }


                return Resources.Load(name,type);
            });
        }

        //传assetPath需要修改UIPackage.AddPackage里的TextAsset为byte[]
        public static UIPackage LoadPackageInPcCustom(byte[] descData, Func<string, string, System.Type, object> call)
        {
            return UIPackage.AddPackage(descData, string.Empty, (string name, string extension, System.Type type, out DestroyMethod destroyMethod) =>
            {
                destroyMethod = DestroyMethod.Unload;
                return call?.Invoke(name, extension, type);
            });
        }

        public static string[] GetPackageDependencies(UIPackage package)
        {
            if (package == null)
                return default;

            int count = package.dependencies != null ? package.dependencies.Length : 0;
            if (count > 0)
            {
                var depList = new string[count];
                int i = 0;
                foreach (var depDict in package.dependencies)
                {
                    depList[i] = depDict["name"];
                    i++;
                }

                return depList;
            }
            
            return default;
        }
    }

}
