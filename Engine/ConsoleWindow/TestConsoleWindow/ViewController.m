//
//  ViewController.m
//  TestConsoleWindow
//
//  Created by Tiger on 2019/8/27.
//  Copyright © 2019 yk. All rights reserved.
//

#import "ViewController.h"
#import "ConsoleWindow.h"

@implementation ViewController

- (void)viewDidLoad {
	[super viewDidLoad];

	// Do any additional setup after loading the view.
	
	ShowConsoleWin();
	
	int i = 0;
	while(true){
		int v = i % 3;
		if (v == 0) {
			ShowErrorMsg("Error Message!\n");
		} else if( v == 1) {
			ShowWarningMsg("Warning Message!\n");
		} else {
			ShowLogMsg("Log Message!\n");
		}
//		usleep(2000000);
		printf("tip:%d\n",i);
		++i;
		if (i > 100) {
			break;
		}
	}
}


- (void)setRepresentedObject:(id)representedObject {
	[super setRepresentedObject:representedObject];

	// Update the view, if already loaded.
}


@end
