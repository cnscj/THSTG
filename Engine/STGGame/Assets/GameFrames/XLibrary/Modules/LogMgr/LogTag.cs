namespace XLibGame
{
    public static class LogTag
    {
        /// <summary>
        /// 日志等级，为不同输出配置用
        /// </summary>

        public const string DEBUG = "BEBUG";
        public const string INFO = "INFO";
        public const string WARNING = "WARNING";
        public const string ERROR = "ERROR";
        public const string FATAL = "FATAL";


        public static int Convert(string tag)
        {
            switch(tag)
            {
                case DEBUG:
                    return LogType.DEBUG;
                case INFO:
                    return LogType.INFO;
                case WARNING:
                    return LogType.WARNING;
                case ERROR:
                    return LogType.ERROR;
                case FATAL:
                    return LogType.FATAL;
                default:
                    return LogType.INFO;
            }
        }
    }

}
