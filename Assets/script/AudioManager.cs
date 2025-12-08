using UnityEngine;

public class AudioManager : MonoBehaviour
{

 [Header("-- Audio Source --")]
 [SerializeField] AudioSource musicSource;
 [SerializeField] AudioSource SFXSource;
 [SerializeField] AudioSource MasterSource;


 [Header("-- Audio Clip --")]
 public AudioClip background;
 public AudioClip testSFX;

 private void Start()
 {
    musicSource.clip = background;
    musicSource.Play();
 }
}
