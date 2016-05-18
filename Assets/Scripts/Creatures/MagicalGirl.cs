using UnityEngine;
using System.Collections;

public class MagicalGirl : CreatureBase {
    // 기분상태에 따라 마취약 투여가 가능해야 함.
    public enum MagicalGirlState { 
        GOOD,
        NORMAL,
        BAD
    }
    const float AnestheticFeelingDown = 80f;
    
    public MagicalGirlState currentState = MagicalGirlState.GOOD;

    bool anestheticReady = false;//마취가능상태
    bool isAnesthetized = false;//현재마취된상태
    bool isEscaped = false;
    bool coolTimeRun = false;

    float afterAnestheticFeeling = 0f;

    CreatureTimer timer = new CreatureTimer(30f);
    CreatureTimer coolTime = new CreatureTimer(10f);

    public override void OnInit()
    {
        anestheticReady = true;//for initial
        model.AddFeeling(100f);
        coolTime.enabled = false;   
    }

    private void ChangeDark(CreatureModel creature)
    {
        
    }

    private void ChangeNormal(CreatureModel creature)
    {
        
    }

    // 역변
    public override void OnFixedUpdate(CreatureModel creature)
    {
        if (isEscaped) return;

        CheckFeeling();
        if (timer.TimerRun()) {
            AnestheticEnd();    
        }
    }

	public override void OnReturn ()
	{
        coolTime.enabled = false;
        coolTime.TimerStop();
		
	}

    private void SkillDarkAttack(UseSkill skill)
    {
        
    }

    public override void OnEnterRoom(UseSkill skill)
    {
        if (skill.skillTypeInfo.id == 40004)
        {
            Anesthetic();
        }
    }

    public override void OnRelease(UseSkill skill)
    {
        if (currentState == MagicalGirlState.GOOD) {
            //smth effect
            AgentMentalRecovery(skill.agent);
        }

        if (skill.skillTypeInfo.id == 40004) {
            if (isAnesthetized) {
                Debug.Log("TimerStart");
                timer.TimerStart(true);
            }
        }
    }

    public void CheckFeeling() { 
        //smth flags check

        if (model.GetFeelingPercent() < 33f && this.currentState != MagicalGirlState.BAD) { 
            //state transition
            this.currentState = MagicalGirlState.BAD;
            if (anestheticReady) {
                anestheticReady = false;
            }
        }

        if (model.GetFeelingPercent() >= 33f && model.GetFeelingPercent() < 66f
            && this.currentState != MagicalGirlState.NORMAL)
        {
            this.currentState = MagicalGirlState.NORMAL;
            if (!isAnesthetized && !anestheticReady) {
                if (coolTime.enabled)
                {
                    if (coolTime.TimerRun())
                    {
                        Debug.Log("coolTime ended");
                        coolTime.enabled = false;
                        anestheticReady = true;
                    }
                }
                else {
                    anestheticReady = true;
                }
            }
        }

        if (model.GetFeelingPercent() >= 66f && this.currentState != MagicalGirlState.GOOD) {
            this.currentState = MagicalGirlState.GOOD;
            if (anestheticReady)
            {
                anestheticReady = false;
            }
        }
    }

    public void Anesthetic() {
        if (isAnesthetized) return;
        if (!anestheticReady) return;

        if (!coolTime.enabled) coolTime.enabled = true;
        Debug.Log("AnestheticStart");
        this.anestheticReady = false;
        this.isAnesthetized = true;
        afterAnestheticFeeling = -1 * AnestheticFeelingDown;
        
    }

    public override bool hasUniqueEscape()
    {
        return false;
    }

    public override bool hasUniqueFinish()
    {
        return false;
    }

    public void AgentMentalRecovery(AgentModel worker) {
        worker.RecoverMental(5);
    }

    public void AnestheticEnd() {
        isAnesthetized = false;
        Debug.Log("AnestheticEnd");
        if (afterAnestheticFeeling < 0)
        {
            model.SubFeeling(-1 * afterAnestheticFeeling);
        }
        else {
            model.AddFeeling(afterAnestheticFeeling);
        }
        coolTime.TimerStart(true);
    }

    public override bool AutoFeelingDown()
    {
        if (isAnesthetized) return false;
        return true;
    }

    public override SkillTypeInfo GetSpecialSkill()
    {
        if (anestheticReady && currentState == MagicalGirlState.NORMAL)
        {
            return SkillTypeList.instance.GetData(40004);
        }
        else {
            return null;
        }
    }

    public override string GetDebugText()
    {
        if (coolTime.enabled == false)
        {
            return this.anestheticReady.ToString() + " " + this.currentState.ToString();
        }
        else {
            return this.coolTime.elapsed.ToString();
        }
    }
}
