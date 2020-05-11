
using System;
using System.Collections.Generic;
using System.Diagnostics;
using XLibrary.Package;

namespace XLibGame.Debugger
{
    public class Profiler : Singleton<Profiler>
    {
        public class Result
        {
            public long startTime;
            public long endTime;
            public StackTrace traceBack;

            public Result() { }
            public long GetSpend()
            {
                return endTime - startTime;
            }
            public StackTrace GetTraceback()
            {
                return traceBack;
            }
        }

        private Stack<Result> m_resultStack;
        private Result m_lastResult;

        public Result Start()
        {
            Result ret = new Result();
            ret.startTime = NowTimeStampMs();
            ret.traceBack = new StackTrace();//调取该方法的堆栈信息
            
            m_resultStack.Push(ret);

            return ret;
        }

        public Result End()
        {
            var ret = m_resultStack.Pop();
            if (ret != null)
            {
                ret.endTime = NowTimeStampMs();
               
            }
            m_lastResult = ret;
            return ret;
        }

        public Result GetResult()
        {
            return m_lastResult ?? m_resultStack.Peek() ?? null;
        }

        private Profiler()
        {
            m_resultStack = new Stack<Result>();
        }

        long NowTimeStampMs()
        {
            return (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000;
        }
    }
}
