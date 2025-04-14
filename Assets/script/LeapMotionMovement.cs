using UnityEngine;
using Leap;
using System.Collections;

/// <summary>
/// 支持 Leap Motion + 键盘 (WASD) 混合控制的角色移动脚本
/// </summary>
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
    public float logInterval = 1.0f;

    private LeapProvider provider;
    private float lastLogTime = 0f;
    private bool calibrationAttempted = false;

    void Start()
    {
        provider = FindObjectOfType<LeapProvider>();
        if (provider == null)
        {
            Debug.LogError("LeapMotionMovement: No LeapProvider found!");
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

        float attemptTime = 0f;
        bool calibrationSuccessful = false;

        while (attemptTime < 5f && !calibrationSuccessful)
        {
            calibrationSuccessful = AttemptCalibration();

            if (!calibrationSuccessful)
            {
                if (enableDebugLogs) Debug.Log("等待手部检测以完成自动校准...");
                yield return new WaitForSeconds(0.5f);
                attemptTime += 0.5f;
            }
        }

        calibrationAttempted = true;

        if (!calibrationSuccessful)
        {
            Debug.LogWarning("LeapMotionMovement: 自动校准失败，请按 '" + calibrateKey + "' 手动校准");
        }
    }

    private bool AttemptCalibration()
    {
        if (provider == null) return false;

        var frame = provider.CurrentFrame;
        if (frame == null || frame.Hands.Count == 0) return false;

        var hand = frame.Hands[0];
        Vector3 palmPosition = hand.PalmPosition;
        centerOffset = new Vector2(palmPosition.x, palmPosition.z);

        Debug.Log($"LeapMotionMovement: Auto-calibrated center position to {centerOffset}");
        return true;
    }

    void Update()
    {
        if (Input.GetKeyDown(calibrateKey))
        {
            CalibrateCenter();
        }

        Vector2 moveInput = Vector2.zero;

        var frame = provider?.CurrentFrame;
        bool handDetected = frame != null && frame.Hands.Count > 0;

        if (handDetected)
        {
            var hand = frame.Hands[0];
            Vector3 palmPosition = hand.PalmPosition;
            Vector2 rawOffset = new Vector2(palmPosition.x, palmPosition.z);
            Vector2 offset = useCalibration ? rawOffset - centerOffset : rawOffset;

            if (enableDebugLogs && Time.time - lastLogTime > logInterval)
            {
                Debug.Log($"LeapMotion: offset={offset}, center={centerOffset}");
                lastLogTime = Time.time;
            }

            if (offset.magnitude > deadZoneRadius)
            {
                Vector2 clamped = Vector2.ClampMagnitude(offset / maxOffset, 1f);
                moveInput = clamped * movementSpeed;
            }
        }
        else
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            moveInput = new Vector2(h, v).normalized * movementSpeed;
        }

        playerScript.SendMessage("MovePressed", moveInput);
    }

    public void CalibrateCenter()
    {
        var frame = provider?.CurrentFrame;
        if (frame == null || frame.Hands.Count == 0)
        {
            Debug.LogWarning("LeapMotionMovement: 无法校准 - 没有检测到手");
            return;
        }

        var hand = frame.Hands[0];
        Vector3 palmPosition = hand.PalmPosition;
        centerOffset = new Vector2(palmPosition.x, palmPosition.z);

        Debug.Log($"LeapMotionMovement: Calibrated center position to {centerOffset}");
    }
}
