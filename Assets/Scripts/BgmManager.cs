using UnityEngine;
using System.Collections;

public class BgmManager : MonoBehaviour {

	public AudioSource audioSource;

	public AudioClip normalBGM;

	private AudioClip currentBGM;

	// Use this for initialization
	void Start () {
		SetBgm (normalBGM);
	}

	void FixedUpdate () {
	
	}

	private void SetBgm(AudioClip clip)
	{
		currentBGM = clip;
		audioSource.clip = normalBGM;
	}

	public void UpdateBgm()
	{
	}
}
