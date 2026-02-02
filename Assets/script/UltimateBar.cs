using UnityEngine;
using UnityEngine.UI;

public class UltimateSystem : MonoBehaviour
{
    public Scrollbar ultimateBar;
    public float energy = 0;      
    public float maxEnergy = 100; 

    void Start()
    {
     
        ultimateBar.size = 0; 
    }

    public void GainEnergy(float amount)
    {
        
        energy += amount;

        
        if (energy > maxEnergy)
        {
            energy = maxEnergy;
        }
    ultimateBar.size = energy / maxEnergy;
    }
}