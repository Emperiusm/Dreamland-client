# RoomPlan Plugin (iOS)

## Overview
Unity uses a native iOS plugin to access RoomPlan (RoomCaptureSession). The plugin exports a USDZ and a JSON metadata file into the app Documents directory.

## API Surface
C interface (Unity interop):
- `rp_start_capture()`
- `rp_stop_capture()`
- `rp_export_capture_usdz()` → C string path to `room.usdz`
- `rp_export_capture_json()` → C string path to `room.json`
- `rp_free_string(ptr)`

C# wrapper: `Dreamland.iOS.RoomPlanCapture`.

## Export Location
`<Documents>/Dreamland/room.usdz`
`<Documents>/Dreamland/room.json`

## Requirements
- iOS 16+
- LiDAR-capable device for RoomPlan

## TODO
- Implement `RoomCaptureSession` delegate hooks for progress + errors.
- Capture clean metadata JSON (currently uses `rawValue`).
- Surface permission errors to Unity.
