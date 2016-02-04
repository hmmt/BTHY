using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AgentSpeech : MonoBehaviour {
    public RectTransform textObject;
    private RectTransform textRect;
    private RectTransform bgRect;

    private Text speechText;
    private TextAnchor standard;
    private string copy;
    private int size;
    public float spacing;
    private Vector2 init_size;
    private Vector2 mini_size;
    private Vector2 bg_size;

    public void Start() {
        textRect = textObject.GetChild(0).GetComponent<RectTransform>();
        bgRect = textObject.GetComponent<RectTransform>();
        speechText = textRect.GetComponent<Text>();

        size = speechText.fontSize;
        standard = speechText.alignment;
        init_size = textRect.sizeDelta;
        mini_size = new Vector2(init_size.x / 2, init_size.y);
        textObject.gameObject.SetActive(false);
    }

    public void FixedUpdate() {

        if (textObject.gameObject.activeSelf) {
            if (Camera.main.orthographicSize < 8)
            {
                speechText.alignment = standard;
                speechText.fontSize = size;
                SetSpeech(copy);
                textRect.sizeDelta = init_size;
                bgRect.sizeDelta = bg_size;
            }
            else {
                speechText.alignment = TextAnchor.MiddleCenter;
                speechText.fontSize = size * 3;
                SetSpeech(". . .");
                textRect.sizeDelta = mini_size;
                bgRect.sizeDelta = mini_size;
            }
        }
    }

    private void SetSpeech(string text) {
        speechText.text = text;
        float h = speechText.preferredHeight;
        textRect.sizeDelta = new Vector2(textRect.sizeDelta.x, h);
        init_size = new Vector2(textRect.sizeDelta.x, textRect.sizeDelta.y);
        bgRect.sizeDelta = new Vector2(textRect.sizeDelta.x + 10f, h + 10f);
        bg_size = new Vector2(bgRect.sizeDelta.x, bgRect.sizeDelta.y);
    }

    public void showSpeech(string speech)
    {
        copy = speech;
        if (!textObject.gameObject.activeSelf){
            SetSpeech(copy);
            textObject.gameObject.SetActive(true);
            TimerCallback.Create(5.0f, textObject.gameObject, delegate()
            {
                textObject.gameObject.SetActive(false);
            });
        }

    }
     
    public void showSpeech(string speech, float time) {
        copy = speech;
        if (!textObject.gameObject.activeSelf)
        {
            SetSpeech(copy);
            textObject.gameObject.SetActive(true);
            TimerCallback.Create(time, textObject.gameObject, delegate()
            {
                textObject.gameObject.SetActive(false);
            });
        }

    }

    public void turnOnDoingSkillIcon(bool turnOn)
    {
        
        textObject.gameObject.SetActive(turnOn);
    }
}

