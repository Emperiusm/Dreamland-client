import Foundation
import RoomPlan

@_silgen_name("UnitySendMessage")
func UnitySendMessage(_ obj: UnsafePointer<CChar>, _ method: UnsafePointer<CChar>, _ msg: UnsafePointer<CChar>)

@objc public class RoomPlanBridge: NSObject {
    private static var captureSession: RoomCaptureSession?
    private static var delegate: CaptureDelegate?
    private static var lastUSDZPath: String?
    private static var lastMetadataPath: String?

    @objc public static func startCapture() {
        if #available(iOS 16.0, *) {
            let session = RoomCaptureSession()
            let delegate = CaptureDelegate()
            captureSession = session
            RoomPlanBridge.delegate = delegate
            session.delegate = delegate
            session.run(configuration: RoomCaptureSession.Configuration())
            sendEvent(type: "capture_started", usdzPath: nil, jsonPath: nil, error: nil)
        } else {
            sendEvent(type: "capture_failed", usdzPath: nil, jsonPath: nil, error: "iOS 16 required")
        }
    }

    @objc public static func stopCapture() {
        if #available(iOS 16.0, *) {
            captureSession?.stop()
        }
    }

    @objc public static func exportCapture() {
        if #available(iOS 16.0, *) {
            guard let session = captureSession else {
                sendEvent(type: "capture_failed", usdzPath: nil, jsonPath: nil, error: "no session")
                return
            }

            let exportURL = FileManager.default.urls(for: .documentDirectory, in: .userDomainMask)[0]
                .appendingPathComponent("Dreamland")
            let usdzURL = exportURL.appendingPathComponent("room.usdz")
            let jsonURL = exportURL.appendingPathComponent("room.json")

            do {
                try FileManager.default.createDirectory(at: exportURL, withIntermediateDirectories: true)
            } catch {
                sendEvent(type: "capture_failed", usdzPath: nil, jsonPath: nil, error: "export_dir_failed")
            }

            session.captureResults?.export(to: usdzURL)
            if let metadata = session.captureResults?.rawValue {
                do {
                    try metadata.data(using: .utf8)?.write(to: jsonURL)
                } catch {
                    sendEvent(type: "capture_failed", usdzPath: nil, jsonPath: nil, error: "metadata_write_failed")
                }
            }

            lastUSDZPath = usdzURL.path
            lastMetadataPath = jsonURL.path
            sendEvent(type: "capture_completed", usdzPath: lastUSDZPath, jsonPath: lastMetadataPath, error: nil)
        }
    }

    @objc public static func exportUSDZPath() -> String {
        return lastUSDZPath ?? ""
    }

    @objc public static func exportMetadataPath() -> String {
        return lastMetadataPath ?? ""
    }

    private static func sendEvent(type: String, usdzPath: String?, jsonPath: String?, error: String?) {
        let payload: [String: String] = [
            "type": type,
            "usdz_path": usdzPath ?? "",
            "json_path": jsonPath ?? "",
            "error": error ?? ""
        ]
        if let data = try? JSONSerialization.data(withJSONObject: payload, options: []),
           let json = String(data: data, encoding: .utf8) {
            json.withCString { msg in
                "RoomPlanBridge".withCString { obj in
                    "OnCaptureEvent".withCString { method in
                        UnitySendMessage(obj, method, msg)
                    }
                }
            }
        }
    }
}

@available(iOS 16.0, *)
private class CaptureDelegate: NSObject, RoomCaptureSessionDelegate {
    func captureSession(_ session: RoomCaptureSession, didUpdate room: CapturedRoom) {
        // Optional: future progress callbacks.
        _ = room
    }

    func captureSession(_ session: RoomCaptureSession, didEndWith data: CapturedRoomData, error: Error?) {
        if let error = error {
            RoomPlanBridge.sendEvent(type: "capture_failed", usdzPath: nil, jsonPath: nil, error: error.localizedDescription)
            return
        }
    }
}

@_cdecl("RoomPlan_StartCapture")
public func RoomPlan_StartCapture() {
    RoomPlanBridge.startCapture()
}

@_cdecl("RoomPlan_StopCapture")
public func RoomPlan_StopCapture() {
    RoomPlanBridge.stopCapture()
}

@_cdecl("RoomPlan_ExportUSDZ")
public func RoomPlan_ExportUSDZ() -> UnsafePointer<CChar>? {
    RoomPlanBridge.exportCapture()
    return strdup(RoomPlanBridge.exportUSDZPath())
}

@_cdecl("RoomPlan_ExportMetadata")
public func RoomPlan_ExportMetadata() -> UnsafePointer<CChar>? {
    return strdup(RoomPlanBridge.exportMetadataPath())
}
