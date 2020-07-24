using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XLibGame
{
    /// <summary>
    /// Log的具体内容
    /// </summary>
    public class LogData
    {
        /// <summary>
        /// Log类型
        /// </summary>
        /// <value>The type.</value>
        public int Type { get; set; }

        /// <summary>
        /// Log具体内容
        /// </summary>
        /// <value>The log.</value>
        public string Log { get; set; }

        /// <summary>
        /// Log堆栈信息
        /// </summary>
        /// <value>The track.</value>
        public string Track { get; set; }
    }
}
