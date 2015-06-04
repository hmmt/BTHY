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

    public UnityEngine.UI.Text TraitText;

    public Transform traitScrollTarget;

	public UnityEngine.UI.Image agentIcon;

	[HideInInspector]
	public static AgentStatusWindow currentWindow = null;

	private AgentModel _target = null;
	private bool enabled = false;

    public AgentModel target
	{
		get{ return _target; }
		set
		{
			if(_target != null && enabled)
				Notice.instance.Remove("UpdateAgentState_"+target.instanceId, this);
			_target = value;
			if(_target != null && enabled)
			{
                Notice.instance.Observe("UpdateAgentState_" + target.instanceId, this);
			}
		}
	}

    public static AgentStatusWindow CreateWindow(AgentModel unit)
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

        Texture2D tex = Resources.Load<Texture2D>("Sprites/" + unit.imgsrc);
        inst.agentIcon.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));

		currentWindow = inst;

		return inst;
	}
	void OnEnable()
	{
		enabled = true;
		if(target != null)
		{
			Notice.instance.Observe("UpdateAgentState_"+target.instanceId, this);
		}
		Notice.instance.Observe ("AgentDie", this);
	}
	void OnDisable()
	{
		enabled = false;
		if(target != null) 
			Notice.instance.Remove("UpdateAgentState_"+target.instanceId, this);
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
	
	public void UpdateCreatureStatus()
	{
		NameText.text =  ""+target.name;
		HPText.text =  ""+target.hp;
		MentalText.text = ""+target.mental;
		LevelText.text = ""+target.level;
		GenderText.text = ""+target.gender;
		WorkDayText.text = ""+target.workDays;

        ShowTraitList();
	}

    public void ShowTraitList()
    {

        foreach (Transform childs in traitScrollTarget.transform)
        {
            Destroy(childs.gameObject);
        }

        float posY = 0;
        for (int i = 0; i < target.traitList.Count; i++)
        {
            GameObject traitSlot = Prefab.LoadPrefab("TraitText");
            Debug.Log(traitSlot.GetComponent<RectTransform>().localPosition);
            traitSlot.transform.SetParent(traitScrollTarget, false);

            Debug.Log(traitSlot.GetComponent<RectTransform>().localPosition);
            
            RectTransform tr = traitSlot.GetComponent<RectTransform>();
            tr.localPosition = new Vector3(0, posY, 0);

            //Debug.Log(traitScrollTarget.GetComponent<RectTransform>().localPosition);

            traitSlot.GetComponent<UnityEngine.UI.Text>().text ="<" +target.traitList[i].name+">";

            posY -= 30f;
        }
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
