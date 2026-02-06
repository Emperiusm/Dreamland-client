# Dreamland Client (Unity)

Unity client for internal beta: scan → process → room bundle → move around.

## Requirements
- Unity **2022.3 LTS** (recommended: `2022.3.18f1`)
- Git LFS installed for binary assets

## Quick Start
1. Open the project in Unity Hub.
2. Create a scene named `InternalBeta` in `Assets/Scenes/`.
3. Add the `Bootstrap` prefab (create from `Assets/Scripts/Bootstrap.cs`) to the scene.
4. Enter API settings in the `Bootstrap` inspector.
5. Press Play.

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
- `Assets/Tests/` - EditMode tests for JSON parsing and config defaults

## Notes
- This is a scaffolding project. Scene and assets should be created in Unity.
- Runtime mesh instantiation is stubbed until real bundle assets are wired.
