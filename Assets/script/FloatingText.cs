using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    private TMP_Text textComponent;
    private float timer;
    private Color startColor;

    [Header("Réglages")]
    public float speed = 2f;
    public float duration = 1f;

    void Start()
    {
        if (textComponent == null)
            textComponent = GetComponent<TMP_Text>();

        if (textComponent != null)
        {
            startColor = textComponent.color;
        }
        else
        {
             Debug.LogError("ERREUR : Pas de TMP_Text trouvé sur le FloatingText !");
        }

        MeshRenderer mesh = GetComponentInChildren<MeshRenderer>();
        if (mesh != null)
        {
            mesh.sortingOrder = 500; 
            mesh.sortingLayerName = "Default";
        }

        transform.localPosition += new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), 0);
    }

    public void SetDamage(int damageAmount)
    {
        if (textComponent == null)
            textComponent = GetComponent<TMP_Text>();
        if(textComponent != null)
        {
            textComponent.text = damageAmount.ToString();
        }
    }

    void Update()
    {
        transform.position += Vector3.up * speed * Time.deltaTime;
        
        timer += Time.deltaTime;
        if (textComponent != null)
        {
            float alpha = Mathf.Lerp(1f, 0f, timer / duration);
            textComponent.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
        }

        if (timer >= duration)
        {
            Destroy(gameObject);
        }
    }
}