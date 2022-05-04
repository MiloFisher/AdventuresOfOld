using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;

public class JLVolumeControl : MonoBehaviour
{
    public AudioMixer masterMixer;
    public AudioMixer musicMixer;
    public AudioMixer effectsMixer;
    public AudioMixer voiceMixer;
    
    public void SetMasterVolume(float sliderValue) {
        masterMixer.SetFloat("MasterMixer", Mathf.Log10(sliderValue) * 20);
    }

    public void SetMusicVolume(float sliderValue) {
        musicMixer.SetFloat("MusicMasterMixer", Mathf.Log10(sliderValue) * 20);
    }

    public void SetEffectsVolume(float sliderValue) {
        effectsMixer.SetFloat("EffectsMasterMixer", Mathf.Log10(sliderValue) * 20);
    }

    public void SetVoiceVolume(float sliderValue) {
        voiceMixer.SetFloat("VoiceMasterMixer", Mathf.Log10(sliderValue) * 20);
    }
}
