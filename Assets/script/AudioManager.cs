using UnityEngine;

public class AudioManager : MonoBehaviour
{
 public static AudioManager instance; 
 public void Awake()
 {
   if (instance != null)
   {
      Destroy(gameObject);
   }
   else
   {
      instance = this;
      DontDestroyOnLoad(gameObject);
   }
 }


 [Header("-- Audio Source --")]
 [SerializeField] AudioSource musicSource;
 [SerializeField] AudioSource SFXSource;
 [SerializeField] AudioSource MasterSource;


 [Header("-- Audio Clip --")]
 public AudioClip background;
 public AudioClip testSFX;


 // Cette fonction permet aux autres scripts de demander de jouer un son
    public void PlaySFX(AudioClip clip)
    {
        // On vérifie qu'on a bien assigné la source dans l'inspecteur pour éviter un crash
        if (SFXSource != null && clip != null)
        {
            SFXSource.PlayOneShot(clip);
        }
    }

    
 private void Start()
 {
    musicSource.clip = background;
    musicSource.Play();
 }
}
