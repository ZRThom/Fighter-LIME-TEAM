using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthBarTest : MonoBehaviour
{

    [Header("Health")]
    public Slider healthSlider; 
    public Slider damageSlider; 
    public float damageDelay = 0.5f;
    public float damageSpeed = 1f;
    public float comboFadeDelay = 0.5f;
    public float comboFadeSpeed = 1f;

    [Header("Ultimate")]
    public Slider ultimateSlider;   
    public float maxUltimate = 100f; 
    public float ultimateGainPerHit = 10f;
    private Coroutine damageCoroutine;
    private Coroutine fadeCoroutine;
    private float currentUltimate = 0f;
    void Start()
    {
        healthSlider.value = 1f;
        healthSlider.value = 1f;
        if (ultimateSlider != null)
        {
            ultimateSlider.minValue = 0f;
            ultimateSlider.maxValue = maxUltimate;
            ultimateSlider.value = 0f;
            currentUltimate = 0f;
        } 
    }

    // damage test
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            SetDamages(0.1f);
            if (ultimateSlider != null)
            {
                currentUltimate += ultimateGainPerHit;
                currentUltimate = Mathf.Clamp(currentUltimate, 0f, maxUltimate);
                ultimateSlider.value = currentUltimate;
            }
        } 
    }

    public void SetDamages(float targetHealthNormalized)
    {
        healthSlider.value = Mathf.Clamp01(targetHealthNormalized);

        if (damageCoroutine != null) StopCoroutine(damageCoroutine);
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        damageCoroutine = StartCoroutine(DamageCombo());
        fadeCoroutine = StartCoroutine(FadeDamageBar());
    }

    IEnumerator DamageCombo()
    {
        yield return new WaitForSeconds(damageDelay);
        while (damageSlider.value > healthSlider.value)
        {
            damageSlider.value = Mathf.MoveTowards(damageSlider.value, healthSlider.value, damageSpeed * Time.deltaTime);
            yield return null;
        }
    }

    IEnumerator FadeDamageBar()
    {
        yield return new WaitForSeconds(comboFadeDelay);
        while (damageSlider.value > healthSlider.value)
        {
            damageSlider.value = Mathf.MoveTowards(damageSlider.value, healthSlider.value, comboFadeSpeed * Time.deltaTime);
            yield return null;
        } 
    }

    public void ResetHealth()
    {
        healthSlider.value = 1f;
        Debug.Log("Health reset!");
    }

    public void ResetUltimate()
    {
        currentUltimate = 0f;
        if (ultimateSlider != null) ultimateSlider.value = 0f;
    }

    public void SetHealth(float normalizedHealth)
    {
        healthSlider.value = Mathf.Clamp01(normalizedHealth);
        damageSlider.value = healthSlider.value;
    }
}