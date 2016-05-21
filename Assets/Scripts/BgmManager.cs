using UnityEngine;
using System.Collections;

public class BgmManager : MonoBehaviour, IObserver {
    private static BgmManager _instance = null;
    public static BgmManager instance {
        get { return _instance; }
    }

	public AudioSource audioSource;

	public AudioClip normalBGM;
	public AudioClip alarmBGM;
	public AudioClip storyBGM;

	private AudioClip currentBGM;

	float escapeTimer = 0;

	// Use this for initialization
	void Awake()
	{
        _instance = this;
	}
	void Start () {
		//SetBgm (normalBGM);
	}

	void OnEnable()
	{
		Notice.instance.Observe(NoticeName.EscapeCreature, this);
	}

	void OnDisable()
	{
		Notice.instance.Remove(NoticeName.EscapeCreature, this);
	}

	void FixedUpdate () {
		if (escapeTimer > 0)
		{
			escapeTimer -= Time.deltaTime;
			SetBgm (alarmBGM);
		}
		else if(GameManager.currentGameManager.state != GameState.STOP)
		{
			SetBgm (normalBGM);
		}
		else if(GameManager.currentGameManager.state == GameState.STOP)
		{
			SetBgm (storyBGM);
		}
	}

	public void SetBgm(AudioClip clip)
	{
		if (clip != currentBGM) {
			audioSource.Stop ();
			currentBGM = clip;
			audioSource.clip = currentBGM;
			audioSource.Play ();
		}
	}

	public void UpdateBgm()
	{
	}

	public void OnNotice(string notice, params object[] param)
	{
		if (notice == NoticeName.EscapeCreature)
		{
			escapeTimer = 5;
		}
	}
}
