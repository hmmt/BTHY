using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour {
    [System.Serializable]    
    public class AudioList {
        public float bpm;
        public int time;//박자
        public int unitTime;
        public int playTime;//마디 갯수
        public List<AudioClip> audioList;
        private List<AudioSource> alterList;
        private List<AudioSource> audioSrc;
        private float unit;
        public float Unit {
            get { return unit * playTime; }
        }
        private List<AudioSource> list;
        private List<AudioSource> playing;

        public int[] volume;

        public int _index = 0;
        private int index {
            get { return _index; }
            set { _index = value; }
        }

        public int Length {
            get { return audioList.Count; }
        }
        private bool _alter;
        public bool alter {
            get { return _alter; }

        }
        private bool _playmode;
        public bool Playmode {
            get { return _playmode; }
            set { _playmode = value; }
        }

        public void Init(GameObject target) {
            _playmode = false;
            audioSrc = new List<AudioSource>();
            alterList = new List<AudioSource>();
            _alter = false;
            foreach(AudioClip clip in audioList){
                AudioSource src = target.AddComponent<AudioSource>();
                src.clip = clip;
                src.playOnAwake = false;
                src.loop = false;
                audioSrc.Add(src);
                AudioSource alterSrc = target.AddComponent<AudioSource>();
                alterSrc.clip = clip;
                alterSrc.playOnAwake = false;
                alterSrc.loop = false;
                alterList.Add(alterSrc);
            }

            if (bpm == 0) bpm = 130;

            unit = unitTime / bpm * time;
            list = audioSrc;
            playing = audioSrc;
            _index = 0;
            volume = new int[Length];
        }

        public void PlayInitial() {
            this._index = 0;
            PlayAll();
            SetVolume(_index);
        }

        public void PlayAll()
        {
            foreach (AudioSource audio in list) {
                Debug.Log(AudioSettings.dspTime);
                audio.PlayOneShot(audio.clip);
            }

            if (_alter)
            {
                list = audioSrc;
                playing = alterList;
            }
            else {
                list = alterList;
                playing = audioSrc;
            }
            
        }

        public void GetTime() {
            if (!_playmode) return;
            foreach(AudioSource audio in this.audioSrc){
               
            }
        }

        public void SetVolume(int index) {
            if (index < 0 || index > playing.Count) {
                return;
            }
            this._index = index;
            for (int i = 0; i < playing.Count; i++)
            {
                if (i.Equals(index)) {
                    playing[i].volume = 1.0f;
                    volume[i] = 1;
                    continue;
                }
                playing[i].volume = 0.0f;
                volume[i] = 0;
            }
        }
    }

    
    private static SoundManager _instance = null;
    public static SoundManager instance {
        get { return _instance; }
    }

    public AudioList bgm;

    void Awake() {
        _instance = this;
        bgm.Init(this.gameObject);
    }

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.U)) {
            PlayWithTrails(bgm);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            Debug.Log("1번");
            bgm.SetVolume(0);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("2번");
            bgm.SetVolume(1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("3번");
            bgm.SetVolume(2);
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Debug.Log("4번");
            bgm.SetVolume(3);
        }
	}

    public void PlayWithTrails(AudioList target) {
        target.PlayInitial();
        StartCoroutine(AudioTimer(target));
    }
    /// <summary>
    /// trail이 시작되는 부분에서 새로운 음악을 재생할 것이다.
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    private IEnumerator AudioTimer(AudioList target)
    {
        Debug.Log(target.Unit);
        yield return new WaitForSeconds(target.Unit);
        target.PlayAll();
        StartCoroutine(AudioTimer(target));
    }
}
