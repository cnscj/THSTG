using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XLibrary.Package;

namespace XLibGame.Debug
{
    /**
     * 将所有日志信息,包括控制台信息输出到文件
     * 实时写入,奔溃时能查到log
     * **/
    public class Logger : MonoSingleton<Logger>
    {
        public bool isAppend = false;       //是否追加写入
        public int length;                  //当前监听数
        public int count;                   //当前写入数
        private StreamWriter m_writer;      //文件流

        //写入内容
        public void Write(string content,bool isAppendLineFeed = true)
        {
            if (m_writer != null)
            {
                if (isAppendLineFeed)
                {
                    m_writer.WriteLine(content);
                }
                else
                {
                    m_writer.Write(content);
                }

                m_writer.Flush();
                count++;
            }
        }

        void Awake()
        {
            string fname = fname = Path.Combine(Application.persistentDataPath, string.Format("Log.txt"));

            if (!isAppend)
            {
                fname = Path.Combine(Application.persistentDataPath, string.Format("Log_{0}.txt", NowUnformatTimeStr()));
            }
            m_writer = m_writer ?? new StreamWriter(fname, isAppend, System.Text.Encoding.Default);
            if (isAppend)
            {
                m_writer.WriteLine("\r\n\r\n---------" + System.DateTime.Now.ToString());
            }

            Application.logMessageReceivedThreaded += OnReceiveLogMsg;  //日志监听
        }

        void OnDestroy()
        {
            if (m_writer != null)
            {
                m_writer.Close();
            }
        }

        void OnReceiveLogMsg(string condition, string stackTrace, LogType type)
        {
            string _type = "";
            switch (type)
            {
                case LogType.Error:
                    _type = "error";
                    break;
                case LogType.Assert:
                    _type = "Assert";
                    break;
                case LogType.Warning:
                    _type = "Warning";
                    break;
                case LogType.Log:
                    _type = "Log";
                    break;
                case LogType.Exception:
                    _type = "Exception";
                    break;
                default:
                    break;
            }
            string msg = "[MSG]:" + condition + "--" + "[station]:" + stackTrace + "-" + "[LogType]:" + _type;
            Write(msg);

            length++;
        }

        string NowUnformatTimeStr()
        {
            DateTime dt = DateTime.Now;
            return string.Format("{0:yyyyMMddHHmmss}", dt);
        }

    }
}
