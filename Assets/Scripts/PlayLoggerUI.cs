using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayLoggerUI : MonoBehaviour, IObserver {

	public GameObject logBoard;

	private int boxHeight = 200;


	void Awake()
	{
		Notice.instance.Observe ("AddPlayerLog", this);
		//Notice.instance.Observe ("AgentDie", this);
	}

	public void AddText(string msg)
	{

		GameObject logTextObj = Prefab.LoadPrefab ("LogText");

		Text textUI = logTextObj.GetComponent<Text> ();

		textUI.transform.SetParent (logBoard.transform, false);
		textUI.transform.localScale = new Vector3(0.6f, 0.6f, 1);

		textUI.text = msg;

		float textHeight = textUI.GetComponent<RectTransform> ().sizeDelta.y;

		Text[] textChildren = logBoard.GetComponentsInChildren<Text> ();

		RectTransform rt = logBoard.GetComponent<RectTransform> ();
		rt.sizeDelta = new Vector2(rt.sizeDelta.x, boxHeight + (textChildren.Length - 1) * textHeight);
		Vector3 pos = rt.localPosition;
		pos.y = boxHeight+(textChildren.Length-1) * textHeight;
		rt.localPosition = pos;


		rt = textUI.GetComponent<RectTransform> ();
		pos = rt.localPosition;
		pos.y = -180-(textChildren.Length-1) * textHeight;
		rt.localPosition = pos;
	}

	public void OnNotice(string notice, params object[] param)
	{
		if("AddPlayerLog" == notice)
		{
			AddText ((string)param [0]);
		}
		else if("AgentDie" == notice)
		{
			AgentUnit agent = (AgentUnit)param[0];
			AddText (agent.name + " is dead..");
		}
	}
}
