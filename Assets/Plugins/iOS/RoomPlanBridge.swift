import Foundation

@objc public class RoomPlanBridge: NSObject {
    @objc public static func startCapture() {
        // TODO: Implement RoomPlan capture using RoomCaptureSession.
        NSLog("[RoomPlan] Swift startCapture stub")
    }

    @objc public static func stopCapture() {
        NSLog("[RoomPlan] Swift stopCapture stub")
    }

    @objc public static func exportCapture() -> String {
        NSLog("[RoomPlan] Swift exportCapture stub")
        return ""
    }
}
