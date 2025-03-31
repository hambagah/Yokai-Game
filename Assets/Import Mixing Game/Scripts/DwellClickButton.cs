/**
 * DwellClickButton.cs
 * 
 * Summary: Creates a "dwell click" interaction for UI buttons.
 * Allows users to activate buttons by hovering over them for a specified time,
 * which is useful for accessibility or control schemes without direct clicking,
 * particularly for Leap Motion hand tracking.
 */
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class DwellClickButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Button Configuration")]
    [Tooltip("The button to activate when dwell time is complete")]
    public Button button;
    
    [Tooltip("Time in seconds user must hover before activation")]
    [Range(0.5f, 5f)]
    public float dwellTime = 1.5f;

    [Tooltip("Whether to automatically activate the button again after the cooldown time")]
    public bool allowRepeatedActivation = false;

    [Tooltip("Cooldown time before the button can be activated again (if repeated activation is allowed)")]
    [Range(0.5f, 5f)]
    public float cooldownTime = 1f;

    [Header("Visual Feedback")]
    [Tooltip("Image to display fill progress during dwell")]
    public Image dwellProgressFill;

    [Tooltip("Color of the progress fill when inactive")]
    public Color inactiveColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);

    [Tooltip("Color of the progress fill when active")]
    public Color activeColor = new Color(0, 1, 0, 0.8f);

    [Tooltip("Whether to animate the button slightly when hovering")]
    public bool animateOnHover = true;

    [Tooltip("Scale factor for hover animation")]
    [Range(1f, 1.2f)]
    public float hoverScaleFactor = 1.05f;

    // Internal state
    private Coroutine dwellCoroutine;
    private Coroutine cooldownCoroutine;
    private Vector3 originalScale;
    private bool isInCooldown = false;

    /**
     * Initialize components and reset visual state
     */
    void Start()
    {
        // Auto-find the button component if not assigned
        if (button == null)
            button = GetComponent<Button>();

        // Store original scale for animation
        originalScale = transform.localScale;

        // Stop any running coroutines
        if (dwellCoroutine != null)
            StopCoroutine(dwellCoroutine);

        // Initialize progress fill
        if (dwellProgressFill)
        {
            dwellProgressFill.fillAmount = 0f;
            dwellProgressFill.color = inactiveColor;
        }
    }

    /**
     * Called when pointer enters the button area
     * Starts the dwell timer if button is interactable
     */
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Only proceed if the button is interactable and not in cooldown
        if (button != null && button.interactable && !isInCooldown)
        {
            dwellCoroutine = StartCoroutine(DwellTimer());
            
            // Apply hover animation if enabled
            if (animateOnHover)
            {
                StartCoroutine(AnimateScale(hoverScaleFactor, 0.1f));
            }
        }
    }

    /**
     * Called when pointer exits the button area
     * Cancels the dwell timer and resets visual feedback
     */
    public void OnPointerExit(PointerEventData eventData)
    {
        // Stop the dwell timer
        if (dwellCoroutine != null)
        {
            StopCoroutine(dwellCoroutine);
            dwellCoroutine = null;
        }

        // Reset progress fill
        if (dwellProgressFill)
        {
            dwellProgressFill.fillAmount = 0f;
            dwellProgressFill.color = inactiveColor;
        }

        // Reset scale animation
        if (animateOnHover)
        {
            StartCoroutine(AnimateScale(1f, 0.1f));
        }
    }

    /**
     * Counts down the dwell time and activates the button when complete
     * Updates visual feedback during the process
     */
    private IEnumerator DwellTimer()
    {
        float timer = 0f;
        
        // Update the visual progress
        if (dwellProgressFill)
        {
            dwellProgressFill.color = activeColor;
        }

        // Count up to the dwell time
        while (timer < dwellTime)
        {
            timer += Time.deltaTime;

            if (dwellProgressFill)
            {
                dwellProgressFill.fillAmount = timer / dwellTime;
            }

            yield return null;
        }

        // Activate button if it's interactable
        if (button != null && button.interactable)
        {
            button.onClick.Invoke();
            Debug.Log("Dwell Click Activated!");

            // If we allow repeated activation, start the cooldown
            if (allowRepeatedActivation)
            {
                isInCooldown = true;
                cooldownCoroutine = StartCoroutine(CooldownTimer());
            }
        }

        // Reset the progress fill
        if (dwellProgressFill)
        {
            dwellProgressFill.fillAmount = 0f;
        }
    }

    /**
     * Waits for the cooldown period before allowing button activation again
     */
    private IEnumerator CooldownTimer()
    {
        yield return new WaitForSeconds(cooldownTime);
        isInCooldown = false;
    }

    /**
     * Smoothly animates the button scale for visual feedback
     */
    private IEnumerator AnimateScale(float targetScale, float duration)
    {
        Vector3 startScale = transform.localScale;
        Vector3 targetScaleVector = originalScale * targetScale;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            transform.localScale = Vector3.Lerp(startScale, targetScaleVector, timer / duration);
            yield return null;
        }

        transform.localScale = targetScaleVector;
    }

    /**
     * Force-activates the button using the dwell mechanism
     * Can be called by external scripts
     */
    public void ForceActivate()
    {
        if (button != null && button.interactable && !isInCooldown)
        {
            StopAllCoroutines();
            button.onClick.Invoke();
            Debug.Log("Button Force Activated!");
            
            // Reset visual state
            if (dwellProgressFill)
            {
                dwellProgressFill.fillAmount = 0f;
            }
            
            transform.localScale = originalScale;
        }
    }
}
