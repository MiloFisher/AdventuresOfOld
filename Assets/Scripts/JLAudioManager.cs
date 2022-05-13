using UnityEngine;
using System;
using UnityEngine.Audio;
using UnityEngine.UI;

public class JLAudioManager : Singleton<JLAudioManager>
{
    public JLSoundClass[] soundArray;

    public Slider masterSlider;
    public Slider musicSlider;
    public Slider effectsSlider;
    public Slider voiceSlider;

    public AudioMixer masterMixer;
    public AudioMixer musicMixer;
    public AudioMixer effectsMixer;
    public AudioMixer voiceMixer;

    void Awake() {
        foreach(JLSoundClass s in soundArray) {
            s.audioSource = gameObject.AddComponent<AudioSource>();
            s.audioSource.clip = s.audioClip;

            s.audioSource.volume = s.volume;
            s.audioSource.pitch = s.pitch;
            s.audioSource.playOnAwake = s.playOnAwake;
            s.audioSource.loop = s.loop;

            s.audioSource.outputAudioMixerGroup = s.audioMixerGroup;
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
    public void PlaySound(string soundName) {
        JLSoundClass s = Array.Find(soundArray, sound => sound.name == soundName);
        if (s == null)
            return;
        s.audioSource.Play();
    }

    public void PlayOneShotSound(string soundName) {
        JLSoundClass s = Array.Find(soundArray, sound => sound.name == soundName);
        if (s == null)
            return;
        s.audioSource.PlayOneShot(s.audioClip);
    }

    public void StopSound(string soundName) {
        JLSoundClass s = Array.Find(soundArray, sound => sound.name == soundName);
        if (s == null)
            return;
        s.audioSource.Stop();
    }

    public void PauseSound(string soundName) {
        JLSoundClass s = Array.Find(soundArray, sound => sound.name == soundName);
        if (s == null)
            return;
        s.audioSource.Pause();
    }

    public void ResumeSound(string soundName) {
        JLSoundClass s = Array.Find(soundArray, sound => sound.name == soundName);
        if (s == null)
            return;
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
