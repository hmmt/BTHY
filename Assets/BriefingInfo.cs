using UnityEngine;
using System.Collections;

public class BriefingInfo : MonoBehaviour {

   public  UnityEngine.UI.Text Briefing;

    public string alwaysBriefing;
    public string tmpBriefing;

	// Use this for initialization
	void Start () {

       // alwaysBriefing = "환영합니다.";
        //alertBriefing = "비상계획팀 - 시설의 비상상황을 관리하는 부서";
        tmpBriefing = "환영합니다.";	
	}
	
	// Update is called once per frame
	void FixedUpdate () {

        Briefing.text = tmpBriefing;
	
	}

    public void SetBriefing(string name)
    {
        if(name == "Netzach")
        {
            tmpBriefing = "비상계획팀 - 시설의 비상상황을 관리하는 부서.\n 열기위한 에너지 비용:10"; 
        }
        else if(name == "Hod")
        {
            tmpBriefing = "자재관리팀 - 시설의 설비를 관리하는 부서.\n 열기위한 에너지 비용:10";
        }
        else if(name == "Yesod")
        {
             tmpBriefing = "솔루션계획팀 - 환상체를 관리하는 작업방식을 관리하는 부서\n 열기위한 에너지 비용:10"; 
        }
        
        else if(name == "Malkuth")
        {
             tmpBriefing = "지휘감시팀 - 모든 상황을 통괄하는 부서.\n 열기위한 에너지 비용:10"; 
        }
    }

    public void SetDefaultBriefing()
    {
        tmpBriefing = "환영합니다."; 
    }
    
}
