using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class LeapDwellConfirm : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("配置")]
    public Button targetButton;
    [Range(0.5f, 5f)] public float dwellDuration = 1.5f;
    [Range(0.5f, 5f)] public float cooldownTime = 1f;
    public bool allowRepeat = false;

    [Header("进度条UI")]
    public Image progressBar; // UI图像组件，必须设为 fill 模式
    public Color idleColor = new Color(1f, 1f, 1f, 0.2f);
    public Color activeColor = new Color(0f, 0.8f, 0.2f, 1f);

    [Header("动画")]
    public bool scaleEffect = true;
    [Range(1f, 1.2f)] public float hoverScale = 1.05f;

    private Vector3 originalScale;
    private Coroutine dwellRoutine;
    private Coroutine cooldownRoutine;
    private bool isCoolingDown = false;

    void Start()
    {
        if (targetButton == null) targetButton = GetComponent<Button>();
        originalScale = transform.localScale;

        if (progressBar)
        {
            progressBar.fillAmount = 0f;
            progressBar.color = idleColor;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isCoolingDown || !targetButton.interactable) return;

        dwellRoutine = StartCoroutine(DwellTimer());

        if (scaleEffect)
            StartCoroutine(ScaleTo(originalScale * hoverScale, 0.1f));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (dwellRoutine != null) StopCoroutine(dwellRoutine);
        dwellRoutine = null;

        if (progressBar)
        {
            progressBar.fillAmount = 0f;
            progressBar.color = idleColor;
        }

        if (scaleEffect)
            StartCoroutine(ScaleTo(originalScale, 0.1f));
    }

    private IEnumerator DwellTimer()
    {
        float t = 0f;

        if (progressBar)
            progressBar.color = activeColor;

        while (t < dwellDuration)
        {
            t += Time.deltaTime;
            if (progressBar)
                progressBar.fillAmount = t / dwellDuration;

            yield return null;
        }

        // 执行按钮点击
        if (targetButton.interactable)
        {
            targetButton.onClick.Invoke();
            Debug.Log($"[LeapDwellConfirm] {targetButton.name} clicked via dwell.");

            if (allowRepeat)
            {
                isCoolingDown = true;
                cooldownRoutine = StartCoroutine(Cooldown());
            }
        }

        if (progressBar)
            progressBar.fillAmount = 0f;
    }

    private IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(cooldownTime);
        isCoolingDown = false;
    }

    private IEnumerator ScaleTo(Vector3 target, float duration)
    {
        Vector3 start = transform.localScale;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(start, target, t / duration);
            yield return null;
        }

        transform.localScale = target;
    }

    // 允许外部强制点击（例如测试、其他组件调用）
    public void ForceClick()
    {
        if (targetButton.interactable)
            targetButton.onClick.Invoke();
    }
}
