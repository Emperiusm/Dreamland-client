# Internal Beta Validation (Scan → Room Navigation)

## Preconditions
- Backend services running (API, worker, orchestrator, MinIO, NATS).
- Seeded internal beta user created via backend script.
- Unity project opened with `Assets/Scenes/InternalBeta.unity`.
- `Assets/Config/DreamlandConfig.asset` updated with API + token values.
- iOS build includes `NSCameraUsageDescription` in Info.plist.
- Device is LiDAR-capable and running iOS 16+ (iPhone 15 Pro is supported).

## Build & Deploy to iPhone (Unity → Xcode → Device)
1. Install Unity **2022.3.18f1** with iOS Build Support via Unity Hub.
   - If you're upgrading to Unity 6000.3 LTS, follow the plan in `docs/unity-6000-upgrade-plan.md` first.
2. Open project in Unity Hub: `Dreamland-client`.
3. Open scene: `Assets/Scenes/InternalBeta.unity`.
4. Set config in `Assets/Config/DreamlandConfig.asset`.
5. Open **File → Build Settings**:
   - Platform: **iOS**
   - Click **Switch Platform**
   - Add scene `InternalBeta` to **Scenes In Build**
6. Player Settings:
   - **Company Name** and **Product Name** set
   - **Bundle Identifier** unique (e.g., `com.emperiusm.dreamland.internal`)
   - **Target iOS Version**: 16.0+
7. Click **Build** and choose an output folder (e.g., `Builds/iOS`).
8. Open the generated Xcode project.
9. In Xcode:
   - Select the target → **Signing & Capabilities**
   - Choose your Apple ID team
   - Enable **Automatically manage signing**
   - Connect iPhone 15 Pro via USB
   - Select device and **Run**

## Validation Steps
1. Launch app on device.
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
- BlueStacks cannot be used (Android only, no ARKit/RoomPlan).
