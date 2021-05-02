using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeControls : MonoBehaviour
{
    [SerializeField] AudioMixer mixer;  // Master audio mixer - controls ALL volume
    [SerializeField] Slider masterSlider;   // Slider controlling master volume i.e. both music and SFX
    [SerializeField] Slider musicSlider;    // Slider controlling SFX volume
    [SerializeField] Slider sfxSlider;  // Slider controlling SFX volume

    void Start()
    {
        // Sets volume sliders to match current volume
        masterSlider.value = PlayerPrefs.GetFloat("Master Vol", 0.5f);
        musicSlider.value = PlayerPrefs.GetFloat("Music Vol", 0.5f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFX Vol", 0.5f);
    }

        // --- Audio related functions below ---
        // Sets current master volume and saves it
        public void SetMasterVol()
    {
        float masterVol = masterSlider.value;
        mixer.SetFloat("MasterVol", Mathf.Log10(masterVol) * 20);
        PlayerPrefs.SetFloat("Master Vol", masterVol);
    }

    // Sets current music volume and saves it
    public void SetMusicVol()
    {
        float musicVol = musicSlider.value;
        mixer.SetFloat("MusicVol", Mathf.Log10(musicVol) * 20);
        PlayerPrefs.SetFloat("Music Vol", musicVol);
    }

    // Sets current SFX volume and saves it
    public void SetSFXVol()
    {
        float sfxVol = sfxSlider.value;
        mixer.SetFloat("SFXVol", Mathf.Log10(sfxVol) * 20);
        PlayerPrefs.SetFloat("SFX Vol", sfxVol);
    }
}
