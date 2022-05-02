using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Audio;

public class JLAudioManager : Singleton<JLAudioManager>
{
    public JLSoundClass[] soundArray;

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
    public void playSound(string soundName) {
        JLSoundClass s = Array.Find(soundArray, sound => sound.name == soundName);
        if (s == null)
            return;
        s.audioSource.Play();
    }

    public void playOneShotSound(string soundName) {
        JLSoundClass s = Array.Find(soundArray, sound => sound.name == soundName);
        if (s == null)
            return;
        s.audioSource.PlayOneShot(s.audioClip);
    }

    public void stopSound(string soundName) {
        JLSoundClass s = Array.Find(soundArray, sound => sound.name == soundName);
        if (s == null)
            return;
        s.audioSource.Stop();
    }

    public void pauseSound(string soundName) {
        JLSoundClass s = Array.Find(soundArray, sound => sound.name == soundName);
        if (s == null)
            return;
        s.audioSource.Pause();
    }

    public void resumeSound(string soundName) {
        JLSoundClass s = Array.Find(soundArray, sound => sound.name == soundName);
        if (s == null)
            return;
        s.audioSource.UnPause();
    }
}
