//
//  Utils.h
//  ConsoleWindow
//
//  Created by litianxing on 2019/12/11.
//  Copyright © 2019 yk. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <Cocoa/Cocoa.h>
#import "ConsoleWindow.h"

NS_ASSUME_NONNULL_BEGIN

@interface Utils : NSObject

+(void)setBoolKey:(NSString*) key withValue:(BOOL) value;
+(void)setIntKey:(NSString*) key withValue:(int) value;
+(void)setStringKey:(NSString*) key withValue:(NSString*) value;
+(void)setFloatKey:(NSString*) key withValue:(float) value;

+(BOOL)getBoolKey:(NSString*) key;
+(NSString*)getStringKey:(NSString*) key;
+(int)getIntKey:(NSString*) key;
+(float)getFloatKey:(NSString*) key;

+(void)openFileWindow:(NSString*) path WithMessage:(NSString*) msg WithDelegate:(SelectFileDelegate)delegate;
+(long long)fileSizeAtPath:(NSString*) path;
@end

NS_ASSUME_NONNULL_END
