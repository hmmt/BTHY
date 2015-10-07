using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class NarrationLoggerUI : MonoBehaviour, IObserver {


    public CreatureModel newInputCreature;
    public CreatureModel oldInputCreature;
    public CreatureModel targetCreature;

    public static NarrationLoggerUI instantNarrationLog;

    public GameObject logBoard;
    public TextListScript script;
    private int logSize = 0;
	
	//private int boxHeight = 200;

	private float boxPosition = 200;

	private float lastTextPosition = -200;
	private float lastTextHeight = 0;

    private float diff = 20f;

    private bool addedText = false;
	
	
	void Awake()
	{
		Notice.instance.Observe ("AddNarrationLog", this);
		//Notice.instance.Observe ("AgentDie", this);
	}

    void Start()
    {
        instantNarrationLog = this;
    }
	
	public void AddText(string msg)
	{
        oldInputCreature = newInputCreature;

		GameObject logTextObj = Prefab.LoadPrefab ("NarrationText");
		
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
		lastTextPosition -= lastTextHeight*rt.localScale.y;
		pos.y = lastTextPosition;
		rt.localPosition = pos;
		rt.sizeDelta = new Vector2 (rt.sizeDelta.x, textHeight);

		lastTextHeight = textHeight;

        if (addedText)
        {
            GameObject logTextLineObj = Prefab.LoadPrefab("NarrationText");
            Text textLine = logTextLineObj.GetComponent<Text>();

            textLine.transform.SetParent(logBoard.transform, false);
            textLine.text = "----------------------------------------------";

            RectTransform textLineRt = logTextLineObj.GetComponent<RectTransform>();
            Vector3 textLinePos = textLineRt.localPosition;

            textLinePos.y = lastTextPosition + 10f;

            textLineRt.localPosition = textLinePos;
        }

        addedText = true;
	}
	
	public void OnNotice(string notice, params object[] param)
	{
        script = gameObject.GetComponent<TextListScript>();
        if ("AddNarrationLog" == notice && targetCreature == (CreatureModel)param[1])
		{

            script.MakeText(" " + (string)param[0]);
            newInputCreature = (CreatureModel)param[1];
		}
        script.SortList();
	}

    //리스트를 받아와서 기존에 있는걸 clear시키고 받아온 리스트들의 로그를 출력한다.
    public void setLogList(CreatureModel focusCreature)
    {
        newInputCreature = focusCreature;

        if (oldInputCreature != newInputCreature)
        {
            logClear();
            foreach(string narrationLog in newInputCreature.narrationList)
            {
                AddText(""+narrationLog);
            }
            oldInputCreature = newInputCreature;
        }
    }

    public void logClear()
    {
        foreach (Transform child in logBoard.transform)
        {
            Destroy(child.gameObject);
        }

        logSize = 0;
        boxPosition = 250;
        lastTextPosition = -200;
        lastTextHeight = 0;

        diff = 20f;

        addedText = false;
    }
}
