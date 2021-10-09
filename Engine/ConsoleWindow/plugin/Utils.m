//
//  Utils.m
//  ConsoleWindow
//
//  Created by litianxing on 2019/12/11.
//  Copyright © 2019 yk. All rights reserved.
//

#import "Utils.h"

@implementation Utils

+(void)setBoolKey:(NSString*) key withValue:(BOOL) value
{
    NSNumber *boolNumber = [NSNumber numberWithBool:value];
    [[NSUserDefaults standardUserDefaults] setObject:boolNumber forKey:key];
    [[NSUserDefaults standardUserDefaults] synchronize];
}
+(void)setIntKey:(NSString*) key withValue:(int) value
{
    NSNumber *intNumber = [NSNumber numberWithInt:value];
    [[NSUserDefaults standardUserDefaults] setObject:intNumber forKey:key];
    [[NSUserDefaults standardUserDefaults] synchronize];
}
+(void)setStringKey:(NSString*) key withValue:(NSString*) value
{
    [[NSUserDefaults standardUserDefaults] setObject:value forKey:key];
    [[NSUserDefaults standardUserDefaults] synchronize];
}
+(void)setFloatKey:(NSString*) key withValue:(float) value
{
    NSNumber *floatNumber = [NSNumber numberWithFloat:value];
    [[NSUserDefaults standardUserDefaults] setObject:floatNumber forKey:key];
    [[NSUserDefaults standardUserDefaults] synchronize];
}

+(BOOL)getBoolKey:(NSString*) key
{
    NSNumber* val = [[NSUserDefaults standardUserDefaults] objectForKey:key];
    return [val boolValue];
}
+(int)getIntKey:(NSString*) key
{
    NSNumber* val = [[NSUserDefaults standardUserDefaults] objectForKey:key];
    return [val intValue];
}
+(NSString*)getStringKey:(NSString*) key
{
    NSString* val = [[NSUserDefaults standardUserDefaults] objectForKey:key];
    return val;
}
+(float)getFloatKey:(NSString*) key
{
    NSNumber* val = [[NSUserDefaults standardUserDefaults] objectForKey:key];
    return [val floatValue];
}

+ (long long)fileSizeAtPath:(NSString*)filePath
{
    NSFileManager* manager = [NSFileManager defaultManager];
    if ([manager fileExistsAtPath:filePath]){
        return [[manager attributesOfItemAtPath:filePath error:nil] fileSize];
    }
    return 0;
}

+(void)openFileWindow:(NSString*) path WithMessage:(NSString*) msg WithDelegate:(SelectFileDelegate) delegate
{
    NSOpenPanel *openPanel = [NSOpenPanel openPanel];
    [openPanel setAllowsMultipleSelection:NO];
//    [openPanel setAllowedFileTypes:[NSImage imageTypes]];
    [openPanel setMessage:msg];
  
    [openPanel beginSheetModalForWindow:[NSApp mainWindow] completionHandler:^(NSModalResponse result) {
        if (result == NSModalResponseOK)
        {
            NSURL* url = [openPanel URL];
            NSString* path = [url absoluteString];
            delegate([path UTF8String]);
        }
    }];
}
@end

