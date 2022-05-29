using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using XLibrary.Package;

namespace XLibGame.Debugger
{
    public class Console: Singleton<Console>
    {
        public void Format(string format, params object[] args)
        {
            UnityEngine.Debug.LogFormat(format, args);
        }

        public void Print(params object[] objs)
        {
            if (objs != null && objs.Length > 0)
            {
                StringBuilder ret = new StringBuilder();

                for (int i = 0; i < objs.Length; i++)
                {
                    if (objs[i] == null)
                    {
                        ret.Append("null");
                    }
                    else
                    {
                        ret.Append(objs[i].ToString());
                    }

                    ret.Append(" ");
                }

                UnityEngine.Debug.Log(ret);
            }
        }

        public void Dump(object obj, string name = null)
        {
            name = name ?? "";
            string ret = JsonUtility.ToJson(obj);
            UnityEngine.Debug.LogFormat("[{0}] = {1}", name, ret);
        }

        public void StackTrace()
        {
            string stackInfo = new System.Diagnostics.StackTrace().ToString();//调取该方法的堆栈信息
            UnityEngine.Debug.Log(stackInfo);
        }

        public void Clear()
        {
            var logEntries = System.Type.GetType("UnityEditorInternal.LogEntries,UnityEditor.dll");
            var clearMethod = logEntries.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            clearMethod.Invoke(null, null);
        }
    }
}

