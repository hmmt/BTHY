using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

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

    public UnityEngine.UI.Image[] areaButtonImage;

    public Transform agentScrollTarget;

    private Dictionary<string, AreaButton> areaBtnDic;
    private bool opened;
    private UIType currentType;

    public string currentSefriaUi = "1";
    public UnityEngine.UI.Button openSefria;
    public GameObject agentSlot;

    public RectTransform AddButton;

    public int extended = -1;


    public int setState;


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
        /*
        foreach (KeyValuePair<string, AreaButton> v in areaBtnDic)
        {
            AreaButton btn = v.Value;
            UpdateButton(btn);
        }*/
        GameObject sliderPanel = GameObject.FindWithTag("EnergyPanel");
        float MaxValueForEnergy = EnergyModel.instance.GetLeftEnergy();
        sliderPanel.GetComponent<MenuLeftEnergy>().SetSlider(MaxValueForEnergy);
        UpdateSefiraButton("1");

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

    public void UpdateSefiraButton(string sefira)
    {
         int sefiraNum = int.Parse(sefira);
        string sefiraName = "";

        if(sefiraNum == 1)
        {
            sefiraName = "Malkuth";
        }

        else if(sefiraNum == 2)
        {
            sefiraName = "Nezzach";
        }

        else if(sefiraNum == 3)
        {
            sefiraName = "Hod";
        }

        else if(sefiraNum == 4)
        {
            sefiraName = "Yessod";
        }

        if(PlayerModel.instance.IsOpenedArea(sefira))
         areaButtonImage[sefiraNum - 1].sprite = Resources.Load<Sprite>("Sprites/UI/StageUI/"+sefiraName+"_On");
        else
            areaButtonImage[sefiraNum - 1].sprite = Resources.Load<Sprite>("Sprites/UI/StageUI/" + sefiraName + "_Off");
    }

    private void UpdateButton(AreaButton btn)
    {

        btn.on.gameObject.SetActive(true);
        btn.off.gameObject.SetActive(false);
        
        if (PlayerModel.instance.IsOpenedArea(btn.name))
        {
            btn.on.gameObject.SetActive(false);
            btn.off.gameObject.SetActive(true);
        }
        else
        {
            btn.on.gameObject.SetActive(true);
            btn.off.gameObject.SetActive(false);
        }
    }

    public void OnClickAddAgent()
    {
        int currentAgentCount = (AgentManager.instance.GetAgentList().Length+AgentManager.instance.agentListSpare.Count);

        if (EnergyModel.instance.GetLeftEnergy() >= agentCost
            && AgentManager.instance.agentCount > currentAgentCount)
        {
            EnergyModel.instance.SetLeftEnergy(EnergyModel.instance.GetLeftEnergy() - agentCost);

            //AgentModel newAgent = AgentManager.instance.BuyAgent(selected);

            AgentManager.instance.BuyAgent();

            StartStageUI.instance.ShowAgentCount();
            ShowAgentList();
            SefiraAgentSlot.instance.ShowAgentSefira(currentSefriaUi);


        }
        else
            Debug.Log("에너지가 모자라거나 고용가능 직원수가 다찼어");
    }

    private GameObject[] setList(AgentModel[] models) {
        GameObject[] list= new GameObject[models.Length];
        int i = 0;
        foreach (AgentModel unit in models)
        {
            GameObject slot = Prefab.LoadPrefab("Slot/SlotPanel");
            slot.SetActive(true);

            slot.transform.SetParent(agentScrollTarget, false);
            list[i] = slot;
            
            AgentSlotScript slotPanel = slot.GetComponent<AgentSlotScript>();
            AgentModel copied = unit;
            
            //ShowPromotionButton(copied, slotPanel.attr1.Promotion);
            
            slotPanel.attr1.Add.gameObject.SetActive(false);

            if (copied.currentSefira != currentSefriaUi)
                slotPanel.attr1.Add.gameObject.SetActive(true);

            slotPanel.attr1.Add.onClick.AddListener(() => SetAgentSefriaButton(copied));
            slotPanel.display = new AgentGetAttributes();
            slotPanel.display.name = unit.name;
            slotPanel.display.hp = "HP : " + unit.hp + "/" + unit.maxHp;
            slotPanel.display.level = "직원등급 : " + unit.level;
            

            switch (copied.currentSefira)
            {
                case "0":
                    slotPanel.display.sefira = ResourceCache.instance.GetSprite("Sprites/UI/StageUI/None_Icon");
                    break;
                case "1":
                    slotPanel.display.sefira = ResourceCache.instance.GetSprite("Sprites/UI/StageUI/Malkuth_Icon");
                    break;
                case "2":
                    slotPanel.display.sefira = ResourceCache.instance.GetSprite("Sprites/UI/StageUI/Netzzach_Icon");
                    break;
                case "3":
                    slotPanel.display.sefira = ResourceCache.instance.GetSprite("Sprites/UI/StageUI/Hod_Icon");
                    break;
                case "4":
                    slotPanel.display.sefira = ResourceCache.instance.GetSprite("Sprites/UI/StageUI/Yessod_Icon");
                    break;
                default:
                    slotPanel.display.sefira = ResourceCache.instance.GetSprite("Sprites/UI/StageUI/None_Icon");
                    break;
            }

            slotPanel.display.hair = ResourceCache.instance.GetSprite(unit.hairImgSrc);
            slotPanel.display.face = ResourceCache.instance.GetSprite(unit.faceImgSrc);
            slotPanel.display.body = ResourceCache.instance.GetSprite(unit.bodyImgSrc);
            slotPanel.display.model = unit;
            slotPanel.model = unit;
            slotPanel.DisplayItems();
            slotPanel.ShowPromotionButton(copied);
            i++;
        }
        return list;
    }
    
    public void SetExtendedList(int i, int state) {
        this.setState = state;
        this.extended = i;
    }

    public int getExtendedList() {
        return this.extended;
    }

    public void ShowAgentList()
    {
        AgentModel[] agents = AgentManager.instance.GetAgentList();
        AgentModel[] spareAgents = AgentManager.instance.agentListSpare.ToArray();
        GameObject[] sub1 = new GameObject[agents.Length];
        GameObject[] sub2 = new GameObject[spareAgents.Length];
        GameObject[] total = new GameObject[agents.Length + spareAgents.Length];
        foreach (Transform child in agentScrollTarget.transform)
        {
            Destroy(child.gameObject);
            
        }
        
        sub1 = setList(agents);
        sub2 = setList(spareAgents);
        for (int i = 0; i < agents.Length; i++) {
            total[i] = sub1[i];
        }
        for (int i = 0; i < spareAgents.Length; i++)
        {
            total[i + agents.Length] = sub2[i];
        }

        
        int count = 0;
        foreach (GameObject child in total) {
            child.GetComponent<AgentSlotScript>().index = count;
            count++;
        }
        
        float posy = 0.0f;
        int cnt = 0;
        
        foreach (GameObject child in total)
        {
            float size;

            RectTransform tr = child.GetComponent<RectTransform>();
            
            AgentSlotScript script = child.GetComponent<AgentSlotScript>();

            if (extended == cnt)
            {
                switch (setState) { 
                    case 0:
                        script.smallstate = true;
                        script.bigstate = false;
                        script.promotionState = false;
                        break;
                    case 1:
                        script.smallstate = true;
                        script.bigstate = true;
                        script.promotionState = false;
                        break;
                    case 2:
                        script.smallstate = false;
                        script.bigstate = false;
                        script.promotionState = true;
                        break;
                }
                script.state = true;
            }
            else
            {
                script.smallstate = true;
                script.bigstate = false;
                script.promotionState = false;
                script.state = false;
               
            }
            size = script.GetSize();
            float normalSize = 0.15f;
            Vector2 max = new Vector2(0.96f, 1f - posy);
            Vector2 min = new Vector2(0.0f, 1f - posy - normalSize);
           
            tr.anchorMin = min;
            tr.anchorMax = max;

            tr.offsetMax = Vector2.zero;
            tr.offsetMin = Vector2.zero;
            posy += size;
            cnt++;
        }
        
        AddButton = GameObject.FindWithTag("AddAgentButton").GetComponent<RectTransform>();
        AddButton.anchorMax = new Vector2(0.96f, 1f - posy);
        AddButton.anchorMin = new Vector2(0f, 1f - (posy + 0.15f));
        // scroll rect size
        Vector2 scrollRectSize = agentScrollTarget.GetComponent<RectTransform>().sizeDelta;
        scrollRectSize.y = 0.0f;
        agentScrollTarget.GetComponent<RectTransform>().sizeDelta = scrollRectSize;
        
        StartStageUI.instance.ShowAgentCount();
    }
    /*
    public void ShowAgentList()
    {
        AgentModel[] agents = AgentManager.instance.GetAgentList();
        AgentModel[] spareAgents = AgentManager.instance.agentListSpare.ToArray();

        foreach (Transform child in agentScrollTarget.transform)
        {
            Destroy(child.gameObject);
        }

        float posy = 0;
        float std = 0.15f;
        

        foreach (AgentModel unit in agents)
        {
            GameObject slot = Prefab.LoadPrefab("Slot/AgentSlotPanelStage");

            slot.transform.SetParent(agentScrollTarget, false);
            
            RectTransform tr = slot.GetComponent<RectTransform>();
            //tr.localPosition = new Vector3(0, posy, 0);
            tr.anchorMax = new Vector2(1f, 1f - posy);
            tr.anchorMin = new Vector2(0f, 1f - (posy + std) );
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

            if (copied.currentSefira == "0")
                slotPanel.currentSefria.sprite = ResourceCache.instance.GetSprite("Sprites/UI/StageUI/None_Icon");
            else if (copied.currentSefira == "1")
                slotPanel.currentSefria.sprite = ResourceCache.instance.GetSprite("Sprites/UI/StageUI/Malkuth_Icon");
            else if(copied.currentSefira == "2")
                slotPanel.currentSefria.sprite = ResourceCache.instance.GetSprite("Sprites/UI/StageUI/Netzzach_Icon");
            else if(copied.currentSefira == "3")
                slotPanel.currentSefria.sprite = ResourceCache.instance.GetSprite("Sprites/UI/StageUI/Hod_Icon");
            else if(copied.currentSefira == "4")
                slotPanel.currentSefria.sprite = ResourceCache.instance.GetSprite("Sprites/UI/StageUI/Yessod_Icon");

            slotPanel.agentBodyIcon.sprite = ResourceCache.instance.GetSprite(unit.bodyImgSrc);
            slotPanel.agentFaceIcon.sprite = ResourceCache.instance.GetSprite(unit.faceImgSrc);
            slotPanel.agentHairIcon.sprite = ResourceCache.instance.GetSprite(unit.hairImgSrc);
            slotPanel.model = unit;
            posy += std;
        }

        foreach (AgentModel unit in spareAgents)
        {
            GameObject slot = Prefab.LoadPrefab("Slot/AgentSlotPanelStage");

            slot.transform.SetParent(agentScrollTarget, false);

            RectTransform tr = slot.GetComponent<RectTransform>();
            //tr.localPosition = new Vector3(0, posy, 0);
            tr.anchorMax = new Vector2(1f, 1f - posy);
            tr.anchorMin = new Vector2(0f, 1f - (posy + std));
            AgentSlotPanelStage slotPanel = slot.GetComponent<AgentSlotPanelStage>();

            AgentModel copied = unit;
            ShowPromotionButton(copied, slotPanel.promotion);

            slotPanel.addSefira.gameObject.SetActive(false);

            if (copied.currentSefira != currentSefriaUi)
                slotPanel.addSefira.gameObject.SetActive(true);

            slotPanel.addSefira.onClick.AddListener(() => SetAgentSefriaButton(copied));

            slotPanel.nameText.text = unit.name;
            slotPanel.HPText.text = "HP : " + unit.hp + "/" + unit.maxHp;
            slotPanel.agentLevel.text = "직원등급 : " + unit.level;

            if (copied.currentSefira == "0")
                slotPanel.currentSefria.sprite = ResourceCache.instance.GetSprite("Sprites/UI/StageUI/None_Icon");
            else if (copied.currentSefira == "1")
                slotPanel.currentSefria.sprite = ResourceCache.instance.GetSprite("Sprites/UI/StageUI/Malkuth_Icon");
            else if (copied.currentSefira == "2")
                slotPanel.currentSefria.sprite = ResourceCache.instance.GetSprite("Sprites/UI/StageUI/Netzzach_Icon");
            else if (copied.currentSefira == "3")
                slotPanel.currentSefria.sprite = ResourceCache.instance.GetSprite("Sprites/UI/StageUI/Hod_Icon");
            else if (copied.currentSefira == "4")
                slotPanel.currentSefria.sprite = ResourceCache.instance.GetSprite("Sprites/UI/StageUI/Yessod_Icon");

            slotPanel.agentBodyIcon.sprite = ResourceCache.instance.GetSprite(unit.bodyImgSrc);
            slotPanel.agentFaceIcon.sprite = ResourceCache.instance.GetSprite(unit.faceImgSrc);
            slotPanel.agentHairIcon.sprite = ResourceCache.instance.GetSprite(unit.hairImgSrc);
            slotPanel.model = unit;
            posy += std;
        }
        AddButton.anchorMax = new Vector2(1f, 1f - posy);
        AddButton.anchorMin = new Vector2(0f, 1f - (posy + std));;
        // scroll rect size
        Vector2 scrollRectSize = agentScrollTarget.GetComponent<RectTransform>().sizeDelta;
        scrollRectSize.y = -std + 0.15f;
        agentScrollTarget.GetComponent<RectTransform>().sizeDelta = scrollRectSize;

        StartStageUI.instance.ShowAgentCount();
    }
    */
    public void CancelSefiraAgent(AgentModel unit)
    {
        Debug.Log("delete");
        if (unit.currentSefira.Equals("1"))
        {
            for (int i = 0; i < AgentManager.instance.malkuthAgentList.Count; i++)
            {
                if (unit.instanceId == AgentManager.instance.malkuthAgentList[i].instanceId)
                {
                    AgentManager.instance.malkuthAgentList.RemoveAt(i);
                    break;
                }
            }
        }

        else if (unit.currentSefira.Equals("2"))
        {
            for (int i = 0; i < AgentManager.instance.nezzachAgentList.Count; i++)
            {
                if (unit.instanceId == AgentManager.instance.nezzachAgentList[i].instanceId)
                {
                    AgentManager.instance.nezzachAgentList.RemoveAt(i);
                    break;
                }
            }
        }

        else if (unit.currentSefira.Equals("3"))
        {
            for (int i = 0; i < AgentManager.instance.hodAgentList.Count; i++)
            {
                if (unit.instanceId == AgentManager.instance.hodAgentList[i].instanceId)
                {
                    AgentManager.instance.hodAgentList.RemoveAt(i);
                    break;
                }
            }
        }

        else if (unit.currentSefira.Equals("4"))
        {
            for (int i = 0; i < AgentManager.instance.yesodAgentList.Count; i++)
            {
                if (unit.instanceId == AgentManager.instance.yesodAgentList[i].instanceId)
                {
                    AgentManager.instance.yesodAgentList.RemoveAt(i);
                    break;
                }
            }
        }
        ShowAgentList();

        if(unit.activated)
            AgentManager.instance.deactivateAgent(unit);
    }

    public void SetAgentSefriaButton(AgentModel unit)
    {      
        bool agentExist = false;
       // Debug.Log(unit);
        CancelSefiraAgent(unit);

        if (currentSefriaUi == "1" )
        {
            for (int i = 0; i < AgentManager.instance.malkuthAgentList.Count; i++)
            {
                if (unit.instanceId == AgentManager.instance.malkuthAgentList[i].instanceId)
                {
                    agentExist = true;
                    break;
                }
            }
            if (!agentExist && AgentManager.instance.malkuthAgentList.Count < 5)
            {
                unit.GetMovableNode().SetCurrentNode(MapGraph.instance.GetSepiraNodeByRandom("1"));
                unit.SetCurrentSefira("1");
                if (!unit.activated)
                AgentManager.instance.activateAgent(unit, currentSefriaUi);
            }
            else
                Debug.Log("이미 추가한 직원");
                
        }

        else if (currentSefriaUi == "2" && PlayerModel.instance.IsOpenedArea("2"))
        {
            for (int i = 0; i < AgentManager.instance.nezzachAgentList.Count; i++)
            {
                if (unit.instanceId == AgentManager.instance.nezzachAgentList[i].instanceId)
                {
                    agentExist = true;
                    break;
                }
            }
            if (!agentExist && AgentManager.instance.nezzachAgentList.Count < 5)
            {
                unit.GetMovableNode().SetCurrentNode(MapGraph.instance.GetSepiraNodeByRandom("2"));
                unit.SetCurrentSefira("2");
                if (!unit.activated)
                AgentManager.instance.activateAgent(unit, currentSefriaUi);
                AgentLayer.currentLayer.GetAgent(unit.instanceId).agentAnimator.SetInteger("Sepira", 2);
            }
            else
                Debug.Log("이미 추가한 직원");
        }

        else if (currentSefriaUi == "3" && PlayerModel.instance.IsOpenedArea("3")  )
        {
            for (int i = 0; i < AgentManager.instance.hodAgentList.Count; i++)
            {
                if (unit.instanceId == AgentManager.instance.hodAgentList[i].instanceId)
                {
                    agentExist = true;
                    break;
                }
            }
            if (!agentExist && AgentManager.instance.hodAgentList.Count < 5)
            {
                unit.GetMovableNode().SetCurrentNode(MapGraph.instance.GetSepiraNodeByRandom("3"));
                unit.SetCurrentSefira("3");
                if (!unit.activated)
                AgentManager.instance.activateAgent(unit, currentSefriaUi);
            }
            else
                Debug.Log("이미 추가한 직원");
        }

        else if (currentSefriaUi == "4" && PlayerModel.instance.IsOpenedArea("4"))
        {
            for (int i = 0; i < AgentManager.instance.yesodAgentList.Count; i++)
            {
                if (unit.instanceId == AgentManager.instance.yesodAgentList[i].instanceId)
                {
                    agentExist = true;
                    break;
                }
            }

            if (!agentExist && AgentManager.instance.yesodAgentList.Count < 5)
            {
                unit.GetMovableNode().SetCurrentNode(MapGraph.instance.GetSepiraNodeByRandom("4"));
                unit.SetCurrentSefira("4");
                if(!unit.activated)
                AgentManager.instance.activateAgent(unit, currentSefriaUi);
            }
            else
                Debug.Log("이미 추가한 직원");
        }
        SefiraAgentSlot.instance.ShowAgentSefira(currentSefriaUi);
        unit.AgentPortrait("body", null);
       // AgentLayer.currentLayer.GetAgent(unit.instanceId).ChangeAgentUniform();
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

                UpdateSefiraButton(currentSefriaUi);
            }
            else
                Debug.Log("에너지가 모자라");
       }
    /*

    //직원 승급 조건 및 버튼 활성화 
    public void ShowPromotionButton(AgentModel agent, UnityEngine.UI.Button button)
    {

        if (agent.expSuccess < 2 && agent.expSuccess >= 0 && agent.level == 1)
        {
            button.gameObject.SetActive(true);
            //button.GetComponentInChildren<UnityEngine.UI.Text>().text = "승급 비용 2";
            button.onClick.AddListener(() => PromotionAgent(agent, 1, button));
        }

        else if (agent.expSuccess < 3 && agent.expSuccess >= 2 && agent.level == 2)
        {
            button.gameObject.SetActive(true);
            //button.GetComponentInChildren<UnityEngine.UI.Text>().text = "승급 비용 5";
            button.onClick.AddListener(() => PromotionAgent(agent, 2, button));
        }

        else
        {
            button.gameObject.SetActive(false);
        }
    }
    */
    public void ChangePromotionPanel(AgentModel agent, int i ,UnityEngine.UI.Button button) {
        
        PromotionAgent(agent, i, button);
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
                TraitTypeInfo RandomLifeValueTrait;

                if (Random.Range(0,1) ==0)
                {
                     RandomLifeValueTrait = TraitTypeList.instance.GetRandomEiTrait(level);
                }

                else
                {
                     RandomLifeValueTrait = TraitTypeList.instance.GetRandomNfTrait(level);
                }

                if (RandomTraitInfo1.id == RandomTraitInfo2.id)
                {
                    while (true)
                    {
                        RandomTraitInfo2 = TraitTypeList.instance.GetRandomInitTrait();
                        if (RandomTraitInfo1.id != RandomTraitInfo2.id)
                            break;
                    }
                }

                agent.applyTrait(RandomTraitInfo1);
                agent.applyTrait(RandomTraitInfo2);
                agent.applyTrait(RandomLifeValueTrait);

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
            //.gameObject.SetActive(true);

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
