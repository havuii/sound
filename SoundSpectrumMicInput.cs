using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundSpectrumMicInput : MonoBehaviour {


	private List<string> options = new List<string>();

	private const int SampleSize = 1024;


	public float rmsValue;
	public float dbValue;
	public float pitchValue;


	private float[] samples;
	private float[] spectrum;
	private float sampleRate;

	public AudioSource source;
	private string mikki; //microphone



	void Start () {

		
		source = GetComponent<AudioSource> ();

		samples = new float[SampleSize];
		spectrum = new float[SampleSize];
		sampleRate = AudioSettings.outputSampleRate;

		// get all available microphones
		foreach (string device in Microphone.devices) {
			if (mikki == null) {
				//set default mic to first mic found.
				mikki = device;
			}
			options.Add(device);
			Debug.Log (options);
		}

		source.Stop ();
		source.clip = Microphone.Start (mikki, true, 10, 44100);
		source.loop = true;
		Debug.Log (Microphone.IsRecording(mikki).ToString());

		if (Microphone.IsRecording (mikki)) {
			while (!(Microphone.GetPosition (mikki) > 0)) {
			}

			Debug.Log ("recording");
			source.Play ();
		} else {
			Debug.Log ("not recording");
		}
			
	}
		

	void Update () {

		AnalyzeSound ();
	}


	private void AnalyzeSound (){
	
		source.GetOutputData (samples, 0);

		int i = 0;
		float sum = 0;
		for (; i < SampleSize; i++) {
			sum += samples [i] * samples [i];
		}
		rmsValue = Mathf.Sqrt (sum / SampleSize);

		dbValue = 20 * Mathf.Log10 (rmsValue / 0.1f);

		source.GetSpectrumData (spectrum, 0, FFTWindow.BlackmanHarris);

		float maxV = 0;
		var maxN = 0;
		for (i = 0; i < SampleSize; i++) {
			if (!(spectrum [i] > maxV) || !(spectrum [i] > 0.0f))
				continue;

			maxV = spectrum [i];
			maxN = i;	
		}

		float freqN = maxN;
		if (maxN > 0f && maxN < SampleSize - 1) {
			var dL = spectrum [maxN - 1] / spectrum [maxN];
			var dR = spectrum [maxN + 1] / spectrum [maxN];
			freqN += 0.5f * (dR * dR - dL * dL);
		}

		pitchValue = freqN * (sampleRate / 2) / SampleSize;
	}
		
}
