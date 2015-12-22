using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

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
        for (int i = 0; i < icons.statuslist.Length; i++) {
            icons.statuslist[i].sprite = target.StatusSprites[i];
        }
        for (int i = 0; i < icons.worklist.Length; i++)
        {
            icons.worklist[i].sprite = target.WorklistSprites[i];
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

        for (int i = 0; i < target.traitList.Count; i++) {
            script.MakeTrait(target.traitList[i].name);
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
		//currentWindow = null;
		//Destroy (gameObject);
	}

	public void Test()
	{
		Debug.Log ("33");
	}

    public void OnClickPortrait() {
        Vector2 pos = currentWindow._target.GetCurrentViewPosition();
        Camera.main.transform.position = new Vector3( pos.x, pos.y, -20f);        
    }

}
