#ifndef __ConsoleWindow_H__
#define __ConsoleWindow_H__

#if defined (__cplusplus)
extern "C" {
#endif
    typedef void (*SelectFileDelegate)(const char * path);

	int GetVersion(void);
	int ShowConsoleWin(void);
	void ShowErrorMsg(char* msg);
	void ShowLogMsg(char* msg);
	void ShowWarningMsg(char* msg);
	double GetCurScreenScaleFactor(void);

    void OpenFileWindow(char* path, char* tipMsg, SelectFileDelegate delegate);
	
#if defined (__cplusplus)
}
#endif

#endif
