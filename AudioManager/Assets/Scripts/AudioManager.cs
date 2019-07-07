using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AudioType {
    SFX,
    BGM,
    AMB
}


public class AudioManager : Singleton<AudioManager> {

    public List<AudioChannel> m_AudioChannels = new List<AudioChannel>();

    public AudioChannel m_BGMChannel;

    /// Creates a new sound, registers it, gives it the properties specified, and starts playing it
    public AudioChannel PlayNewAudioChannel(AudioType audioType, AudioClip audioClip, bool loop = false, bool interrupts = false, Action<AudioChannel> callbacks = null) {
        AudioChannel channel = NewAudioChannel(audioType, audioClip, loop, interrupts, callbacks);
        channel.PlaySound();
        return channel;
    }

    /// Creates a new sound, registers it, and gives it the properties specified
    public AudioChannel NewAudioChannel(AudioType audioType, AudioClip audioClip, bool loop = false, bool interrupts = false, Action<AudioChannel> callbacks = null) {

        //Create a GameObject to represent the audio channel and child it to the Audiomanager
        GameObject newObject = new GameObject();
        newObject.transform.parent = transform;
        newObject.name = audioClip.name;

        //Create Audio Channel
        AudioChannel newChannel = newObject.AddComponent<AudioChannel>();
        newChannel.InitialiseSound(audioClip, loop, interrupts, callbacks);

        //Add to list of active AudioChannels
        m_AudioChannels.Add(newChannel);

        return newChannel;
    }
}
