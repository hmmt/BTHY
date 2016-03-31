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
    private static float length = 240f;
    public Sprite[] BgSprite;
    private Image Bg;

    public void Start() {
        textRect = textObject.GetChild(0).GetComponent<RectTransform>();
        bgRect = textObject.GetComponent<RectTransform>();
        speechText = textRect.GetComponent<Text>();

        size = speechText.fontSize;
        standard = speechText.alignment;
        init_size = textRect.sizeDelta;

        mini_size = new Vector2(init_size.x / 2, init_size.y);
        textObject.gameObject.SetActive(false);
        //Bg = textObject.GetComponent<Image>();

    }

    public void FixedUpdate() {

        if (textObject.gameObject.activeSelf) {
            if (Camera.main.orthographicSize < 8)
            {
                speechText.alignment = standard;
                speechText.fontSize = size;
                SetSpeech(copy);
                /*
                int index = 1;
                if (speechText.preferredWidth > length) {
                    int head = (int)(speechText.preferredWidth / length);
                    index += head;
                    
                }
                if (index > BgSprite.Length) index = BgSprite.Length-1;
                */
               // Bg.sprite = BgSprite[index];
                //textRect.sizeDelta = new Vector2(speechText.preferredWidth, (speechText.fontSize + 2f) * index);
                //bgRect.sizeDelta = new Vector2(textRect.sizeDelta.x + 20f, textRect.sizeDelta.y + 20f);
                
            }
            else {
                speechText.alignment = TextAnchor.MiddleCenter;
                speechText.fontSize = size * 3;
                SetSpeech(". . .");
                textRect.sizeDelta = mini_size;
                bgRect.sizeDelta = mini_size;
                //Bg.sprite = BgSprite[0];
            }
        }
    }

    private void SetSpeech(string text) {
        speechText.text = text;
        //bgRect.sizeDelta = new Vector2(textRect.sizeDelta.x + 100f, h + 10f);
        bgRect.sizeDelta = new Vector2(speechText.rectTransform.sizeDelta.x + 10f,
                                    speechText.rectTransform.sizeDelta.y + 10f);
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

