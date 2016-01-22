using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class SystemLoggerUI : MonoBehaviour, IObserver {
	public GameObject logBoard;
	
	private float boxPosition = 55;

	private float lastTextPosition = -370;
	private float lastTextHeight = 0;

    private float diff = 15f;

    private bool addedText = false;

    void OnEnable()
    {
        Notice.instance.Observe("AddSystemLog", this);
    }

    void OnDisable()
    {
        Notice.instance.Remove("AddSystemLog", this);
    }
	
	public void AddText(string msg)
	{
		GameObject logTextObj = Prefab.LoadPrefab ("SystemText");
		
		Text textUI = logTextObj.GetComponent<Text> ();

		
		textUI.transform.SetParent (logBoard.transform, false);
		//textUI.transform.localScale = new Vector3(0.6f, 0.6f, 1);
		
		textUI.text = msg;

		//float textHeight = textUI.GetComponent<RectTransform> ().sizeDelta.y;
        float textHeight = textUI.preferredHeight + diff;
		Text[] textChildren = logBoard.GetComponentsInChildren<Text> ();
		
		RectTransform rt = logBoard.GetComponent<RectTransform> ();
		//rt.sizeDelta = new Vector2(rt.sizeDelta.x, boxHeight + (textChildren.Length - 1) * textHeight);

        Vector3 pos = rt.anchoredPosition;
		boxPosition += textHeight * rt.localScale.y;
		pos.y = boxPosition;
        rt.anchoredPosition = pos;

		rt.sizeDelta = new Vector2(rt.sizeDelta.x, boxPosition);


        RectTransform uirt = textUI.GetComponent<RectTransform>();
        pos = uirt.localPosition;
		lastTextPosition -= lastTextHeight;
		pos.y = lastTextPosition;
        uirt.localPosition = pos;
        uirt.sizeDelta = new Vector2(uirt.sizeDelta.x, textHeight);

		lastTextHeight = textHeight;

        if (addedText)
        {
            GameObject logTextLineObj = Prefab.LoadPrefab("SystemText");
            Text textLine = logTextLineObj.GetComponent<Text>();

            textLine.transform.SetParent(logBoard.transform, false);
            textLine.text = "-------------------------------------------------";

            RectTransform textLineRt = logTextLineObj.GetComponent<RectTransform>();
            Vector3 textLinePos = textLineRt.localPosition;

            textLinePos.y = lastTextPosition + 20f;

            textLineRt.localPosition = textLinePos;
        }

        addedText = true;
	}
	
	public void OnNotice(string notice, params object[] param)
	{
		if("AddSystemLog" == notice)
		{
			AddText (" "+(string)param [0]);
		}
	}
}
