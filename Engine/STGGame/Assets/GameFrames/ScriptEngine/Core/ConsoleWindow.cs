using UnityEngine;
using System.Runtime.InteropServices;

public class ConsoleWindow : MonoBehaviour
{
#if UNITY_STANDALONE_OSX //|| UNITY_EDITOR_OSX
    const string ConsoleWindowsDLL = "ConsoleWindow";
    [DllImport(ConsoleWindowsDLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern int ShowConsoleWin();

    [DllImport(ConsoleWindowsDLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern void ShowErrorMsg(string msg);

    [DllImport(ConsoleWindowsDLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern void ShowWarningMsg(string msg);

    [DllImport(ConsoleWindowsDLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern void ShowLogMsg(string msg);
    
    void ShowConsole()
    {
        ShowConsoleWin();
    }

    void HandleLog(string message, string stackTrace, LogType type)
    {
        message = message + "\n";
        if (type == LogType.Log)
        {
            ShowLogMsg(message); 
        }
        else if (type == LogType.Warning)
        {
            ShowWarningMsg(message);
        }
        else
        {
            ShowErrorMsg(message + stackTrace);
        }
    }
#elif UNITY_STANDALONE_WIN

    void ShowConsole()
    {
        ConsoleHelper.ClearConsole();
    }

    void HandleLog(string message, string stackTrace, LogType type)
    {
        message = message + "\n";
        ConsoleHelper.Write(type, message);
    }
#else
    void ShowConsole()
    {

    }

    void HandleLog(string message, string stackTrace, LogType type)
    {

    }
#endif

#if !UNITY_EDITOR
    void Awake()
    {
        ShowConsole();
        Application.logMessageReceived += HandleLog;
    }
    
    void OnDestroy()
    {
        Application.logMessageReceived -= HandleLog;
    }
#endif
}
