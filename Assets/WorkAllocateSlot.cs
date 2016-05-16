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
    public Image Bg;

    public Text tagSlot;

    public Slider hp;
    public Image mentalBreak;
    public Image paicIcon;

    public Image currentAction;

    public Sprite spriteSetting = null;
    private Sprite currentWorkingSprite = null;

    float elapsed = 0f;
    float frequency = 10f;

    bool isSelected = false;
    bool shouldCheck = true;
    bool isWorking = false;

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
        SetCurrentStateIcon();
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

        this.gameObject.SetActive(state);
        //portrait.gameObject.SetActive(state);
    }

    public void CheckState() {
        if (this.model == null) return;
        
        if (this.model.currentSkill != null || model.GetState() == AgentAIState.MANAGE)
        {
            Bg.sprite = normal;

            tagSlot.text = this.model.target.metaInfo.name;
            //Debug.Log(this.model.currentSkill.skillTypeInfo.name);
            if (WorkAllocateWindow.currentWindow.GetTargetCreature() != null && this.model.target != null)
            {
                if (this.model.target == WorkAllocateWindow.currentWindow.GetTargetCreature())
                {
                    Bg.sprite = working;

                    isWorking = true;
                }
                else if (this.model.currentSkill != null) {
                    if (this.model.currentSkill.targetCreature == WorkAllocateWindow.currentWindow.GetTargetCreature()) {
                        Bg.sprite = working;

                        isWorking = true;
                    }
                }
            }
            //GetIcon
            return;
        }
        else {
            Bg.sprite = normal;
            isWorking = false;
            tagSlot.text = AgentModel.GetLevelGradeText(this.model) + 
                " " + this.model.LifeStyle() + " " + this.model.name;
        }
    }

    public void Release() {
        this.isSelected = false;
        if (this.model == null) return;
        if (this.model.currentSkill != null) {
            if (this.model.currentSkill.targetCreature == WorkAllocateWindow.currentWindow.GetTargetCreature())
            {
                Bg.sprite = working;
            }
            else Bg.sprite = normal;
        }
        else this.Bg.sprite = normal;

        ReleaseIcon();
    }

	void Update()
	{
		//if(WorkAllocateWindow.currentWindow.currentTargetCreature
        if (this.model == null) return;
        //UpdateState();
        elapsed += Time.deltaTime;
        if (elapsed > frequency) {
            elapsed = 0f;
            UpdateState();
        }

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
        if (WorkAllocateWindow.currentWindow.observeState == false && WorkAllocateWindow.currentWindow.selectedSkillId == -1) return;
        if (this.model.currentSkill != null || this.model.GetState() == AgentAIState.MANAGE)
        {
            return;
        }

        if (isSelected) {
            isSelected = false;
            WorkAllocateWindow.currentWindow.OnDeselectAgent(this.model);
            Release();
            return;
        }

        WorkAllocateWindow.currentWindow.OnSelectAgent(this.model);
        
        //WorkAllocateWindow.currentWindow.OnClickAgent(this.model.instanceId);
        
        this.isSelected = true;
        this.Bg.sprite = selected;
    }

    public void SetCurrentStateIcon() {
        if (this.model == null) return;
        Debug.Log("Set current State");
        this.hp.maxValue = this.model.maxHp;
        this.hp.minValue = 0;

        UpdateState();

        /*
        if (this.model.IsPanic())
        {
            this.paicIcon.gameObject.SetActive(true);
            this.paicIcon.sprite = AgentModel.GetPanicActionIcon(this.model);
        }
        else {
            this.paicIcon.gameObject.SetActive(false);
        }
        */
        
    }

    public void UpdateState() {
        float colorValue = (float)this.model.mental / this.model.maxMental;
        Color mentalColor = this.mentalBreak.color;
        mentalColor.a = 1- colorValue;
        this.mentalBreak.color = mentalColor;

        this.hp.value = model.hp;

        if (this.model.CurrentPanicAction != null)
        {
            this.paicIcon.gameObject.SetActive(true);
            CheckPanic();
        }
        this.paicIcon.gameObject.SetActive(false);
        SetCurrentActionIcon();
    }

    public void CheckPanic() {
        if (this.model.CurrentPanicAction is PanicReady)
        {
            this.paicIcon.sprite = AgentModel.GetPanicIcon();
        }
        else
            this.paicIcon.sprite = AgentModel.GetPanicActionIcon(this.model);
    }

    public void SetCurrentActionIcon() {
        if (this.model == null) return;
        if (shouldCheck == false) return;
        Sprite s = null;
        if (this.model.currentSkill != null || this.model.GetState() == AgentAIState.MANAGE)
        {
            isWorking = true;
            if (this.model.currentSkill == null || this.model.currentSkill.skillTypeInfo == null)
            {
                s = spriteSetting;
            }
            else {
                int iconId = AgentModel.GetWorkIconId(this.model.currentSkill.skillTypeInfo);
                s = IconManager.instance.GetWorkIcon(iconId).GetDefault().icon;
                if (s == null)
                {
                    Debug.Log(iconId);
                }
            }
            
        }
        else {
            
            if (model.GetState() == AgentAIState.SUPPRESS_CREATURE || model.GetState() == AgentAIState.SUPPRESS_WORKER)
            {
                Debug.Log("Suppress");
                s = AgentModel.GetSuppressIcon(this.model);
            }
            else
            {
                isWorking = false;
                //currentAction.gameObject.SetActive(false);
                return;
            }
        }
        

        currentAction.sprite = s;
    }

    public void SetIconByWindow(Sprite s, long id) {
        if (this.model == null) return;
        if (isWorking) return;
        if (s == null) {
            currentAction.gameObject.SetActive(false);
        }
        if (this.model.HasSkill(SkillTypeList.instance.GetData(id)))
        {
            currentAction.gameObject.SetActive(true);
            shouldCheck = false;
            this.currentAction.sprite = s;
        }
        else {
            currentAction.gameObject.SetActive(false);
        }
        spriteSetting = s;
    }

    public void ReleaseIcon() {
        shouldCheck = true;
        SetCurrentActionIcon();
    }
}
