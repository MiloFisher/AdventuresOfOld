using UnityEngine;
using System;
using UnityEngine.Audio;
using UnityEngine.UI;
using System.Collections.Generic;

public class JLAudioManager : Singleton<JLAudioManager>
{
    public JLSoundClass[] soundArray;
    private Dictionary<string, JLSoundClass> soundDictionary;

    public Slider masterSlider;
    public Slider musicSlider;
    public Slider effectsSlider;
    public Slider voiceSlider;

    public AudioMixer masterMixer;
    public AudioMixer musicMixer;
    public AudioMixer effectsMixer;
    public AudioMixer voiceMixer;

    void Awake() {
        // Create empty dictionary
        soundDictionary = new Dictionary<string, JLSoundClass>();

        foreach(JLSoundClass s in soundArray) {
            s.audioSource = gameObject.AddComponent<AudioSource>();
            s.audioSource.clip = s.audioClip;

            s.audioSource.volume = s.volume;
            s.audioSource.pitch = s.pitch;
            s.audioSource.playOnAwake = s.playOnAwake;
            s.audioSource.loop = s.loop;

            s.audioSource.outputAudioMixerGroup = s.audioMixerGroup;

            // Add sound to dictionary with name as a key
            soundDictionary.Add(s.name, s);
        }
    }

    private void Start() {
        if (masterSlider)
        {
            float mixerValue;
            bool result = masterMixer.GetFloat("MasterMixer", out mixerValue);
            if (result)
                masterSlider.value = Mathf.Clamp(Mathf.Pow(10, PlayerPrefs.GetFloat("MasterMixer", mixerValue) / 20), 0.0001f, 4f);
        }
        if (musicSlider)
        {
            float mixerValue;
            bool result = musicMixer.GetFloat("MusicMasterMixer", out mixerValue);
            if (result)
                musicSlider.value = Mathf.Clamp(Mathf.Pow(10, PlayerPrefs.GetFloat("MusicMasterMixer", mixerValue) / 20), 0.0001f, 4f);
        }
        if (effectsSlider)
        {
            float mixerValue;
            bool result = effectsMixer.GetFloat("EffectsMasterMixer", out mixerValue);
            if (result)
                effectsSlider.value = Mathf.Clamp(Mathf.Pow(10, PlayerPrefs.GetFloat("EffectsMasterMixer", mixerValue) / 20), 0.0001f, 4f);
        }
        if (voiceSlider)
        {
            float mixerValue;
            bool result = voiceMixer.GetFloat("VoiceMasterMixer", out mixerValue);
            if (result)
                voiceSlider.value = Mathf.Clamp(Mathf.Pow(10, PlayerPrefs.GetFloat("VoiceMasterMixer", mixerValue) / 20), 0.0001f, 4f);
        }
    }

    // FUNCTIONS TO USE FOR BUTTONS RATHER THAN SCRIPTS
    public void PlaySound(string soundName, float startTime = 0, float endTime = -1) {
        if (!soundDictionary.ContainsKey(soundName))
            return;
        JLSoundClass s = soundDictionary[soundName];

        // Keeping old way commented out here for reference:

        //JLSoundClass s = Array.Find(soundArray, sound => sound.name == soundName);
        //if (s == null)
        //    return;

        s.audioSource.time = startTime;
        s.audioSource.Play();
        if(endTime > startTime)
            s.audioSource.SetScheduledEndTime(AudioSettings.dspTime + (endTime - startTime));
    }

    public void PlaySound(string soundName) {
        if (!soundDictionary.ContainsKey(soundName))
            return;
        JLSoundClass s = soundDictionary[soundName];
        s.audioSource.Play();
    }

    public void PlayOneShotSound(string soundName) {
        if (!soundDictionary.ContainsKey(soundName))
            return;
        JLSoundClass s = soundDictionary[soundName];
        s.audioSource.PlayOneShot(s.audioClip);
    }

    public void StopSound(string soundName) {
        if (!soundDictionary.ContainsKey(soundName))
            return;
        JLSoundClass s = soundDictionary[soundName];
        s.audioSource.Stop();
    }

    public void PauseSound(string soundName) {
        if (!soundDictionary.ContainsKey(soundName))
            return;
        JLSoundClass s = soundDictionary[soundName];
        s.audioSource.Pause();
    }

    public void ResumeSound(string soundName) {
        if (!soundDictionary.ContainsKey(soundName))
            return;
        JLSoundClass s = soundDictionary[soundName];
        s.audioSource.UnPause();
    }

    public void SetPitch(string soundName, float pitch)
    {
        if (!soundDictionary.ContainsKey(soundName))
            return;
        JLSoundClass s = soundDictionary[soundName];
        s.audioSource.pitch = pitch;
    }

    public void SetMasterVolume(float sliderValue)
    {
        float val = Mathf.Log10(sliderValue) * 20;
        masterMixer.SetFloat("MasterMixer", val);
        PlayerPrefs.SetFloat("MasterMixer", val);
    }

    public void SetMusicVolume(float sliderValue)
    {
        float val = Mathf.Log10(sliderValue) * 20;
        musicMixer.SetFloat("MusicMasterMixer", val);
        PlayerPrefs.SetFloat("MusicMasterMixer", val);
    }

    public void SetEffectsVolume(float sliderValue)
    {
        float val = Mathf.Log10(sliderValue) * 20;
        effectsMixer.SetFloat("EffectsMasterMixer", val);
        PlayerPrefs.SetFloat("EffectsMasterMixer", val);
    }

    public void SetVoiceVolume(float sliderValue)
    {
        float val = Mathf.Log10(sliderValue) * 20;
        voiceMixer.SetFloat("VoiceMasterMixer", val);
        PlayerPrefs.SetFloat("VoiceMasterMixer", val);
    }
}
