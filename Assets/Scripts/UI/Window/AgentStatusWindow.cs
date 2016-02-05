using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[System.Serializable]
public class AgentIcons {
    public Image[] statuslist;
    public Image[] worklist;
    
}

public class AgentStatusWindow : MonoBehaviour, IObserver {
	public Text NameText;
	public Text LevelText;
    public Text DepartMent;
    public Text TraitText;
    public Text AgentLifeStyle;
    //public Transform traitScrollTarget;

	public Image AgentFace;
    public Image AgentHair;
    public Image AgentBody;

    public AgentIcons icons;
    public string[] statusDesc;
    public string[] worklistDesc;

	[HideInInspector]
	public static AgentStatusWindow currentWindow = null;

	private AgentModel _target = null;
	private bool enabled = false;
    private List<List<GameObject>> iconList;
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
            inst = newObj.GetComponent<AgentStatusWindow>();
        }
        else
        {
            newObj = Prefab.LoadPrefab("AgentStatusWindow");
            
            inst = newObj.GetComponent<AgentStatusWindow>();
            inst.iconList = new List<List<GameObject>>();
            inst.worklistDesc = new string[inst.icons.worklist.Length];
            inst.statusDesc = new string[inst.icons.statuslist.Length];

            for (int i = 0; i < inst.icons.statuslist.Length; i++)
            {
                GameObject target = inst.icons.statuslist[i].gameObject;

                EventTrigger trigger = target.AddComponent<EventTrigger>();
                EventTrigger.Entry enter = new EventTrigger.Entry();
                EventTrigger.Entry exit = new EventTrigger.Entry();
                enter.eventID = EventTriggerType.PointerEnter;
                exit.eventID = EventTriggerType.PointerExit;
                OverlayObject overlayItem = target.AddComponent<OverlayObject>();
                enter.callback.AddListener((eventdata) => { overlayItem.Overlay(); });
                exit.callback.AddListener((eventdata) => { overlayItem.Hide(); });
                trigger.triggers.Add(enter);
                trigger.triggers.Add(exit);
            }

            for (int i = 0; i < inst.icons.worklist.Length; i++)
            {
                GameObject target = inst.icons.worklist[i].gameObject;

                EventTrigger trigger = target.AddComponent<EventTrigger>();
                EventTrigger.Entry enter = new EventTrigger.Entry();
                EventTrigger.Entry exit = new EventTrigger.Entry();
                enter.eventID = EventTriggerType.PointerEnter;
                exit.eventID = EventTriggerType.PointerExit;
                OverlayObject overlayItem = target.AddComponent<OverlayObject>();
                enter.callback.AddListener((eventdata) => { overlayItem.Overlay(); });
                exit.callback.AddListener((eventdata) => { overlayItem.Hide(); });
                trigger.triggers.Add(enter);
                trigger.triggers.Add(exit);
            }
        }
		
		inst.target = unit;
        
		inst.UpdateCreatureStatus ();
        inst.AgentHair.sprite = ResourceCache.instance.GetSprite(unit.hairImgSrc);
        inst.AgentBody.sprite = ResourceCache.instance.GetSprite(unit.bodyImgSrc);
        inst.AgentFace.sprite = ResourceCache.instance.GetSprite(unit.faceImgSrc);
        
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

    public string GetSefiraName(string sefira) {
        string temp = "";
        switch (sefira) { 
            case "1":
                temp = "지휘감시팀";
                break;
            case "2":
                temp = "비상계획팀";
                break;
            case "3":
                temp = "자재관리팀";
                break;
            case "4":
                temp = "솔루션계획팀";
                break;
            default:
                temp = "";
                break;
        
        }
        return temp;
    }
	
	public void UpdateCreatureStatus()
	{
        DepartMent.text = "" + GetSefiraName( target.currentSefira);
		NameText.text =  ""+target.name;
		LevelText.text = ""+target.level + "등급";
        statusDesc[0] = target.hp + "";
        statusDesc[1] = target.mental + "";
        statusDesc[2] = target.movement + "";
        statusDesc[3] = target.workSpeed + "";
		/*
        worklistDesc[0] = target.directSkill.name;
        worklistDesc[1] = target.indirectSkill.name;
        worklistDesc[2] = target.blockSkill.name;*/
        OverlayObject[] mannualAry = new OverlayObject[4];
        for (int i = 0; i < icons.statuslist.Length; i++) {
            icons.statuslist[i].sprite = target.StatusSprites[i];
            OverlayObject overlay = icons.statuslist[i].GetComponent<OverlayObject>();
            overlay.text = statusDesc[i];
        }

        for (int i = 0; i < icons.worklist.Length; i++)
        {
            icons.worklist[i].sprite = target.WorklistSprites[i];
            icons.worklist[i].GetComponent<OverlayObject>().text = worklistDesc[i];
        }

        //ShowTraitList();
        ShowTrait();    
	}
    /*
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

            traitSlot.GetComponent<UnityEngine.UI.Text>().text = "<" + target.traitList[i].name + ">\n"+"  - "+target.traitList[i].description;
          
            posY -= 55f;
        }
    }
    */
    public void ShowTrait() {
        TraitListScript script = transform.GetComponent<TraitListScript>();
        script.DeleteAll();

        foreach (List<GameObject> temp in iconList) {
            foreach (GameObject t in temp) {
                Destroy(t);
            }
            temp.Clear();
        }
        iconList.Clear();

        for (int i = 0; i < target.traitList.Count; i++) {
            iconList.Add(script.MakeTrait(target.traitList[i]));
        }
        AgentLifeStyle.text = target.LifeStyle();
        script.SortTrait();
    }

    public void OnClickClose()
	{
		CloseWindow ();
	}
	
	public void CloseWindow()
	{
        GameObject.FindGameObjectWithTag("AnimAgentController")
            .GetComponent<Animator>().SetBool("isTrue", true);
	}

    public void OnClickPortrait() {
        Vector2 pos = currentWindow._target.GetCurrentViewPosition();
        Camera.main.transform.position = new Vector3( pos.x, pos.y, -20f);        
    }

}
