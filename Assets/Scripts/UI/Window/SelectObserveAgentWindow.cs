using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SelectObserveAgentWindow : MonoBehaviour
{

    public Transform agentScrollTarget;
    public Transform anchor;

    private int state1 = 0;

    private CreatureUnit targetCreature = null;
    private IsolateRoom targetRoom = null;

    List<GameObject> selectedAgentList = new List<GameObject>();

    public static SelectObserveAgentWindow currentWindow = null;

    public static SelectObserveAgentWindow CreateWindow(IsolateRoom room)
    {
        if (currentWindow != null)
        {
            currentWindow.CloseWindow();
        }

        GameObject newObj = Instantiate(Resources.Load<GameObject>("Prefabs/SelectObserveAgentWindow")) as GameObject;

        SelectObserveAgentWindow inst = newObj.GetComponent<SelectObserveAgentWindow>();

        inst.targetRoom = room;
        inst.targetCreature = room.targetUnit;
        inst.ShowAgentList();

        currentWindow = inst;

        return inst;
    }

    public static SelectObserveAgentWindow CreateWindow(CreatureUnit unit)
    {
        if (currentWindow != null)
        {
            currentWindow.CloseWindow();
        }

        GameObject newObj = Prefab.LoadPrefab("SelectObserveAgentWindow");

        SelectObserveAgentWindow inst = newObj.GetComponent<SelectObserveAgentWindow>();

        inst.targetCreature = unit;
        inst.ShowAgentList();

        currentWindow = inst;

        return inst;
    }

    // Use this for initialization
    void Awake()
    {
        UpdatePosition();
    }

    // Update is called once per frame
    void Update()
    {
    }

    void FixedUpdate()
    {
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        if (targetCreature != null && false)
        {
            Vector3 targetPos = targetCreature.transform.position;

            anchor.position = Camera.main.WorldToScreenPoint(targetPos);
        }
        else if (targetRoom != null)
        {
            Vector3 targetPos = targetRoom.transform.position;

            anchor.position = Camera.main.WorldToScreenPoint(targetPos + new Vector3(0, -3, 0));
        }
    }

    public void OnClickAgentOK()
    {

    }
    public void OnClickClose()
    {
        CloseWindow();
    }

    public void SelectAgentSkill(AgentUnit agent)
    {
        ObserveCreature.Create(agent,targetCreature);
        //UseSkill.InitUseSkillAction(skillInfo, agent, targetCreature);
        CloseWindow();
    }

    public void ShowAgentList()
    {
        AgentUnit[] agents = AgentManager.instance.GetAgentList();

        float posy = 0;
        foreach (AgentUnit unit in agents)
        {
            GameObject slot = Prefab.LoadPrefab("AgentSlotPanelObserve");

            slot.transform.SetParent(agentScrollTarget, false);

            RectTransform tr = slot.GetComponent<RectTransform>();
            tr.localPosition = new Vector3(0, posy, 0);

            AgentSlotPanelObserve slotPanel = slot.GetComponent<AgentSlotPanelObserve>();
            slotPanel.skillButton1.image.sprite = Resources.Load<Sprite>("Sprites/" + unit.directSkill.imgsrc);


            AgentUnit copied = unit;
            slotPanel.skillButton1.onClick.AddListener(() => SelectAgentSkill(copied));


            Texture2D tex = Resources.Load<Texture2D>("Sprites/" + unit.imgsrc);
            slotPanel.agentIcon.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));

            Debug.Log(tr.localPosition);

            posy -= 50f;
        }

        UpdatePosition();
    }
    public void CloseWindow()
    {
        //gameObject.SetActive (false);
        currentWindow = null;
        Destroy(gameObject);
    }
}
