using UnityEngine;
using UnityEngine.UI;

public class RageBarUI : MonoBehaviour
{
    public Slider rageSlider;
    public GameObject rageReadyImage;
    void Awake()
    {
        if (rageReadyImage != null)
        {
            rageReadyImage.SetActive(false);
        }
    }

    public void SetValue(float current, float max)
    {
        if (rageSlider == null)
        {
            return;
        }
        rageSlider.minValue = 0f;
        rageSlider.maxValue = max;
        rageSlider.value = Mathf.Clamp(current, 0f, max);
        if (rageReadyImage != null)
        {
            rageReadyImage.SetActive(rageSlider.value >= rageSlider.maxValue);
        }
    }

    public void ResetBar(float max)
    {
        SetValue(0f, max);
    }
}
