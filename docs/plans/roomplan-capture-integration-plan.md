# RoomPlan Capture Integration Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Add real RoomPlan capture + device gating + permission handling + real artifact upload to the Unity client.

**Architecture:** Unity calls a native iOS RoomPlan plugin to start/stop capture and export USDZ + JSON. Unity gates capture by device/OS, requests permissions, uploads artifacts to presigned URLs, and emits telemetry events. Native plugin reports progress/completion via UnitySendMessage callbacks.

**Tech Stack:** Unity 6000.3 LTS, iOS RoomPlan (Swift), Unity C# scripts, glTFast, UnityWebRequest.

---

### Task 1: Add device/OS gating and permission checks

**Files:**
- Create: `Assets/Scripts/iOS/DeviceGate.cs`
- Modify: `Assets/Scripts/Bootstrap.cs`
- Test: `Assets/Tests/DeviceGateTests.cs`

**Step 1: Write the failing test**

```csharp
using NUnit.Framework;
using Dreamland.iOS;

public class DeviceGateTests
{
    [Test]
    public void IsSupported_ReturnsFalse_ForUnsupportedOS()
    {
        var result = DeviceGate.IsSupported("15.0", true);
        Assert.IsFalse(result);
    }
}
```

**Step 2: Run test to verify it fails**

Run: Unity EditMode tests for `DeviceGateTests`
Expected: FAIL with missing DeviceGate.

**Step 3: Write minimal implementation**

```csharp
namespace Dreamland.iOS
{
    public static class DeviceGate
    {
        public static bool IsSupported(string osVersion, bool hasLiDAR)
        {
            if (!hasLiDAR) return false;
            if (!Version.TryParse(osVersion, out var parsed)) return false;
            return parsed.Major >= 16;
        }
    }
}
```

**Step 4: Run test to verify it passes**

Run: Unity EditMode tests for `DeviceGateTests`
Expected: PASS

**Step 5: Commit**

```bash
git add Assets/Scripts/iOS/DeviceGate.cs Assets/Tests/DeviceGateTests.cs Assets/Scripts/Bootstrap.cs
git commit -m "feat: add device gating for RoomPlan"
```

---

### Task 2: Add native capture callbacks and Unity bridge

**Files:**
- Modify: `Assets/Plugins/iOS/RoomPlanBridge.swift`
- Modify: `Assets/Plugins/iOS/RoomPlanBridge.mm`
- Modify: `Assets/Scripts/iOS/RoomPlanCapture.cs`
- Test: `Assets/Tests/RoomPlanBridgeTests.cs`

**Step 1: Write the failing test**

```csharp
using NUnit.Framework;
using Dreamland.iOS;

public class RoomPlanBridgeTests
{
    [Test]
    public void CaptureState_DefaultsToIdle()
    {
        Assert.AreEqual(RoomPlanCapture.CaptureState.Idle, RoomPlanCapture.State);
    }
}
```

**Step 2: Run test to verify it fails**

Run: Unity EditMode tests for `RoomPlanBridgeTests`
Expected: FAIL (State not implemented).

**Step 3: Implement minimal callback state**

- Add UnitySendMessage hooks in Swift to notify capture status.
- Track `CaptureState` in C# (Idle/Running/Completed/Failed).
- Add `OnCaptureEvent(string payload)` handler in C#.

**Step 4: Run test to verify it passes**

Run: Unity EditMode tests for `RoomPlanBridgeTests`
Expected: PASS

**Step 5: Commit**

```bash
git add Assets/Plugins/iOS/RoomPlanBridge.swift Assets/Plugins/iOS/RoomPlanBridge.mm Assets/Scripts/iOS/RoomPlanCapture.cs Assets/Tests/RoomPlanBridgeTests.cs
git commit -m "feat: add RoomPlan capture callbacks"
```

---

### Task 3: Replace placeholder upload with real USDZ + JSON

**Files:**
- Modify: `Assets/Scripts/Bootstrap.cs`
- Modify: `Assets/Scripts/DreamlandApiClient.cs`
- Test: `Assets/Tests/ManifestBuilderTests.cs`

**Step 1: Write the failing test**

```csharp
using NUnit.Framework;
using Dreamland;

public class ManifestBuilderTests
{
    [Test]
    public void ManifestBuilder_IncludesUSDZArtifact()
    {
        var manifest = ManifestBuilder.Build("scan-id", "user-id", "room.usdz", 123, "sha256");
        Assert.IsTrue(manifest.Contains("room.usdz"));
    }
}
```

**Step 2: Run test to verify it fails**

Run: Unity EditMode tests for `ManifestBuilderTests`
Expected: FAIL (ManifestBuilder missing).

**Step 3: Implement minimal manifest builder and upload**

- Add a manifest builder that takes USDZ path + JSON metadata.
- Update `Bootstrap` to call `RoomPlanCapture.ExportCapture()` and upload the real files.
- Update `DreamlandApiClient` to support file upload by path.

**Step 4: Run test to verify it passes**

Run: Unity EditMode tests for `ManifestBuilderTests`
Expected: PASS

**Step 5: Commit**

```bash
git add Assets/Scripts/Bootstrap.cs Assets/Scripts/DreamlandApiClient.cs Assets/Scripts/ManifestBuilder.cs Assets/Tests/ManifestBuilderTests.cs
git commit -m "feat: upload real RoomPlan artifacts"
```

---

### Task 4: Update docs with capture flow

**Files:**
- Modify: `docs/internal-beta-validation.md`
- Modify: `docs/roomplan-plugin.md`

**Step 1: Update documentation**

- Add steps for starting/stopping capture and exporting artifacts.
- Document device gating and permission handling.

**Step 2: Commit**

```bash
git add docs/internal-beta-validation.md docs/roomplan-plugin.md
git commit -m "docs: update capture flow guidance"
```

---

Plan complete and saved to `docs/plans/2026-02-06-roomplan-capture-integration-plan.md`.

Two execution options:

1. Subagent-Driven (this session) - I dispatch fresh subagent per task, review between tasks, fast iteration
2. Parallel Session (separate) - Open new session with executing-plans, batch execution with checkpoints

Which approach?
