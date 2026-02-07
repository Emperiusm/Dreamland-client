# Unity 6000.3 (LTS) Upgrade Plan

## Goal
Move the client from Unity 2022.3 LTS to Unity 6000.3 LTS with minimal risk, preserving internal beta flow (scan -> upload -> bundle -> move).

## References
- Unity upgrade guidance: upgrade sequentially, update packages before opening in the new editor, and test after migration.
- URP 17 is required for Unity 6000.3 LTS.

## Branch
- Create: `upgrade/unity-6000`

## Phase 1: Stabilize on 2022.3
1. Open project in Unity 2022.3 LTS.
2. Package Manager: update packages to latest compatible versions.
3. Save, close Unity.
4. Commit changes in `Packages/manifest.json` (and `Packages/packages-lock.json` if created).
5. Run internal beta validation.

## Phase 2: 2023 LTS hop
1. Install Unity 2023 LTS in Unity Hub (with iOS Build Support).
2. Open project in Unity 2023 LTS.
3. Let Unity upgrade assets.
4. Package Manager: update packages.
5. Fix compile/runtime issues.
6. Run internal beta validation.

## Phase 3: Unity 6000.3 LTS
1. Install Unity 6000.3 LTS in Unity Hub (with iOS Build Support).
2. Open project in Unity 6000.3 LTS.
3. Upgrade URP to 17.x and update dependent packages.
4. Ensure a URP Pipeline Asset is assigned in Project Settings -> Graphics.
5. Run internal beta validation.

## Validation Checklist
- App launches on device.
- RoomPlan capture works (iOS 16+, LiDAR device).
- Upload succeeds.
- `/scans/:scan_id/status` reaches `ready`.
- `/rooms/:room_id/bundle` returns manifest URL.
- `RoomBundleLoader` loads `mesh_lod0` and colliders.
- Player movement works (no clipping).
- Telemetry events emitted.

## Expected File Changes
- `ProjectSettings/ProjectVersion.txt`
- `Packages/manifest.json`
- `Packages/packages-lock.json` (if generated)
- `ProjectSettings/GraphicsSettings.asset` (if created/updated)
