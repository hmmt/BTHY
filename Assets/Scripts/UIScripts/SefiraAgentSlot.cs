using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.EventSystems;

public class SefiraAgentSlot : MonoBehaviour {
    /*
    public  List<AgentModel> MalkuthAgentList = new List<AgentModel>();
    public  List<AgentModel> HodAgentList = new List<AgentModel>();
    public  List<AgentModel> NezzachAgentList = new List<AgentModel>();
    public  List<AgentModel> YesodAgentList = new List<AgentModel>();
    */
    public SetAgentSefira[] slot = new SetAgentSefira[5];

    public Sprite[] bgImage;

    public  string currentSefira;

    public static SefiraAgentSlot _instance=null;

    public static SefiraAgentSlot instance
    {
        get
        {
            return _instance;
        }
    }
    void Awake()
    {
        //Debug.Log(gameObject);
        _instance = this;
        Init();
    }

    public void Init() {

    }

    public void CancelSefiraAgent(AgentModel unit, int index)
    {
        Debug.Log(index);
        ListSlotScript script = AgentListScript.instance.findListSlotScript(unit);
        AgentManager._instance.deactivateAgent(unit);
        ShowAgentSefira(StageUI.instance.currentSefriaUi);
        script.SetChange();
        
    }

    public  void ShowAgentSefira(string sefira)
    {
        Sefira targetSefira = SefiraManager.instance.getSefira(sefira);
        if (targetSefira == null) {
            EmptySefira();
            return;
        }
        /*
        foreach (AgentModel am in targetSefira.agentList) {
            Debug.Log(am.name);
        }
        */
        SetSefira(targetSefira.agentList);
    }

    public void EmptySefira() {
        for (int i = 4; i >= 0; i--)
        {
            slot[i].Bg.sprite = bgImage[0];
			/*
            slot[i].agentBody.sprite = ResourceCache.instance.GetSprite("Sprites/Agent/AgentNone");
            slot[i].agentFace.sprite = ResourceCache.instance.GetSprite("Sprites/Agent/AgentNone");
            slot[i].agentHair.sprite = ResourceCache.instance.GetSprite("Sprites/Agent/AgentNone");
            */
            slot[i].agentLevel.text = "없음";
            slot[i].agentName.text = "없음";
            slot[i].cancelButton.gameObject.SetActive(false);
            slot[i].Cancel.gameObject.SetActive(false);
            slot[i].bodyObject.gameObject.SetActive(false);
            slot[i].Model = null;
        }
    }

    public void SetSefira(List<AgentModel> model) {
        for (int i = 4; i >= model.Count; i--)
        {
            slot[i].Bg.sprite = bgImage[0];
			/*
            slot[i].agentBody.sprite = ResourceCache.instance.GetSprite("Sprites/Agent/AgentNone");
            slot[i].agentFace.sprite = ResourceCache.instance.GetSprite("Sprites/Agent/AgentNone");
            slot[i].agentHair.sprite = ResourceCache.instance.GetSprite("Sprites/Agent/AgentNone");
            */
            slot[i].agentLevel.text = "없음";
            slot[i].agentName.text = "없음";
            slot[i].cancelButton.gameObject.SetActive(false);
            slot[i].Cancel.gameObject.SetActive(false);
            slot[i].bodyObject.gameObject.SetActive(false);
            slot[i].Model = null;
        }

        for (int i = 0; i < model.Count; i++)
        {
            int copied = i;
            //Debug.Log(copied);
            AgentModel agentModel = model[copied];

            agentModel.GetPortrait("body", null);
            slot[copied].Bg.sprite = bgImage[1];
            slot[copied].agentBody.sprite = slot[copied].headImg;
            slot[copied].agentFace.sprite = agentModel.tempFaceSprite;
            slot[copied].agentHair.sprite = agentModel.tempHairSprite;
            slot[copied].Model = agentModel;

            slot[copied].agentLevel.text = "" + agentModel.level;
            slot[copied].agentName.text = "" + agentModel.name;
            slot[copied].Cancel.gameObject.SetActive(true);
            slot[copied].bodyObject.gameObject.SetActive(true);
            /*
            slot[i].cancelButton.gameObject.SetActive(true);
            slot[i].cancelButton.onClick.RemoveAllListeners();
            slot[i].cancelButton.onClick.AddListener(() => CancelSefiraAgent(agentModel, copied));
             */
        }
        
    }

}
