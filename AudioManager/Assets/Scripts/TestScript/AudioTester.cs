using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTester : MonoBehaviour
{
    public AudioClip BGMClip1;
    public AudioClip BGMClip2;

    public AudioClip InteruptClip1;

    public AudioClip SFXClip1;

    private AudioChannel bgm1;


    // Update is called once per frame
    void Update()
    {
        //Play Loop song example
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1)) {
            bgm1 = AudioManager.Instance.PlayNewAudioChannel(AudioType.BGM, BGMClip1, loop: true);
        }

        //Fade Out then Fade In example
        if (bgm1 && (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))) {
            bgm1.FadeOutSound(callbacks: delegate (AudioChannel s) {
                AudioChannel bgm2 = AudioManager.Instance.NewAudioChannel(AudioType.BGM, BGMClip2, loop: true);
                bgm2.FadeInSound();
            });
        }

        //Fade Out and Fade In Example (CrossFade)
        if (bgm1 && (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))) {
            bgm1.FadeOutSound();
            AudioChannel bgm2 = AudioManager.Instance.NewAudioChannel(AudioType.BGM, BGMClip2, loop: true);
            bgm2.FadeInSound();
        }

        //Play SFX example
        if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4)) {
            AudioManager.Instance.PlayNewAudioChannel(AudioType.SFX, SFXClip1);
        }

        //Play Interrupting sound example
        if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5)) {
            AudioManager.Instance.PlayNewAudioChannel(AudioType.SFX, InteruptClip1, interrupts: true);
        }
    }
}
