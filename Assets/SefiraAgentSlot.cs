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
        Debug.Log(gameObject);
        _instance = this;
    }

    public void CancelSefiraAgent(AgentModel unit, int index)
    {
        /*
        if (unit.currentSefira.Equals("1"))
        {
            MalkuthAgentList.RemoveAt(index);
        }

        else if (unit.currentSefira.Equals("2"))
        {
            NezzachAgentList.RemoveAt(index);
        }

        else if (unit.currentSefira.Equals("3"))
        {

            HodAgentList.RemoveAt(index);
        }

        else if (unit.currentSefira.Equals("4"))
        {
            YesodAgentList.RemoveAt(index);
        }
        */

        AgentManager._instance.deactivateAgent(unit);
        ShowAgentSefira(StageUI.instance.currentSefriaUi);
    }

    public  void ShowAgentSefira(string sefria)
    {
        if (sefria == "1")
        {

            for (int i = 4; i >= AgentManager.instance.malkuthAgentList.Count; i--)
            {
                slot[i].agentBody.sprite = ResourceCache.instance.GetSprite("Sprites/Agent/AgentNone");
                slot[i].agentFace.sprite = ResourceCache.instance.GetSprite("Sprites/Agent/AgentNone");
                slot[i].agentHair.sprite = ResourceCache.instance.GetSprite("Sprites/Agent/AgentNone");
                slot[i].agentLevel.text = "없음";
                slot[i].agentName.text = "없음";
                slot[i].cancelButton.gameObject.SetActive(false);
              }

            for (int i = 0; i < AgentManager.instance.malkuthAgentList.Count; i++)
            {
                Debug.Log("Malcute");
                int copied = i;
                AgentModel agentModel = AgentManager.instance.malkuthAgentList[i];

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

        else if (sefria == "2")
        {

            for (int i = 4; i >= AgentManager.instance.nezzachAgentList.Count; i--)
            {
                slot[i].agentBody.sprite = ResourceCache.instance.GetSprite("Sprites/Agent/AgentNone");
                slot[i].agentFace.sprite = ResourceCache.instance.GetSprite("Sprites/Agent/AgentNone");
                slot[i].agentHair.sprite = ResourceCache.instance.GetSprite("Sprites/Agent/AgentNone");

                slot[i].agentLevel.text = "없음";
                slot[i].agentName.text = "없음";
                slot[i].cancelButton.gameObject.SetActive(false);
            }

            for (int i = 0; i < AgentManager.instance.nezzachAgentList.Count; i++)
            {
                int copied = i;
                AgentModel agentModel = AgentManager.instance.nezzachAgentList[i];

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

        else if (sefria == "3")
        {

            for (int i = 4; i >= AgentManager.instance.hodAgentList.Count; i--)
            {
                slot[i].agentBody.sprite = ResourceCache.instance.GetSprite("Sprites/Agent/AgentNone");
                slot[i].agentFace.sprite = ResourceCache.instance.GetSprite("Sprites/Agent/AgentNone");
                slot[i].agentHair.sprite = ResourceCache.instance.GetSprite("Sprites/Agent/AgentNone");
                slot[i].agentLevel.text = "없음";
                slot[i].agentName.text = "없음";
                slot[i].cancelButton.gameObject.SetActive(false);
            }

            for (int i = 0; i < AgentManager.instance.hodAgentList.Count; i++)
            {
                int copied = i;
                AgentModel agentModel = AgentManager.instance.hodAgentList[i];

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

        else if (sefria == "4")
        {
            for (int i = 4; i >= AgentManager.instance.yesodAgentList.Count; i--)
            {
                slot[i].agentBody.sprite = ResourceCache.instance.GetSprite("Sprites/Agent/AgentNone");
                slot[i].agentFace.sprite = ResourceCache.instance.GetSprite("Sprites/Agent/AgentNone");
                slot[i].agentHair.sprite = ResourceCache.instance.GetSprite("Sprites/Agent/AgentNone");
                slot[i].agentLevel.text = "없음";
                slot[i].agentName.text = "없음";
                slot[i].cancelButton.gameObject.SetActive(false);
            }

            for (int i = 0; i < AgentManager.instance.yesodAgentList.Count; i++)
            {
                int copied = i;
                AgentModel agentModel = AgentManager.instance.yesodAgentList[i];

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
}
