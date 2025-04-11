using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class DwellClickButton_main : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Button Configuration")]
    public Button button;
    public float dwellTime = 2f;
    public bool allowRepeatedActivation = false;
    public float cooldownTime = 1f;

    [Header("Visual Feedback")]
    public Image dwellProgressFill;
    public Color inactiveColor = Color.white;
    public Color activeColor = Color.green;
    public bool animateOnHover = true;
    public float hoverScaleFactor = 1.05f;

    private float timer;
    private bool hovering;
    private bool canClick = true;
    private Vector3 originalScale;

    void Start()
    {
        if (button == null) button = GetComponent<Button>();
        if (dwellProgressFill != null)
            dwellProgressFill.fillAmount = 0f;

        originalScale = transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log($"{gameObject.name} OnPointerEnter triggered!");
        if (!canClick) return;

        hovering = true;
        StartCoroutine(DwellTimer());
        if (animateOnHover) transform.localScale = originalScale * hoverScaleFactor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log($"{gameObject.name} OnPointerExit triggered!");
        hovering = false;
        ResetFill();
        if (animateOnHover) transform.localScale = originalScale;
    }

    private IEnumerator DwellTimer()
    {
        timer = 0f;

        while (hovering && timer < dwellTime)
        {
            timer += Time.deltaTime;
            if (dwellProgressFill != null)
            {
                dwellProgressFill.fillAmount = timer / dwellTime;
                dwellProgressFill.color = Color.Lerp(inactiveColor, activeColor, timer / dwellTime);
            }
            yield return null;
        }

        if (hovering && canClick)
        {
            ForceActivate();
            if (!allowRepeatedActivation)
            {
                canClick = false;
                yield return new WaitForSeconds(cooldownTime);
                canClick = true;
            }
        }

        ResetFill();
    }

    private void ResetFill()
    {
        if (dwellProgressFill != null)
        {
            dwellProgressFill.fillAmount = 0f;
            dwellProgressFill.color = inactiveColor;
        }
    }

    public void ForceActivate()
    {
        Debug.Log($"ForceActivate triggered on {gameObject.name}");

        // ��ʽ 1��ֱ�Ӵ��� Enter �ύ�¼����Ƽ���
        GameEventsManager.instance.inputEvents.SubmitPressed();

        // ��ʽ 2����Ȼ���ð�ť�󶨣����ڼ�����ͨ��ť��
        if (button != null)
        {
            button.onClick.Invoke();
        }
    }

}
