using UnityEngine;
using System.Collections;

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
        int length = AgentManager.instance.GetAgentList().Length;
        int count = AgentManager.instance.agentCount;
        AgentCountNum.text = length + " / " + count;
    }
}
