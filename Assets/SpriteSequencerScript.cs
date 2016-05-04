using UnityEngine;
using System.Collections;

public class SpriteSequencerScript : MonoBehaviour {
    //src = Resouces in Resouce folder
    public string src;
    public int fps;
    public float speed = 1f;
    public SpriteRenderer renderer = null;
    public bool isRunning = false;
    public bool isEnabled = true;

    Sprite[] sprites;
    public void Awake() {
        sprites = Resources.LoadAll<Sprite>(src);
        if (renderer == null) {
            renderer = this.GetComponent<SpriteRenderer>();
        }
        if (fps == 0) {
            fps = 30;
        }
        if (speed == 0) {
            speed = 1f;
        }
        if (sprites.Length < 1) {
            this.isEnabled = false;
        }
    }

    public void Start() {
        Run();
    }

    public void OnEnabled() {
        Run();
    }

    public void OnDisalbed(){
        Stop();
    }

    public void Run() {
        if (!isEnabled) return;
        if (isRunning) return;
        isRunning = true;
        StartCoroutine(Sequencer());
    }

    public void Stop() {
        if (!isEnabled) return;
        if (!isRunning) return;
        isRunning = false;
        StopCoroutine(Sequencer());
    }

    IEnumerator Sequencer() {
        float unitTime = (1f / speed) / fps;
        int index = 0;
        while (true) {
            renderer.sprite = sprites[(index++)%sprites.Length];
            
            yield return new WaitForSeconds(unitTime);
        }

    }
}
