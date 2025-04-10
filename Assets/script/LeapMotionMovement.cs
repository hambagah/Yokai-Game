using UnityEngine;
using Leap.Unity;
using Leap;

public class LeapMotionMovement : MonoBehaviour
{
    public Player playerScript;
    public float movementSpeed = 1f;
    public float deadZoneRadius = 0.05f; // 中心无响应区域（米）
    public float maxOffset = 0.15f;       // 偏移多大时为最大速度

    private LeapProvider provider;

    void Start()
    {
        provider = FindObjectOfType<LeapProvider>();
    }

    void Update()
    {
        if (provider == null) return;

        var frame = provider.CurrentFrame;
        if (frame == null || frame.Hands.Count == 0) return;

        var hand = frame.Hands[0];
        Vector3 palmPosition = hand.PalmPosition; // 已经是 UnityEngine.Vector3（单位是米）

        // 只取 x（左右）和 z（前后）
        Vector2 offset = new Vector2(palmPosition.x, palmPosition.z);

        if (offset.magnitude < deadZoneRadius)
        {
            playerScript.SendMessage("MovePressed", Vector2.zero);
        }
        else
        {
            Vector2 clamped = Vector2.ClampMagnitude(offset / maxOffset, 1.0f);
            playerScript.SendMessage("MovePressed", clamped * movementSpeed);
        }
    }
}
