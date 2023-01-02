using UnityEngine;

using System.Collections.Generic; // So we can use List<>
using System.Collections;
using FrostweepGames.Plugins.Native;
using UnityEngine.Audio;
using System;
using System.IO;

// [RequireComponent(typeof(AudioSource))]

public class KoAudio : MonoBehaviour 
{
	
	public AudioSource audioSource;
    public AudioClip audioClip;

    public void Start() 
	{   
        audioSource = GetComponent<AudioSource>();
	}

	public void Update()
	{
		
	} 
	
    public void PlayRecordedAudio()
    {
        if (audioClip == null)
            return;

        audioSource.clip = audioClip;
        audioSource.Play();
    }

    
	void UpdateMicrophone()
	{
		audioClip = Resources.Load("KoAudio.wav") as AudioClip;

		// Mute the sound with an Audio Mixer group becuase we don't want the player to hear it
		Debug.Log("Avatar is speaking");

	}
}