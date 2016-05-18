using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LoggingScript : MonoBehaviour {
    public GameObject item;
    public RectTransform parent;
    public float leftSpacing;
    public int fontSize;
    public float horizontalSpacing;
    public float verticalSpacing;
    public float lineSpacing;

    private List<LogItemScript> list;

    public void Awake() {
        list = new List<LogItemScript>();
    }

    public void MakeText(string context) {
        GameObject target = Instantiate(item);
        LogItemScript script = target.GetComponent<LogItemScript>();
        RectTransform rect = target.GetComponent<RectTransform>();

        script.SetText(context, fontSize, horizontalSpacing, verticalSpacing);
        rect.SetParent(parent);
        rect.localPosition = Vector3.zero;
        rect.anchoredPosition = new Vector2(leftSpacing, 0f);
        rect.localScale = Vector3.one;
        script.index = list.Count;
        list.Add(script);
    }

    public void Sort() {
        float posy = 0.0f;

        foreach (LogItemScript item in list) {
            RectTransform rect = item.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, -posy );

            posy += (item.height + lineSpacing + verticalSpacing);

        }

        parent.sizeDelta = new Vector2(parent.sizeDelta.x, posy);

    }

    public void DeleteAll()
    {
        foreach (LogItemScript item in list) {
            Destroy(item.gameObject);
        }
        list.Clear();
    }
	
}
