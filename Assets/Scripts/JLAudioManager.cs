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
                masterSlider.value = Mathf.Clamp(Mathf.Pow(10, mixerValue / 20), 0.0001f, 4f);
        }
        if (musicSlider)
        {
            float mixerValue;
            bool result = musicMixer.GetFloat("MusicMasterMixer", out mixerValue);
            if (result)
                musicSlider.value = Mathf.Clamp(Mathf.Pow(10, mixerValue / 20), 0.0001f, 4f);
        }
        if (effectsSlider)
        {
            float mixerValue;
            bool result = effectsMixer.GetFloat("EffectsMasterMixer", out mixerValue);
            if (result)
                effectsSlider.value = Mathf.Clamp(Mathf.Pow(10, mixerValue / 20), 0.0001f, 4f);
        }
        if (voiceSlider)
        {
            float mixerValue;
            bool result = voiceMixer.GetFloat("VoiceMasterMixer", out mixerValue);
            if (result)
                voiceSlider.value = Mathf.Clamp(Mathf.Pow(10, mixerValue / 20), 0.0001f, 4f);
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
