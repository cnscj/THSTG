//
//  NSTextView+HandleKeyEvent.m
//  ConsoleWindow
//
//  Created by Tiger on 2019/8/27.
//  Copyright © 2019 yk. All rights reserved.
//

#import "NSTextView+HandleKeyEvent.h"
#import <AppKit/AppKit.h>

@implementation NSTextView (HandleKeyEvent)

- (BOOL)performKeyEquivalent:(NSEvent *)event
{
	if (([event modifierFlags] & NSEventModifierFlagDeviceIndependentFlagsMask) == NSEventModifierFlagCommand) {
		// The command key is the ONLY modifier key being pressed.
		if ([[event charactersIgnoringModifiers] isEqualToString:@"x"]) {
			return [NSApp sendAction:@selector(cut:) to:[[self window] firstResponder] from:self];
		} else if ([[event charactersIgnoringModifiers] isEqualToString:@"c"]) {
			return [NSApp sendAction:@selector(copy:) to:[[self window] firstResponder] from:self];
		} else if ([[event charactersIgnoringModifiers] isEqualToString:@"v"]) {
			return [NSApp sendAction:@selector(paste:) to:[[self window] firstResponder] from:self];
		} else if ([[event charactersIgnoringModifiers] isEqualToString:@"a"]) {
			return [NSApp sendAction:@selector(selectAll:) to:[[self window] firstResponder] from:self];
		}
	}
	return [super performKeyEquivalent:event];
}

@end
