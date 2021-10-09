
#import <Cocoa/Cocoa.h>

@interface ConsoleWindowController : NSWindowController
{
    NSTextView *textView;
    NSClipView *clipView;
    NSScroller *vScroller;
    NSScrollView *scrollView;
    
    IBOutlet NSButton *checkScroll;
    IBOutlet NSButton *topCheckBox;
    NSMutableArray *linesCount;
    NSUInteger traceCount;
    //console pipe
    NSPipe *_pipe;
    NSFileHandle *_pipeReadHandle;
    
    BOOL _autoScrollDirty;
    BOOL _autoTopDirty;
}

@property (assign) IBOutlet NSTextView *textView;
@property (assign) IBOutlet NSClipView *clipView;
@property (assign) IBOutlet NSScroller *vScroller;
@property (assign) IBOutlet NSScrollView *scrollView;

- (void) addMsg:(NSString*)msg with:(NSColor*)color;
- (void) trace:(NSString*)msg;
- (IBAction)onClear:(id)sender;
- (IBAction)onScrollChange:(id)sender;
- (IBAction)onTopChange:(id)sender;
- (void)initPipe;
@end



