#import <Foundation/Foundation.h>
#import <AppKit/AppKit.h>

#import "ConsoleWindowController.h"
#import "ConsoleWindow.h"
#import "Utils.h"

int GetVersion()
{
    return 100;
}

ConsoleWindowController* _consoleController;
int ShowConsoleWin()
{
    if (!_consoleController)
    {
        _consoleController = [[ConsoleWindowController alloc] initWithWindowNibName:@"ConsoleWindow"];
    }
    [_consoleController.window orderFrontRegardless];

    return 0;
}

void ShowErrorMsg(char* msg)
{
    [_consoleController addMsg:[NSString stringWithUTF8String:msg] with:[NSColor redColor]];
}

void ShowLogMsg(char* msg)
{
    [_consoleController addMsg:[NSString stringWithUTF8String:msg] with:[NSColor grayColor]];
}

void ShowWarningMsg(char* msg)
{
    [_consoleController addMsg:[NSString stringWithUTF8String:msg] with:[NSColor yellowColor]];
}

double GetCurScreenScaleFactor()
{
	return [[NSScreen mainScreen] backingScaleFactor];
}


void OpenFileWindow(char* _path, char* _msg, SelectFileDelegate delegate)
{
    NSString* path = [NSString stringWithUTF8String:_path];
    NSString* msg = [NSString stringWithUTF8String:_msg];
    [Utils openFileWindow:path WithMessage:msg WithDelegate:delegate];
}
