/**
 * DwellClickButton.cs
 * 
 * Summary: Creates a "dwell click" interaction for UI buttons.
 * Allows users to activate buttons by hovering over them for a specified time,
 * which is useful for accessibility or control schemes without direct clicking.
 */
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class DwellClickButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Tooltip("The button to activate when dwell time is complete")]
    public Button button;
    
    [Tooltip("Time in seconds user must hover before activation")]
    public float dwellTime = 1.5f;

    [Header("Visual Feedback")]
    [Tooltip("Image to display fill progress during dwell")]
    public Image dwellProgressFill;

    // Internal state
    private Coroutine dwellCoroutine;

    /**
     * Initialize components and reset visual state
     */
    void Start()
    {
        if (button == null)
            button = GetComponent<Button>();

        if (dwellCoroutine != null)
            StopCoroutine(dwellCoroutine);

        if (dwellProgressFill)
            dwellProgressFill.fillAmount = 0f;
    }

    /**
     * Called when pointer enters the button area
     * Starts the dwell timer
     */
    public void OnPointerEnter(PointerEventData eventData)
    {
        dwellCoroutine = StartCoroutine(DwellTimer());
    }

    /**
     * Called when pointer exits the button area
     * Cancels the dwell timer and resets visual feedback
     */
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

    /**
     * Counts down the dwell time and activates the button when complete
     * Updates visual feedback during the process
     */
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
