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

        AgentManager._instance.deactivateAgent(unit);
        ShowAgentSefira(StageUI.instance.currentSefriaUi);
    }

    public  void ShowAgentSefira(string sefria)
    {
        if (sefria == "1")
        {

            for (int i = 4; i >= MalkuthAgentList.Count; i--)
            {
                slot[i].agentBody.sprite = Resources.Load<Sprite>("Sprites/Agent/AgentNone");
                slot[i].agentFace.sprite = Resources.Load<Sprite>("Sprites/Agent/AgentNone");
                slot[i].agentHair.sprite = Resources.Load<Sprite>("Sprites/Agent/AgentNone");
                slot[i].agentLevel.text = "없음";
                slot[i].agentName.text = "없음";
                slot[i].cancelButton.gameObject.SetActive(false);
              }

            for (int i = 0; i < MalkuthAgentList.Count; i++ )
            {
                int copied = i;

                MalkuthAgentList[i].AgentPortrait("body", null);

                Texture2D tex3 = Resources.Load<Texture2D>(MalkuthAgentList[i].bodyImgSrc);
                slot[i].agentBody.sprite = Sprite.Create(tex3, new Rect(0, 0, tex3.width, tex3.height), new Vector3(0.5f, 0.5f, 0.5f));
                Texture2D tex1 = Resources.Load<Texture2D>(MalkuthAgentList[i].faceImgSrc);
                slot[i].agentFace.sprite = Sprite.Create(tex1, new Rect(0, 0, tex1.width, tex1.height), new Vector3(0.5f, 0.5f, -1f));
                Texture2D tex2 = Resources.Load<Texture2D>(MalkuthAgentList[i].hairImgSrc);
                slot[i].agentHair.sprite = Sprite.Create(tex2, new Rect(0, 0, tex2.width, tex2.height), new Vector3(0.5f, 0.5f, -1f));

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
                slot[i].agentBody.sprite = Resources.Load<Sprite>("Sprites/Agent/AgentNone");
                slot[i].agentFace.sprite = Resources.Load<Sprite>("Sprites/Agent/AgentNone");
                slot[i].agentHair.sprite = Resources.Load<Sprite>("Sprites/Agent/AgentNone");

                slot[i].agentLevel.text = "없음";
                slot[i].agentName.text = "없음";
                slot[i].cancelButton.gameObject.SetActive(false);
            }

            for (int i = 0; i < NezzachAgentList.Count; i++)
            {
                int copied = i;

                NezzachAgentList[i].AgentPortrait("body", null);

                slot[i].agentBody.sprite = Resources.Load<Sprite>(NezzachAgentList[i].bodyImgSrc);
                slot[i].agentFace.sprite = Resources.Load<Sprite>(NezzachAgentList[i].faceImgSrc);
                slot[i].agentHair.sprite = Resources.Load<Sprite>(NezzachAgentList[i].hairImgSrc);

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
                slot[i].agentBody.sprite = Resources.Load<Sprite>("Sprites/Agent/AgentNone");
                slot[i].agentFace.sprite = Resources.Load<Sprite>("Sprites/Agent/AgentNone");
                slot[i].agentHair.sprite = Resources.Load<Sprite>("Sprites/Agent/AgentNone");
              slot[i].agentLevel.text = "없음";
                slot[i].agentName.text = "없음";
                slot[i].cancelButton.gameObject.SetActive(false);
            }

            for (int i = 0; i < HodAgentList.Count; i++)
            {
                int copied = i;
                HodAgentList[i].AgentPortrait("body", null);

                slot[i].agentBody.sprite = Resources.Load<Sprite>(HodAgentList[i].bodyImgSrc);
                slot[i].agentFace.sprite = Resources.Load<Sprite>(HodAgentList[i].faceImgSrc);
                slot[i].agentHair.sprite = Resources.Load<Sprite>(HodAgentList[i].hairImgSrc);

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
                slot[i].agentBody.sprite = Resources.Load<Sprite>("Sprites/Agent/AgentNone");
                slot[i].agentFace.sprite = Resources.Load<Sprite>("Sprites/Agent/AgentNone");
                slot[i].agentHair.sprite = Resources.Load<Sprite>("Sprites/Agent/AgentNone");
                slot[i].agentLevel.text = "없음";
                slot[i].agentName.text = "없음";
                slot[i].cancelButton.gameObject.SetActive(false);
            }

            for (int i = 0; i < YesodAgentList.Count; i++)
            {
                int copied = i;

                YesodAgentList[i].AgentPortrait("body", null);

                slot[i].agentBody.sprite = Resources.Load<Sprite>(YesodAgentList[i].bodyImgSrc);
                slot[i].agentFace.sprite = Resources.Load<Sprite>(YesodAgentList[i].faceImgSrc);
                slot[i].agentHair.sprite = Resources.Load<Sprite>(YesodAgentList[i].hairImgSrc);

                slot[i].agentLevel.text = "" + YesodAgentList[i].level;
                slot[i].agentName.text = "" + YesodAgentList[i].name;
                slot[i].cancelButton.gameObject.SetActive(true);
                slot[i].cancelButton.onClick.RemoveAllListeners();
                slot[i].cancelButton.onClick.AddListener(() => CancelSefiraAgent(YesodAgentList[copied], copied));
            }
          }
    }
}
