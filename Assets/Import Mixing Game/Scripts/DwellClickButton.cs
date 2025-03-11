using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class DwellClickButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Button button;
    public float dwellTime = 1.5f;

    [Header("Visual Feedback (optional)")]
    public Image dwellProgressFill;  // ✅ Declared correctly once here.

    private Coroutine dwellCoroutine;

    void Start()
    {
        if (button == null)
            button = GetComponent<Button>();

        if (dwellCoroutine != null)
            StopCoroutine(dwellCoroutine);

        if (dwellProgressFill)
            dwellProgressFill.fillAmount = 0f;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        dwellCoroutine = StartCoroutine(DwellTimer());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (dwellCoroutine != null)
        {
            StopCoroutine(dwellCoroutine);
            dwellCoroutine = null;
        }

        if (dwellProgressFill)
            dwellProgressFill.fillAmount = 0f;
    }

    private IEnumerator DwellTimer()
    {
        float timer = 0f;
        while (timer < dwellTime)
        {
            timer += Time.deltaTime;

            if (dwellProgressFill)
                dwellProgressFill.fillAmount = timer / dwellTime;

            yield return null;
        }

        button.onClick.Invoke();
        Debug.Log("Dwell Click Activated!");

        if (dwellProgressFill)
            dwellProgressFill.fillAmount = 0f;
    }
}
