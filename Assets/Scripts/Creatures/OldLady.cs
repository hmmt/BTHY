using UnityEngine;
using System.Collections;

public class OldLady : CreatureBase {

    AgentModel workedAgent = null;
    Animator currentAnimPlayingAnimator = null;
    AgentModel currentAnimPlayingAgent = null;
    UseSkill currentSkill = null;
    int equalAgentWorkCount = 0;
    int currentDamage = 0;
    bool violentWorked = false;
    bool specialSkillActivated = false;
    bool isPlayingAnim = false;

    public override void OnSkillGoalComplete(UseSkill skill)
    {
        //ActivatSkill(skill);
    }

    public override void OnSkillTickUpdate(UseSkill skill)
    {
        if (Prob(this.CalculateCurrentSuccess()))
        {
            //성공
            Debug.Log("에너지생산");
            OnSkillSucceedWorkTick(skill);
        }
        else
        {
            //실패 
            OnSkillFailWorkTick(skill);
        }
    }

    public void ActivateSkill(UseSkill skill)
    {
        Debug.Log("OldLady ActivateSkill()");
    }

    public override void OnEnterRoom(UseSkill skill)
    {
        //AgentCheck(skill);
		SpecialAttack(skill);

        if (specialSkillActivated) { 
            //애니메이션 재생 후 작업 취소
            this.currentSkill = skill;
        }

        if (skill.skillTypeInfo.id == 5) {
            this.violentWorked = true;
        }
    }

    public override void OnRelease(UseSkill skill)
    {
        //violentwork()
        ExitWorkProcess(skill.agent);
    }

    int CalculateCurrentMentalDamage()
    {
        switch (equalAgentWorkCount) {
            case 0: return 1;
            case 1: return 2;
            case 2: return 3;
            case 3: return 5;
            case 4: return 7;
            default: return 7;
        }
    }

    int CalculateCurrentSuccess() {
        switch (equalAgentWorkCount) {
            case 0: return 50;
            case 1: return 60;
            case 2: return 70;
            case 3: return 85;
            case 4: return 100;
            default: return 100;
        }
    }

    public override void OnFixedUpdate(CreatureModel creature)
    {
        if (isPlayingAnim) {
            if (this.currentAnimPlayingAnimator != null) {

				//Debug.Log ("agent : "+this.currentAnimPlayingAnimator.GetInteger("AttackEnd")+", creature : "+this.model.GetAnimScript().animator.GetInteger("AttackEnd"));
                if (this.currentAnimPlayingAnimator.GetInteger("AttackEnd") == 2 
                    && this.model.GetAnimScript().animator.GetInteger("AttackEnd") == 2) {
                    AnimatorManager.instance.ResetAnimatorTransform(currentAnimPlayingAgent.instanceId);
                    currentAnimPlayingAgent.ResetAnimator();
					currentAnimPlayingAgent.WorkEndReaction ();
                    currentAnimPlayingAgent = null;
                    currentAnimPlayingAnimator = null;
                    isPlayingAnim = false;
                    this.model.GetAnimScript().animator.SetInteger("AttackEnd", 0);
                }
            }
        }
    }

    void AgentCheck(UseSkill skill) {

        if (this.workedAgent == null) {
            this.workedAgent = skill.agent;
            this.model.GetAnimScript().animator.SetInteger("Attack", 1);
        }
        else if (model.Equals(this.workedAgent)){
            this.equalAgentWorkCount++;
            this.model.GetAnimScript().animator.SetInteger("Attack", 1);
        }
        else
        {
            this.equalAgentWorkCount = 0;

            
            if (!violentWorked)
            {
                if (Prob(75))
                {
                    Debug.Log("Activated Special Skill");
                    SpecialAttack(skill);
                }
                else {
                    Debug.Log("Not activated special skill");
                    this.model.GetAnimScript().animator.SetInteger("Attack", 1);
                }
            }

            this.workedAgent = skill.agent;
        }
        violentWorked = false;

        
    }

    void SpecialAttack(UseSkill skill) {
        skill.PauseWorking(11f);
        
        specialSkillActivated = true;
        int value = (int)(skill.agent.maxMental * 0.8f)/4;
        
        isPlayingAnim = true;
        //need animation playing
        model.GetAnimScript().animator.SetInteger("Special", 1);
        Animator agentAnim = AgentLayer.currentLayer.GetAgent(skill.agent.instanceId).puppetAnim;
        AnimatorManager.instance.ResetAnimatorTransform(skill.agent.instanceId);
        AnimatorManager.instance.ChangeAnimatorByName(skill.agent.instanceId, AnimatorName.OldLady_AgentCTRL,
                                                      agentAnim, true, false);
        agentAnim.SetInteger("Attack", 2);
        currentAnimPlayingAnimator = agentAnim;
        currentAnimPlayingAgent = skill.agent;

		skill.agent.TakeMentalDamage(value);
    }

    void ExitWorkProcess(AgentModel model) {
        int mentalDamage = this.CalculateCurrentMentalDamage();
        model.TakeMentalDamage(mentalDamage);
        //애니메이션 재생
        //판정걸 것
        if (violentWorked) {
            ViolentWorked();
            
        }
        this.model.GetAnimScript().animator.SetInteger("Attack", 0);
        
    }

    void ViolentWorked() {
        model.SubFeeling(1000);
    }

    public override string GetDebugText()
    {
        string output = "";
        if (this.workedAgent != null){
            output += workedAgent.name;
        }
        output += " " + this.equalAgentWorkCount.ToString();
        return output;
    }

    public override bool isAttackInWorkProcess()
    {
        return false;
    }

    public override void OnTimerEnd()
    {
        if (this.currentSkill != null) {
            this.currentSkill.agent.StopAction();
            //finish forcely
            this.currentSkill.FinishForcely();
            this.currentSkill = null;
        }
    }
}
