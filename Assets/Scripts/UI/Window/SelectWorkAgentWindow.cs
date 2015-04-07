using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SelectWorkAgentWindow : MonoBehaviour, AgentSlot.IReceiver {

	public Transform agentScrollTarget;
	public Transform anchor;

	private int state1 = 0;

	private CreatureUnit targetCreature = null;
    private IsolateRoom targetRoom = null;
	
	List<GameObject> selectedAgentList = new List<GameObject>();

	public static SelectWorkAgentWindow currentWindow = null;

    public static SelectWorkAgentWindow CreateWindow(IsolateRoom room)
    {
        if (currentWindow != null)
        {
            currentWindow.CloseWindow();
        }

        GameObject newObj = Instantiate(Resources.Load<GameObject>("Prefabs/SelectWorkAgentWindow")) as GameObject;

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

		GameObject newObj = Instantiate(Resources.Load<GameObject> ("Prefabs/SelectWorkAgentWindow")) as GameObject;
		
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
		/*
		if(selectedAgentList.Count == 0)
		{
			// show messagebox
			return;
		}
		GlobalObjectManager.instance.GetSelectActionWindow ().ShowSelectActon (selectedAgentList.ToArray (), target);
		
		CloseWindow ();
		*/
	}
	public void OnClickClose()
	{
		CloseWindow ();
	}

	public void SelectAgentSkill(AgentUnit agent, SkillTypeInfo skillInfo)
	{
		//UseSkill.InitUseSkillAction(skillInfo, selectedAgentList[0].GetComponent<AgentUnit>(), targetCreature);
		UseSkill.InitUseSkillAction(skillInfo, agent, targetCreature);
		CloseWindow ();
	}

	public void ShowAgentList()
	{
		AgentUnit[] agents = AgentFacade.instance.GetAgentList ();

		float posy = 0;
		foreach(AgentUnit unit in agents)
		{
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
            

			AgentUnit copied = unit;
			slotPanel.skillButton1.onClick.AddListener(()=>SelectAgentSkill(copied, copied.directSkill));
			slotPanel.skillButton2.onClick.AddListener(()=>SelectAgentSkill(copied, copied.indirectSkill));
			slotPanel.skillButton3.onClick.AddListener(()=>SelectAgentSkill(copied, copied.blockSkill));

			if(targetCreature.specialSkill != null)
				slotPanel.skillButton4.onClick.AddListener(()=>SelectAgentSkill(copied, targetCreature.specialSkill));
			else
				slotPanel.skillButton4.gameObject.SetActive(false);

			Texture2D tex = Resources.Load<Texture2D> ("Sprites/"+unit.imgsrc);
			slotPanel.agentIcon.sprite = Sprite.Create(tex, new Rect(0,0,tex.width, tex.height), new Vector2(0.5f, 0.5f));

			posy -= 100f;
		}

		UpdatePosition ();
	}

	/*
	public void ShowSelectAgent(GameObject target)
	{
		this.target = target;
		gameObject.SetActive (true);
		selectedAgentList = new List<GameObject> ();
		
		Transform selectAgent = transform.FindChild ("SelectAgent");
		Transform agentList = selectAgent.FindChild ("AgentList");
		
		//agentList.DetachChildren ();
		foreach(Transform child in agentList)
		{
			Destroy(child.gameObject);
		}
		
		AgentUnit[] agents = AgentFacade.instance.GetAgentList ();
		
		float ypos = 0;
		for(int i=0; i<agents.Length; i++)
		{
			GameObject slot = Instantiate(Resources.Load<GameObject> ("Prefabs/AgentSlot")) as GameObject;
			AgentSlot agentSlot = slot.GetComponent<AgentSlot>();
			agentSlot.receiver = this;
			agentSlot.slotIndex = i;
			
			Transform name = slot.transform.FindChild("Name");
			name.gameObject.GetComponent<TextMesh>().text = agents[i].name;
			
			slot.transform.parent = agentList;
			slot.transform.localPosition = new Vector3(0,ypos,0);
			
			ypos -= 0.4f;
		}
		
		UpdatePosition ();
	}
	*/
	public void OnClickSlot(GameObject slotObject)
	{
		AgentSlot agentSlot = slotObject.GetComponent<AgentSlot> ();
		
		AgentUnit[] agents = AgentFacade.instance.GetAgentList ();
		AgentUnit unit = agents [agentSlot.slotIndex];
		
		if(!selectedAgentList.Contains(unit.gameObject))
		{
			if(selectedAgentList.Count > 0)
				return;
			selectedAgentList.Add(unit.gameObject);
			agentSlot.SetSelect(true);
		}
		else
		{
			selectedAgentList.Remove(unit.gameObject);
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