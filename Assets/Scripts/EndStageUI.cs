using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class EndStageUI : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

public class EndStage {
    private static EndStage _instance;
    public static EndStage instance {
        get {
            if (_instance == null) {
                _instance = new EndStage();

            }
            return _instance;
        }
    }

    public AgentModel bestWorker;
    public List<WorkerModel> deadList;

    private ResultReportScript script;
    private List<AgentModel> agentList;
    private List<OfficerModel> officerList;

    private EndStage() {
        deadList = new List<WorkerModel>();
        agentList = new List<AgentModel>();
        officerList = new List<OfficerModel>();
        script = GameObject.FindWithTag("ReportPanel").GetComponent<ResultReportScript>();
    }

    public void init(AgentModel best) {

        this.bestWorker = best;
        script.SetBestAgent(best);
    }
}