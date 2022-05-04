using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Audio;

public class JLAudioManager : Singleton<JLAudioManager>
{
    public JLSoundClass[] soundArray;

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
