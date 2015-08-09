using UnityEngine;
using System.Collections.Generic;

public class SelectSefiraAgentWindow : MonoBehaviour {

    public Transform agentScrollTarget;
    public Transform anchor;

    private int state1 = 0;

    private string targetSefiraName;
    private GameObject targetSefira;

    List<AgentModel> selectedAgentList = new List<AgentModel>();

    public static SelectSefiraAgentWindow currentWindow = null;

    public static SelectSefiraAgentWindow CreateWindow(GameObject sefira, string sefiraName)
    {
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
        UpdatePosition();
    }

    void FixedUpdate()
    {
        UpdatePosition();
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
        if (targetSefiraName == "1")
        {
            if (SefiraAgentSlot.instance.MalkuthAgentList.Count < 5)
            {
                SefiraAgentSlot.instance.MalkuthAgentList.Add(agent);
                deleteSefiraList(agent);
                agent.SetCurrentSefira(targetSefiraName);
            }
            else
                Debug.Log("말쿠트 초과");
        }

        else if (targetSefiraName == "2")
        {
            if (SefiraAgentSlot.instance.NezzachAgentList.Count < 5)
            {
                SefiraAgentSlot.instance.NezzachAgentList.Add(agent);
                deleteSefiraList(agent);
                agent.SetCurrentSefira(targetSefiraName);
            }
            else
                Debug.Log("네짜흐 초과");
        }

        else if (targetSefiraName == "3")
        {
            if (SefiraAgentSlot.instance.HodAgentList.Count < 5)
            {
                SefiraAgentSlot.instance.HodAgentList.Add(agent);
                deleteSefiraList(agent);
                agent.SetCurrentSefira(targetSefiraName);
            }
            else
                Debug.Log("호드 초과");
        }

        else if (targetSefiraName == "4")
        {
            if (SefiraAgentSlot.instance.YesodAgentList.Count < 5)
            {
                SefiraAgentSlot.instance.YesodAgentList.Add(agent);
                deleteSefiraList(agent);
                agent.SetCurrentSefira(targetSefiraName);
            }
            else
                Debug.Log("예소드 초과");
        }
    }

    public void deleteSefiraList(AgentModel agent)
    {
        if (agent.currentSefira == "1")
        {
            SefiraAgentSlot.instance.MalkuthAgentList.Remove(agent);
        }

        else if (agent.currentSefira == "2")
        {
            SefiraAgentSlot.instance.NezzachAgentList.Remove(agent);
        }

        else if (agent.currentSefira == "3")
        {
            SefiraAgentSlot.instance.HodAgentList.Remove(agent);
        }

        else if (agent.currentSefira == "4")
        {
            SefiraAgentSlot.instance.YesodAgentList.Remove(agent);
        }
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
            if (unit.currentSefira == targetSefiraName || unit.GetState() == AgentCmdState.WORKING)
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
