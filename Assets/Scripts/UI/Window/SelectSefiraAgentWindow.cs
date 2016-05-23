using UnityEngine;
using System.Collections.Generic;

public class SelectSefiraAgentWindow : MonoBehaviour {

    public Transform agentScrollTarget;
    public Transform anchor;
    public AgentList listScript;

    private int state1 = 0;

    private string targetSefiraName;
    private GameObject targetSefira;

    List<AgentModel> selectedAgentList = new List<AgentModel>();

    public static SelectSefiraAgentWindow currentWindow = null;

    public static SelectSefiraAgentWindow CreateWindow(GameObject sefira, string sefiraName)
    {
        return null;
        if (currentWindow != null)
        {
            currentWindow.CloseWindow();
        }

        GameObject newObj = Prefab.LoadPrefab("SelectSefiraAgentWindow");

        SelectSefiraAgentWindow inst = newObj.GetComponent<SelectSefiraAgentWindow>();
        //inst.ShowSelectAgent (unit.gameObject);
        inst.targetSefira = sefira;
        inst.targetSefiraName = sefiraName;
        inst.ShowAgentList();

        currentWindow = inst;

        return inst;
    }

    // Use this for initialization
    void Awake()
    {
        listScript = GameObject.FindWithTag("SefiraAgentListPanel").GetComponent<AgentList>();
        UpdatePosition();
    }

    void FixedUpdate()
    {
        //UpdatePosition();
    }

    private void UpdatePosition()
    {
        if (targetSefira != null)
        {
            Vector3 targetPos = targetSefira.transform.position;

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

    public void addSefiraList(AgentModel agent)
    {
        /*
        if (targetSefiraName == "1")
        {
            if (AgentManager.instance.malkuthAgentList.Count < 5)
            {
                agent.SetCurrentSefira(targetSefiraName);
            }
            else
                Debug.Log("말쿠트 초과");
        }

        else if (targetSefiraName == "2")
        {
            if (AgentManager.instance.nezzachAgentList.Count < 5)
            {
                agent.SetCurrentSefira(targetSefiraName);
            }
            else
                Debug.Log("네짜흐 초과");
        }

        else if (targetSefiraName == "3")
        {
            if (AgentManager.instance.hodAgentList.Count < 5)
            {
                agent.SetCurrentSefira(targetSefiraName);
            }
            else
                Debug.Log("호드 초과");
        }

        else if (targetSefiraName == "4")
        {
            if (AgentManager.instance.yesodAgentList.Count < 5)
            {
                agent.SetCurrentSefira(targetSefiraName);
            }
            else
                Debug.Log("예소드 초과");
        }
         */

        Sefira sefira = SefiraManager.instance.GetSefira(targetSefiraName);
        if (sefira.agentList.Count < 5)
        {
            agent.SetCurrentSefira(targetSefiraName);
        }
        else {
            Debug.Log(sefira.name + " 초과");
        }

        listScript.ShowAgentListD();
    }

    public void OnSelectAgent(AgentModel agent)
    {     
        //agent.SetCurrentSefira(targetSefiraName);
        addSefiraList(agent);
        ShowAgentList();
    }

    public void ShowAgentList()
    {
        AgentModel[] agents = AgentManager.instance.GetAgentList();

        foreach (Transform child in agentScrollTarget.transform)
        {
            Destroy(child.gameObject);
        }

        float posy = 0;
        foreach (AgentModel unit in agents)
        {
            AgentAIState state = unit.GetState();
            if (unit.currentSefira == targetSefiraName
                || state == AgentAIState.MANAGE
                || state == AgentAIState.OBSERVE)
                continue;
            GameObject slot = Prefab.LoadPrefab("AgentSlotPanelSefira");

            slot.transform.SetParent(agentScrollTarget, false);

            RectTransform tr = slot.GetComponent<RectTransform>();
            tr.localPosition = new Vector3(0, posy, 0);
            AgentSlotPanelSefira slotPanel = slot.GetComponent<AgentSlotPanelSefira>();

            slotPanel.nameText.text = unit.name;
            slotPanel.HPText.text = "HP : " + unit.hp + "/"+unit.maxHp;

            AgentModel copied = unit;
            slotPanel.button.onClick.AddListener(() => OnSelectAgent(copied));
			/*
            slotPanel.agentBody.sprite = ResourceCache.instance.GetSprite(unit.bodyImgSrc);
            slotPanel.agentFace.sprite = ResourceCache.instance.GetSprite(unit.faceImgSrc);
            slotPanel.agentHair.sprite = ResourceCache.instance.GetSprite(unit.hairImgSrc);
            */

            posy -= 100f;
        }

        // scroll rect size
        Vector2 scrollRectSize = agentScrollTarget.GetComponent<RectTransform>().sizeDelta;
        scrollRectSize.y = -posy + 100f;
        agentScrollTarget.GetComponent<RectTransform>().sizeDelta = scrollRectSize;

        UpdatePosition();
    }


    public void OnClickSlot(GameObject slotObject)
    {
        AgentSlot agentSlot = slotObject.GetComponent<AgentSlot>();

        AgentModel[] agents = AgentManager.instance.GetAgentList();
        AgentModel unit = agents[agentSlot.slotIndex];

        if (!selectedAgentList.Contains(unit))
        {
            if (selectedAgentList.Count > 0)
                return;
            selectedAgentList.Add(unit);
            agentSlot.SetSelect(true);
        }
        else
        {
            selectedAgentList.Remove(unit);
            agentSlot.SetSelect(false);
        }
        OnClickAgentOK();
    }

    public void CloseWindow()
    {
        currentWindow = null;
        Destroy(gameObject);
    }
}
