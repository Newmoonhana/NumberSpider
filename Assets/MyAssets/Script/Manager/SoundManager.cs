using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource ClickPlayer;
    public AudioClip MoveBlockAudio;
    public AudioClip ClickButtonAudio;
    public AudioClip FusionBlockAudio;
    public AudioClip ScoreUpAudio;
    public bool SoundOn = true;

    private void Awake()
    {
        ClickPlayer = transform.GetChild(0).GetComponent<AudioSource>();
    }

    public void Play(AudioSource soure, AudioClip clip)
    {
        if (SoundOn)
        {
            soure.clip = clip;
            soure.Play();
        }
    }
    public void Pause(AudioSource soure)
    {
        soure.Pause();
    }
    public void Relay(AudioSource soure, AudioClip clip)
    {
        soure.clip = clip;
    }
}
