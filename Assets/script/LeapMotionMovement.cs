using UnityEngine;
using Leap.Unity;
using Leap;

public class LeapMotionMovement : MonoBehaviour
{
    public Player playerScript;
    public float movementSpeed = 1f;
    public float deadZoneRadius = 0.05f; // ��������Ӧ�����ף�
    public float maxOffset = 0.15f;       // ƫ�ƶ��ʱΪ����ٶ�

    private LeapProvider provider;

    void Start()
    {
        provider = FindObjectOfType<LeapProvider>();
    }

    void Update()
{
    if (provider == null) return;

    var frame = provider.CurrentFrame;
    if (frame == null || frame.Hands.Count == 0)
    {
        // 没检测到手，停止移动
        playerScript.SendMessage("MovePressed", Vector2.zero);
        return;
    }

    var hand = frame.Hands[0];
    Vector3 palmPosition = hand.PalmPosition;

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
