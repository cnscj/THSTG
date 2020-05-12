using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SEGame
{
    /// <summary>
    /// Profiler watcher. 一个小工具，用来统计一段代码的执行时间，主要想扩展出一个收集函数调用列表的时间消耗的统一方法。
    /// </summary>
    internal sealed class ProfilerWatcher
    {
        internal class Watcher : System.Diagnostics.Stopwatch
        {
            internal string msg;

            public override string ToString()
            {
                return string.Format("[ProfilerWatcher] {0}: {1}ms", msg, ElapsedMilliseconds.ToString());
            }
        }

        static List<Watcher> m_lastWatcherList;
        static Stack<Watcher> m_watcherStack = new Stack<Watcher>();

        /// <summary>
        /// 开始一个Stopwatch，跟 End() 成对使用
        /// </summary>

        //[System.Diagnostics.Conditional("DEBUG")]
        internal static void Begin(string msg)
        {
            //if (m_lastWatcherList == null && m_watcherStack.Count > 1)
            //{
            //    Debug.LogError("End()要和Begin()成对使用");
            //}

            var watcher = new Watcher();
            watcher.msg = msg;
            watcher.Start();

            m_watcherStack.Push(watcher);
        }

        /// <summary>
        /// 结束一个Stopwatch，跟 Begin(string msg) 成对使用
        /// </summary>
        internal static Watcher End()
        {
            if (m_watcherStack.Count > 0)
            {
                var watcher = m_watcherStack.Pop();
                watcher.Stop();

                if (m_lastWatcherList != null)
                    m_lastWatcherList.Insert(0, watcher);
                
                return watcher;
            }
            else
            {
                Debug.LogError("End()要和Begin()成对使用");
                return new Watcher();
            }
        }

        /// <summary>
        /// 取End()的返回结果并打印
        /// </summary>
        //[System.Diagnostics.Conditional("DEBUG")]
        internal static void EndAndPrint(LogType lt = LogType.Log)
        {
            Watcher watcher = End();
            if (watcher != null)
            {
                PrintWatcher(watcher, lt);
            }
        }

        internal static void ClearAll()
        {
            m_lastWatcherList = null;
            m_watcherStack.Clear();
        }

        /// <summary>
        /// Begins the group. 开始一个组，跟 EndGroup() 成对调用
        /// 调用该方法之后，再多次成对地调用 Begin(string msg)和End()，可以返回一个数组。 
        /// End()顺序是按Begin()堆栈的顺序执行，如果希望是队列的形式，提需求，扩展，在BeginGroup()这边设置类型。
        /// </summary>
        //[System.Diagnostics.Conditional("DEBUG")]
        internal static void BeginGroup()
        {
            if (m_lastWatcherList != null)
            {
                Debug.LogWarningFormat("一次只能有一个Group");
                return;
            }

            m_lastWatcherList = new List<Watcher>();
        }

        /// <summary>
        /// Ends the group. 结束一个组，跟 BeginGroup() 成对调用
        /// 调用该方法之前，确保所有Begin(string msg)都已经有对应的End()
        /// </summary>
        internal static List<Watcher> EndGroup()
        {
            var list = m_lastWatcherList;
            m_lastWatcherList = null;

            if (m_watcherStack.Count > 0)
            {
                Debug.LogError("还有Watcher没有End()就调EndGroup()了！！！");
                m_watcherStack.Clear();
            }

            return list;
        }

        /// <summary>
        /// 取EndGroup()的返回结果并打印
        /// </summary>
        //[System.Diagnostics.Conditional("DEBUG")]
        internal static void EndGroupAndPrint(LogType lt = LogType.Log)
        {
            List<Watcher> watchers = EndGroup();
            if (watchers != null)
            {
                foreach(var watcher in watchers)
                {
                    PrintWatcher(watcher, lt);
                }
            }
        }

        static void PrintWatcher(Watcher watcher, LogType lt)
        {
            if (lt == LogType.Error)
            {
                Debug.LogError(watcher.ToString());
            }
            else if (lt == LogType.Warning)
            {
                Debug.LogWarning(watcher.ToString());
            }
            else
            {
                Debug.Log(watcher.ToString());
            }
        }
    }
}
