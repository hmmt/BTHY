using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
/*
public class StartStageUI : MonoBehaviour, IObserver {
    
    public UnityEngine.UI.Text AgentCountNum;

    public int agentHireCost;
    public int areaOpenCost;

    private static StartStageUI _instance=null;
    public static StartStageUI instance
    {
        get
        {
            return _instance;
        }
    }

    public AreaButton[] areaButtons;
    public Image[] areaButtonImage;

    private Dictionary<string, AreaButton> areaBtnDic;
    private bool opened;

    public Transform agentScrollTarget;

    public string currentSefiraUi = "1";//SefiraObject로?
    public Button OpenSefira;

    public GameObject agentSlot;

    public RectTransform addButton;
    public RectTransform openText;
    public RectTransform agentInfo;//agent information(extension) panel
    public RectTransform infoSlot;//Center panel

    public AgentListScript listScript;

    private float scrollSizeScale;
    private float initialYPos;
    private float scrollInitialY;

    public int extended = -1;
    private int sortMode = 0;

    public bool setState = false;//info extension panel exist in display?
    private bool nextScene = false;

    void Awake()
    {
        _instance = this;
        agentHireCost = 1;
        areaOpenCost = 10;
        OpenSefira.gameObject.SetActive(false);
        openText.gameObject.SetActive(false);
        areaBtnDic = new Dictionary<string, AreaButton>();
        nextScene = false;
        foreach (AreaButton btn in areaButtons) {
            areaBtnDic.Add(btn.name, btn);
        }
    }

    void OnEnable() {
        Notice.instance.Observe(NoticeName.AreaOpenUpdate, this);
    }

    void OnDisable() {
        Notice.instance.Remove(NoticeName.AreaOpenUpdate, this);
    }

    private void Init() {
        PlayerModel playerModel = PlayerModel.instance;

        GameObject sliderPanel = GameObject.FindWithTag("EnergyPanel");
        float MAxValueForEnergy = EnergyModel.instance.GetLeftEnergy();
        sliderPanel.GetComponent<MenuLeftEnergy>().SetSlider(MAxValueForEnergy);
        currentSefiraUi = "0";

        UpdateSefiraButton("1");

        scrollSizeScale = agentScrollTarget.GetComponent<RectTransform>().rect.height;

        initialYPos = scrollSizeScale / 2;
        scrollInitialY = agentScrollTarget.localPosition.y;

        agentInfo.gameObject.SetActive(false);
        agentSlot.gameObject.SetActive(false);
        listScript.ShowAgentListWithChange();
    }

    public void OnUpdateOpenedArea(string name) {
        if (!opened) return;

        AreaButton btn;
        if (areaBtnDic.TryGetValue(name, out btn) == false) {
            Debug.Log("Not exist button :" + name);
            return;
        }

    }

    public void UpdateSefiraButton(string sefira) {
        int sefiraNum = int.Parse(sefira);
        string sefiraName = "";
        sefiraName = SefiraManager.instance.getSefira(sefiraNum).name;

        if (PlayerModel.instance.IsOpenedArea(sefira))
            areaButtonImage[sefiraNum - 1].sprite = Resources.Load<Sprite>("Sprites/UI/StageUI/" + sefiraName + "_On");
        else
            areaButtonImage[sefiraNum - 1].sprite = Resources.Load<Sprite>("Sprites/UI/StageUI/" + sefiraName + "_Off");
    }

    private void UpdateButton(AreaButton btn) {
        btn.on.gameObject.SetActive(true);
        btn.off.gameObject.SetActive(false);

        if (PlayerModel.instance.IsOpenedArea(btn.name))
        {
            btn.on.gameObject.SetActive(false);
            btn.off.gameObject.SetActive(true);
        }
        else {
            btn.on.gameObject.SetActive(true);
            btn.off.gameObject.SetActive(false);

        }
    }

    public void OnClickAddAgent() {
        int currentAgentCount = (AgentManager.instance.GetAgentList().Length + AgentManager.instance.agentListSpare.Count);

        if ((EnergyModel.instance.GetLeftEnergy() >= agentHireCost)
            && (AgentManager.instance.agentCount > currentAgentCount))
        {
            EnergyModel.instance.SetLeftEnergy(EnergyModel.instance.GetLeftEnergy() - agentHireCost);
            AgentManager.instance.BuyAgent();
            ShowAgentCount();
            SefiraAgentSlot.instance.ShowAgentSefira(currentSefiraUi);
            listScript.ShowAgentList();
        }
        else Debug.Log("Not enough Energy or supply limit exeded");
    }

    public void CancelSefiraAgent(AgentModel unit) {
        string agentSefira = unit.currentSefira;

        switch(agentSefira){
            case "1":
                AgentManager.instance.malkuthAgentList.Remove(unit);
                break;
            case "2":
                AgentManager.instance.nezzachAgentList.Remove(unit);
                break;
            case "3":
                AgentManager.instance.hodAgentList.Remove(unit);
                break;
            case "4":
                AgentManager.instance.yesodAgentList.Remove(unit);
                break;
        }
        if (unit.activated) AgentManager.instance.deactivateAgent(unit);
    }

    public void SetAgentSefiraButton(AgentModel unit) {
        bool agentExist = false;
        CancelSefiraAgent(unit);

        if (PlayerModel.instance.IsOpenedArea(currentSefiraUi)) return;

        switch (currentSefiraUi) { 
            case "1":
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
                    unit.GetMovableNode().SetCurrentNode(MapGraph.instance.GetSepiraNodeByRandom(currentSefiraUi));
                    unit.SetCurrentSefira(currentSefiraUi);
                    if (!unit.activated)
                        AgentManager.instance.activateAgent(unit, currentSefiraUi);
                }
                else
                    Debug.Log("이미 추가한 직원");
                break;
            case "2":
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
                    unit.GetMovableNode().SetCurrentNode(MapGraph.instance.GetSepiraNodeByRandom(currentSefiraUi));
                    unit.SetCurrentSefira(currentSefiraUi);
                    if (!unit.activated)
                        AgentManager.instance.activateAgent(unit, currentSefiraUi);
                }
                else
                    Debug.Log("이미 추가한 직원");
                break;
            case "3":
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
                    unit.GetMovableNode().SetCurrentNode(MapGraph.instance.GetSepiraNodeByRandom(currentSefiraUi));
                    unit.SetCurrentSefira(currentSefiraUi);
                    if (!unit.activated)
                        AgentManager.instance.activateAgent(unit, currentSefiraUi);
                }
                else
                    Debug.Log("이미 추가한 직원");
                break;
            case "4":
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
                    unit.GetMovableNode().SetCurrentNode(MapGraph.instance.GetSepiraNodeByRandom(currentSefiraUi));
                    unit.SetCurrentSefira(currentSefiraUi);
                    if (!unit.activated)
                        AgentManager.instance.activateAgent(unit, currentSefiraUi);
                }
                else
                    Debug.Log("이미 추가한 직원");
                break;
        }
        SefiraAgentSlot.instance.ShowAgentSefira(currentSefiraUi);
        unit.GetPortrait("body", null);
    }

    public void ShowAgentCount()
    {
        int length = AgentManager.instance.GetAgentList().Length + AgentManager.instance.agentListSpare.Count;
        int count = AgentManager.instance.agentCount;
        AgentCountNum.text = length + " / " + count;
    }

    public void SetCurrentSefira(string areaName) {
        currentSefiraUi = areaName;

        SefiraAgentSlot.instance.ShowAgentSefira(currentSefiraUi);
        openText.gameObject.SetActive(false);
        listScript.ShowAgentListWithChange();
        if (areaName.Equals("0")) {
            agentSlot.gameObject.SetActive(false);
            OpenSefira.gameObject.SetActive(false);
            return;
        }

        if (PlayerModel.instance.IsOpenedArea(currentSefiraUi)) {
            agentSlot.gameObject.SetActive(true);
            OpenSefira.gameObject.SetActive(false);
            openText.gameObject.SetActive(false);
        }
        else
        {
            OpenSefira.gameObject.SetActive(true);
            agentSlot.gameObject.SetActive(false);
            openText.gameObject.SetActive(true);
            openText.GetChild(0).GetComponent<Text>().text = areaOpenCost + "";
        }
    }

    public void OnClickBuyArea() {
        if (EnergyModel.instance.GetLeftEnergy() >= areaOpenCost)
        {
            EnergyModel.instance.SetLeftEnergy(EnergyModel.instance.GetLeftEnergy() - areaOpenCost);
            PlayerModel.instance.OpenArea(currentSefiraUi);

            AgentManager.instance.agentCount += 5;
            StartStageUI.instance.ShowAgentCount();

            OpenSefira.gameObject.SetActive(false);
            agentSlot.gameObject.SetActive(true);

            UpdateSefiraButton(currentSefiraUi);
            listScript.ShowAgentListWithChange();
        }
        else
            Debug.Log("에너지가 모자라");
    }



    void IObserver.OnNotice(string notice, params object[] param)
    {
        throw new System.NotImplementedException();
    }
    

}
*/

public class StartStageUI : MonoBehaviour {

    public UnityEngine.UI.Text AgentCountNum;

    private static StartStageUI _instance=null;

    public static StartStageUI instance
    {
        get
        {
            return _instance;
        }
    }

    void Awake()
    {
        _instance = this;
    }

    public void ShowAgentCount()
    {
        int length = AgentManager.instance.GetAgentList().Length + AgentManager.instance.agentListSpare.Count;
        int count = AgentManager.instance.agentCount;
        AgentCountNum.text = length + " / " + count;
    }
}

