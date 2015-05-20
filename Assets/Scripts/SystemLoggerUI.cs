using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class SystemLoggerUI : MonoBehaviour, IObserver {
	public GameObject logBoard;
	
	//private int boxHeight = 200;

	private float boxPosition = 200;

	private float lastTextPosition = -300;
	private float lastTextHeight = 0;

    private float diff = 15f;

    private bool addedText = false;
	
	
	void Awake()
	{
		Notice.instance.Observe ("AddSystemLog", this);
		//Notice.instance.Observe ("AgentDie", this);
	}

    void Start()
    {
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

		//Debug.Log(textHeight);
        
		
		Text[] textChildren = logBoard.GetComponentsInChildren<Text> ();
		
		RectTransform rt = logBoard.GetComponent<RectTransform> ();
		//rt.sizeDelta = new Vector2(rt.sizeDelta.x, boxHeight + (textChildren.Length - 1) * textHeight);

		Vector3 pos = rt.localPosition;
		//pos.y = boxHeight+(textChildren.Length-1) * textHeight;
		boxPosition += textHeight;
		pos.y = boxPosition;
		rt.localPosition = pos;

		rt.sizeDelta = new Vector2(rt.sizeDelta.x, boxPosition);
		
		
		rt = textUI.GetComponent<RectTransform> ();
		pos = rt.localPosition;
		// pos.y = -180-(textChildren.Length-1) * textHeight;
		//pos.y = -boxPosition + 20;
		lastTextPosition -= lastTextHeight;
		pos.y = lastTextPosition;
		rt.localPosition = pos;
		rt.sizeDelta = new Vector2 (rt.sizeDelta.x, textHeight);

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
            Debug.Log("텍스트가 뜬다");
			AddText (" "+(string)param [0]);
		}
	}
}
