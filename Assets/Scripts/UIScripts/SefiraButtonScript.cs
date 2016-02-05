using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SefiraButtonScript : MonoBehaviour {
    public AgentModel model;
    public Button[] sefira = new Button[10];
    private AgentListPanelScript script;
    private AgentList sc;

    public void Start() {
        sc = GameObject.FindWithTag("SefiraAgentListPanel").GetComponent<AgentList>();
    }

    public void InitButton() {
        string[] list = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" };
        bool[] isopend = new bool[10];
        for (int i = 0; i < 10; i++)
        {
            isopend[i] = PlayerModel.instance.IsOpenedArea(list[i]);
            sefira[i].gameObject.SetActive(isopend[i]);
        }
    }

    public void SetScript(AgentListPanelScript s) {
        script = s;
        model = s.model;
    }

    public void SetSefira(string sefira) {
        
        string current = model.currentSefira;

        if (sefira.Equals(current))
        {
            Debug.Log("같은 부서");
        }
        else
        {
            if (sefira == "1")
            {
                if (AgentManager.instance.malkuthAgentList.Count < 5)
                {
                    model.SetCurrentSefira(sefira);
                }
                else
                    Debug.Log("말쿠트 초과");
            }

            else if (sefira == "2")
            {
                if (AgentManager.instance.nezzachAgentList.Count < 5)
                {
                    model.SetCurrentSefira(sefira);
                }
                else
                    Debug.Log("네짜흐 초과");
            }

            else if (sefira == "3")
            {
                if (AgentManager.instance.hodAgentList.Count < 5)
                {
                    model.SetCurrentSefira(sefira);
                }
                else
                    Debug.Log("호드 초과");
            }

            else if (sefira == "4")
            {
                if (AgentManager.instance.yesodAgentList.Count < 5)
                {
                    model.SetCurrentSefira(sefira);
                }
                else
                    Debug.Log("예소드 초과");
            }

        }

        sc.extended = -1;
        sc.ShowAgentListD();
    }


}
