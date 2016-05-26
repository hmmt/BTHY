using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class UIEffectModule
{
    public string name;
    public string src;
    public bool hasSequence;
    public float time;
    public Image targetImage = null;

    public int defaultFPS = 30;

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
        if (targetImage != null) {
            targetImage.gameObject.SetActive(false);
        }
        
        if (hasSequence)
        {
            Sprite[] sprites = Resources.LoadAll<Sprite>(src);
            list = new List<Sprite>(sprites);
        }
        else {
            spriteUnit = Resources.Load<Sprite>(src);
            Debug.Log(spriteUnit.name);
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
    public Image EffectTargetImage;

    public void Awake() {
        _instance = this;
    }

    public void Start() {
        NoiseImage.gameObject.SetActive(false);
        EffectTargetImage.gameObject.SetActive(false);
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

    public UIEffectModule GetModule(string name) {
        UIEffectModule output = null;
        foreach (UIEffectModule m in this.modules) {
            if (m.name == name) {
                output = m;
                break;
            }
        }
        return output;
    }

    public void ActivateUIEffect(string targetName, float displayTime, int fps, bool useDefaultImage) {
        StartCoroutine(ModuleActivator(GetModule(targetName), displayTime, fps, useDefaultImage));
    }

    IEnumerator ModuleActivator(UIEffectModule target, float displayTime, int fps, bool useDefaultImage) {
        int unit = fps;
        if (fps == 0) {
            unit = target.defaultFPS;
        }

        float elapsed = 0;
        if (NoiseImage.gameObject.activeInHierarchy) {
            NoiseImage.gameObject.SetActive(false);
        }
        Image targetImage = null;
        if (useDefaultImage || target.targetImage == null)
        {
            targetImage = EffectTargetImage;
        }
        else {
            EffectTargetImage.gameObject.SetActive(false);
            targetImage = target.targetImage;
        }

        targetImage.gameObject.SetActive(true);
        targetImage.sprite = target.current;
        while (true) {
            if (elapsed > displayTime) {
                break;
            }

            yield return new WaitForSeconds(1 / (float)unit);
            target.Update();
            elapsed += Time.deltaTime;
            targetImage.sprite = target.current;
        }
        targetImage.gameObject.SetActive(false);


    }
}
