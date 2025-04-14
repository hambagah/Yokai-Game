using UnityEngine;
using Leap;
using System.Collections;

public class LeapMotionMovement : MonoBehaviour
{
    public Player playerScript;

    [Header("Movement Settings")]
    public float movementSpeed = 1f;
    public float deadZoneRadius = 0.05f;
    public float maxOffset = 0.15f;

    [Header("Calibration")]
    public Vector2 centerOffset = Vector2.zero;
    public bool useCalibration = true;
    public KeyCode calibrateKey = KeyCode.C;
    public bool autoCalibrate = true;
    public float autoCalibrationDelay = 1f;

    [Header("Debug")]
    public bool enableDebugLogs = true;
    public float logInterval = 1f;

    private LeapProvider provider;
    private float lastLogTime = 0f;
    private bool calibrationAttempted = false;

    // 新增防抖动 / 悬浮移动状态修复
    private Vector2 lastValidInput = Vector2.zero;
    private int noHandFrameCount = 0;
    private const int maxLostFrames = 5;

    void Start()
    {
        provider = FindObjectOfType<LeapProvider>();
        if (provider == null)
        {
            Debug.LogError("LeapMotionMovement: No LeapProvider found.");
            return;
        }

        if (autoCalibrate)
        {
            StartCoroutine(AutoCalibrateWithRetry());
        }
    }

    private IEnumerator AutoCalibrateWithRetry()
    {
        yield return new WaitForSeconds(autoCalibrationDelay);

        float timeout = 5f;
        float elapsed = 0f;
        while (elapsed < timeout)
        {
            if (AttemptCalibration())
            {
                calibrationAttempted = true;
                yield break;
            }

            yield return new WaitForSeconds(0.5f);
            elapsed += 0.5f;
        }

        Debug.LogWarning("LeapMotionMovement: Auto-calibration failed. Use 'C' to manually calibrate.");
        calibrationAttempted = true;
    }

    private bool AttemptCalibration()
    {
        var frame = provider.CurrentFrame;
        if (frame == null || frame.Hands.Count == 0) return false;

        var hand = frame.Hands[0];
        centerOffset = new Vector2(hand.PalmPosition.x, hand.PalmPosition.z);
        Debug.Log("LeapMotionMovement: Auto-calibrated to " + centerOffset);
        return true;
    }

    void Update()
    {
        if (Input.GetKeyDown(calibrateKey))
        {
            CalibrateCenter();
        }

        if (provider == null)
        {
            LogIfNeeded("LeapProvider is null");
            return;
        }

        var frame = provider.CurrentFrame;
        if (frame == null || frame.Hands.Count == 0)
        {
            noHandFrameCount++;

            // 如果连续几帧都没有手，才真正清除输入（避免闪断）
            if (noHandFrameCount > maxLostFrames)
            {
                playerScript.SendMessage("MovePressed", Vector2.zero);
                lastValidInput = Vector2.zero;
                LogIfNeeded("No hands detected - sending zero input");
            }
            return;
        }

        noHandFrameCount = 0;

        var hand = frame.Hands[0];
        Vector3 palmPos = hand.PalmPosition;
        Vector2 raw = new Vector2(palmPos.x, palmPos.z);
        Vector2 offset = useCalibration ? raw - centerOffset : raw;

        if (offset.magnitude < deadZoneRadius)
        {
            playerScript.SendMessage("MovePressed", Vector2.zero);
            lastValidInput = Vector2.zero;
            LogIfNeeded("Within dead zone");
        }
        else
        {
            Vector2 clamped = Vector2.ClampMagnitude(offset / maxOffset, 1f);
            Vector2 move = clamped * movementSpeed;
            playerScript.SendMessage("MovePressed", move);
            lastValidInput = move;
            LogIfNeeded($"Move sent: {move}");
        }
    }

    public void CalibrateCenter()
    {
        var frame = provider?.CurrentFrame;
        if (frame == null || frame.Hands.Count == 0)
        {
            Debug.LogWarning("LeapMotionMovement: Cannot calibrate - no hand found.");
            return;
        }

        var hand = frame.Hands[0];
        centerOffset = new Vector2(hand.PalmPosition.x, hand.PalmPosition.z);
        Debug.Log("LeapMotionMovement: Manual calibration set to " + centerOffset);
    }

    private void LogIfNeeded(string message)
    {
        if (enableDebugLogs && Time.time - lastLogTime > logInterval)
        {
            Debug.Log("LeapMotionMovement: " + message);
            lastLogTime = Time.time;
        }
    }
}
