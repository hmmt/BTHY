using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AgentStatusWindow : MonoBehaviour, IObserver {
	public UnityEngine.UI.Text NameText;
	public UnityEngine.UI.Text HPText;
	public UnityEngine.UI.Text MentalText;
	public UnityEngine.UI.Text LevelText;
	public UnityEngine.UI.Text GenderText;
	public UnityEngine.UI.Text WorkDayText;

  
	public Transform anchor;

	public UnityEngine.UI.Image agentIcon;

	[HideInInspector]
	public static AgentStatusWindow currentWindow = null;

	private AgentUnit _target = null;
	private bool enabled = false;

	public AgentUnit target
	{
		get{ return _target; }
		set
		{
			if(_target != null && enabled)
				Notice.instance.Remove("UpdateAgentState_"+target.gameObject.GetInstanceID(), this);
			_target = value;
			if(_target != null && enabled)
			{
				Notice.instance.Observe("UpdateAgentState_"+target.gameObject.GetInstanceID(), this);
			}
		}
	}
	
	public static AgentStatusWindow CreateWindow(AgentUnit unit)
	{
        GameObject newObj;
        AgentStatusWindow inst;
        if (currentWindow != null)
        {
            newObj = currentWindow.gameObject;
            //currentWindow.CloseWindow();
        }
        else
        {
            newObj = Prefab.LoadPrefab("AgentStatusWindow");
        }
		
		 inst = newObj.GetComponent<AgentStatusWindow> ();

		inst.target = unit;
		inst.UpdateCreatureStatus ();
		inst.UpdatePosition ();

		inst.agentIcon.sprite = unit.spriteRenderer.sprite;

		currentWindow = inst;

		return inst;
	}
	void OnEnable()
	{
		enabled = true;
		if(target != null)
		{
			Notice.instance.Observe("UpdateAgentState_"+target.gameObject.GetInstanceID(), this);
		}
		Notice.instance.Observe ("AgentDie", this);
	}
	void OnDisable()
	{
		enabled = false;
		if(target != null) 
			Notice.instance.Remove("UpdateAgentState_"+target.gameObject.GetInstanceID(), this);
	}

	void FixedUpdate()
	{
		UpdatePosition ();
	}

	public void OnNotice(string notice, params object[] param)
	{
		if(notice.Split('_')[0] ==  "UpdateAgentState")
		{
			UpdateCreatureStatus ();
		}
		else if(notice == "AgentDie")
		{
			target = null;
		}
	}
	
	public void OnOpen()
	{
		
	}
	
	private void UpdatePosition()
	{
		if(target != null)
		{
			Vector3 targetPos = target.transform.position;
			
			anchor.position = Camera.main.WorldToScreenPoint(targetPos);
		}
	}
	
	public void UpdateCreatureStatus()
	{
		NameText.text =  ""+target.name;
		HPText.text =  ""+target.hp;
		MentalText.text = ""+target.mental;
		LevelText.text = ""+target.level;
		GenderText.text = ""+target.gender;
		WorkDayText.text = ""+target.workDays;
	}
	
	public void OnClickClose()
	{
		CloseWindow ();
	}
	
	public void CloseWindow()
	{
        GameObject.FindGameObjectWithTag("AnimAgentController")
            .GetComponent<Animator>().SetBool("isTrue", true);
		//currentWindow = null;
		//Destroy (gameObject);
	}

	public void Test()
	{
		Debug.Log ("33");
	}
}
