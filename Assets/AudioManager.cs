using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioItem {
    public AudioClip clip;
    public long id;
}

public class AudioManager : MonoBehaviour {
    private static AudioManager _instance = null;
    public static AudioManager instance {
        get { return _instance; }
    }

    public bool isLoaded = false;
    private List<AudioItem> list;

    public void Awake() {
        _instance = this;
        list = new List<AudioItem>();
    }

    public void Init() {
        isLoaded = true;
    }

    public AudioItem GetAudio(long id) {
        if (isLoaded == false) return null;

        AudioItem output = null;
        foreach (AudioItem item in this.list) {
            output = item;
            break;
        }
        return output;
    }

    
	
}
