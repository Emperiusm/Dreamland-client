#pragma once

#ifdef __cplusplus
extern "C" {
#endif

void rp_start_capture(void);
void rp_stop_capture(void);
const char *rp_export_capture_usdz(void);
const char *rp_export_capture_json(void);
void rp_free_string(const char *value);

#ifdef __cplusplus
}
#endif
