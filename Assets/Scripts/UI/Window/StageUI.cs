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

        UpdateButton(btn);
    }

    private void UpdateButton(AreaButton btn)
    {
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
        if (EnergyModel.instance.GetLeftEnergy() >= agentCost)
        {
            EnergyModel.instance.SetLeftEnergy(EnergyModel.instance.GetLeftEnergy() - agentCost);

            long[] idList = PlayerModel.instance.GetAvailableAgentList();

            long selected = idList[Random.Range(0, idList.Length)];

            AgentManager.instance.BuyAgent(selected);

            ShowAgentList();
        }
        else
            Debug.Log("에너지가 모자라");
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

            slotPanel.nameText.text = unit.name;
            slotPanel.HPText.text = "HP : " + unit.hp + "/" + unit.maxHp;

            Texture2D tex = Resources.Load<Texture2D>("Sprites/" + unit.imgsrc);
            slotPanel.agentIcon.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));

            posy -= 100f;
        }

        // scroll rect size
        Vector2 scrollRectSize = agentScrollTarget.GetComponent<RectTransform>().sizeDelta;
        scrollRectSize.y = -posy + 100f;
        agentScrollTarget.GetComponent<RectTransform>().sizeDelta = scrollRectSize;
    }

    public void OnClickBuyArea(string areaName)
    {
        if (EnergyModel.instance.GetLeftEnergy() >=areaCost)
        {
            EnergyModel.instance.SetLeftEnergy(EnergyModel.instance.GetLeftEnergy() - areaCost);
            PlayerModel.instance.OpenArea(areaName);
        }
        else
            Debug.Log("에너지가 모자라");
        //MapGraph.instance.

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
