using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class AreaButton
{
    public string name;
    public UnityEngine.UI.Button on;
    public UnityEngine.UI.Image off;
}

public class StageUI : MonoBehaviour, IObserver {

    public enum UIType {START_STAGE, END_STAGE};

    public int agentCost;
    public int areaCost;

    private static StageUI _instance;
    public static StageUI instance
    {
        get { return _instance; }
    }
    public Canvas canvas;

    public GameObject startStageUi;
    public GameObject endStageUi;
    public GameObject commonStageUi;

    public AreaButton[] areaButtons;

    public Transform agentScrollTarget;

    private Dictionary<string, AreaButton> areaBtnDic;
    private bool opened;
    private UIType currentType;

    public string currentSefriaUi = "1";
    public UnityEngine.UI.Button openSefria;
    public GameObject agentSlot;

    void Awake()
    {
        _instance = this;

        agentCost = 1;
        areaCost = 10;

        areaBtnDic = new Dictionary<string, AreaButton>();
        foreach (AreaButton btn in areaButtons)
        {
            areaBtnDic.Add(btn.name, btn);
        }
    }

    void OnEnable()
    {
        Notice.instance.Observe(NoticeName.AreaOpenUpdate, this);
    }
    void OnDisable()
    {
        Notice.instance.Remove(NoticeName.AreaOpenUpdate, this);
    }

    private void Init()
    {
        PlayerModel playerModel = PlayerModel.instance;
        foreach (KeyValuePair<string, AreaButton> v in areaBtnDic)
        {
            AreaButton btn = v.Value;
            UpdateButton(btn);
        }
        agentSlot.gameObject.SetActive(false);
        ShowAgentList();
    }

    public void OnUpdateOpenedArea(string name)
    {
        if (opened == false)
            return;

        AreaButton btn;
        if (areaBtnDic.TryGetValue(name, out btn) == false)
        {
            Debug.Log("btn Not Found");
            return;
        }

        //UpdateButton(btn);
    }

    private void UpdateButton(AreaButton btn)
    {

        btn.on.gameObject.SetActive(true);
        btn.off.gameObject.SetActive(false);
        /*
        if (PlayerModel.instance.IsOpenedArea(btn.name))
        {
            btn.on.gameObject.SetActive(false);
            btn.off.gameObject.SetActive(true);
        }
        else
        {
            btn.on.gameObject.SetActive(true);
            btn.off.gameObject.SetActive(false);
        }*/
    }

    public void OnClickAddAgent()
    {
        if (EnergyModel.instance.GetLeftEnergy() >= agentCost && AgentManager.instance.agentCount >= AgentManager.instance.GetAgentList().Length)
        {
            EnergyModel.instance.SetLeftEnergy(EnergyModel.instance.GetLeftEnergy() - agentCost);

            long[] idList = PlayerModel.instance.GetAvailableAgentList();

            long selected = idList[Random.Range(0, idList.Length)];

            AgentModel newAgent = AgentManager.instance.BuyAgent(selected);

            if (SefiraAgentSlot.instance.MalkuthAgentList.Count <= 5)
            {
                SefiraAgentSlot.instance.MalkuthAgentList.Add(newAgent);
            }

            else if (SefiraAgentSlot.instance.MalkuthAgentList.Count > 5 )

            AgentManager.instance.BuyAgent(selected);

            StartStageUI.instance.ShowAgentCount();
            ShowAgentList();
            SefiraAgentSlot.instance.ShowAgentSefira(currentSefriaUi);
        }
        else
            Debug.Log("에너지가 모자라거나 고용가능 직원수가 다찼어");
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
            GameObject slot = Prefab.LoadPrefab("Slot/AgentSlotPanelStage");

            slot.transform.SetParent(agentScrollTarget, false);

            RectTransform tr = slot.GetComponent<RectTransform>();
            tr.localPosition = new Vector3(0, posy, 0);
            AgentSlotPanelStage slotPanel = slot.GetComponent<AgentSlotPanelStage>();

            AgentModel copied = unit;
            ShowPromotionButton(copied, slotPanel.promotion);

            slotPanel.addSefira.gameObject.SetActive(false);

            if (copied.currentSefira != currentSefriaUi)
                slotPanel.addSefira.gameObject.SetActive(true);

            slotPanel.addSefira.onClick.AddListener(() => SetAgentSefriaButton(copied));

            slotPanel.nameText.text = unit.name;
            slotPanel.HPText.text = "HP : " + unit.hp + "/" + unit.maxHp;
            slotPanel.agentLevel.text = "직원등급 : "+unit.level;

            if (copied.currentSefira == "1")
                slotPanel.currentSefria.sprite = Resources.Load<Sprite>("Sprites/UI/StageUI/Malkuth_Ico");
            else if(copied.currentSefira == "2")
                slotPanel.currentSefria.sprite = Resources.Load<Sprite>("Sprites/UI/StageUI/Netzzach_Icon");
            else if(copied.currentSefira == "3")
                slotPanel.currentSefria.sprite = Resources.Load<Sprite>("Sprites/UI/StageUI/Hod_Icon");
            else if(copied.currentSefira == "4")
                slotPanel.currentSefria.sprite = Resources.Load<Sprite>("Sprites/UI/StageUI/Yesod_Icon");

            Texture2D tex = Resources.Load<Texture2D>("Sprites/" + unit.imgsrc);
            slotPanel.agentIcon.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));

            posy -= 100f;
        }

        // scroll rect size
        Vector2 scrollRectSize = agentScrollTarget.GetComponent<RectTransform>().sizeDelta;
        scrollRectSize.y = -posy + 100f;
        agentScrollTarget.GetComponent<RectTransform>().sizeDelta = scrollRectSize;

        StartStageUI.instance.ShowAgentCount();
    }

    public void CancelSefiraAgent(AgentModel unit)
    {
        if (unit.currentSefira.Equals("1"))
        {
            for (int i = 0; i < SefiraAgentSlot.instance.MalkuthAgentList.Count; i++)
            {
                if (unit.instanceId == SefiraAgentSlot.instance.MalkuthAgentList[i].instanceId)
                {
                    SefiraAgentSlot.instance.MalkuthAgentList.RemoveAt(i);
                    break;
                }
            }
        }

        else if (unit.currentSefira.Equals("2"))
        {
            for (int i = 0; i < SefiraAgentSlot.instance.NezzachAgentList.Count; i++)
            {
                if (unit.instanceId == SefiraAgentSlot.instance.NezzachAgentList[i].instanceId)
                {
                    SefiraAgentSlot.instance.NezzachAgentList.RemoveAt(i);
                    break;
                }
            }
        }

        else if (unit.currentSefira.Equals("3"))
        {
            for (int i = 0; i < SefiraAgentSlot.instance.HodAgentList.Count; i++)
            {
                if (unit.instanceId == SefiraAgentSlot.instance.HodAgentList[i].instanceId)
                {
                    SefiraAgentSlot.instance.HodAgentList.RemoveAt(i);
                    break;
                }
            }
        }

        else if (unit.currentSefira.Equals("4"))
        {
            for (int i = 0; i < SefiraAgentSlot.instance.YesodAgentList.Count; i++)
            {
                if (unit.instanceId == SefiraAgentSlot.instance.YesodAgentList[i].instanceId)
                {
                    SefiraAgentSlot.instance.YesodAgentList.RemoveAt(i);
                    break;
                }
            }
        }
    }

    public void SetAgentSefriaButton(AgentModel unit)
    {      
        bool agentExist = false;

        CancelSefiraAgent(unit);

        if(currentSefriaUi == "1")
        {
            for (int i = 0; i < SefiraAgentSlot.instance.MalkuthAgentList.Count; i++)
            {
                if (unit.instanceId == SefiraAgentSlot.instance.MalkuthAgentList[i].instanceId)
                {
                    agentExist = true;
                }
            }
            if (!agentExist)
            {
                SefiraAgentSlot.instance.MalkuthAgentList.Add(unit);
                unit.GetMovableNode().SetCurrentNode(MapGraph.instance.GetSepiraNodeByRandom("1"));
                unit.SetCurrentSefira("1");
            }
            else
                Debug.Log("이미 추가한 직원");
                
        }

        else if (currentSefriaUi == "2" && PlayerModel.instance.IsOpenedArea("2"))
        {
            for (int i = 0; i < SefiraAgentSlot.instance.NezzachAgentList.Count; i++)
            {
                if (unit.instanceId == SefiraAgentSlot.instance.NezzachAgentList[i].instanceId)
                {
                    agentExist = true;
                }
            }
            if (!agentExist)
            {
                SefiraAgentSlot.instance.NezzachAgentList.Add(unit);
                unit.GetMovableNode().SetCurrentNode(MapGraph.instance.GetSepiraNodeByRandom("2"));
                unit.SetCurrentSefira("2");
            }
            else
                Debug.Log("이미 추가한 직원");
        }

        else if (currentSefriaUi == "3" && PlayerModel.instance.IsOpenedArea("3"))
        {
            for (int i = 0; i < SefiraAgentSlot.instance.HodAgentList.Count; i++)
            {
                if (unit.instanceId == SefiraAgentSlot.instance.HodAgentList[i].instanceId)
                {
                    agentExist = true;
                }
            }
            if (!agentExist)
            {
                SefiraAgentSlot.instance.HodAgentList.Add(unit);
                unit.GetMovableNode().SetCurrentNode(MapGraph.instance.GetSepiraNodeByRandom("3"));
                unit.SetCurrentSefira("3");
            }
            else
                Debug.Log("이미 추가한 직원");
        }

        else if (currentSefriaUi == "4" && PlayerModel.instance.IsOpenedArea("4"))
        {
            for (int i = 0; i < SefiraAgentSlot.instance.YesodAgentList.Count; i++)
            {
                if (unit.instanceId == SefiraAgentSlot.instance.YesodAgentList[i].instanceId)
                {
                    agentExist = true;
                }
            }
            if (!agentExist)
            {
                SefiraAgentSlot.instance.YesodAgentList.Add(unit);
                unit.GetMovableNode().SetCurrentNode(MapGraph.instance.GetSepiraNodeByRandom("4"));
                unit.SetCurrentSefira("4");
            }
            else
                Debug.Log("이미 추가한 직원");
        }
        SefiraAgentSlot.instance.ShowAgentSefira(currentSefriaUi);
        ShowAgentList();
   }

    public void SetCurrentSefria(string areaName)
    {
        currentSefriaUi = areaName;
        SefiraAgentSlot.instance.ShowAgentSefira(currentSefriaUi);
        ShowAgentList();

        if (PlayerModel.instance.IsOpenedArea(currentSefriaUi))
        {
            agentSlot.gameObject.SetActive(true);
            openSefria.gameObject.SetActive(false);
        }
         else
        {
            openSefria.gameObject.SetActive(true);
            agentSlot.gameObject.SetActive(false);
        }
    }

    public void OnClickBuyArea()
    {
            if (EnergyModel.instance.GetLeftEnergy() >=areaCost)
            {
                EnergyModel.instance.SetLeftEnergy(EnergyModel.instance.GetLeftEnergy() - areaCost);
                PlayerModel.instance.OpenArea(currentSefriaUi);

                AgentManager.instance.agentCount += 5;
                StartStageUI.instance.ShowAgentCount();

                openSefria.gameObject.SetActive(false);
                agentSlot.gameObject.SetActive(true);
            }
            else
                Debug.Log("에너지가 모자라");
       }


    //직원 승급 조건 및 버튼 활성화 
    public void ShowPromotionButton(AgentModel agent, UnityEngine.UI.Button button)
    {

        if (agent.expSuccess < 2 && agent.expSuccess >= 0 && agent.level == 1)
        {
            button.gameObject.SetActive(true);
            button.GetComponentInChildren<UnityEngine.UI.Text>().text = "승급 비용 2";
            button.onClick.AddListener(() => PromotionAgent(agent,1,button));
        }

        else if (agent.expSuccess < 3 && agent.expSuccess >= 2 && agent.level == 2)
        {
            button.gameObject.SetActive(true);
            button.GetComponentInChildren<UnityEngine.UI.Text>().text = "승급 비용 5";
            button.onClick.AddListener(() => PromotionAgent(agent, 2, button));
        }

        else
        {
            button.gameObject.SetActive(false);
        }
    }

    public void PromotionAgent(AgentModel agent, int level, UnityEngine.UI.Button button)
    {

        int level2Cost = 2;
        int level3Cost = 5;

        if (level == 1)
        {
            if (EnergyModel.instance.GetLeftEnergy() >= level2Cost)
            {
                EnergyModel.instance.SetLeftEnergy(EnergyModel.instance.GetLeftEnergy() - level2Cost);
                agent.level = 2;

                TraitTypeInfo RandomTraitInfo1 = TraitTypeList.instance.GetTraitWithLevel(level);
                TraitTypeInfo RandomTraitInfo2 = TraitTypeList.instance.GetTraitWithLevel(level);
                TraitTypeInfo WorkTrait = TraitTypeList.instance.GetRandomLevelWorkTrait(level);

                if (RandomTraitInfo1.id == RandomTraitInfo2.id)
                {
                    while (true)
                    {
                        RandomTraitInfo2 = TraitTypeList.instance.GetRandomInitTrait();
                        if (RandomTraitInfo1.id != RandomTraitInfo2.id)
                            break;
                    }
                }

                agent.traitList.Add(RandomTraitInfo1);
                agent.traitList.Add(RandomTraitInfo2);
                agent.traitList.Add(WorkTrait);

                agent.applyTrait(RandomTraitInfo1);
                agent.applyTrait(RandomTraitInfo2);
                agent.applyTrait(WorkTrait);

                button.gameObject.SetActive(false);
                ShowAgentList();
            }

            else
            {
                Debug.Log("코스트 부족");
            }
        }

        else if (level == 2)
        {
            if (EnergyModel.instance.GetLeftEnergy() >= level3Cost)
            {
                EnergyModel.instance.SetLeftEnergy(EnergyModel.instance.GetLeftEnergy() - level3Cost);
                agent.level = 3;

                TraitTypeInfo RandomTraitInfo1 = TraitTypeList.instance.GetTraitWithLevel(level);
                TraitTypeInfo RandomTraitInfo2 = TraitTypeList.instance.GetTraitWithLevel(level);

                if (RandomTraitInfo1.id == RandomTraitInfo2.id)
                {
                    while (true)
                    {
                        RandomTraitInfo2 = TraitTypeList.instance.GetRandomInitTrait();
                        if (RandomTraitInfo1.id != RandomTraitInfo2.id)
                            break;
                    }
                }

                agent.traitList.Add(RandomTraitInfo1);
                agent.traitList.Add(RandomTraitInfo2);

                agent.applyTrait(RandomTraitInfo1);
                agent.applyTrait(RandomTraitInfo2);

                button.gameObject.SetActive(false);
                ShowAgentList();
            }

            else
            {
                Debug.Log("코스트 부족");
            }
        }

        else
        {
            Debug.Log("승급버튼 문제");
        }
    }


    // ok btn
    public void Close()
    {
        opened = false;
        //canvas.gameObject.SetActive(false);

        if (currentType == UIType.START_STAGE)
        {
            commonStageUi.gameObject.SetActive(false);
            startStageUi.gameObject.SetActive(false);
            GameManager.currentGameManager.StartGame();
        }
        else if (currentType == UIType.END_STAGE)
        {
            //commonStageUi.gameObject.SetActive(false);
            //endStageUi.gameObject.SetActive(false);
            GameManager.currentGameManager.ExitStage();
        }
    }

    public void Open(UIType uiType)
    {
        opened = true;
        currentType = uiType;
        commonStageUi.gameObject.SetActive(true);

        Debug.Log(currentType);

        if (currentType == UIType.START_STAGE)
            startStageUi.gameObject.SetActive(true);

        else if (currentType == UIType.END_STAGE)
            endStageUi.gameObject.SetActive(true);

        Init();
            //canvas.gameObject.SetActive(true);

    }


    public void OnNotice(string notice, params object[] param)
    {
        if (notice == NoticeName.AreaOpenUpdate)
        {
            OnUpdateOpenedArea((string)param[0]);
        }
    }

    public UIType getCurrnetType()
    {
        return currentType;
    }
}
