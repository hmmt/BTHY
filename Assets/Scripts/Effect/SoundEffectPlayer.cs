using UnityEngine;
using System.Collections;

public class SoundEffectPlayer : MonoBehaviour {

	private float destroyTime; 
	private float elapsedTime = 0;

	void FixedUpdate()
	{
		elapsedTime += Time.deltaTime;

		if (elapsedTime > destroyTime)
			Destroy (gameObject);
	}

	public static void PlayOnce(string filename, Vector2 position)
	{
		GameObject newEffect = Prefab.LoadPrefab ("SoundEffectPlayer");

		SoundEffectPlayer effect = newEffect.GetComponent<SoundEffectPlayer> ();


		AudioSource source = newEffect.GetComponent<AudioSource> ();
		//source.clip = Resources.Load<AudioClip> ("Sounds/" + filename);

		AudioClip clip = Resources.Load<AudioClip> ("Sounds/" + filename);

		newEffect.transform.position = new Vector3 (position.x, position.y, Camera.main.transform.position.z);

		//source.volume

		source.PlayOneShot(clip);

		effect.destroyTime = clip.length;
	}
}
