using UnityEngine;
using Leap.PhysicalHands;

public class Tail : MonoBehaviour
{
    public enum TailType { Tap, Hold, Slide } // 三种音符类型
    public TailType tailType;

    public float lifetime = 3f; // 存活时间
    public float holdTimeThreshold = 1f; // 长按时间阈值
    public float slideSpeedMultiplier = 1.5f; // 滑动音符速度倍率

    private bool isHit = false;
    private bool isHolding = false;
    private bool isSliding = false;
    private float holdStartTime = 0f;
    private Vector3 lastHandPosition; // 记录手的位置
    private Rigidbody rb; // Rigidbody 用于滑动音符
    private PhysicalHandsManager handsManager;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        // rb.isKinematic = true; // **避免 Tail 被手推飞**
        //rb.velocity = Vector3.zero; // **避免乱飞**
        Destroy(gameObject, lifetime);
        // 查找 HandsManager
        handsManager = FindObjectOfType<PhysicalHandsManager>();
        if (handsManager != null)
        {
            handsManager.onContact.AddListener(OnHandContact);
        }
        else
        {
            Debug.LogError("PhysicalHandsManager 未找到！请确保它已挂载到场景中！");
        }
    }
    private void OnDestroy()
    {
        handsManager.onContact.RemoveListener(OnHandContact);
    }
    void OnHandContact(ContactHand contactHand, Rigidbody rbody)
    {
        if (isHit || rbody == null || rbody.gameObject != gameObject) return;

        Debug.Log($"Tail {tailType} 被手触碰!");

        if (tailType == TailType.Tap)
        {
            HandleTap();
        }
        else if (tailType == TailType.Hold)
        {
            isHolding = true;
            holdStartTime = Time.time;
        }
        else if (tailType == TailType.Slide)
        {
            isSliding = true;
            lastHandPosition = contactHand.transform.position;
            rb.isKinematic = false; // **允许滑动**
        }
    }

    void Update()
    {
        if (isHolding && Time.time - holdStartTime >= holdTimeThreshold)
        {
            HandleHold();
        }
    }

    void FixedUpdate()
    {
        if (isSliding)
        {
            if (rb == null) return; // **防止 MissingReferenceException**
            
            Vector3 handDelta = handsManager.LeftHand.transform.position - lastHandPosition;
            rb.velocity = handDelta * slideSpeedMultiplier / Time.fixedDeltaTime;
            lastHandPosition = handsManager.LeftHand.transform.position;
        }
    }
    void HandleTap()
    {
        isHit = true;
        Debug.Log("Tail 点击成功!");
        ScoreManager.Instance.AddScore(TailType.Tap);
        DestroyTail();
    }

    void HandleHold()
    {
        isHit = true;
        Debug.Log("Tail 长按成功!");
        ScoreManager.Instance.AddScore(TailType.Hold);
        DestroyTail();
    }

    void HandleSlide()
    {
        isHit = true;
        Debug.Log("Tail 滑动成功!");
        ScoreManager.Instance.AddScore(TailType.Slide);
        DestroyTail();
    }

    void DestroyTail()
    {
        if (rb != null)
        {
            rb.isKinematic = true; // **防止销毁前继续运动**
        }
        Destroy(gameObject);
    }
}
