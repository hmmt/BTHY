using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIEffectManager : MonoBehaviour {
    private static UIEffectManager _instance = null;
    public static UIEffectManager instance {
        get {
            return _instance;
        }
    }

    public Image NoiseImage;

    public void Awake() {
        _instance = this;
    }

    public void Start() {
        NoiseImage.gameObject.SetActive(false);
    }

    public void Noise(float time) {
        this.NoiseImage.gameObject.SetActive(true);
        //smth effect
        StartCoroutine(DelayedDisable(NoiseImage.gameObject, time));
    }

    public IEnumerator DelayedDisable(GameObject target, float delayedTime) {
        yield return new WaitForSeconds(delayedTime);
        target.gameObject.SetActive(false);
    }
}
