using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class SefiraAgentSlot : MonoBehaviour {

    public  List<AgentModel> MalkuthAgentList = new List<AgentModel>();
    public  List<AgentModel> HodAgentList = new List<AgentModel>();
    public  List<AgentModel> NezzachAgentList = new List<AgentModel>();
    public  List<AgentModel> YesodAgentList = new List<AgentModel>();

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
        _instance = this;
    }

    public void CancelSefiraAgent(AgentModel unit, int index)
    {
        if (unit.currentSefira.Equals("1"))
        {
            MalkuthAgentList.RemoveAt(index);
            Debug.Log("Count "+MalkuthAgentList.Count);
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

        ShowAgentSefira(StageUI.instance.currentSefriaUi);
    }

    public  void ShowAgentSefira(string sefria)
    {        
        if (sefria == "1")
        {

            for (int i = 4; i >= MalkuthAgentList.Count; i--)
            {
                slot[i].agentImage.sprite = null;
                slot[i].agentLevel.text = "없음";
                slot[i].agentName.text = "없음";
                slot[i].cancelButton.gameObject.SetActive(false);
              }

            for (int i = 0; i < MalkuthAgentList.Count; i++ )
            {
                int copied = i;
               slot[i].agentImage.sprite = Resources.Load<Sprite>("Sprites/" + MalkuthAgentList[i].imgsrc);
               slot[i].agentLevel.text = "" + MalkuthAgentList[i].level;
               slot[i].agentName.text = ""+MalkuthAgentList[i].name;
               slot[i].cancelButton.gameObject.SetActive(true);
               slot[i].cancelButton.onClick.RemoveAllListeners();
               slot[i].cancelButton.onClick.AddListener(() => CancelSefiraAgent(MalkuthAgentList[copied], copied));
            }
        }

        else if (sefria == "2")
        {

            for (int i = 4; i >= NezzachAgentList.Count; i--)
            {
                slot[i].agentImage.sprite = null;
                slot[i].agentLevel.text = "없음";
                slot[i].agentName.text = "없음";
                slot[i].cancelButton.gameObject.SetActive(false);
            }

            for (int i = 0; i < NezzachAgentList.Count; i++)
            {
                int copied = i;
                slot[i].agentImage.sprite = Resources.Load<Sprite>("Sprites/" + NezzachAgentList[i].imgsrc);
                slot[i].agentLevel.text = "" + NezzachAgentList[i].level;
                slot[i].agentName.text = "" + NezzachAgentList[i].name;
                slot[i].cancelButton.gameObject.SetActive(true);
                slot[i].cancelButton.onClick.RemoveAllListeners();
                slot[i].cancelButton.onClick.AddListener(() => CancelSefiraAgent(NezzachAgentList[copied], copied));
            }
         }

        else if (sefria == "3")
        {

            for (int i = 4; i >= HodAgentList.Count; i--)
            {
                slot[i].agentImage.sprite = null;
                slot[i].agentLevel.text = "없음";
                slot[i].agentName.text = "없음";
                slot[i].cancelButton.gameObject.SetActive(false);
            }

            for (int i = 0; i < HodAgentList.Count; i++)
            {
                int copied = i;
                slot[i].agentImage.sprite = Resources.Load<Sprite>("Sprites/" + HodAgentList[i].imgsrc);
                slot[i].agentLevel.text = "" + HodAgentList[i].level;
                slot[i].agentName.text = "" + HodAgentList[i].name;
                slot[i].cancelButton.gameObject.SetActive(true);
                slot[i].cancelButton.onClick.RemoveAllListeners();
                slot[i].cancelButton.onClick.AddListener(() => CancelSefiraAgent(HodAgentList[copied], copied));
            }
          }

        else if (sefria == "4")
        {
            for (int i = 4; i >= YesodAgentList.Count; i--)
            {
                slot[i].agentImage.sprite = null;
                slot[i].agentLevel.text = "없음";
                slot[i].agentName.text = "없음";
                slot[i].cancelButton.gameObject.SetActive(false);
            }

            for (int i = 0; i < YesodAgentList.Count; i++)
            {
                int copied = i;
                slot[i].agentImage.sprite = Resources.Load<Sprite>("Sprites/" + YesodAgentList[i].imgsrc);
                slot[i].agentLevel.text = "" + YesodAgentList[i].level;
                slot[i].agentName.text = "" + YesodAgentList[i].name;
                slot[i].cancelButton.gameObject.SetActive(true);
                slot[i].cancelButton.onClick.RemoveAllListeners();
                slot[i].cancelButton.onClick.AddListener(() => CancelSefiraAgent(YesodAgentList[copied], copied));
            }
          }
    }
}
