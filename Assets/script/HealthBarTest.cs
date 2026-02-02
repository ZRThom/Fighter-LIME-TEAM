using UnityEngine;
using UnityEngine.UI;
using System.Collections; 

public class HealthBarTest : MonoBehaviour
{
    [Header("UI References")]
    public Scrollbar healthScrollbar;
    public Image fillImage;

    [Header("Color Settings")]
    public Color highColor = Color.green;
    public Color mediumColor = Color.yellow;
    public Color lowColor = Color.red;

    [Header("Animation Settings")]
    public float animationSpeed = 5f;
    private Coroutine currentAnimation;

    void Start()
    {
        if (fillImage != null && healthScrollbar != null) 
        {
            UpdateColor(); 
        }
    }

    public void SetDamages(float damageAmount)
    {
        if (healthScrollbar == null || fillImage == null) return;

        float targetValue = healthScrollbar.size - damageAmount;
    
        if (targetValue < 0) targetValue = 0;

        if (currentAnimation != null) StopCoroutine(currentAnimation);

        currentAnimation = StartCoroutine(AnimateHealthBar(targetValue));
    }


    IEnumerator AnimateHealthBar(float targetValue)
    {

        while (Mathf.Abs(healthScrollbar.size - targetValue) > 0.001f)
        {

            healthScrollbar.size = Mathf.Lerp(healthScrollbar.size, targetValue, Time.deltaTime * animationSpeed);
            
            UpdateColor();

            yield return null;
        }


        healthScrollbar.size = targetValue;
        UpdateColor();
    }


    void UpdateColor()
    {
        if (healthScrollbar.size > 0.5f)
        {
            fillImage.color = highColor;
        }
        else if (healthScrollbar.size > 0.2f)
        {
            fillImage.color = mediumColor;
        }
        else
        {
            fillImage.color = lowColor;
        }
    }

    public void ResetHealth()
    {
        healthScrollbar.size = 1f;
        UpdateColor();
        Debug.Log("Health reset!");
    }
}