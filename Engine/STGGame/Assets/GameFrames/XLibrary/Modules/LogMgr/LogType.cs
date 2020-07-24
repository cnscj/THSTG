namespace XLibGame
{
    public static class LogType
    {
        /// <summary>
        /// 日志等级，为不同输出配置用
        /// </summary>
        public const int DEBUG = 0;
        public const int INFO = 1;
        public const int WARNING = 2;
        public const int ERROR = 3;
        public const int FATAL = 4;

        public static string Convert(int type)
        {
            switch (type)
            {
                case DEBUG:
                    return LogTag.DEBUG;
                case INFO:
                    return LogTag.INFO;
                case WARNING:
                    return LogTag.WARNING;
                case ERROR:
                    return LogTag.ERROR;
                case FATAL:
                    return LogTag.FATAL;
                default:
                    return LogTag.INFO;
            }
        }
    }
}
