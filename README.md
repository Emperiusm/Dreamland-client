# Dreamland Client (Unity)

Unity client for internal beta: scan → process → room bundle → move around.

## Requirements
- Unity **2022.3 LTS** (recommended: `2022.3.18f1`)
- Git LFS installed for binary assets

## Quick Start
1. Open the project in Unity Hub.
2. Open `Assets/Scenes/InternalBeta.unity`.
3. Select `Assets/Config/DreamlandConfig.asset` and set API values.
4. Press Play.

## API Contracts
The client expects the following endpoints from the server:
- `POST /scans/privacy-checklist`
- `POST /scans/session`
- `POST /scans/:scan_id/commit`
- `GET /scans/:scan_id/status`
- `GET /scans/:scan_id/room`
- `GET /rooms/:room_id/bundle`
- `POST /events`

See `docs/DEV_SETUP.md` in the backend repo for local infra setup and seed user steps.

## Structure
- `Assets/Scripts/` - API client, bundle loader, and player controller
- `Assets/Tests/` - EditMode tests for JSON parsing and URL resolution

## Notes
- The current flow uploads a placeholder manifest and asset to exercise the pipeline.
- `RoomBundleLoader` uses glTFast to load processed meshes when available.
