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


            AgentModel copied = unit;
			slotPanel.skillButton1.onClick.AddListener(()=>SelectAgentSkill(copied, copied.directSkill));
			slotPanel.skillButton2.onClick.AddListener(()=>SelectAgentSkill(copied, copied.indirectSkill));
			slotPanel.skillButton3.onClick.AddListener(()=>SelectAgentSkill(copied, copied.blockSkill));

			if(targetCreature.model.specialSkill != null)
                slotPanel.skillButton4.onClick.AddListener(() => SelectAgentSkill(copied, targetCreature.model.specialSkill));
			else
				slotPanel.skillButton4.gameObject.SetActive(false);

			Texture2D tex = Resources.Load<Texture2D> ("Sprites/"+unit.imgsrc);
			slotPanel.agentIcon.sprite = Sprite.Create(tex, new Rect(0,0,tex.width, tex.height), new Vector2(0.5f, 0.5f));

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
}