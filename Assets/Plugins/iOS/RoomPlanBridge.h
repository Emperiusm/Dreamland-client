#pragma once

#ifdef __cplusplus
extern "C" {
#endif

void rp_start_capture(void);
void rp_stop_capture(void);
const char *rp_export_capture(void);

#ifdef __cplusplus
}
#endif
