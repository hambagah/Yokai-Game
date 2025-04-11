using UnityEngine;
using Leap;
using Leap.Unity;

public class LeapEnterGestureTrigger : MonoBehaviour
{
    public float triggerVelocity = 1.2f; // 调整这个阈值控制敏感度
    public float cooldownTime = 1.0f;

    private LeapProvider provider;
    private float lastTriggerTime = -Mathf.Infinity;
    private float previousY = 0f;
    private bool initialized = false;

    void Start()
    {
        provider = FindObjectOfType<LeapProvider>();
    }

    void Update()
    {
        if (provider == null || Time.time - lastTriggerTime < cooldownTime) return;

        var frame = provider.CurrentFrame;
        if (frame.Hands.Count == 0) {
            initialized = false;
            return;
        }

        var hand = frame.Hands[0];
        float currentY = hand.PalmPosition.y;

        if (!initialized)
        {
            previousY = currentY;
            initialized = true;
            return;
        }

        float velocityY = (currentY - previousY) / Time.deltaTime;
        previousY = currentY;

        if (Mathf.Abs(velocityY) > triggerVelocity)
        {
            // 模拟按下 Enter
            GameEventsManager.instance.inputEvents.SubmitPressed();
            Debug.Log("Leap ENTER gesture triggered.");
            lastTriggerTime = Time.time;
        }
    }
}
