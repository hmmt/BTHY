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

    public AreaButton[] areaButtons;


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
        PlayerModel playerModel = PlayerModel.instnace;
        foreach (KeyValuePair<string, AreaButton> v in areaBtnDic)
        {
            AreaButton btn = v.Value;
            UpdateButton(btn);
        }
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
        if (PlayerModel.instnace.IsOpenedArea(btn.name))
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

            long[] idList = PlayerModel.instnace.GetAvailableAgentList();

            long selected = idList[Random.Range(0, idList.Length)];

            AgentManager.instance.BuyAgent(selected);
        }
        else
            Debug.Log("에너지가 모자라");
    }

    public void OnClickBuyArea(string areaName)
    {
        if (EnergyModel.instance.GetLeftEnergy() >=areaCost)
        {
            EnergyModel.instance.SetLeftEnergy(EnergyModel.instance.GetLeftEnergy() - areaCost);
            PlayerModel.instnace.OpenArea(areaName);
        }
        else
            Debug.Log("에너지가 모자라");
        //MapGraph.instance.

    }



    // ok btn
    public void Close()
    {
        opened = false;
        canvas.gameObject.SetActive(false);

        if (currentType == UIType.START_STAGE)
        {
            GameManager.currentGameManager.StartGame();
        }
        else if (currentType == UIType.END_STAGE)
        {
            GameManager.currentGameManager.ExitStage();
        }
    }

    public void Open(UIType uiType)
    {
        opened = true;
        currentType = uiType;
        canvas.gameObject.SetActive(true);
        Init();
    }


    public void OnNotice(string notice, params object[] param)
    {
        if (notice == NoticeName.AreaOpenUpdate)
        {
            OnUpdateOpenedArea((string)param[0]);
        }
    }
}
