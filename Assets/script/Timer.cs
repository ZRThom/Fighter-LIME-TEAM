using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public float timeLeft = 99;
    public TMP_Text timerCountText; 

    private bool activeTimer = true;

    void Update()
    {
        if (activeTimer)
        {
            if (timeLeft > 0)
            {
                timeLeft -= Time.deltaTime;
            }
            else
            {
                timeLeft = 0;
                activeTimer = false;
            }


            if(timerCountText != null)
            {
                 timerCountText.text = Mathf.CeilToInt(timeLeft).ToString();
            }
        }
    }

    public void StopTimer()
    {
        activeTimer = false;
    }

    public void ResetTimer()
    {
        timeLeft = 99;
        activeTimer = true;
    }
}