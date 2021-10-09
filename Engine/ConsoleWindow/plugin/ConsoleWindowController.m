
#import "ConsoleWindowController.h"
#import "Utils.h"

@interface ConsoleWindowController ()

@end

#define SKIP_LINES_COUNT    3
#define MAX_LINE_LEN        40960
#define MAX_LINES_COUNT     300

@implementation ConsoleWindowController
@synthesize textView;
@synthesize clipView;
@synthesize vScroller;
@synthesize scrollView;

- (id)initWithWindow:(NSWindow *)window
{
    self = [super initWithWindow:window];
    if (self)
    {
        // Initialization code here.
        linesCount = [[NSMutableArray arrayWithCapacity:MAX_LINES_COUNT + 1] retain];
    }

    return self;
}

- (void)dealloc
{
    [linesCount release];
    [super dealloc];
}
- (void) loadWindow{
    [super loadWindow];
    
}
- (void)windowDidLoad
{
    [super windowDidLoad];
    // Implement this method to handle any initialization after your window controller's window has been loaded from its nib file.
    [self initPipe];
    [self initConfig];
    [self initEvent];
}

-(void)initConfig
{
    BOOL autoScroll = [Utils getBoolKey:@"autoScroll"];
    if (autoScroll)
    {
        [checkScroll setState:NSControlStateValueOn];
    }
    else
    {
        [checkScroll setState:NSControlStateValueOff];
    }
    _autoScrollDirty = autoScroll;
    [self changeScroll];
    
    BOOL autoTop = [Utils getBoolKey:@"autoTop"];
    if (autoTop)
    {
        [topCheckBox setState:NSControlStateValueOn];
    }
    else
    {
        [topCheckBox setState:NSControlStateValueOff];
    }
    _autoTopDirty = autoTop;
    [self changeTop];
}

- (void)initPipe{
//    set console pipe
    _pipe = [NSPipe pipe] ;
    _pipeReadHandle = [_pipe fileHandleForReading] ;

    int outfd = [[_pipe fileHandleForWriting] fileDescriptor];
    if (dup2(outfd, fileno(stderr)) != fileno(stderr) || dup2(outfd, fileno(stdout)) != fileno(stdout))
    {
        perror("Unable to redirect output");
        //        [self showAlert:@"Unable to redirect output to console!" withTitle:@"player error"];
    }
    else
    {
        [[NSNotificationCenter defaultCenter] addObserver: self selector: @selector(handleNotification:) name: NSFileHandleReadCompletionNotification object: _pipeReadHandle] ;
        [_pipeReadHandle readInBackgroundAndNotify] ;
    }
}
- (void)handleNotification:(NSNotification *)note
{
    //NSLog(@"Received notification: %@", note);
    [_pipeReadHandle readInBackgroundAndNotify] ;
    NSData *data = [[note userInfo] objectForKey:NSFileHandleNotificationDataItem];
    NSString *str = [[[NSString alloc] initWithData:data encoding:NSUTF8StringEncoding] autorelease];
    
    if (str)
    {
        //show log to console
        [self trace:str];
    }else{
    }
    
}

- (void) trace:(NSString*)msg
{
    NSColor *color = [NSColor grayColor];
    if([msg containsString:@"ERROR"])
        color = [NSColor redColor];
    else if([msg containsString:@"WARNING"])
        color = [NSColor yellowColor];
    [self addMsg:msg with:color];
}

-(void) initEvent
{

}

- (void) addMsg:(NSString*)msg with:(NSColor*) color
{
    if (traceCount >= SKIP_LINES_COUNT && [msg length] > MAX_LINE_LEN)
    {
        msg = [NSString stringWithFormat:@"%@ ...", [msg substringToIndex:MAX_LINE_LEN - 4]];
    }
    traceCount++;
	
	NSArray *types = [NSArray arrayWithObjects:[NSFont fontWithName:@"Monaco" size:12.0], color, nil];
	NSArray *attrs = [NSArray arrayWithObjects:NSFontAttributeName, NSForegroundColorAttributeName, nil];
	NSDictionary *attrsDictionary = [NSDictionary dictionaryWithObjects:types forKeys:attrs];
    NSAttributedString *string = [[NSAttributedString alloc] initWithString:msg attributes:attrsDictionary];
    NSNumber *len = [NSNumber numberWithUnsignedInteger:[string length]];
    [linesCount addObject:len];
	NSTextStorage *storage = [textView textStorage];
	[storage beginEditing];
    [storage appendAttributedString:string];
    if ([linesCount count] >= MAX_LINES_COUNT)
    {
        NSRange rg = textView.accessibilityVisibleCharacterRange;
        NSUInteger location = rg.location;
        NSUInteger length = rg.length;
        
        len = [linesCount objectAtIndex:0];
        NSUInteger lenuint = [len unsignedIntegerValue];
        [storage deleteCharactersInRange:NSMakeRange(0, lenuint)];
        
        [linesCount removeObjectAtIndex:0];
        [storage endEditing];
        
        if (location > 0 && location >= lenuint)
        {
            location -= lenuint;
        }
        else
        {
            location = 0;
            length = 0;
        }
        [textView scrollRangeToVisible: NSMakeRange(location, length)];
    }
    [textView setNeedsDisplay:true];
	[storage endEditing];
    
    [self changeScroll];
}

- (void) changeScroll
{
    BOOL scroll = ([checkScroll state] == NSControlStateValueOn);
    if(scroll)
    {
        [textView scrollRangeToVisible: NSMakeRange(textView.string.length, 0)];
    }

    if (_autoScrollDirty != scroll)
    {
        _autoScrollDirty = scroll;
        [Utils setBoolKey:@"autoScroll" withValue:scroll];
    }
}

-(void) changeTop
{
    BOOL isTop = ([topCheckBox state] == NSControlStateValueOn);
    if(isTop)
    {
        [self.window setLevel:NSFloatingWindowLevel];
    }
    else
    {
        [self.window setLevel:NSNormalWindowLevel];
    }
    if (_autoTopDirty != isTop)
    {
        _autoTopDirty = isTop;
        [Utils setBoolKey:@"autoTop" withValue:isTop];
    }
}

- (IBAction)onClear:(id)sender
{
    NSTextStorage *storage = [textView textStorage];
    [storage setAttributedString:[[[NSAttributedString alloc] initWithString:@""] autorelease]];
	[linesCount removeAllObjects];
}

- (IBAction)onScrollChange:(id)sender
{
    [self changeScroll];
}

- (IBAction)onTopChange:(id)sender
{
    [self changeTop];
}
@end
