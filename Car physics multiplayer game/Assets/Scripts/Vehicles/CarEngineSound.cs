using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarEngineSound : MonoBehaviour {

	public AudioSource engineAudioSource;
	public AudioClip[] engineSamples;
	public AudioClip idleSound;
	public float idleThreshold = 0.1f;
	public float minPitch = 0.3f;
	public float maxPitch = 1.65f;
	public float globalVolume = 0.25f;
	public float minVolume = 0.5f;
	public float maxVolume = 1.0f;
	public float idleVolume = 1.0f;

	private CarPhysics fizixScript;
	private int currentClip = 0;

	void Start () {
		fizixScript = GetComponent<CarPhysics>();
		engineAudioSource.loop = true;
		engineAudioSource.clip = idleSound;
		engineAudioSource.volume = idleVolume * globalVolume;
		engineAudioSource.Play();
	}

	void Update () {

		if (1.0f - fizixScript.accelMult < idleThreshold) {
			if (engineAudioSource.clip != idleSound) {
				engineAudioSource.clip = idleSound;
				engineAudioSource.pitch = 1.0f;
				engineAudioSource.volume = idleVolume * globalVolume;
				engineAudioSource.Play();
			}
		} else {
			if (engineAudioSource.clip != engineSamples[3]) {
				engineAudioSource.clip = engineSamples[3];
				engineAudioSource.Play();
			}
			var lerpVal = (1.0f - fizixScript.accelMult - idleThreshold) / (1.0f - idleThreshold);
			engineAudioSource.pitch = Mathf.Lerp(minPitch, maxPitch, lerpVal);
			engineAudioSource.volume = Mathf.Lerp(minVolume, maxVolume, lerpVal) * globalVolume;
		}

		/*
		var sampleNum = Mathf.FloorToInt((1.0f - fizixScript.accelMult) * (engineSamples.Length + 1));
		if (sampleNum >= engineSamples.Length) {
			sampleNum = engineSamples.Length - 1;
		}
		if (sampleNum != currentClip) {
			//engineAudioSource.Stop();
			if (sampleNum == 0)
				engineAudioSource.clip = idleSound;
			else
				engineAudioSource.clip = engineSamples[sampleNum - 1];
			engineAudioSource.pitch = 1.0f;
			engineAudioSource.Play();
			currentClip = sampleNum;
		} else {
			var nextStep = ((currentClip + 1.0f) / (engineSamples.Length + 1.0f));
			var prevStep = ((float)currentClip / (engineSamples.Length + 1.0f));
			var lerpVal = ((1.0f - fizixScript.accelMult) - prevStep) / (nextStep - prevStep);
			engineAudioSource.pitch = Mathf.Lerp(1.0f, maxPitch, lerpVal);
		}
		*/
	}
}
