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
    //public Canvas canvas;

    public GameObject startStageUi;
    public GameObject endStageUi;
    public GameObject commonStageUi;
    public Image CommonBg;

    public AreaButton[] areaButtons;

    public Image[] areaButtonImage;

    public Transform agentScrollTarget;
    public Transform agentListforScroll;

    private Dictionary<string, AreaButton> areaBtnDic;

    private bool opened;
    private UIType currentType;

    public string currentSefriaUi = "1";
    public Button OpenSefira;
    public GameObject agentSlot;

    public RectTransform AddButton;
    public RectTransform openText;
    public RectTransform agentInformation;
    public RectTransform infoSlot;

    public AgentListScript listScript;

    private float scrollSizeScale;
    private float initialYPos;
    private float scrollInitialY;

    public int extended = -1;
    //private int sortMode = 0;

    public bool setState = false;//확장정보창의 존재 유무
    private bool nextScene = false;

    public int[] levelCost;

    void Awake()
    {
        _instance = this;
        
        agentCost = 1;
        areaCost = 10;
        OpenSefira.gameObject.SetActive(false);
        openText.gameObject.SetActive(false);
        areaBtnDic = new Dictionary<string, AreaButton>();
        nextScene = false;
        foreach (AreaButton btn in areaButtons)
        {
            areaBtnDic.Add(btn.name, btn);
        }
        scrollSizeScale = agentScrollTarget.GetComponent<RectTransform>().rect.height;
        initialYPos = scrollSizeScale / 2;
        scrollInitialY = agentScrollTarget.localPosition.y;
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

        if (currentType == UIType.START_STAGE)
        {
            GameObject sliderPanel = GameObject.FindWithTag("EnergyPanel");
            float MaxValueForEnergy = EnergyModel.instance.GetLeftEnergy();
            sliderPanel.GetComponent<MenuLeftEnergy>().SetSlider(MaxValueForEnergy);
            currentSefriaUi = "0";
            UpdateSefiraButton("1");
            
            agentInformation.gameObject.SetActive(false);
            agentSlot.gameObject.SetActive(false);
            listScript.SetAddbuttonState(true);
            listScript.ShowAgentListWithChange();
        }
        else if (currentType == UIType.END_STAGE) {
            listScript.SetAddbuttonState(false);
        }
        //ShowAgentList();
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
           
            SefiraAgentSlot.instance.ShowAgentSefira(currentSefriaUi);
            listScript.ShowAgentList();
            //ShowAgentList();
            /*
            float standard = agentListforScroll.GetComponent<RectTransform>().rect.height;
            float move = agentScrollTarget.GetComponent<RectTransform>().rect.height;
            if (move > standard)
            {
                float heightformove = move - standard;
                agentScrollTarget.localPosition = new Vector3(agentScrollTarget.localPosition.x,
                                                             scrollInitialY + heightformove, 0.0f);
            }
            */
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
                        
            slotPanel.model = copied;
            slotPanel.setValues();
            i++;
        }
        return list;
    }

    public void CloseInformation() {
        this.setState = false;
        this.extended = -1;
        agentInformation.gameObject.SetActive(false);
        listScript.ShowAgentList();
        //ShowAgentList();
    }

    public void SetExtendedList(bool state, int index) {
        this.setState = state;
        this.extended = index;
    }

    public int getExtendedList() {
        return this.extended;
    }
    /*
    public void ShowAgentList()
    {
        AgentModel[] agents = AgentManager.instance.GetAgentList();
        AgentModel[] spareAgents = AgentManager.instance.agentListSpare.ToArray();
        GameObject[] sub1 = new GameObject[agents.Length];
        GameObject[] sub2 = new GameObject[spareAgents.Length];
        GameObject[] total = new GameObject[agents.Length + spareAgents.Length];
        Debug.Log("Called");
        foreach (Transform child in agentScrollTarget.transform)
        {
            if (child.tag == "AddAgentButton") continue;
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


       // posy = SortAgentList(posy, total, sortMode);
        float posy = SortAgentList(total, 0, 0);
        AddButton = GameObject.FindWithTag("AddAgentButton").GetComponent<RectTransform>();
        AddButton.localPosition = new Vector3(0f,posy, 0f);
        posy -= AddButton.GetComponent<RectTransform>().rect.height;

        Vector2 scrollRectSize = agentScrollTarget.GetComponent<RectTransform>().sizeDelta;
        scrollRectSize.y = -posy;
        agentScrollTarget.GetComponent<RectTransform>().sizeDelta = scrollRectSize;

        StartStageUI.instance.ShowAgentCount();
    }
     */
    /*
        직원들을 정렬한다
     * total = 전체 직원들 오브젝트가 들어가있는 리스트
     * mode = 정렬 모드를 선택 (0 = 기본 정렬모드: 즉, 생성 순서대로 저장됨
     *                          1 = 이름
     *                          2 = 등급
     *                          3 = 부서
     *                          4 = 가치관)
     *                          가치관의 순서는 1:합리주의
     *                                          2:낙천주의
     *                                          3:원칙주의
     *                                          4:평화주의
     * order = 정렬 순서 (오름차순 = 0, 내림차순 = 1) 
     */
    /*
    public float SortAgentList( GameObject[] total, int mode, int order) {
        float temp = 0.0f;
        GameObject[] list = new GameObject[total.Length];
        int cnt = 0;
        float posy = 0.0f;
        string.Compare("adasf", "adfag");
        switch (mode) { 
            case 0:
                if (order == 0)
                {
                    for (int i = 0; i < total.Length; i++)
                    {
                        list[i] = total[i];
                    }
                }
                else {
                    for (int i = 0; i < total.Length; i++)
                    {
                        list[i] = total[total.Length - i - 1 ];//역순으로 정렬
                    }
                }
                break;
            case 1:

                if (order == 0)
                {
                    List<GameObject> sortList = new List<GameObject>();
                    List<AgentModel> modelList = new List<AgentModel>();
                    foreach (GameObject child in total) {
                        sortList.Add(child);
                        AgentModel model = child.GetComponent<AgentSlotScript>().model;
                        modelList.Add(model);
                    }

                    modelList.Sort(AgentModel.CompareByName);
                    foreach (GameObject child in total) { 
                        
                    }
                    
                    
                }
                else { 
                    
                }

                break;
            case 2:

                break;
            case 3:

                break;
        }
        

        foreach (GameObject child in total)
        {
            RectTransform tr = child.GetComponent<RectTransform>();
            AgentSlotScript script = child.GetComponent<AgentSlotScript>();

            script.index = cnt;
            if (cnt % 2 == 0)
            {
                script.PanelImage.sprite = ResourceCache.instance.GetSprite("UIResource/Collection/Semi");
            }
            else
            {
                script.PanelImage.sprite = ResourceCache.instance.GetSprite("UIResource/Collection/Dark");
            }

            float size;
            tr.localPosition = new Vector3(0.0f, posy, 0);
            script.model.calcLevel();
            if (this.extended == cnt)
            {
                if (this.setState)
                {
                    agentInformation.gameObject.SetActive(true);
                    agentInformation.GetComponent<AgentExtendedScript>().SetValue(script);
                    //infoSlot.GetComponent<InfoSlotScript>().SelectedAgent(script);
                }
                else
                {

                    agentInformation.gameObject.SetActive(false);
                }
                size = script.GetSize() * 3;
                agentInformation.localPosition = new Vector3(0.0f, posy - tr.rect.height * 2, 0);
            }
            else
                size = script.GetSize();

            posy -= size;

            cnt++;
        }

        temp = posy;
        return temp;
    }
    */
    public void CancelSefiraAgent(AgentModel unit)
    {

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
        if(unit.activated)
            AgentManager.instance.deactivateAgent(unit);
    }

    public void SetAgentSefriaButton(AgentModel unit)
    {      
        bool agentExist = false;
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
        unit.GetPortrait("body", null);
       // AgentLayer.currentLayer.GetAgent(unit.instanceId).ChangeAgentUniform();
        //ShowAgentList();
        
   }

    public void SetCurrentSefria(string areaName)
    {

        currentSefriaUi = areaName;

        SefiraAgentSlot.instance.ShowAgentSefira(currentSefriaUi);
        openText.gameObject.SetActive(false);   
        //ShowAgentList();
        listScript.ShowAgentListWithChange();
        if(areaName.Equals("0")){
            agentSlot.gameObject.SetActive(false);
            OpenSefira.gameObject.SetActive(false);
            
            return;
        }


        if (PlayerModel.instance.IsOpenedArea(currentSefriaUi))
        {
            agentSlot.gameObject.SetActive(true);
            OpenSefira.gameObject.SetActive(false);
            openText.gameObject.SetActive(false);
        }
         else
        {
            OpenSefira.gameObject.SetActive(true);
            agentSlot.gameObject.SetActive(false);
            openText.gameObject.SetActive(true);
            openText.GetChild(0).GetComponent<Text>().text = areaCost + "";
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

                OpenSefira.gameObject.SetActive(false);
                agentSlot.gameObject.SetActive(true);

                UpdateSefiraButton(currentSefriaUi);
                listScript.ShowAgentListWithChange();
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

    public void PromotionAgent(AgentModel agent, int level, UnityEngine.UI.Button button)
    {

        int level2Cost = 2;
        int level3Cost = 5;
        Debug.Log("이거부름");
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
                //ShowAgentList();
                listScript.findListSlotScript(agent).SetChange();
                if (listScript.extended) {
                    listScript.SetExtended();
                }
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
                //ShowAgentList();
                listScript.findListSlotScript(agent).SetChange();
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

    public void PromoteAgent(AgentModel agent , Button button) {
        int index = agent.level - 1;
        if (EnergyModel.instance.GetLeftEnergy() < levelCost[index])
        {
            Debug.Log("Not enough energy");
            return;
        }
        else { 
            EnergyModel.instance.SetLeftEnergy(EnergyModel.instance.GetLeftEnergy() - levelCost[index]);
        }

        TraitTypeInfo[] addList = TraitTypeList.instance.GetTrait(agent);

        agent.level = agent.level + 1;
        foreach (TraitTypeInfo t in addList) {
            agent.applyTrait(t);
        }
        agent.calcLevel();
        button.gameObject.SetActive(false);
        ListSlotScript listSlot = listScript.findListSlotScript(agent);
        listSlot.SetChange();
       
        if (listScript.extended)
        {
            listScript.SetExtended();
        }
        if (agent.level < 5) button.gameObject.SetActive(true);
    }

    // ok btn
    public void Close()
    {
        opened = false;
        //canvas.gameObject.SetActive(false);

        commonStageUi.gameObject.SetActive(false);
        CommonBg.gameObject.SetActive(false);
        if (currentType == UIType.START_STAGE)
        {
            startStageUi.gameObject.SetActive(false);
            
            GameManager.currentGameManager.StartGame();
        }
        else if (currentType == UIType.END_STAGE)
        {
            endStageUi.gameObject.SetActive(false);
            GameManager.currentGameManager.ExitStage();
            OnExitScene();
        }

        SefiaOpenedCheck.SetSefira();

    }

    public void Open(UIType uiType)
    {
        opened = true;
        currentType = uiType;
        commonStageUi.gameObject.SetActive(true);
        CommonBg.gameObject.SetActive(true);

        Debug.Log(currentType);

        if (currentType == UIType.START_STAGE)
        {
            startStageUi.gameObject.SetActive(true);

        }
        else if (currentType == UIType.END_STAGE)
        {
            endStageUi.gameObject.SetActive(true);
        }
            //.gameObject.SetActive(true);
        Init();
        
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

    public void OnExitScene() {
        if (nextScene) return;
        nextScene = true;
        //StartCoroutine(StoryScneLoader());
        //StoryScneLoader();
        End();
    }

    System.Collections.IEnumerator End() {
        Debug.Log("끝");
        
        //GameManager.currentGameManager.ExitStage();
        AsyncOperation async = Application.LoadLevelAsync("StorySceneTemp");


        return null;
    }
}

namespace UISpace
{
    
    public class StartUIManager
    {
        private static StartUIManager _instance = null;
        public static StartUIManager instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new StartUIManager();
                }

                return _instance;
            }
        }

    }

    public class EndUIManager
    {
        private static EndUIManager _instance = null;
        public static EndUIManager instance
        {
            get
            {
                if (_instance == null)
                    _instance = new EndUIManager();
                return _instance;
            }
        }
    }

    public class CommonUIManager
    {
        private static CommonUIManager _instance = null;
        public static CommonUIManager instance
        {
            get
            {
                if (_instance == null)
                    _instance = new CommonUIManager();
                return _instance;
            }
        }

    }
}