
using System;
using UnityEngine;

namespace XLibGame
{
    public class Logger : BaseLog
    {
        private static Logger s_instance;

        public Logger GetInstance()
        {
            if (s_instance == null)
            {
                s_instance = new Logger();
            }
            return s_instance;
        }

        public bool CaptureUnityLog
        {
            set
            {
                if (value)
                    Application.logMessageReceivedThreaded += OnReceiveLogMsg;  //日志监听
                else
                    Application.logMessageReceivedThreaded -= OnReceiveLogMsg;  //移除监听
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        private Logger()
        {
            Level = LogType.INFO;
            _logAppender = new RollingFileAppender();
        }

        void OnReceiveLogMsg(string condition, string stackTrace, UnityEngine.LogType type)
        {
            switch (type)
            {
                case UnityEngine.LogType.Log:
                    Info(condition);
                    break;
                case UnityEngine.LogType.Error:
                    Error(condition, stackTrace);
                    break;
                case UnityEngine.LogType.Assert:
                    Fatal(condition, stackTrace);
                    break;
                case UnityEngine.LogType.Warning:
                    Warning(condition, stackTrace);
                    break;
                case UnityEngine.LogType.Exception:
                    Fatal(condition, stackTrace);
                    break;
                default:
                    Info(condition);
                    break;
            }

        }
    }
}
