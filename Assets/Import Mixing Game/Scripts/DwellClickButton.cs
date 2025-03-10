using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class DwellClickButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Button button; // Reference to the UI Button
    public float dwellTime = 1.5f; // Time in seconds before auto-click
    private Coroutine dwellCoroutine;

    private void Start()
    {
        if (button == null)
        {
            button = GetComponent<Button>(); // Auto-assign if not set
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Start dwell timer when pointer hovers
        dwellCoroutine = StartCoroutine(DwellTimer());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Cancel dwell timer if pointer leaves
        if (dwellCoroutine != null)
        {
            StopCoroutine(dwellCoroutine);
            dwellCoroutine = null;
        }
    }

    private IEnumerator DwellTimer()
    {
        yield return new WaitForSeconds(dwellTime); // Wait for dwell time
        button.onClick.Invoke(); // Simulate button click
        Debug.Log("Dwell Click Activated!");
    }
}
