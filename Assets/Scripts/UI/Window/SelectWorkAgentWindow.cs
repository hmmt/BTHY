using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SelectWorkAgentWindow : MonoBehaviour, AgentSlot.IReceiver {

	public Transform agentScrollTarget;
	public Transform anchor;

	private int state1 = 0;

	private CreatureUnit targetCreature = null;
    private IsolateRoom targetRoom = null;

    List<AgentModel> selectedAgentList = new List<AgentModel>();

	public static SelectWorkAgentWindow currentWindow = null;

    public static SelectWorkAgentWindow CreateWindow(IsolateRoom room)
    {
        if (currentWindow != null)
        {
            currentWindow.CloseWindow();
        }
        GameObject newObj = Prefab.LoadPrefab("SelectWorkAgentWindow");

        SelectWorkAgentWindow inst = newObj.GetComponent<SelectWorkAgentWindow>();
        //inst.ShowSelectAgent (unit.gameObject);
        inst.targetRoom = room;
        inst.targetCreature = room.targetUnit;
        inst.ShowAgentList();

        currentWindow = inst;
        return inst;
    }
	
	public static SelectWorkAgentWindow CreateWindow(CreatureUnit unit)
	{
		if(currentWindow != null)
		{
			currentWindow.CloseWindow();
		}
        GameObject newObj = Prefab.LoadPrefab("SelectWorkAgentWindow");
		
		SelectWorkAgentWindow inst = newObj.GetComponent<SelectWorkAgentWindow> ();
		//inst.ShowSelectAgent (unit.gameObject);
		inst.targetCreature = unit;
		inst.ShowAgentList ();

		currentWindow = inst;
		
		return inst;
	}
	
	// Use this for initialization
	void Awake () {
		UpdatePosition ();
	}
	
	// Update is called once per frame
	void Update () {	
	}
	
	void FixedUpdate()
	{
		UpdatePosition ();
	}
	
	private void UpdatePosition()
	{
		if(targetCreature != null && false)
		{
			/*
			Vector3 targetPos = targetCreature.transform.position;
			
			Vector3 newPos = transform.position;
			newPos.x = targetPos.x+offset.x;
			newPos.y = targetPos.y+offset.y;
			
			transform.position = newPos;
			*/

			Vector3 targetPos = targetCreature.transform.position;
			
			anchor.position = Camera.main.WorldToScreenPoint(targetPos);
		}
        else if (targetRoom != null)
        {
            Vector3 targetPos = targetRoom.transform.position;

            anchor.position = Camera.main.WorldToScreenPoint(targetPos+new Vector3(0,-3,0));
        }
	}
	
	public void OnClickAgentOK()
	{

	}
	public void OnClickClose()
	{
		CloseWindow ();
	}

    public void SelectAgentSkill(AgentModel agent, SkillTypeInfo skillInfo)
	{
		UseSkill.InitUseSkillAction(skillInfo, agent, targetCreature.model);
		CloseWindow ();
	}

	public void ShowAgentList()
	{
        AgentModel[] agents = AgentManager.instance.GetAgentList();

		float posy = 0;
        foreach (AgentModel unit in agents)
		{
            if (unit.GetState() == AgentCmdState.WORKING)
                continue;

			GameObject slot = Prefab.LoadPrefab ("AgentSlotPanel");

			slot.transform.SetParent (agentScrollTarget, false);

			RectTransform tr = slot.GetComponent<RectTransform>();
			tr.localPosition = new Vector3(0,posy,0);
			//slot.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(System.
			//slot.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(()=>CloseWindow());
			AgentSlotPanel slotPanel = slot.GetComponent<AgentSlotPanel>();

            slotPanel.skillButton1.image.sprite = Resources.Load<Sprite>("Sprites/" + unit.directSkill.imgsrc);
            slotPanel.skillButton2.image.sprite = Resources.Load<Sprite>("Sprites/" + unit.indirectSkill.imgsrc);
            slotPanel.skillButton3.image.sprite = Resources.Load<Sprite>("Sprites/" + unit.blockSkill.imgsrc);

            slotPanel.agentName.text = unit.name;
            slotPanel.agentHealth.text = HealthCheck(unit);
            slotPanel.agentMental.text = MentalCheck(unit);
            slotPanel.agentLevel.text = ""+unit.level;

            AgentModel copied = unit;
			slotPanel.skillButton1.onClick.AddListener(()=>SelectAgentSkill(copied, copied.directSkill));
			slotPanel.skillButton2.onClick.AddListener(()=>SelectAgentSkill(copied, copied.indirectSkill));
			slotPanel.skillButton3.onClick.AddListener(()=>SelectAgentSkill(copied, copied.blockSkill));

			if(targetCreature.model.specialSkill != null)
                slotPanel.skillButton4.onClick.AddListener(() => SelectAgentSkill(copied, targetCreature.model.specialSkill));
			else
				slotPanel.skillButton4.gameObject.SetActive(false);

            Texture2D tex3 = Resources.Load<Texture2D>(unit.bodyImgSrc);
            slotPanel.agentBody.sprite = Sprite.Create(tex3, new Rect(0, 0, tex3.width, tex3.height), new Vector3(0.5f, 0.5f, 0.5f));
            Texture2D tex1 = Resources.Load<Texture2D>(unit.faceImgSrc);
            slotPanel.agentFace.sprite = Sprite.Create(tex1, new Rect(0, 0, tex1.width, tex1.height), new Vector3(0.5f, 0.5f, -1f));
            Texture2D tex2 = Resources.Load<Texture2D>(unit.hairImgSrc);
            slotPanel.agentHair.sprite = Sprite.Create(tex2, new Rect(0, 0, tex2.width, tex2.height), new Vector3(0.5f, 0.5f, -1f)); 
            posy -= 100f;
		}

        // scroll rect size
        Vector2 scrollRectSize = agentScrollTarget.GetComponent<RectTransform>().sizeDelta;
        scrollRectSize.y = -posy + 100f;
        agentScrollTarget.GetComponent<RectTransform>().sizeDelta = scrollRectSize;

		UpdatePosition ();
	}

	
	public void OnClickSlot(GameObject slotObject)
	{
		AgentSlot agentSlot = slotObject.GetComponent<AgentSlot> ();

        AgentModel[] agents = AgentManager.instance.GetAgentList();
        AgentModel unit = agents[agentSlot.slotIndex];
		
		if(!selectedAgentList.Contains(unit))
		{
			if(selectedAgentList.Count > 0)
				return;
			selectedAgentList.Add(unit);
			agentSlot.SetSelect(true);
		}
		else
		{
			selectedAgentList.Remove(unit);
			agentSlot.SetSelect(false);
		}
		OnClickAgentOK ();
	}

	public void CloseWindow()
	{
		//gameObject.SetActive (false);
		currentWindow = null;
		Destroy (gameObject);
	}

    public string MentalCheck(AgentModel unit)
    {
        if (unit.mental >= unit.maxMental * 2 / 3f)
        {
            return "멘탈 : 건강";
        }

        else if (unit.mental <= unit.maxMental * 2 / 3f && unit.mental >= unit.maxMental * 1 / 3f)
        {
            return "멘탈 : 보통";
        }

        else if (unit.mental >= unit.maxMental * 1 / 3f)
        {
            return "멘탈 : 심각";
        }

        else
        {
            return "멘탈 : ???";
        }

    }

    public string HealthCheck(AgentModel unit)
    {

        if (unit.hp >= unit.hp * 2 / 3f)
        {
            return "신체 : 건강";
        }

        else if (unit.hp <= unit.maxHp * 2 / 3f && unit.hp >= unit.maxHp * 1 / 3f)
        {
            return "신체 : 보통";
        }

        else if (unit.hp >= unit.maxHp * 1 / 3f)
        {
            return "신체 : 심각";
        }

        else
        {
            return "신체 : ???";
        }
    }
}