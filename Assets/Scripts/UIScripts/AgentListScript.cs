using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class AgentListScript : MonoBehaviour {
    private static AgentListScript _instance;
    public static AgentListScript instance {
        get {
            return _instance; 
        }
    }
    
    public GameObject SlotObject;
    public RectTransform agentScrollTarget;
    public RectTransform addbutton;
    public RectTransform PromotionPanel;
    public RectTransform infoPanel;

    public int mode, order;
    public float spacing;

    private PromotionPanelScript promoteScript;
    private RectTransform agentListforScroll;
    public RectTransform reference;
    private float initialPosy;
    private int lastIndex = 0;
    private float extendSize = 0.0f;
    private AgentExtendedScript infoScript;
    private ListSlotScript infoDisplayedAgent;

    private static List<GameObject> agentList = new List<GameObject>();
    private static List<AgentModel> modelList = new List<AgentModel>();

    private int extendedIndex = -1;
    public bool extended = false;
    private ListSlotScript extendedScript;

    public void Awake() {
        _instance = this;
        promoteScript = PromotionPanel.GetComponent<PromotionPanelScript>();
        infoScript = infoPanel.GetComponent<AgentExtendedScript>();

        initialPosy = agentScrollTarget.localPosition.y;
        agentListforScroll = reference;
       // agentListforScroll = (RectTransform)Instantiate(agentScrollTarget);
        mode = 0;
        order = 0;
    }
    /*
    public void Start()
    {
        Debug.Log(StageUI.instance.getCurrnetType());
        addbutton.gameObject.SetActive(false);
        if (StageUI.instance.getCurrnetType().Equals(StageUI.UIType.START_STAGE))
        {
            addbutton.gameObject.SetActive(true);
        }
    }

    public void OnDisable()
    {

    }
    */
    public void Init() {
        
        foreach (RectTransform rt in agentScrollTarget) {
            if(rt.tag.Equals("AgentListSlot"))  Destroy(rt.gameObject);
        }
        agentList.Clear();
        foreach (AgentModel mo in modelList)
        {
            Debug.Log(mo.name);
            SetExistAgentList(mo);
        }
        //agentScrollTarget.sizeDelta = agentListforScroll.sizeDelta;
        ShowAgentList();
    }
    
    public void SetExtended(ListSlotScript script) {
        if (extended) {
            extendedScript.state = false;
        }
        this.infoDisplayedAgent = script;
        extended = true;
        extendedIndex = script.index;
        extendedScript = script;
        extendedScript.state = true;
        extendSize = script.GetHeight() * 2;
        infoPanel.gameObject.SetActive(true);
        
        infoScript.SetValue(script.getModel());
        infoPanel.localPosition = new Vector3(infoPanel.localPosition.x,
                                             -script.GetHeight() * (extendedIndex + 1), 0.0f);
        ShowAgentList();
        /*
        agentScrollTarget.localPosition = new Vector3(agentScrollTarget.localPosition.x,
                                                      script.GetHeight() * script.index,
                                                      0.0f);
        */
    }

    private void SetExtendLast() {
        infoPanel.transform.SetAsLastSibling();
    }

    public void SetExtended() {
        this.SetExtended(infoDisplayedAgent);
    }

    public void SetExtendedDisabled() {
        this.infoDisplayedAgent = null;
        extendSize = 0.0f;
        extended = false;
        extendedIndex = -1;
        extendedScript.state = false;
        extendedScript = null;
        infoPanel.gameObject.SetActive(false);
        ShowAgentList();
    }

    public ListSlotScript findListSlotScript(AgentModel model) {
        ListSlotScript script;
        foreach (GameObject child in agentList) {
            script = child.GetComponent<ListSlotScript>();
            if (script.getModel().Equals(model)) {
                return script;
            }
        }
        return null;
    }

    public GameObject findObjectSlotByModel(AgentModel model) {
        GameObject obj;
        foreach (GameObject child in agentList) {
            if (child.GetComponent<ListSlotScript>().getModel().Equals(model)) {
                obj = child;
                return obj;
            }
        }
        return null;
    }

    public GameObject findObjectSlot(ListSlotScript script) {
        GameObject obj;
        foreach (GameObject child in agentList) {
            if (child.GetComponent<ListSlotScript>().Equals(script)) {
                obj = child;
                return obj;
            }
        }
        return null;
    }

    public void ShowAgentList() {
        float posy = 0.0f;
        for (int i = 0; i < modelList.Count; i++) {
            GameObject slot = findObjectSlotByModel(modelList[i]);
            RectTransform rt = slot.GetComponent<RectTransform>();
            ListSlotScript script = slot.GetComponent<ListSlotScript>();

            rt.localPosition = new Vector3(agentListforScroll.localPosition.x, -posy, 0.0f);
            script.getModel().calcLevel();
            
            if (script.Equals(extendedScript))
            {
                posy += script.GetHeight();
                infoPanel.localPosition = new Vector3(0.0f, -posy, 0.0f);
                posy += extendSize+spacing;
            }
            else {
                posy += (script.GetHeight() + spacing);
            }
        }
        
        if (posy + addbutton.rect.height > agentListforScroll.rect.height)
        {
            agentScrollTarget.sizeDelta = new Vector2(agentScrollTarget.sizeDelta.x, posy + addbutton.rect.height);
        }
        addbutton.localPosition = new Vector3(0.0f, -posy, 0.0f);


        //setScrollPos();
        SetExtendLast();
    }
    /*
    public void ShowAgentList() {
        float cumulative = 0.0f;
        float additional = 0.0f;
        Debug.Log("Extended: " + extendedIndex);
        int childIndex = 0;
        bool found = false;
        foreach (GameObject child in agentList) {
            RectTransform rt = child.GetComponent<RectTransform>();
            ListSlotScript script = child.GetComponent<ListSlotScript>();

            int index = modelList.FindIndex(x => x == script.getModel());
            script.index = index;

            Debug.Log(index + " " + childIndex);
            float posy = script.GetHeight() * index;
            cumulative += script.GetHeight();
            
            if (extended) {
                /*
                if (index.Equals(extendedIndex)) {
                    infoPanel.localPosition = new Vector3(infoPanel.localPosition.x,
                                             -script.GetHeight() * (index + 1), 0.0f);
                }
                if (index > extendedIndex) {
                    posy += script.GetHeight() * 2;
                }
            }
            if (extended)
            {
                if (index.Equals(extendedIndex))
                {
                    Debug.Log("Found");
                    found = true;
                    
                }
                else if (found)
                {
                    posy += script.GetHeight() * 2;
                }
            }
            rt.localPosition = new Vector3(0.0f, -posy, 0.0f);
            script.getModel().calcLevel();
            childIndex++;
        }
        
        if (extended) {

            additional = extendedScript.GetHeight() * 2;
            cumulative += additional;
        }

       if (cumulative + addbutton.rect.height > agentListforScroll.rect.height) {
            agentScrollTarget.sizeDelta = new Vector2(agentScrollTarget.sizeDelta.x, cumulative + addbutton.rect.height);
        }
       addbutton.localPosition = new Vector3(0.0f, -cumulative, 0.0f);

       
       setScrollPos();
    }
      */

    public void ShowAgentListWithChange()
    {
        float cumulative = 0.0f;
        float additional = 0.0f;

        int childIndex = 0;
        bool found = false;
        foreach (GameObject child in agentList)
        {
            RectTransform rt = child.GetComponent<RectTransform>();
            ListSlotScript script = child.GetComponent<ListSlotScript>();
            script.SetChange();

            int index = modelList.FindIndex(x => x == script.getModel());
            script.index = index;
            float posy = (script.GetHeight()+spacing) * index;
            cumulative += (script.GetHeight() +spacing);
            if (extended)
            {
                if (index.Equals(extendedIndex)) {
                    //Debug.Log("Found");
                    found = true;
                    
                }else if (found)
                {
                    posy += script.GetHeight() *2 + spacing;
                }
            }
            //rt.localPosition = new Vector3(0.0f, -posy, 0.0f);
            rt.localPosition = new Vector3(agentListforScroll.localPosition.x, -posy, 0.0f);
            script.getModel().calcLevel();
            childIndex++;
        }
        if (extended) {
            
            additional = (extendedScript.GetHeight() * 2) +spacing;
            cumulative += additional;
        }
        
        if (cumulative + addbutton.rect.height > agentListforScroll.rect.height)    
        {
            agentScrollTarget.sizeDelta = new Vector2(agentScrollTarget.sizeDelta.x, cumulative + addbutton.rect.height);
        }
        addbutton.localPosition = new Vector3(0.0f, -cumulative, 0.0f);

        //setScrollPos();
        SetExtendLast();
    }

    private void setScrollPos() {
        float move, std;
        move = agentScrollTarget.rect.height;
        std = agentListforScroll.rect.height;
        if (move > std) {
            float heightForMove = move - std;
            agentScrollTarget.anchoredPosition = new Vector2(agentScrollTarget.anchoredPosition.x,
                                                            initialPosy + heightForMove);
        }

    }

    private void setScrollPos(float posy) {
        agentScrollTarget.anchoredPosition = new Vector2(agentScrollTarget.anchoredPosition.x,
                                                        posy);
    }

    public void ChangeOrder(int order) {
        this.order = order;
    }

    /*
     *  직원들을 정렬한다
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
    public void SortAgentList(int mode) {
        
        if (this.mode == mode) {
            InversteList(modelList);
            ShowAgentList();
            setScrollPos();
            return ;
        }
        this.mode = mode;
        switch (mode) { 
            case 0:
                modelList.Sort(AgentModel.CompareByID);
                break;
            case 1:
                modelList.Sort(AgentModel.CompareByName);
                break;
            case 2:
                modelList.Sort(AgentModel.CompareByLevel);
                break;
            case 3:
                modelList.Sort(AgentModel.CompareBySefira);
                break;
            case 4:
                modelList.Sort(AgentModel.CompareByLifestyle);
                break;
        }
        
        ShowAgentList();
        setScrollPos();
    }

    private void InversteList(List<AgentModel> list) {
        AgentModel[] tempList = new AgentModel[list.Count];
        list.CopyTo(tempList);
        list.Clear();
        for (int i = tempList.Length; i > 0; i--)
        {
            modelList.Add(tempList[i - 1]);
        }
    }
    /*
    public void SetAgentList()
    {

        
        foreach (AgentModel am in AgentManager.instance.agentAdded)
        {
            modelList.Add(am);
            GameObject inst = Instantiate(SlotObject);
            ListSlotScript script = inst.GetComponent<ListSlotScript>();
            script.Init(am);
            inst.transform.SetParent(agentScrollTarget);
            agentList.Add(inst);
            AgentManager.instance.agentAdded.Dequeue();
        }
        

    }
    */
    public void SetAgentList(AgentModel am) {
        modelList.Add(am);
        GameObject inst = Instantiate(SlotObject);
        RectTransform instTr = inst.GetComponent<RectTransform>();
        ListSlotScript script = inst.GetComponent<ListSlotScript>();
        script.Init(am);
        inst.transform.SetParent(agentScrollTarget);
        inst.transform.localScale = Vector3.one;
        agentList.Add(inst);
        script.ui.allocate.onClick.AddListener(() => script.OnAllocateClick());
        //script.ui.allocate.onClick.AddListener(() => StageUI.instance.SetAgentSefriaButton(am));
    }

    private void SetExistAgentList(AgentModel am) {
        GameObject inst = Instantiate(SlotObject);
        RectTransform instTr = inst.GetComponent<RectTransform>();
        ListSlotScript script = inst.GetComponent<ListSlotScript>();
        script.Init(am);
        inst.transform.SetParent(agentScrollTarget);
        inst.transform.localScale = Vector3.one;
        agentList.Add(inst);
        script.ui.allocate.onClick.AddListener(() => script.OnAllocateClick());
    }

    public void ActivatePromotionPanel(ListSlotScript script) {
        if (extended == true) {
            SetExtendedDisabled();
        }

        SetPromotionEnable(false);

        promoteScript.Deactivate();
        GameObject target = findObjectSlot(script);
        //Vector3 pos = target.transform.localPosition;
        promoteScript.SetTarget(target);
        promoteScript.UpdatePanel(script);
        target.gameObject.SetActive(false);
        PromotionPanel.gameObject.SetActive(true);
        //PromotionPanel.localPosition = pos;
        
    }

    public void SetPromotionEnable(bool state) {
        this.addbutton.gameObject.SetActive(state);

        for (int i = 0; i < modelList.Count; i++) {
            GameObject slot = findObjectSlotByModel(modelList[i]);
            slot.gameObject.SetActive(state);
        }

    }

    public void SetAddbuttonState(bool b) {
        this.addbutton.gameObject.SetActive(b);
    }

}
