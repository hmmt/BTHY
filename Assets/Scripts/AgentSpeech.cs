using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AgentSpeech : MonoBehaviour {
    public RectTransform rt;
    private Text speechText;
    private TextAnchor standard;
    private string copy;
    private int size;
    private Vector2 init_size;
    private Vector2 mini_size;

    public void Start() {
        //rt = transform.GetComponent<RectTransform>();
        speechText = rt.GetChild(0).GetComponent<Text>();
        size = speechText.fontSize;
        standard = speechText.alignment;
        init_size = rt.sizeDelta;
        mini_size = new Vector2(rt.sizeDelta.x / 2, rt.sizeDelta.y);
    }

    public void FixedUpdate() {
        if (rt.gameObject.activeSelf ) {
            if (Camera.main.orthographicSize < 8)
            {
                speechText.alignment = standard;
                speechText.fontSize = size;
                SetSpeech(copy);
                rt.sizeDelta = init_size;
            }
            else
            {
                speechText.alignment = TextAnchor.MiddleCenter;
                speechText.fontSize = size * 3;
                SetSpeech(". . .");
                rt.sizeDelta = mini_size;
            }
        }
    }

    private void SetSpeech(string text) {
        speechText.text = text;
        float h = speechText.preferredHeight;
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, h + 10f);
    }

    public void showSpeech(string speech)
    {
        copy = speech;
        if (!rt.gameObject.activeSelf) {
            SetSpeech(speech);

            rt.gameObject.SetActive(true);
            TimerCallback.Create(5.0f, rt.gameObject, delegate()
            {
                rt.gameObject.SetActive(false);
            });

        }
    }

    public void turnOnDoingSkillIcon(bool turnOn)
    {
        rt.gameObject.SetActive(turnOn);
    }
}

