using UnityEngine; 
using System.Collections;
using UnityEngine.UI;

public class AgentSpeech : MonoBehaviour {
    public RectTransform textObject;//parent for text and image 
    public Sprite[] BgSprite;

    public Text textItem;
    public Image textBackground;
    RectTransform textRectTransform;
    RectTransform bgRectTransform;

    Vector2 currentSizedelta;
    Vector2 smallSizeDelta;

    public float maxX;
    public float spacingX, spacingY;
    float sizeY;
    int initialFontSize;
    private TextAnchor standard;
    private string copy;
    Sprite currentBg;
    Sprite renderingTarget;
    TimerCallback currentTimer = null;
    
    public void Start() {
        //this.textItem.canvasRenderer.relativeDepth = this.textItem.transform.parent.GetComponent<CanvasRenderer>().relativeDepth + 2;
        
        this.standard = this.textItem.alignment;
        this.textRectTransform = this.textItem.rectTransform;
        this.bgRectTransform = this.textBackground.rectTransform;
        this.sizeY = this.textItem.fontSize+4f;
        this.initialFontSize = this.textItem.fontSize;
        currentSizedelta = new Vector2(textRectTransform.sizeDelta.x, textRectTransform.sizeDelta.y);
        smallSizeDelta = new Vector2(currentSizedelta.x / 2, currentSizedelta.y / 2);
        renderingTarget = this.textBackground.sprite;

        textObject.gameObject.SetActive(false);
    }

    public void FixedUpdate() {

        if (textObject.gameObject.activeSelf)
        {

            if (Camera.main.orthographicSize < 8)
            {
                textItem.alignment = standard;
                textItem.fontSize = this.initialFontSize;
                SetSpeech(copy);
                textRectTransform.sizeDelta = currentSizedelta;
                bgRectTransform.sizeDelta = new Vector2(currentSizedelta.x + spacingX,
                                                        currentSizedelta.y + spacingY);
                this.textBackground.sprite = this.currentBg;
            }
            else
            {
                textItem.alignment = TextAnchor.MiddleCenter;
                textItem.fontSize = initialFontSize * 3;
                SetSpeech(". . . ");
                textRectTransform.sizeDelta = smallSizeDelta;
                bgRectTransform.sizeDelta = smallSizeDelta;
                this.textBackground.sprite = this.BgSprite[0];
            }
        }
        else {
            currentTimer = null;
        }
    }

    private void SetSpeech(string text) {
        this.textItem.text = text;
        int bgIndex = 0;
        if (this.textItem.preferredWidth > maxX)
        {
            int rate = (int)(this.textItem.preferredWidth / maxX);
            textRectTransform.sizeDelta = new Vector2(maxX, (rate + 2) * sizeY);
            if (rate > 5) bgIndex = 5;
            else {
                bgIndex = rate;
            }
        }
        else {
            textRectTransform.sizeDelta = new Vector2(this.textItem.preferredWidth, sizeY);
            bgIndex = 1;
        }
        this.currentBg = BgSprite[bgIndex];
        this.bgRectTransform.sizeDelta = new Vector2(textRectTransform.sizeDelta.x + spacingX,
                                                     textRectTransform.sizeDelta.y + spacingY);
        this.currentSizedelta = textRectTransform.sizeDelta;
    }

    public void showSpeech(string speech)
    {
        copy = speech;
        if (!textObject.gameObject.activeSelf)
        {
            SetSpeech(copy);
            textObject.gameObject.SetActive(true);
            currentTimer = TimerCallback.Create(5.0f, textObject.gameObject, delegate()
            {
                textObject.gameObject.SetActive(false);
            });
        }
        else
        {
            if (currentTimer != null)
            {
                SetSpeech(copy);
                currentTimer.ExpandTime(5.0f);
            }
        }

    }

    public void showSpeech(string speech, float time)
    {
        copy = speech;
        if (!textObject.gameObject.activeSelf)
        {
            SetSpeech(copy);
            textObject.gameObject.SetActive(true);
            currentTimer = TimerCallback.Create(time, textObject.gameObject, delegate()
            {
                textObject.gameObject.SetActive(false);
            });

        }
        else {
            if (currentTimer != null)
            {
                SetSpeech(copy);
                currentTimer.ExpandTime(time);
            }
        }
        


    }

    public void turnOnDoingSkillIcon(bool turnOn)
    {
        textObject.gameObject.SetActive(turnOn);
    }
}

