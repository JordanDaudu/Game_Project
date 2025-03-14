using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    // References to UI elements
    public Slider healthBar;        // Instant health bar (red bar)
    public Image delayedBar;        // Delayed effect bar (yellow bar)

    private float velocity = 0.0f;  // Required for SmoothDamp (smooth animation effect)
    public FloatValue playerCurrentHealth; // Reference to player's current health value

    private Coroutine delayedRoutine; // Coroutine for handling delayed bar animation

    void Start()
    {
        // Initialize health bar values
        healthBar.maxValue = playerCurrentHealth.initialValue;
        healthBar.value = playerCurrentHealth.initialValue;
        delayedBar.fillAmount = 1f; // Start the delayed bar fully filled
    }

    public void UpdateHealth()
    {
        // Normalize health value (convert from absolute value to 0-1 range)
        float normalizedHealth = playerCurrentHealth.RuntimeValue / playerCurrentHealth.initialValue;

        // Update instant health bar immediately
        healthBar.value = playerCurrentHealth.RuntimeValue;

        // Stop any previous coroutine and start a new one for the delayed effect
        if (delayedRoutine != null)
            StopCoroutine(delayedRoutine);

        delayedRoutine = StartCoroutine(UpdateDelayedBar(normalizedHealth));
    }

    private IEnumerator UpdateDelayedBar(float targetFill)
    {
        yield return new WaitForSeconds(0.1f); // Small delay before the delayed effect starts

        // Blinking effect (flashes 3 times to indicate damage)
        for (int i = 0; i < 3; i++)
        {
            delayedBar.enabled = false; // Hide the delayed bar
            yield return new WaitForSeconds(0.1f);
            delayedBar.enabled = true;  // Show the delayed bar
            yield return new WaitForSeconds(0.1f);
        }

        // Smoothly reduce the delayed bar to match the current health
        while (delayedBar.fillAmount > targetFill)
        {
            delayedBar.fillAmount = Mathf.SmoothDamp(delayedBar.fillAmount, targetFill, ref velocity, 0.2f);
            yield return null;
        }

        delayedBar.fillAmount = targetFill; // Ensure final value matches exactly
    }
}
