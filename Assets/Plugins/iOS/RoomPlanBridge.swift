import Foundation
import RoomPlan

@objc public class RoomPlanBridge: NSObject {
    private static var captureSession: RoomCaptureSession?
    private static var lastUSDZPath: String?
    private static var lastMetadataPath: String?

    @objc public static func startCapture() {
        if #available(iOS 16.0, *) {
            let session = RoomCaptureSession()
            captureSession = session
            session.run(configuration: RoomCaptureSession.Configuration())
            NSLog("[RoomPlan] startCapture")
        } else {
            NSLog("[RoomPlan] RoomPlan requires iOS 16+")
        }
    }

    @objc public static func stopCapture() {
        if #available(iOS 16.0, *) {
            captureSession?.stop()
            NSLog("[RoomPlan] stopCapture")
        }
    }

    @objc public static func exportCapture() {
        if #available(iOS 16.0, *) {
            guard let session = captureSession else {
                NSLog("[RoomPlan] exportCapture: no session")
                return
            }

            let exportURL = FileManager.default.urls(for: .documentDirectory, in: .userDomainMask)[0]
                .appendingPathComponent("Dreamland")
            let usdzURL = exportURL.appendingPathComponent("room.usdz")
            let jsonURL = exportURL.appendingPathComponent("room.json")

            do {
                try FileManager.default.createDirectory(at: exportURL, withIntermediateDirectories: true)
            } catch {
                NSLog("[RoomPlan] failed to create export dir: \(error)")
            }

            session.captureResults?.export(to: usdzURL)
            if let metadata = session.captureResults?.rawValue {
                do {
                    try metadata.data(using: .utf8)?.write(to: jsonURL)
                } catch {
                    NSLog("[RoomPlan] failed to write metadata: \(error)")
                }
            }

            lastUSDZPath = usdzURL.path
            lastMetadataPath = jsonURL.path
            NSLog("[RoomPlan] exportCapture complete")
        }
    }

    @objc public static func exportUSDZPath() -> String {
        return lastUSDZPath ?? ""
    }

    @objc public static func exportMetadataPath() -> String {
        return lastMetadataPath ?? ""
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
