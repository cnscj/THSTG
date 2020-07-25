
using System;
using System.Text;

namespace THGame
{
    public abstract class BaseLog
    {
        // _logAppender
        protected ILogAppender _logAppender;

        /// <summary>
        /// 设置日志级别
        /// </summary>
        /// <value>The log level.</value>
        public int Level
        {
            get;
            set;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        protected BaseLog()
        {
            Level = LogType.INFO;
        }
        /// <summary>
        /// debug Log
        /// </summary>
        /// <param name="log">Log.</param>
        public virtual void Debug(object log)
        {
            if (Level > LogType.DEBUG)
            {
                return;
            }
            LogData data = CreateLogData(log, string.Empty, LogType.DEBUG);
            data.Log = log.ToString();
            data.Type = LogType.DEBUG;
            _logAppender.Log(data);
        }
        /// <summary>
        /// info Log
        /// </summary>
        /// <param name="log">Log.</param>
        public virtual void Info(object log)
        {
            if (Level > LogType.INFO)
            {
                return;
            }
            LogData data = CreateLogData(log, string.Empty, LogType.INFO);
            _logAppender.Log(data);
        }

        /// <summary>
        /// warn Log
        /// </summary>
        /// <param name="log">Log.</param>
        public virtual void Warning(object log, string stackTrace)
        {
            if (Level > LogType.WARNING)
            {
                return;
            }
            LogData data = CreateLogData(log, stackTrace, LogType.WARNING);
            _logAppender.Log(data);
        }
        public virtual void Warning(object log, Exception e)
        {
            Warning(log, GetExceptionTrack(e));
        }
        /// <summary>
        /// error Log
        /// </summary>
        /// <param name="log">Log.</param>
        public virtual void Error(object log, string stackTrace)
        {
            if (Level > LogType.ERROR)
            {
                return;
            }
            LogData data = CreateLogData(log, stackTrace, LogType.ERROR);
            _logAppender.Log(data);
        }
        public virtual void Error(object log, Exception e)
        {
            Error(log, GetExceptionTrack(e));
        }
        /// <summary>
        /// error Log
        /// </summary>
        /// <param name="log">Log.</param>
        public virtual void Fatal(object log, string stackTrace)
        {
            if (Level > LogType.FATAL)
            {
                return;
            }
            LogData data = CreateLogData(log, stackTrace, LogType.FATAL);
            _logAppender.Log(data);
        }
        public virtual void Fatal(object log, Exception e)
        {
            Fatal(log, GetExceptionTrack(e));
        }
        /// <summary>
        /// 获取异常堆栈
        /// </summary>
        /// <returns>The exception track.</returns>
        /// <param name="e">E.</param>
        protected string GetExceptionTrack(Exception e)
        {
            StringBuilder builder = new StringBuilder(120);
            builder.Append(e.Message).Append("\n");
            if (!string.IsNullOrEmpty(e.StackTrace))
            {
                builder.Append(e.StackTrace);
            }
            return builder.ToString();
        }

        protected LogData CreateLogData(string condition, string stackTrace, int type)
        {
            LogData data = new LogData();
            data.Log = condition;
            data.Track = stackTrace;
            data.Type = type;

            return data;
        }

        protected LogData CreateLogData(object logObj, string stackTrace, int type)
        {
            return CreateLogData(logObj.ToString(), stackTrace, type);
        }
    }
}
