#import <Foundation/Foundation.h>
#import "RoomPlanBridge.h"

static NSString *lastExportPath = @"";

void rp_start_capture(void) {
  NSLog(@"[RoomPlan] start_capture (stub)");
}

void rp_stop_capture(void) {
  NSLog(@"[RoomPlan] stop_capture (stub)");
}

const char *rp_export_capture(void) {
  NSLog(@"[RoomPlan] export_capture (stub)");
  lastExportPath = @"";
  return [lastExportPath UTF8String];
}
