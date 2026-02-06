#import <Foundation/Foundation.h>
#import "RoomPlanBridge.h"
#import "UnityInterface.h"

extern "C" {
  void RoomPlan_StartCapture(void);
  void RoomPlan_StopCapture(void);
  const char *RoomPlan_ExportUSDZ(void);
  const char *RoomPlan_ExportMetadata(void);
}

void rp_start_capture(void) {
  RoomPlan_StartCapture();
}

void rp_stop_capture(void) {
  RoomPlan_StopCapture();
}

const char *rp_export_capture_usdz(void) {
  return RoomPlan_ExportUSDZ();
}

const char *rp_export_capture_json(void) {
  return RoomPlan_ExportMetadata();
}

void rp_free_string(const char *value) {
  if (value != NULL) {
    free((void *)value);
  }
}
