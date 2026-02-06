# RoomPlan Plugin (iOS)

## Overview
Unity uses a native iOS plugin to access RoomPlan (RoomCaptureSession). The plugin exports a USDZ and a JSON metadata file into the app Documents directory and sends capture events back to Unity.

## API Surface
C interface (Unity interop):
- `rp_start_capture()`
- `rp_stop_capture()`
- `rp_export_capture_usdz()` → C string path to `room.usdz`
- `rp_export_capture_json()` → C string path to `room.json`
- `rp_free_string(ptr)`

Unity C# wrapper: `Dreamland.iOS.RoomPlanCapture`.

## Export Location
`<Documents>/Dreamland/room.usdz`
`<Documents>/Dreamland/room.json`

## Capture Events
Events sent to Unity GameObject `RoomPlanBridge` method `OnCaptureEvent`:
- `capture_started`
- `capture_completed`
- `capture_failed`

Payload includes `usdz_path`, `json_path`, and `error`.

## Requirements
- iOS 16+
- LiDAR-capable device for RoomPlan

## Metadata Shaping
`RoomPlanMetadata` parses the exported JSON and maps fields into the manifest builder. Missing values fall back to defaults.

## TODO
- Progress callbacks (RoomCaptureSession updates).
- Structured metadata conversion for all capture fields.
- Permission error surfacing in Unity UI.
