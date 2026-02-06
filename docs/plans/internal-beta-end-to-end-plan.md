# Internal Beta End-to-End Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Deliver a fully working internal beta pipeline: RoomPlan capture → upload → processing → GLB load → movement → telemetry, with proper gating, retries, and validation.

**Architecture:** Unity client handles capture, permissions, upload retries, and runtime loading via glTFast. Native iOS RoomPlan plugin exports USDZ + JSON metadata. Backend already provides presigned URLs and processing pipeline; this plan focuses on client reliability, schema validation, and UX.

**Tech Stack:** Unity 2022.3 LTS, iOS RoomPlan (Swift), Unity C#, glTFast, UnityWebRequest.

---

### Task 1: Create unified end-to-end checklist in Unity

**Files:**
- Create: `Assets/Scripts/RunbookChecklist.cs`
- Modify: `Assets/Scripts/Bootstrap.cs`
- Test: `Assets/Tests/RunbookChecklistTests.cs`

**Step 1: Write the failing test**

```csharp
using NUnit.Framework;
using Dreamland;

public class RunbookChecklistTests
{
    [Test]
    public void ChecklistContainsCoreSteps()
    {
        var list = RunbookChecklist.Steps;
        Assert.IsTrue(list.Contains("capture"));
        Assert.IsTrue(list.Contains("upload"));
        Assert.IsTrue(list.Contains("processing"));
        Assert.IsTrue(list.Contains("bundle"));
        Assert.IsTrue(list.Contains("movement"));
    }
}
```

**Step 2: Run test to verify it fails**

Run: Unity EditMode tests for `RunbookChecklistTests`
Expected: FAIL (class missing).

**Step 3: Write minimal implementation**

```csharp
namespace Dreamland
{
    public static class RunbookChecklist
    {
        public static readonly string[] Steps = {
            "capture", "upload", "processing", "bundle", "movement"
        };
    }
}
```

**Step 4: Run test to verify it passes**

Run: Unity EditMode tests for `RunbookChecklistTests`
Expected: PASS

**Step 5: Commit**

```bash
git add Assets/Scripts/RunbookChecklist.cs Assets/Tests/RunbookChecklistTests.cs Assets/Scripts/Bootstrap.cs
git commit -m "feat: add end-to-end runbook checklist"
```

---

### Task 2: Add Info.plist permission strings

**Files:**
- Create: `Assets/Plugins/iOS/Info.plist`
- Modify: `Assets/Plugins/iOS/RoomPlanBridge.mm.meta`

**Step 1: Write the failing test**

```csharp
using NUnit.Framework;
using System.IO;

public class InfoPlistTests
{
    [Test]
    public void InfoPlistContainsCameraUsage()
    {
        var plist = File.ReadAllText("Assets/Plugins/iOS/Info.plist");
        Assert.IsTrue(plist.Contains("NSCameraUsageDescription"));
    }
}
```

**Step 2: Run test to verify it fails**

Run: Unity EditMode tests for `InfoPlistTests`
Expected: FAIL (file missing).

**Step 3: Write minimal implementation**

Add `Info.plist` with:
- `NSCameraUsageDescription`
- `NSMicrophoneUsageDescription` (optional, if future voice)

**Step 4: Run test to verify it passes**

Run: Unity EditMode tests for `InfoPlistTests`
Expected: PASS

**Step 5: Commit**

```bash
git add Assets/Plugins/iOS/Info.plist Assets/Plugins/iOS/RoomPlanBridge.mm.meta Assets/Tests/InfoPlistTests.cs
git commit -m "feat: add iOS permission strings"
```

---

### Task 3: Improve device capability checks

**Files:**
- Modify: `Assets/Scripts/iOS/DeviceGate.cs`
- Test: `Assets/Tests/DeviceGateTests.cs`

**Step 1: Write failing test**

```csharp
[Test]
public void HasLiDAR_ReturnsFalse_ForNonProModel()
{
    Assert.IsFalse(DeviceGate.HasLiDAR("iPhone 14"));
}
```

**Step 2: Run test to verify it fails**

Run: Unity EditMode tests for `DeviceGateTests`
Expected: FAIL (current heuristic too permissive).

**Step 3: Implement minimal improvement**

- Add explicit allowlist for iPhone Pro models (12 Pro and later).
- Add allowlist for iPad Pro 2020+.

**Step 4: Run test to verify it passes**

Run: Unity EditMode tests for `DeviceGateTests`
Expected: PASS

**Step 5: Commit**

```bash
git add Assets/Scripts/iOS/DeviceGate.cs Assets/Tests/DeviceGateTests.cs
git commit -m "feat: tighten LiDAR capability checks"
```

---

### Task 4: Upload retry/backoff and error states

**Files:**
- Create: `Assets/Scripts/UploadRetry.cs`
- Modify: `Assets/Scripts/DreamlandApiClient.cs`
- Modify: `Assets/Scripts/Bootstrap.cs`
- Test: `Assets/Tests/UploadRetryTests.cs`

**Step 1: Write the failing test**

```csharp
using NUnit.Framework;
using Dreamland;

public class UploadRetryTests
{
    [Test]
    public void BackoffSequenceMatchesExpected()
    {
        var delays = UploadRetry.GetBackoffDelays(3);
        Assert.AreEqual(new float[] {1f, 2f, 4f}, delays);
    }
}
```

**Step 2: Run test to verify it fails**

Run: Unity EditMode tests for `UploadRetryTests`
Expected: FAIL (class missing).

**Step 3: Implement minimal retry helper**

- Exponential backoff delays.
- Allow max attempts from config.

**Step 4: Run test to verify it passes**

Run: Unity EditMode tests for `UploadRetryTests`
Expected: PASS

**Step 5: Commit**

```bash
git add Assets/Scripts/UploadRetry.cs Assets/Tests/UploadRetryTests.cs Assets/Scripts/DreamlandApiClient.cs Assets/Scripts/Bootstrap.cs
git commit -m "feat: add upload retries and error handling"
```

---

### Task 5: Manifest schema validation + golden fixture

**Files:**
- Create: `Assets/Tests/Fixtures/bundle_manifest.json`
- Create: `Assets/Tests/ManifestSchemaTests.cs`
- Modify: `Assets/Scripts/RoomBundleManifest.cs`

**Step 1: Write failing test**

```csharp
using NUnit.Framework;
using System.IO;
using Dreamland;

public class ManifestSchemaTests
{
    [Test]
    public void FixtureContainsRequiredFields()
    {
        var json = File.ReadAllText("Assets/Tests/Fixtures/bundle_manifest.json");
        var manifest = RoomBundleManifest.Parse(json);
        Assert.IsNotEmpty(manifest.bundleId);
        Assert.IsNotEmpty(manifest.assets["mesh_lod0"]);
    }
}
```

**Step 2: Run test to verify it fails**

Run: Unity EditMode tests for `ManifestSchemaTests`
Expected: FAIL (fixture missing).

**Step 3: Add fixture + minimal validation**

- Add `bundle_manifest.json` fixture using current worker output format.
- Update parser to handle missing fields safely.

**Step 4: Run test to verify it passes**

Run: Unity EditMode tests for `ManifestSchemaTests`
Expected: PASS

**Step 5: Commit**

```bash
git add Assets/Tests/Fixtures/bundle_manifest.json Assets/Tests/ManifestSchemaTests.cs Assets/Scripts/RoomBundleManifest.cs
git commit -m "feat: add manifest fixture validation"
```

---

### Task 6: RoomPlan metadata shaping

**Files:**
- Create: `Assets/Scripts/RoomPlanMetadata.cs`
- Modify: `Assets/Scripts/ManifestBuilder.cs`
- Test: `Assets/Tests/RoomPlanMetadataTests.cs`

**Step 1: Write failing test**

```csharp
using NUnit.Framework;
using Dreamland;

public class RoomPlanMetadataTests
{
    [Test]
    public void MetadataParserExtractsRoomCategory()
    {
        var json = "{\"roomCategory\":\"bedroom\"}";
        var metadata = RoomPlanMetadata.Parse(json);
        Assert.AreEqual("bedroom", metadata.RoomCategory);
    }
}
```

**Step 2: Run test to verify it fails**

Run: Unity EditMode tests for `RoomPlanMetadataTests`
Expected: FAIL (class missing).

**Step 3: Implement minimal parser**

- Parse JSON into a struct with roomCategory, duration, deviceModel, osVersion.
- Use fallback values if missing.

**Step 4: Run test to verify it passes**

Run: Unity EditMode tests for `RoomPlanMetadataTests`
Expected: PASS

**Step 5: Commit**

```bash
git add Assets/Scripts/RoomPlanMetadata.cs Assets/Tests/RoomPlanMetadataTests.cs Assets/Scripts/ManifestBuilder.cs
git commit -m "feat: shape RoomPlan metadata into manifest"
```

---

### Task 7: Add asset caching for GLB downloads

**Files:**
- Create: `Assets/Scripts/AssetCache.cs`
- Modify: `Assets/Scripts/RoomBundleLoader.cs`
- Test: `Assets/Tests/AssetCacheTests.cs`

**Step 1: Write failing test**

```csharp
using NUnit.Framework;
using Dreamland;

public class AssetCacheTests
{
    [Test]
    public void CacheKeyIncludesBundleHash()
    {
        var key = AssetCache.BuildKey("bundle123", "mesh/room.glb");
        Assert.IsTrue(key.Contains("bundle123"));
    }
}
```

**Step 2: Run test to verify it fails**

Run: Unity EditMode tests for `AssetCacheTests`
Expected: FAIL (class missing).

**Step 3: Implement cache**

- Use Application.persistentDataPath
- Cache by bundle hash + asset path

**Step 4: Run test to verify it passes**

Run: Unity EditMode tests for `AssetCacheTests`
Expected: PASS

**Step 5: Commit**

```bash
git add Assets/Scripts/AssetCache.cs Assets/Tests/AssetCacheTests.cs Assets/Scripts/RoomBundleLoader.cs
git commit -m "feat: add GLB asset cache"
```

---

### Task 8: Documentation update

**Files:**
- Modify: `docs/internal-beta-validation.md`
- Modify: `docs/roomplan-plugin.md`

**Step 1: Update docs**

- Add capture metadata shaping notes.
- Add upload retry and cache behavior.

**Step 2: Commit**

```bash
git add docs/internal-beta-validation.md docs/roomplan-plugin.md
git commit -m "docs: update beta flow with retries and cache"
```

---

Plan complete and saved to `docs/plans/2026-02-06-internal-beta-end-to-end-plan.md`.

Two execution options:

1. Subagent-Driven (this session) - I dispatch fresh subagent per task, review between tasks, fast iteration
2. Parallel Session (separate) - Open new session with executing-plans, batch execution with checkpoints

Which approach?
