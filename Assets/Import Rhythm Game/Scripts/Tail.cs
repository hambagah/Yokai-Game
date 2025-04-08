using UnityEngine;
using Leap.PhysicalHands;
using System.Collections;
using System.Collections.Generic;
public class Tail : MonoBehaviour
{
    public enum TailType { Tap, Hold, Slide } // 三种音符类型
    public TailType tailType;

    public float lifetime = 3f; // 存活时间
    public float holdTimeThreshold = 1f; // 长按时间阈值
    public float slideSpeedMultiplier = 1.5f; // 滑动音符速度倍率

    public float speed = 3f;
    public Vector3 moveDir;
    private bool isHit = false;
    private bool isHolding = false;
    private bool isSliding = false;
    private float holdStartTime = 0f;
    private Vector3 lastHandPosition; // 记录手的位置
    private Rigidbody rb; // Rigidbody 用于滑动音符
    private PhysicalHandsManager handsManager;
    [SerializeField] Material redMT;
    [SerializeField] Renderer render;
    public float curTime;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; // **避免 Tail 被手推飞**
        //rb.velocity = Vector3.zero; // **避免乱飞**
        Destroy(gameObject, lifetime);
        // 查找 HandsManager
        handsManager = FindObjectOfType<PhysicalHandsManager>();
        handsManager.onContact.AddListener(OnHandContact);
        StartCoroutine(DelayDestroy());
        //rb.velocity = moveDir * speed;
    }
    IEnumerator DelayDestroy()
    {
        yield return new WaitForSeconds(ScoreManager.Instance.missTime);
        ScoreManager.Instance.AddGrade((int)NoteHitType.Miss);
        DamageNumManager.st.Create(NoteHitType.Miss, transform.position);
    }
    private void OnDestroy()
    {
        handsManager.onContact.RemoveListener(OnHandContact);
    }

    public void SetState()
    {
        render.sharedMaterial = redMT;
    }
    void OnHandContact(ContactHand contactHand, Rigidbody rbody)
    {
        if (isHit || rbody == null || rbody.gameObject != gameObject) return;

        NoteInfo info = new()
        {
            curTime = curTime,
            pos = transform.position
        };
        ScoreManager.Instance.OnHit(info);
        Destroy(gameObject);
        /*
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
        }*/
    }
    
    void Update()
    {
        curTime += Time.deltaTime;
        /*
        if (isHolding && Time.time - holdStartTime >= holdTimeThreshold)
        {
            HandleHold();
        }*/
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveDir * speed * Time.deltaTime);
        /*
        if (isSliding)
        {
            if (rb == null) return; // **防止 MissingReferenceException**
            
            Vector3 handDelta = handsManager.LeftHand.transform.position - lastHandPosition;
            rb.velocity = handDelta * slideSpeedMultiplier / Time.fixedDeltaTime;
            lastHandPosition = handsManager.LeftHand.transform.position;
        }

        rb.MovePosition(rb.position + moveDir * speed * Time.deltaTime);*/
    }
    void HandleTap()
    {
        isHit = true;
        Debug.Log("Tail 点击成功!");
        DestroyTail();
    }

    void HandleHold()
    {
        isHit = true;
        Debug.Log("Tail 长按成功!");
        DestroyTail();
    }

    void HandleSlide()
    {
        isHit = true;
        Debug.Log("Tail 滑动成功!");
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
