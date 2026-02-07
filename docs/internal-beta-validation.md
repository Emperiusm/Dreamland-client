# Internal Beta Validation (Scan → Room Navigation)

## Preconditions
- Backend services running (API, worker, orchestrator, MinIO, NATS).
- Seeded internal beta user created via backend script.
- Unity project opened with `Assets/Scenes/InternalBeta.unity`.
- `Assets/Config/DreamlandConfig.asset` updated with API + token values.
- iOS build includes `NSCameraUsageDescription` in Info.plist.
- Device is LiDAR-capable and running iOS 16+ (iPhone 15 Pro is supported).

## Build & Deploy to iPhone (Unity → Xcode → Device)
1. Install Unity Hub (the launcher that installs Unity versions).
2. In Unity Hub, install Unity **6000.3 LTS** and include **iOS Build Support**.
3. Add the project to Unity Hub:
   - Projects tab → **Open** → select the `Dreamland-client` folder.
4. Open the project from Unity Hub.
5. If Unity prompts to upgrade project files, click **Continue**.
6. Package Manager: update packages to latest compatible versions.
7. Upgrade URP (Universal Render Pipeline) to 17.x and update dependent packages.
8. Ensure a URP Pipeline Asset is assigned in Project Settings -> Graphics.
9. Open scene: `Assets/Scenes/InternalBeta.unity`.
10. Set config in `Assets/Config/DreamlandConfig.asset`.
11. Open **File → Build Settings**:
   - Platform: **iOS**
   - Click **Switch Platform**
   - Add scene `InternalBeta` to **Scenes In Build**
12. Player Settings:
   - **Company Name** and **Product Name** set
   - **Bundle Identifier** unique (e.g., `com.emperiusm.dreamland.internal`)
   - **Target iOS Version**: 16.0+
13. Click **Build** and choose an output folder (e.g., `Builds/iOS`).
14. Open the generated Xcode project.
15. In Xcode:
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
