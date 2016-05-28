using UnityEngine;
using System.Collections;

public class SoundEffectPlayer : MonoBehaviour {

	private float destroyTime; 
	private float elapsedTime = 0;
    private bool onshot = true;

    public bool halted = false;
    AudioSource src = null;

	void FixedUpdate()
	{
        if (this.onshot)
        {
            elapsedTime += Time.deltaTime;

            if (elapsedTime > destroyTime)
                Stop();
        }
	}

    public static SoundEffectPlayer PlayOnce(string filename, Vector2 position)
	{
		GameObject newEffect = Prefab.LoadPrefab ("SoundEffectPlayer");

		SoundEffectPlayer effect = newEffect.GetComponent<SoundEffectPlayer> ();


		AudioSource source = newEffect.GetComponent<AudioSource> ();
		//source.clip = Resources.Load<AudioClip> ("Sounds/" + filename);

		AudioClip clip = Resources.Load<AudioClip> ("Sounds/" + filename);

		newEffect.transform.position = new Vector3 (position.x, position.y, Camera.main.transform.position.z);

        
		source.PlayOneShot(clip);

		effect.destroyTime = clip.length;
        return effect;
	}

    public static SoundEffectPlayer PlayOnce(string filename, Vector2 position, float volume)
    {
        GameObject newEffect = Prefab.LoadPrefab("SoundEffectPlayer");

        SoundEffectPlayer effect = newEffect.GetComponent<SoundEffectPlayer>();


        AudioSource source = newEffect.GetComponent<AudioSource>();
        //source.clip = Resources.Load<AudioClip> ("Sounds/" + filename);

        AudioClip clip = Resources.Load<AudioClip>("Sounds/" + filename);

        newEffect.transform.position = new Vector3(position.x, position.y, Camera.main.transform.position.z);


        source.PlayOneShot(clip);

        effect.destroyTime = clip.length;
        return effect;
    }

    public static SoundEffectPlayer PlayOnce(string filename, Vector2 position, AudioRolloffMode mode)
    {
        GameObject newEffect = Prefab.LoadPrefab("SoundEffectPlayer");

        SoundEffectPlayer effect = newEffect.GetComponent<SoundEffectPlayer>();


        AudioSource source = newEffect.GetComponent<AudioSource>();
        //source.clip = Resources.Load<AudioClip> ("Sounds/" + filename);

        AudioClip clip = Resources.Load<AudioClip>("Sounds/" + filename);

        newEffect.transform.position = new Vector3(position.x, position.y, Camera.main.transform.position.z);

        source.rolloffMode = mode;
        source.PlayOneShot(clip);

        effect.destroyTime = clip.length;
        return effect;
    }

    /// <summary>
    /// for stop, Invoker should have this class
    /// </summary>
    /// <param name="filename"></param>
    /// <param name="position"></param>
    public static SoundEffectPlayer Play(string filename, Transform transf) {
        GameObject newEffect = Prefab.LoadPrefab("SoundEffectPlayer");
        Vector2 position = transf.position;
        newEffect.transform.SetParent(transf);
        newEffect.transform.localScale = Vector3.one;

        SoundEffectPlayer effect = newEffect.GetComponent<SoundEffectPlayer>();
        effect.onshot = false;

        AudioSource source = newEffect.GetComponent<AudioSource>();
        //source.clip = Resources.Load<AudioClip> ("Sounds/" + filename);

        AudioClip clip = Resources.Load<AudioClip>("Sounds/" + filename);
        newEffect.transform.position = new Vector3(position.x, position.y, Camera.main.transform.position.z);
        //Debug.Log(clip.name + " " + filename);
        //source.volume
        source.clip = clip;
        source.loop = true;
        source.Play();
        effect.src = source;
        return effect;
    }

    /// <summary>
    /// volume -> 0 ~ 1
    /// </summary>
    /// <param name="filename"></param>
    /// <param name="transf"></param>
    /// <param name="volume">0.0 ~ 1.0</param>
    /// <returns></returns>
    public static SoundEffectPlayer Play(string filename, Transform transf, float volume)
    {
        GameObject newEffect = Prefab.LoadPrefab("SoundEffectPlayer");
        Vector2 position = transf.position;
        newEffect.transform.SetParent(transf);
        newEffect.transform.localScale = Vector3.one;

        SoundEffectPlayer effect = newEffect.GetComponent<SoundEffectPlayer>();
        effect.onshot = false;

        AudioSource source = newEffect.GetComponent<AudioSource>();
        //source.clip = Resources.Load<AudioClip> ("Sounds/" + filename);

        AudioClip clip = Resources.Load<AudioClip>("Sounds/" + filename);
        newEffect.transform.position = new Vector3(position.x, position.y, Camera.main.transform.position.z);
        Debug.Log(clip.name + " " + filename);
        //source.volume
        source.clip = clip;
        source.loop = true;
        source.volume = volume;
        source.Play();
        effect.src = source;
        return effect;
    }

    public void Halt() {
        src.Stop();
        halted = true;
    }

    public void ReStart() {
        src.Play();
        halted = false;
    }

    public void Stop() {
        
        Destroy(gameObject);
    }


}
