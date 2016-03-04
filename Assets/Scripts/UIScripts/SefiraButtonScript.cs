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
        Sefira targetSefira = SefiraManager.instance.getSefira(sefira);

        if (targetSefira == null)
        {
            Debug.Log("세피라 입력 에러");
            return;
        }
        else {
            if (targetSefira == SefiraManager.instance.getSefira(model.currentSefira))
            {
                Debug.Log("같은 부서");
            }
            else
            {
                if (targetSefira.agentList.Count < 5)
                {
                    model.SetCurrentSefira(targetSefira.indexString);
                }
                else
                    Debug.Log(targetSefira.name + " 수용 직원 인원 초과");
            }
        }
        sc.extended = -1;
        sc.ShowAgentListD();
    }


}
