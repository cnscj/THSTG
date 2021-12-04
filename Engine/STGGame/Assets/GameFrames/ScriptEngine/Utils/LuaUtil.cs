using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SEGame
{
    public static class LuaUtil
    {
        #region LuaLuaLoader

        public static IEnumerator LoadStreamAsync(string filePath, Action<byte[]> onSuccess)
        {
            using (FileStream fsRead = new FileStream(filePath, FileMode.Open))
            {
                int fsLen = (int)fsRead.Length;
                int stLen = 1024 * 1024;
                byte[] heByte = new byte[fsLen];

                int hadReadedLen = 0;
                int needReadLen = hadReadedLen + stLen < fsLen ? stLen : fsLen - hadReadedLen;
                while (true)
                {
                    int r = fsRead.Read(heByte, hadReadedLen, needReadLen);
                    hadReadedLen = hadReadedLen + r;
                    if (hadReadedLen >= fsLen)
                    {
                        onSuccess?.Invoke(heByte);
                        yield break;
                    }
                    else
                    {
                        yield return null;
                    }
                }
            }
        }

        #endregion
    }

}
