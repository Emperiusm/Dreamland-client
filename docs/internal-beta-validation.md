# Internal Beta Validation (Scan → Room Navigation)

## Preconditions
- Backend services running (API, worker, orchestrator, MinIO, NATS).
- Seeded internal beta user created via backend script.
- Unity project opened with `Assets/Scenes/InternalBeta.unity`.
- `Assets/Config/DreamlandConfig.asset` updated with API + token values.
- iOS build includes `NSCameraUsageDescription` in Info.plist.
- Device is LiDAR-capable and running iOS 16+.

## Validation Steps
1. Press Play in Unity.
2. Confirm device passes RoomPlan gating (iOS 16+ and LiDAR).
3. Accept camera permission when prompted.
4. Start capture, wait for auto stop, and export artifacts.
5. Upload USDZ + JSON and commit scan (with retry/backoff).
6. Confirm `/scans/:scan_id/status` reaches `ready`.
7. Confirm `/rooms/:room_id/bundle` returns a manifest URL.
8. Confirm room meshes load (glTFast) and colliders attach.
9. Confirm `room_load_success` and `movement_start` events are emitted.
10. Validate telemetry events for scan/processing/bundle in backend store.

## Pass/Fail Criteria
- PASS if scan completes, room loads, and player can move without clipping.
- FAIL if processing does not complete, bundle is missing, or movement fails.

## Notes
- RoomPlan capture requires iOS 16+ and LiDAR devices.
- If bundle loading fails, check manifest URL and asset paths.
- Asset cache uses `Application.persistentDataPath` and is keyed by bundle hash.
