using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ConversationItem : MonoBehaviour {
    public Image backGround;
    public Text context;
    public float MaxSize;

    public float HorizontalSpacing;
    public float VerticalSpacing;

    private RectTransform ImageRect;
    private RectTransform TextRect;
    private RectTransform rect;
    private float Height = 0.0f;

    public void Awake() {
        rect = this.GetComponent<RectTransform>();
        ImageRect = this.backGround.GetComponent<RectTransform>();
        TextRect = this.context.GetComponent<RectTransform>();
        MaxSize = MaxSize - HorizontalSpacing;
    }

    public void SetText(string text, bool poss) {
        this.context.text = text;

        float tempHeight = context.preferredHeight;
        float tempWidth = context.preferredWidth;
        float sizeX = 0f;
        if (tempWidth < MaxSize)
        {
            sizeX = tempWidth;
        }
        else {
            sizeX = MaxSize;
        }

        TextRect.sizeDelta = new Vector2(sizeX, TextRect.sizeDelta.y);

        rect.sizeDelta = new Vector2(sizeX + HorizontalSpacing, tempHeight + VerticalSpacing + TextRect.sizeDelta.y);
        if (poss)
        {
            TextRect.anchoredPosition = new Vector2(-HorizontalSpacing/2, -VerticalSpacing / 2);//if pivot is upper center
        }
        else {
            TextRect.anchoredPosition = new Vector2(HorizontalSpacing/2, -VerticalSpacing / 2);//if pivot is upper center
        }
       
        
        this.Height = tempHeight + VerticalSpacing;
    }

    public float GetHeight() {
        return Height;
    }

    public RectTransform GetRect() {
        return this.rect;
    }
}
