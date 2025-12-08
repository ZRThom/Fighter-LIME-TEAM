using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;



public class MenuSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer MyMixer;
    [SerializeField] private Slider MasterSlider;
    [SerializeField] private Slider MusicSlider;
    [SerializeField] private Slider SFXSlider;

    public TMP_Dropdown ResolutionsDropdown;

    Resolution[] resolutions;

    private void Start()
    {
        // Chargement Master
        if (PlayerPrefs.HasKey("masterVolume")) LoadMaster();
        else SetMasterVolume();

        // Music
        if (PlayerPrefs.HasKey("musicVolume")) LoadMusic();
        else SetMusicVolume();

        // SFX
        if (PlayerPrefs.HasKey("sfxVolume")) LoadSFX();
        else SetSFXVolume();

        resolutions = Screen.resolutions;
        ResolutionsDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0; 
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
            resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        ResolutionsDropdown.AddOptions(options);
        ResolutionsDropdown.value = currentResolutionIndex;
        ResolutionsDropdown.RefreshShownValue();
    }

    public void SetMasterVolume()
    {
        float sliderValue = MasterSlider.value;
        MyMixer.SetFloat("master", Mathf.Log10(Mathf.Pow(sliderValue, 3)) * 20);
        PlayerPrefs.SetFloat("masterVolume", sliderValue);
    }

    public void SetMusicVolume()
    {
        float sliderValue = MusicSlider.value;
        MyMixer.SetFloat("music", Mathf.Log10(Mathf.Pow(sliderValue, 3)) * 20);
        PlayerPrefs.SetFloat("musicVolume", sliderValue);
    }

    public void SetSFXVolume()
    {
        float sliderValue = SFXSlider.value;
        MyMixer.SetFloat("sfx", Mathf.Log10(Mathf.Pow(sliderValue, 3)) * 20);
        PlayerPrefs.SetFloat("sfxVolume", sliderValue);
    }

    private void LoadMaster()
    {
        MasterSlider.value = PlayerPrefs.GetFloat("masterVolume");
        SetMasterVolume();
    }

    private void LoadMusic()
    {
        MusicSlider.value = PlayerPrefs.GetFloat("musicVolume");
        SetMusicVolume();
    }

    private void LoadSFX()
    {
        SFXSlider.value = PlayerPrefs.GetFloat("sfxVolume");
        SetSFXVolume();
    }

    public void SetFullScreen(bool IsFullScreen)
    {
        Screen.fullScreen = IsFullScreen;

    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
}
