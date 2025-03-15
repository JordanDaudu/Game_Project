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

    private Coroutine healthRoutine; // Coroutine for handling health bar (red) animation
    private Coroutine delayedRoutine; // Coroutine for handling delayed bar animation

    Color originalColor;

    void Start()
    {
        // Initialize health bar values
        healthBar.maxValue = playerCurrentHealth.initialValue;
        healthBar.value = playerCurrentHealth.initialValue;
        delayedBar.fillAmount = 1f; // Start the delayed bar fully filled
        originalColor = delayedBar.color;
    }

    public void UpdateHealth()
    {
        // Normalize health value (convert from absolute value to 0-1 range)
        float normalizedHealth = playerCurrentHealth.RuntimeValue / playerCurrentHealth.initialValue;

        // Stop any previous coroutine and start a new one for the main bar effect
        if (healthRoutine != null)
            StopCoroutine(healthRoutine);
        // Stop any previous coroutine and start a new one for the delayed effect
        if (delayedRoutine != null)
        {
            StopCoroutine(delayedRoutine);
            delayedBar.color = originalColor;
            delayedBar.enabled = true;
        }

        if (playerCurrentHealth.RuntimeValue > healthBar.value) // Healing
        {
            // Start delayed bar first, then update red bar
            delayedRoutine = StartCoroutine(UpdateDelayedBarThenHealth(normalizedHealth));

             //healthRoutine = StartCoroutine(SmoothUpdateHealthBar(playerCurrentHealth.RuntimeValue));
        }
        else // Update instant health bar immediately
        {
            healthBar.value = playerCurrentHealth.RuntimeValue;
            // Start delayed bar
            delayedRoutine = StartCoroutine(UpdateDelayedBar(normalizedHealth));
        }

        // Start delayed bar
        //delayedRoutine = StartCoroutine(UpdateDelayedBar(normalizedHealth));
    }
    // Update delayed bar accordingly
    private IEnumerator UpdateDelayedBar(float targetFill)
    {
        yield return new WaitForSeconds(0.1f); // Small delay before the delayed effect starts

        if(delayedBar.fillAmount > targetFill) // Taking damage
        {
            // Blinking effect (flashes 3 times to indicate damage)
            for (int i = 0; i < 3; i++)
            {
                delayedBar.enabled = false; // Hide the delayed bar
                yield return new WaitForSeconds(0.1f);
                delayedBar.enabled = true;  // Show the delayed bar
                yield return new WaitForSeconds(0.1f);
            }
        }
        else // Gaining health
        {
            StartCoroutine(GlowEffect()); // Start pulsing effect
        }

        // Smoothly reduce the delayed bar to match the current health
        while (Mathf.Abs(delayedBar.fillAmount - targetFill) > 0.01f) // Moves both up & down
        {
            delayedBar.fillAmount = Mathf.SmoothDamp(delayedBar.fillAmount, targetFill, ref velocity, 0.2f);
            yield return null;
        }

        delayedBar.fillAmount = targetFill; // Ensure final value matches exactly
    }
    // Pulse glow effect when healing
    private IEnumerator GlowEffect()
    {
        Color originalColor = delayedBar.color;
        for (int i = 0; i < 3; i++)
        {
            delayedBar.color = Color.green; // Glow color
            yield return new WaitForSeconds(0.1f);
            delayedBar.color = originalColor;
            yield return new WaitForSeconds(0.1f);
        }
    }
    // Main bar (red) delay effect
    private IEnumerator SmoothUpdateHealthBar(float targetHealth)
    {
        float speed = 0.7f; // (higher = slower)

        while (Mathf.Abs(healthBar.value - targetHealth) > 0.01f)
        {
            healthBar.value = Mathf.SmoothDamp(healthBar.value, targetHealth, ref velocity, speed);

            yield return null;
        }
        healthBar.value = targetHealth; // Ensure exact final value
    }
    private IEnumerator UpdateDelayedBarThenHealth(float targetFill)
    {
        yield return StartCoroutine(UpdateDelayedBar(targetFill)); // Wait for delayed bar to finish

        if (healthRoutine != null)
            StopCoroutine(healthRoutine);

        healthRoutine = StartCoroutine(SmoothUpdateHealthBar(playerCurrentHealth.RuntimeValue)); // Now update red bar
    }

}
