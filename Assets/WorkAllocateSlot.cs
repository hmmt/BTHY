using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WorkAllocateSlot : MonoBehaviour {
    public AgentModel model;

    public Image Face;
    public Image Hair;
    public Text CoolTime;

    public GameObject portrait;

    public Sprite normal;
    public Sprite selected;
    public Sprite working;
    public Image Icon;
    public Image Bg;

    public Slider hp;
    public Image mentalBreak;
    public Image paicIcon;

    bool isSelected = false;

    public void SetModel(AgentModel model)
    {
        this.model = model;
        if (this.model == null)
        {
            Empty(false);

            return;
        }
        else
        {
            Empty(true);
        }

        AgentModel.SetPortraitSprite(model, Face, Hair);
        //SetCoolTime;
        SetCoolTime();
        CheckState();
    }

    public void SetCoolTime() { 
        
    }

    public void Reset()
    {
        this.model = null;
    }

    public void Empty(bool state) { 
        //Disable objects
        portrait.gameObject.SetActive(state);
        Icon.gameObject.SetActive(state);
        
    }

    public void CheckState() {
        if (this.model == null) return;
        if (this.model.currentSkill != null)
        {
            Bg.sprite = working;
            //GetIcon
            return;
        }
        else {
            Bg.sprite = normal;
        }
    }

    public void Release() {
        this.isSelected = false;
        if (this.model.currentSkill != null) this.Bg.sprite = working;
        else this.Bg.sprite = normal;
    }

	void Update()
	{
		//if(WorkAllocateWindow.currentWindow.currentTargetCreature
        if (this.model == null) return;

        if (model.isDead())
        {
            CoolTime.text = "<DEAD AGENT>";
            CoolTime.color = Color.red;
            return;
        }
        else if(!isSelected){
            CheckState();
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

    public void OnClick() {
        if (this.model == null) return;
        if (this.model.currentSkill != null)
        {
            return;
        }
        //WorkAllocateWindow.currentWindow.OnClickAgent(this.model.instanceId);
        WorkAllocateWindow.currentWindow.OnSelectAgent(this.model);
        this.isSelected = true;
        this.Bg.sprite = selected;
    }
}
