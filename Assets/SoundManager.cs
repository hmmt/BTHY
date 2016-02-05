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
        public bool changing;

        public int[] volume;

        private int _index = 0;
        public int index {
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
            changing = false;
        }

        public void PlayInitial() {
            this._index = 0;
            PlayAll();
            SetVolume(_index);
            PrintAudioVolumeAll();
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

        public AudioSource GetPlayingAudio() {
            return playing[_index];
        }

        public AudioSource GetAudioWithIndex(int index) {
            if (index < 0 || index > playing.Count) return null;
            return playing[index];
        }

        public void ChangePlayingAudio(int index) {
            this._index = index;
        }

        public void SetVolumeWithValue(int index, float volume) {
            if (volume > 1.0f || volume < 0.0f) return;
            AudioSource src = GetAudioWithIndex(index);
            src.volume = volume;
        }

        public void SetVolumeWithValue(AudioSource src, float volume) {
            if (volume > 1.0f || volume < 0.0f) return;
            src.volume = volume;
        }

        public void PrintAudioVolumeAll() {
            foreach (AudioSource src in playing) {
                Debug.Log(src.volume);
            }
        }

    }
    
    private static SoundManager _instance = null;
    public static SoundManager instance {
        get { return _instance; }
    }

    public AudioList bgm;
    private bool changing = false;

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
            StartCoroutine(AudioChangeWithFadeEffect(bgm, 0));
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            StartCoroutine(AudioChangeWithFadeEffect(bgm, 1));
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            StartCoroutine(AudioChangeWithFadeEffect(bgm, 2));
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            StartCoroutine(AudioChangeWithFadeEffect(bgm, 3));
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

    private IEnumerator AudioChangeWithFadeEffect(AudioList target, int index) {
        if (changing) {
            yield break;
        }
        int cnt = 100;
        AudioSource fadeOut, fadeIn;
        changing = true;
        fadeOut = target.GetPlayingAudio();
        fadeIn = target.GetAudioWithIndex(index);
        
        if(fadeOut.Equals(fadeIn)){
            changing = false;
            yield break;
        }

        while (cnt > 0)
        {
            yield return new WaitForSeconds(0.02f);
            cnt--;
            float volume = cnt * 0.01f;
            target.SetVolumeWithValue(fadeIn, 1f - volume);
            target.SetVolumeWithValue(fadeOut, volume);
        }
        target.index = index;
        changing = false;
    }
}
