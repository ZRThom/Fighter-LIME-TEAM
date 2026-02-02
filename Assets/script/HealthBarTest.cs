using UnityEngine;
using UnityEngine.UI;

public class HealthBarTest : MonoBehaviour
{
    public Scrollbar healthScrollbar; 
    public Image fillImage;           
    
    public Color highColor = Color.green;
    public Color mediumColor = Color.yellow;
    public Color lowColor = Color.red;

    void Start()
    {
        if(fillImage != null) fillImage.color = highColor;
    }

    public void SetDamages(float value)
    {
        if (healthScrollbar == null || fillImage == null) return;

        healthScrollbar.size -= value;

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
}