using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WorkAllocateSlot : MonoBehaviour {
    public AgentModel model;

    public Image Face;
    public Image Hair;
    public Text CoolTime;

    public void Init(AgentModel model) {
        this.model = model;
        AgentModel.SetPortraitSprite(model, Face, Hair);
        //SetCoolTime;
        SetCoolTime();
    }

    public void SetCoolTime() { 
        
    }

	void Update()
	{
		//if(WorkAllocateWindow.currentWindow.currentTargetCreature

		if (model.isDead ()) {
			CoolTime.text = "<DEAD AGENT>";
			CoolTime.color = Color.red;
			return;
		}

		float delay = model.GetSkillDelay (SkillTypeList.instance.GetData (WorkAllocateWindow.currentWindow.selectedSkillId));

		if (delay == -1)
		{
			CoolTime.text = "unusable skill!";
			CoolTime.color = Color.red;
			return;
		}



		if (delay > 0) {
			CoolTime.text = ((int)delay).ToString ();
			CoolTime.color = Color.black;
			return;
		}

		if (model.GetState () == AgentAIState.MANAGE) {
			CoolTime.text = "busy....";
			CoolTime.color = Color.red;
			return;
		}

		CoolTime.text = "";

		//CoolTime.text = 
	}
}
