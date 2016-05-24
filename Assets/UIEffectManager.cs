using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class UIEffectModule {
    public string src;
    public bool hasSequence;
    public float time;

    public List<Sprite> list;
    int index = 0;
    
    public Sprite spriteUnit;

    public Sprite current {
        get {
            if (hasSequence == false)
            {
                return spriteUnit;
            }
            else {
                return list[index];
            }
        }
    }

    public void Init() {
        if (hasSequence)
        {
            Sprite[] sprites = Resources.LoadAll<Sprite>(src);
            list = new List<Sprite>(sprites);
        }
        else {
            spriteUnit = Resources.Load<Sprite>(src);
        }
    }

    public void Update() {
        if (!hasSequence) {
            return;
        }
        index = (index + 1) % list.Count;
    }
}

public class UIEffectManager : MonoBehaviour {
    private static UIEffectManager _instance = null;
    public static UIEffectManager instance {
        get {
            return _instance;
        }
    }

    public List<UIEffectModule> modules;

    [HideInInspector]
    public UIEffectModule currentModule;

    public Image NoiseImage;

    public void Awake() {
        _instance = this;
    }

    public void Start() {
        NoiseImage.gameObject.SetActive(false);

        foreach (UIEffectModule module in this.modules) {
            module.Init();
        }
    }

    public void Noise(float time) {
        this.NoiseImage.gameObject.SetActive(true);
        //smth effect
        StartCoroutine(DelayedDisable(NoiseImage.gameObject, time));
    }

    public void NoiseScreen(float time, int fps) {
        this.NoiseImage.gameObject.SetActive(true);
        StartCoroutine(NoiseScreenRoutine(fps, NoiseImage));
    }

    public IEnumerator DelayedDisable(GameObject target, float delayedTime) {
        yield return new WaitForSeconds(delayedTime);
        target.gameObject.SetActive(false);
    }

    public IEnumerator NoiseScreenRoutine(int fps, Image image)
    {
        currentModule = modules[0];
        float elapsed = 0;
        while (true) {
            if (elapsed > currentModule.time) {
                break;
            }
            yield return new WaitForSeconds(1 / (float)fps);
            currentModule.Update();
            elapsed += Time.deltaTime;
            image.sprite = currentModule.current;
        }
        currentModule = null;
        image.gameObject.SetActive(false);
    }
}
