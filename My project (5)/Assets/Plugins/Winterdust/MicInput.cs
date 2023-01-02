using UnityEngine;
using Gesticulator;
using System.Collections.Generic; // So we can use List<>
using System.Collections;
using FrostweepGames.Plugins.Native;
using UnityEngine.Audio;
using System;
using System.IO;

// [RequireComponent(typeof(AudioSource))]

public class MicInput : MonoBehaviour 
{
	
	public float minThreshold = 0;
	public float frequency = 0.0f;
	public int audioSampleRate = 44100;
	public string microphone;


	private List<string> options = new List<string>();
	private int samples = 8192; 
	private AudioSource audioSource;
    private AudioClip audioClip;
    
    public void Start() 
	{
        //get components you'll need
        audioSource = GetComponent<AudioSource> ();

		// get all available microphones
		foreach (string device in Microphone.devices) {
			if (microphone == null) {
				//set default mic to first mic found.
				microphone = device;
			}
			options.Add(device);
		}
		microphone = options[PlayerPrefsManager.GetMicrophone ()];
		
		UpdateMicrophone ();
	}

	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			microphone = options[PlayerPrefsManager.GetMicrophone ()];
			Record();
			Debug.Log("key down");
			
		}

		if(Input.GetKeyUp(KeyCode.Space))
		{
			Stop();
			ExportAudio();
			PyGesticulatorTestor_Avatar.main();
			Debug.Log("key up");

		}
	} 
	
    public void PlayRecordedAudio()
    {
        if (audioClip == null)
            return;

        audioSource.clip = audioClip;
        audioSource.Play();
    }

    public void Stop()
    {

        if (!HasConnectedMicDevices())
            return;
        if (!IsRecordingNow(microphone))
            return;

        //audioSource.Stop();
        Microphone.End(microphone);
    }

    public void Record()
    {
        if (!HasConnectedMicDevices())
		{
			Debug.Log("error");
            return;
		}
        UpdateMicrophone();
        Debug.Log("recording started with " + microphone);
        audioSource.clip = audioClip;
        
    }

    
    public void ExportAudio()
    {
        SavWav.Save("Test1", audioClip);
		
    }

    
	void UpdateMicrophone(){
		//Start recording to audioclip from the mic
		audioClip = Microphone.Start(microphone, true, 10, audioSampleRate);
        
		// Mute the sound with an Audio Mixer group becuase we don't want the player to hear it
		Debug.Log(Microphone.IsRecording(microphone).ToString());
	}

    public static bool HasConnectedMicDevices()
    {
        return Microphone.devices.Length > 0;
    }

    public static bool IsRecordingNow(string deviceName)
    {
        return Microphone.IsRecording(deviceName);
    }
}