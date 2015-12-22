using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class SefiraAgentSlot : MonoBehaviour {
    /*
    public  List<AgentModel> MalkuthAgentList = new List<AgentModel>();
    public  List<AgentModel> HodAgentList = new List<AgentModel>();
    public  List<AgentModel> NezzachAgentList = new List<AgentModel>();
    public  List<AgentModel> YesodAgentList = new List<AgentModel>();
    */
    public SetAgentSefira[] slot = new SetAgentSefira[5];

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
    }

    public void CancelSefiraAgent(AgentModel unit, int index)
    {
        ListSlotScript script = AgentListScript.instance.findListSlotScript(unit);
        AgentManager._instance.deactivateAgent(unit);
        ShowAgentSefira(StageUI.instance.currentSefriaUi);
        script.SetChange();
        
    }

    public  void ShowAgentSefira(string sefria)
    {   
        switch (sefria) { 
            case "0":
                EmptySefira();
                break;
            case "1":
                SetSefira(AgentManager.instance.malkuthAgentList);
                break;
            case "2":
                SetSefira(AgentManager.instance.nezzachAgentList);
                break;
            case "3":
                SetSefira(AgentManager.instance.hodAgentList);
                break;
            case "4":
                SetSefira(AgentManager.instance.yesodAgentList);
                break;
            
        }
    }

    public void EmptySefira() {
        for (int i = 4; i >= 0; i--)
        {
            slot[i].agentBody.sprite = ResourceCache.instance.GetSprite("Sprites/Agent/AgentNone");
            slot[i].agentFace.sprite = ResourceCache.instance.GetSprite("Sprites/Agent/AgentNone");
            slot[i].agentHair.sprite = ResourceCache.instance.GetSprite("Sprites/Agent/AgentNone");
            slot[i].agentLevel.text = "없음";
            slot[i].agentName.text = "없음";
            slot[i].cancelButton.gameObject.SetActive(false);
        }
    }

    public void SetSefira(List<AgentModel> model) {
        for (int i = 4; i >= model.Count; i--)
        {
            slot[i].agentBody.sprite = ResourceCache.instance.GetSprite("Sprites/Agent/AgentNone");
            slot[i].agentFace.sprite = ResourceCache.instance.GetSprite("Sprites/Agent/AgentNone");
            slot[i].agentHair.sprite = ResourceCache.instance.GetSprite("Sprites/Agent/AgentNone");
            slot[i].agentLevel.text = "없음";
            slot[i].agentName.text = "없음";
            slot[i].cancelButton.gameObject.SetActive(false);
        }

        for (int i = 0; i < model.Count; i++)
        {
            int copied = i;
            AgentModel agentModel = model[i];

            agentModel.AgentPortrait("body", null);

            slot[i].agentBody.sprite = ResourceCache.instance.GetSprite(agentModel.bodyImgSrc);
            slot[i].agentFace.sprite = ResourceCache.instance.GetSprite(agentModel.faceImgSrc);
            slot[i].agentHair.sprite = ResourceCache.instance.GetSprite(agentModel.hairImgSrc);

            slot[i].agentLevel.text = "" + agentModel.level;
            slot[i].agentName.text = "" + agentModel.name;
            slot[i].cancelButton.gameObject.SetActive(true);
            slot[i].cancelButton.onClick.RemoveAllListeners();
            slot[i].cancelButton.onClick.AddListener(() => CancelSefiraAgent(agentModel, copied));
        }
    
    }
}
