using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XLibrary.Package;

namespace XLibGame.Debugger
{
    /**
     * 将所有日志信息,包括控制台信息输出到文件
     * 实时写入,奔溃时能查到log
     * **/
    public class Logger : Singleton<Logger>
    {
        public string logPath = "Log.txt";          //日志文件名
        public float maxKb = -1f;                   //日志文件大小上限
        public int length { get; protected set; }   //当前监听数
        public int count { get; protected set; }    //当前写入数
        private StreamWriter m_writer;              //文件流

        ~Logger()
        {
            Close();
        }

        //写入内容
        public void Write(string content, bool isAppendLineFeed = true)
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

        public void Open()
        {
            if (m_writer == null)
            {
                string folder = Path.GetDirectoryName(logPath);
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                if (maxKb > 0f)
                {
                    if (File.Exists(logPath))
                    {
                        FileInfo fileInfo = new FileInfo(logPath);
                        float fileKb = fileInfo.Length / 1000f;
                        if (fileKb >= maxKb)
                        {
                            File.Delete(logPath);
                        }
                    }
                }

                m_writer = m_writer ?? new StreamWriter(logPath, true, System.Text.Encoding.Default);
                m_writer.WriteLine("\r\n\r\n");
                m_writer.WriteLine("---------" + System.DateTime.Now.ToString() + "---------");

                Application.logMessageReceivedThreaded += OnReceiveLogMsg;  //日志监听 
            }
        }

        public void Close()
        {
            if (m_writer != null)
            {
                Application.logMessageReceivedThreaded -= OnReceiveLogMsg;  //日志监听
                m_writer.Close();
                m_writer = null;

            }
        }

        void OnReceiveLogMsg(string condition, string stackTrace, LogType type)
        {
            bool isDetail = true;
            string typeStr = "";
            switch (type)
            {
                case LogType.Error:
                    typeStr = "Error";
                    break;
                case LogType.Assert:
                    typeStr = "Assert";
                    break;
                case LogType.Warning:
                    typeStr = "Warning";
                    break;
                case LogType.Log:
                    typeStr = "Log";
                    isDetail = false;
                    break;
                case LogType.Exception:
                    typeStr = "Exception";
                    break;
                default:
                    break;
            }
            string msg = string.Format("[{0}][{1}]:{2}", NowTimeStr(), typeStr, condition);
            if(isDetail)
            {
                msg += ("\r\n" + stackTrace);
            }
            
            Write(msg);

            length++;
        }

        string NowUnformatTimeStr()
        {
            DateTime dt = DateTime.Now;
            return string.Format("{0:yyyyMMddHHmmss}", dt);
        }

        public string NowTimeStr()
        {
            DateTime dt = DateTime.Now;
            return string.Format("{0:HH:mm:ss}", dt);
        }
    }
}
