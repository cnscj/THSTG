using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using UnityEngine;
using System.Diagnostics;

namespace ASGame
{
    public class ConsoleHelper
    {
        [DllImport("kernel32")]
        private static extern bool AllocConsole();
        [DllImport("kernel32")]
        private static extern bool FreeConsole();
        [DllImport("kernel32")]
        private static extern IntPtr GetStdHandle(uint stdHandle);
        [DllImport("kernel32")]
        private static extern bool GetConsoleMode(IntPtr hConsole, out uint consoleMode);
        [DllImport("kernel32")]
        private static extern bool SetConsoleMode(IntPtr hConsole, uint consoleMode);
        [DllImport("kernel32.dll")]
        private static extern bool ReadConsole(IntPtr hConsoleInput, [Out] StringBuilder lpBuffer, uint nNumberOfCharsToRead, out uint lpNumberOfCharsRead, IntPtr lpReserved);
        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool WriteConsole(IntPtr hConsoleOutput, string msg, uint msgLength, out uint m_CharsWritten, int Reserved);
        [DllImport("kernel32.dll")]
        private static extern bool FlushConsoleInputBuffer(IntPtr hConsoleInput);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern int SetConsoleTextAttribute(IntPtr hConsoleOutput, uint wAttributes);
        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleScreenBufferSize(IntPtr hConsoleOutput, IntPtr dwSize);
        [DllImport("Kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();
        [DllImport("USER32.DLL")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, int bRevert);
        [DllImport("USER32.DLL")]
        private static extern IntPtr RemoveMenu(IntPtr hMenu, int nPosition, int wFlags);
        [DllImport("kernel32")]
        private static extern bool SetConsoleCtrlHandler(HandlerRoutine Routine, bool Add);
        private delegate bool HandlerRoutine(uint dwControlType);

        [DllImport("kernel32.dll")]
        static extern bool SetConsoleWindowInfo(IntPtr hConsoleOutput, bool bAbsolute, IntPtr lpConsoleWindow);


        [StructLayout(LayoutKind.Sequential)]
        private class COORD
        {
            public short X;
            public short Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private class SMALL_RECT
        {
            public short Left;
            public short Top;
            public short Right;
            public short Bottom;
        }

        public static IntPtr StdOutHandle = (IntPtr)(-1);
        public static IntPtr StdInHandle = (IntPtr)(-1);

        const int STD_OUTPUT_HANDLE = -11;
        const int STD_INPUT_HANDLE = -10;

        const uint ENABLE_MOUSE_INPUT = 0x0010;

        const uint FOREGROUND_BLUE = 0x0001; // text color contains blue.
        const uint FOREGROUND_GREEN = 0x0002; // text color contains green.
        const uint FOREGROUND_RED = 0x0004; // text color contains red.
        const uint FOREGROUND_INTENSITY = 0x0008; // text color is intensified.
        const uint BACKGROUND_BLUE = 0x0010; // background color contains blue.
        const uint BACKGROUND_GREEN = 0x0020; // background color contains green.
        const uint BACKGROUND_RED = 0x0040; // background color contains red.
        const uint BACKGROUND_INTENSITY = 0x0080; // background color is intensified.

        const int SC_RESTORE = 0xF120;     //还原   
        const int SC_MOVE = 0xF010;   //移动   
        const int SC_SIZE = 0xF000;   //大小   
        const int SC_MINIMIZE = 0xF020;   //最小化   
        const int SC_MAXIMIZE = 0xF030;   //最大化   
        const int SC_CLOSE = 0xF060;   //关闭  
        const int MF_BYCOMMAND = 0x0000;
        const int MF_REMOVE = 0x1000;


        private const int CTRL_CLOSE_EVENT = 2;
        //Ctrl+C，系统会发送次消息
        private const int CTRL_C_EVENT = 0;
        //Ctrl+break，系统会发送次消息
        private const int CTRL_BREAK_EVENT = 1;
        //用户退出（注销），系统会发送次消息
        private const int CTRL_LOGOFF_EVENT = 5;
        //系统关闭，系统会发送次消息
        private const int CTRL_SHUTDOWN_EVENT = 6;


        public static bool ConsoleAlive = false;

        struct LogContent
        {
            public LogType LogType;
            public string Log;
        }
        static Queue<LogContent> LogQueue = new Queue<LogContent>();
        static Mutex WriteLogMutex = new Mutex();

        private static bool RoutineHandler(uint ctrlType)
        {
            UnityEngine.Debug.Log("ctrlType = " + ctrlType);
            switch (ctrlType)
            {
                case CTRL_C_EVENT:
                    Console.WriteLine("C");
                    // return true; //这里返回true，表示阻止响应系统对该程序的操作
                    break;
                case CTRL_BREAK_EVENT:
                    Console.WriteLine("BREAK");
                    break;
                case CTRL_CLOSE_EVENT:
                    Console.WriteLine("CLOSE");
                    break;
                case CTRL_LOGOFF_EVENT:
                    Console.WriteLine("LOGOFF");
                    break;
                case CTRL_SHUTDOWN_EVENT:
                    Console.WriteLine("SHUTDOWN");
                    break;
            }
            //return true;//表示阻止响应系统对该程序的操作
            return false;//忽略处理，让系统进行默认操作
        }

        public static bool InitConsole()
        {
            UnityEngine.Debug.Log("InitConsole");

            // if (Application.platform != RuntimePlatform.WindowsPlayer)
            //     return false;

            AllocConsole();

            HandlerRoutine consoleDelegete = new HandlerRoutine(RoutineHandler);
            SetConsoleCtrlHandler(consoleDelegete, true);


            StdOutHandle = GetStdHandle(unchecked((uint)STD_OUTPUT_HANDLE));
            StdInHandle = GetStdHandle(unchecked((uint)STD_INPUT_HANDLE));

            // 允许控制台右键菜单
            uint dwConsoleMode = 0;
            GetConsoleMode(StdInHandle, out dwConsoleMode);
            SetConsoleMode(StdInHandle, ~ENABLE_MOUSE_INPUT & dwConsoleMode);

            // forbid close
            IntPtr hWnd = GetConsoleWindow();
            IntPtr hMenu = GetSystemMenu(hWnd, 0);
            RemoveMenu(hMenu, SC_CLOSE, MF_BYCOMMAND);

            COORD BufferSize = new COORD()
            {
                X = 120,
                Y = 320,
            };

            SMALL_RECT WinRect = new SMALL_RECT()
            {
                Left = 0,
                Top = 0,
                Right = (short)(BufferSize.X - 1),
                Bottom = 30,
            };

            IntPtr pBuffer = Marshal.AllocHGlobal(Marshal.SizeOf(BufferSize));
            Marshal.StructureToPtr(BufferSize, pBuffer, false);
            if (!SetConsoleScreenBufferSize(StdOutHandle, pBuffer))
            {
                //return false;
            }
            Marshal.FreeHGlobal(pBuffer);

            IntPtr pWinRect = Marshal.AllocHGlobal(Marshal.SizeOf(WinRect));
            Marshal.StructureToPtr(WinRect, pWinRect, false);
            if (!SetConsoleWindowInfo(StdOutHandle, true, pWinRect))
            {
                //return false;
            }
            Marshal.FreeHGlobal(pWinRect);

            ConsoleAlive = true;

            Thread th = new Thread(new ThreadStart(WorkingThread));
            th.Start();

            return true;
        }

        public static bool UninitConsole()
        {
            if (!ConsoleAlive)
                return false;

            FreeConsole();
            ConsoleAlive = false;
            return true;
        }

        public static void ClearConsole()
        {
            UninitConsole();
            InitConsole();
        }

        public static void WorkingThread()
        {
            while (ConsoleAlive)
            {
                if (LogQueue.Count == 0)
                {
                    Thread.Sleep(100);
                    continue;
                }

                WriteLogMutex.WaitOne();
                LogContent logContent = LogQueue.Dequeue();
                WriteLogMutex.ReleaseMutex();

                string log = logContent.Log;
                switch (logContent.LogType)
                {
                    case LogType.Assert:
                    case LogType.Error:
                    case LogType.Exception:
                        ShowErrorColor();
                        break;
                    case LogType.Warning:
                        ShowWarningColor();
                        break;
                    default:
                        ShowInfoColor();
                        break;
                }

                uint charWritten = 0;
                WriteConsole(StdOutHandle, log, (uint)log.Length, out charWritten, 0);
                if (!log.EndsWith("\n") && !log.EndsWith("\r"))
                    WriteConsole(StdOutHandle, "\n", 1, out charWritten, 0);
                RestoreColor();
            }
        }

        public static void Write(LogType logType, string log)
        {
            WriteLogMutex.WaitOne();
            LogQueue.Enqueue(new LogContent() { LogType = logType, Log = log });
            WriteLogMutex.ReleaseMutex();
        }

        public static void SetColor(uint color)
        {
            SetConsoleTextAttribute(StdOutHandle, color);
        }

        public static void ShowWarningColor()
        {
            SetColor(FOREGROUND_RED | FOREGROUND_GREEN);
        }
        public static void ShowErrorColor()
        {
            SetColor(FOREGROUND_RED | FOREGROUND_GREEN | FOREGROUND_BLUE | FOREGROUND_INTENSITY | BACKGROUND_RED);
        }
        public static void ShowInfoColor()
        {
            SetColor(FOREGROUND_GREEN);
        }
        public static void RestoreColor()
        {
            SetColor(FOREGROUND_RED | FOREGROUND_GREEN | FOREGROUND_BLUE);
        }
    }

}
