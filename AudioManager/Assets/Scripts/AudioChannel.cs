using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AudioState {
    INITIALIZE,
    READY,
    PLAYING,
    PAUSED,
    INTERRUPTED,
    STOPPING,
    STOPPED
}


public class AudioChannel : MonoBehaviour
{
    //Inherent values of an AudioChannel, set by Audio Manager via InitialiseSound
    private AudioClip m_Clip;
    private AudioSource m_Source;
    private Action<AudioChannel> m_Callbacks;
    private bool m_IsLoop;
    private bool m_IsInterrupt;

    //NOTE: Ideally should be private and expose a getter
    public AudioState m_CurrState = AudioState.INITIALIZE;

    private List<AudioChannel> m_InterruptedChannels;

    //Fading Variables
    private float m_FadeTimerSpeed;
    private float m_FadeTimerCount = 1.0f;

    #region AudioChannel Private Methods
    //Main State Machine Body
    private void Update() {
        switch (m_CurrState) {
            case AudioState.PLAYING:
                PlayingState();
                break;
            case AudioState.STOPPING:
                StoppingState();
                break;
        }
    }

    // Return bool if clip finished playing, looping clips will never be true
    private bool hasSoundFinished() {
        return !m_IsLoop && GetSongProgress() >= 1f;     
    }

    private void UpdateFadeOutTimer() {
        //Update Volume
        m_FadeTimerCount -= m_FadeTimerSpeed;
        m_Source.volume = m_FadeTimerCount;

        //Stop if timer fade out
        if (m_FadeTimerCount < 0.0f) {
            StopSound();
        }
    }

    private void UpdateFadeInTimer() {
        //Update Volume, Unity will auto clamp volume
        m_FadeTimerCount += m_FadeTimerSpeed;
        m_Source.volume = m_FadeTimerCount;
    }
    #endregion

    #region AudioChannel State Methods
    private void PlayingState() {
        //If sound has finished before fade, stop it anyways
        if (hasSoundFinished()) {
            StopSound();
            return;
        }

        //Update FadeIn Timer
        UpdateFadeInTimer();
    }

    private void StoppingState() {
        //If sound has finished before fade, stop it anyways
        if (hasSoundFinished()) {
            StopSound();
            return;
        }

        //Update FadeOut Timer
        UpdateFadeOutTimer();
    }
    #endregion

    #region AudioChannel API
    //NOTE: As of now you can init even midway while a clip is playing, may wanna change this next time
    public void InitialiseSound(AudioClip clip, bool loop, bool interrupts, Action<AudioChannel> callbacks) {
        m_Clip = clip;
        m_IsLoop = loop;
        m_IsInterrupt = interrupts;
        m_Callbacks = callbacks;
        m_InterruptedChannels = new List<AudioChannel>();

        //Create audio source if null
        if(m_Source == null) {
            m_Source = gameObject.AddComponent<AudioSource>();
            m_Source.clip = clip;
        }

        m_CurrState = AudioState.READY;
    }

    //Plays the sound if it has been initialised, paused or interrupted
    public void PlaySound(){
        if(m_CurrState == AudioState.READY || m_CurrState == AudioState.PAUSED || m_CurrState == AudioState.INTERRUPTED) {

            //Check if sound interrupts all other sounds
            if (m_IsInterrupt) {
                foreach(AudioChannel audioChannel in AudioManager.Instance.m_AudioChannels) {
                    if(audioChannel.m_CurrState == AudioState.PLAYING && audioChannel != this) {
                        m_InterruptedChannels.Add(audioChannel);
                        audioChannel.InterruptSound();
                    }
                }
            }

            m_CurrState = AudioState.PLAYING;
            m_Source.Play();

        } else {
            Debug.LogError("Audio Channel for " + gameObject.name + " is not ready to be played, have you Initialised it?");
        }
    }

    //Pause the sound
    //Note: As of now, has the same function as InterruptSound();
    public void PauseSound() {
        if (m_CurrState == AudioState.PLAYING) {
            m_CurrState = AudioState.PAUSED;
            m_Source.Pause();
        } else {
            Debug.LogError("Audio Channel for " + gameObject.name + " is not currently playing and cannot be paused");
        }
    }

    //Interrupt the sound
    public void InterruptSound() {
        if (m_CurrState == AudioState.PLAYING) {
            m_CurrState = AudioState.INTERRUPTED;
            m_Source.Pause();
        } else {
            Debug.LogError("Audio Channel for " + gameObject.name + " is not currently playing and cannot be interrupted");
        }
    }

    //Reset the sound to its beginning
    public void ResetSound() {
        m_Source.time = 0f;
    }

    //Fadeout sound, only execute when song is playing
    public void FadeOutSound(float fadeSpeed = .01f, Action<AudioChannel> callbacks = null) {
        if (m_CurrState == AudioState.PLAYING) {
            m_CurrState = AudioState.STOPPING;

            m_Callbacks += callbacks;

            m_FadeTimerSpeed = fadeSpeed;
            m_FadeTimerCount = 1.0f;       
        }
    }

    //FadeIn sound, only execute when song is ready
    public void FadeInSound(float fadeSpeed = .01f, Action<AudioChannel> callbacks = null) {
        if (m_CurrState == AudioState.READY) {
            m_Callbacks += callbacks;

            m_FadeTimerSpeed = fadeSpeed;
            m_FadeTimerCount = 0.0f;

            PlaySound();
        }    
    }

    //Clean up when sound finishes, for now can be called at any state
    public void StopSound() {
        m_CurrState = AudioState.STOPPED;

        //Resume all interuppted audio channels 
        m_InterruptedChannels.ForEach(currChannel => currChannel.PlaySound());
        m_InterruptedChannels.Clear();

        //Activate all Callbacks
        if (m_Callbacks != null)
            m_Callbacks(this);

        m_Source = null;

        //Remove Self from AudioManager List
        AudioManager.Instance.m_AudioChannels.Remove(this);

        //TODO: Deactivate and Pool instead
        Destroy(gameObject);
    }

    //Returns float from 0.0 to 1.0 based on audio clip progress
    public float GetSongProgress() {
       if (m_Source == null || m_Clip == null){
            return 0f;
       }
       return (float)m_Source.timeSamples / (float)m_Clip.samples;       
    }
    #endregion
}
