# Internal Beta Validation (Scan → Room Navigation)

## Preconditions
- Backend services running (API, worker, orchestrator, MinIO, NATS).
- Seeded internal beta user created via backend script.
- Unity project opened with `Assets/Scenes/InternalBeta.unity`.
- `Assets/Config/DreamlandConfig.asset` updated with API + token values.

## Validation Steps
1. Press Play in Unity.
2. Confirm `scan_start` and `scan_upload_complete` events are emitted.
3. Confirm `/scans/:scan_id/status` reaches `ready` in backend logs.
4. Confirm `/rooms/:room_id/bundle` returns a manifest URL.
5. Confirm room meshes load (glTFast) or placeholder appears if missing.
6. Confirm `room_load_success` and `movement_start` events are emitted.

## Pass/Fail Criteria
- PASS if scan completes, room loads, and player can move without clipping.
- FAIL if processing does not complete, bundle is missing, or movement fails.

## Notes
- RoomPlan capture is stubbed in Unity and requires native iOS plugin implementation.
- If bundle loading fails, check manifest URL and asset paths.
