using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class JLSoundClass
{
    public string name;
    public AudioClip audioClip;
    public AudioMixerGroup audioMixerGroup;

    [Range(0f, 1f)]
    public float volume;
    [Range(0.1f, 3f)]
    public float pitch;

    public bool playOnAwake;
    public bool loop;

    [HideInInspector]
    public AudioSource audioSource;
}
