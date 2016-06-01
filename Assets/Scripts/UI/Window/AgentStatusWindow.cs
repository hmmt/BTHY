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

public class AgentStatusWindow : MonoBehaviour, IObserver, IActivatableObject {
	public Text NameText;
	public Text LevelText;
    public Text DepartMent;
    public Text TraitText;
    public Text AgentLifeStyle;
    public Text AgentName;

    public Color d;
    public Color i;
    public Color s;
    public Color c;
    //public Transform traitScrollTarget;

    public Image AgentFace;
    public Image AgentHair;
    public Image AgentBody;

    public Sprite[] MentalImage;

    public AgentIcons icons;
    public string[] statusDesc;
    public string[] worklistDesc;

    public Slider Health;
    public Image MentalIcon;
    public Image[] HasWork;

    public Image[] workIconImage;
    public Image SuppressIcon;

    public Sprite HasWorkSelected;
    public Sprite HasWorkNotSelected;

	[HideInInspector]
	public static AgentStatusWindow currentWindow = null;

    [HideInInspector]
    public ActivatableObjectPos windowPos = ActivatableObjectPos.RIGHTUPPER;

    public RectTransform eventTriggerTarget;

    bool activatableObjectInitiated = false;

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

    public delegate void ClickedEvent();

    public static AgentStatusWindow CreateWindow(AgentModel unit)
    {
        if (currentWindow.gameObject.activeSelf)
        {
            if (currentWindow.target == unit)
            {
                //may be need data update
                return currentWindow;
            }
        }
        else {
            currentWindow.gameObject.SetActive(true);
            currentWindow.Activate();
        }


        /*
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
        */
        currentWindow.target = unit;
        currentWindow.UpdateModel(currentWindow.target);
        //inst.AgentBody.sprite = ResourceCache.instance.GetSprite(unit.bodyImgSrc);



        Canvas canvas = currentWindow.transform.GetChild(0).GetComponent<Canvas>();
        canvas.worldCamera = UIActivateManager.instance.GetCam();

        return currentWindow;
	}

    public void Awake() {
        currentWindow = this;
        currentWindow.Init();
        currentWindow.gameObject.SetActive(false);
    }

    public void Init()
    {
        if (currentWindow.activatableObjectInitiated == false)
        {
            currentWindow.UIActivateInit();
        }
        currentWindow.iconList = new List<List<GameObject>>();
        currentWindow.worklistDesc = new string[currentWindow.icons.worklist.Length];
        currentWindow.statusDesc = new string[currentWindow.icons.statuslist.Length];
        
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

    public void UpdateModel(AgentModel newTarget) {
        Health.maxValue = target.defaultMaxHp;
        UpdateCreatureStatus();
        SetHasWorkIcon(target);
        AgentHair.sprite = newTarget.tempHairSprite;
        AgentFace.sprite = newTarget.tempFaceSprite;
        this._target = newTarget;
    }
	
	public void UpdateCreatureStatus()
	{
        DepartMent.text = "" + GetSefiraName( target.currentSefira);
		NameText.text =  ""+target.name;
        LevelText.text = AgentModel.GetLevelGradeText(target);
        statusDesc[0] = target.hp + "";
        statusDesc[1] = target.mental + "";
        statusDesc[2] = target.movement + "";
        statusDesc[3] = target.workSpeed + "";

        Health.value = target.hp;

        MentalIcon.sprite = GetMentalSprite(target);
        for (int i = 0; i < icons.statuslist.Length; i++)
        {
            icons.statuslist[i].sprite = target.StatusSprites[i];
            
        }

		/*
        worklistDesc[0] = target.directSkill.name;
        worklistDesc[1] = target.indirectSkill.name;
        worklistDesc[2] = target.blockSkill.name;*/
        /*
        OverlayObject[] mannualAry = new OverlayObject[4];
        
        for (int i = 0; i < icons.worklist.Length; i++)
        {
            icons.worklist[i].sprite = target.WorklistSprites[i];
            icons.worklist[i].GetComponent<OverlayObject>().text = worklistDesc[i];
        }
        */
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
        AgentName.text = target.name;

        switch (target.agentLifeValue)
        {
            case PersonalityType.D: AgentLifeStyle.color = d; break;
            case PersonalityType.I: AgentLifeStyle.color = i; break;
            case PersonalityType.S: AgentLifeStyle.color = s; break;
            case PersonalityType.C: AgentLifeStyle.color = c; break;
        }
        script.SortTrait();
    }

    public void OnClickClose()
	{
		CloseWindow ();
	}
	
	public void CloseWindow()
	{
        Deactivate();
        currentWindow.gameObject.SetActive(false);
	}

    public void OnClickPortrait() {
        Vector2 pos = currentWindow._target.GetCurrentViewPosition();
        Camera.main.transform.position = new Vector3( pos.x, pos.y, -20f);        
    }

    public Sprite GetMentalSprite(AgentModel target)
    {
        if (target.defaultMaxMental == 0) target.defaultMaxMental = 10000;
        int value = target.mental / target.defaultMaxMental * 100;

        if (value >= 0 && value < 25)
        {
            return this.MentalImage[0];
        }
        else if (value >= 25 && value < 50)
        {
            return this.MentalImage[1];
        }
        else if (value >= 50 && value < 75)
        {
            return this.MentalImage[2];
        }
        else if (value >= 75 && value <= 100)
        {
            return this.MentalImage[3];
        }
        else
        {
            Debug.Log("Error + MentalVaue : " + value.ToString());
        }
        return this.MentalImage[0];
    }

    public void SetHasWorkIcon(AgentModel model) {
        /*
        List<SkillCategory> skillList = new List<SkillCategory>(SkillManager.instance.list.ToArray());
        int max = 5;
        if (skillList.Count < 5) max = skillList.Count;
        for (int i = 0; i < max; i++) {
            if (model.GetUniqueSkillCategory(skillList[i].name) != null)
            {
                
            }
        }*/
        int i = 0;
        foreach (Sprite s in AgentModel.GetAgentSkillSprite(model)) {
            if (i > this.workIconImage.Length) return;
            workIconImage[i].sprite = s;
            i++;
        }
        switch (model.weapon) { 
            case AgentWeapon.GUN:
                SuppressIcon.sprite = IconManager.instance.GetIcon("Gun").icon;
                break;
            case AgentWeapon.NORMAL:
                SuppressIcon.sprite = IconManager.instance.GetIcon("Stick").icon;
                break;
            case AgentWeapon.SHIELD:
                SuppressIcon.sprite = IconManager.instance.GetIcon("Block").icon;
                break;
        }
    }

    public void Activate()
    {

        UIActivateManager.instance.Activate(this, this.windowPos);
    }

    public void Deactivate()
    {
        UIActivateManager.instance.Deactivate(this.windowPos);
    }

    public void OnEnter()
    {
        UIActivateManager.instance.OnEnter(this);
    }

    public void OnExit()
    {
        UIActivateManager.instance.OnExit();
    }

    public void UIActivateInit()
    {
        activatableObjectInitiated = true;
        EventTrigger eventTrigger = this.eventTriggerTarget.GetComponent<EventTrigger>();
        if (eventTrigger == null)
        {
            eventTrigger = this.eventTriggerTarget.gameObject.AddComponent<EventTrigger>();
        }

        EventTrigger.Entry enter = new EventTrigger.Entry();
        EventTrigger.Entry exit = new EventTrigger.Entry();

        enter.eventID = EventTriggerType.PointerEnter;
        exit.eventID = EventTriggerType.PointerExit;

        enter.callback.AddListener((eventData) => { OnEnter(); });
        exit.callback.AddListener((eventData) => { OnExit(); });

        eventTrigger.triggers.Add(enter);
        eventTrigger.triggers.Add(exit);
    }

    public void Close()
    {
        this.CloseWindow();
    }
}
