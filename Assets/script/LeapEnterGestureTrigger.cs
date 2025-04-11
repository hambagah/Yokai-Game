using UnityEngine;
using Leap;
using Leap.Unity;

public class LeapEnterGestureTrigger : MonoBehaviour
{
    public float triggerVelocity = 1.2f;
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
        if (frame.Hands.Count == 0)
        {
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

        // ✅ 防止在选项阶段触发 ENTER
        if (Mathf.Abs(velocityY) > triggerVelocity && !IsChoiceButtonsActive())
        {
            GameEventsManager.instance.inputEvents.SubmitPressed();
            Debug.Log("Leap ENTER gesture triggered.");
            lastTriggerTime = Time.time;
        }
    }

    /// <summary>
    /// 检查是否有 DialogueChoices 中的按钮是激活的（Visible + Active）
    /// </summary>
    private bool IsChoiceButtonsActive()
    {
        GameObject choices = GameObject.Find("DialogueChoices");
        if (choices == null || !choices.activeInHierarchy) return false;

        foreach (Transform child in choices.transform)
        {
            if (child.gameObject.activeInHierarchy) return true;
        }
        return false;
    }

    private bool IsInDialogue()
    {
        GameObject dialogueCanvas = GameObject.Find("DialogueCanvas");
        return dialogueCanvas != null && dialogueCanvas.activeInHierarchy;
    }
}