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
		/*
        skill.PauseWorking();

        OutsideTextEffect effect = OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/oldlady/OldLady_AttackTypo_01", CreatureOutsideTextLayout.CENTER_BOTTOM, 0.5f, 7.5f);
        effect.transform.localScale = new Vector3(1.1f, 1.1f, 1);

        // skill이 이미 release 될 상황 고려 필요
        effect.GetComponent<DestroyHandler>().AddReceiver(delegate() { skill.ResumeWorking(); });

        SoundEffectPlayer.PlayOnce("creature/old_lady/oldlady_special", skill.targetCreatureView.transform.position);

        OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/oldlady/OldLady_AttackTypo_02", CreatureOutsideTextLayout.CENTER_BOTTOM, 1.0f, 7.0f)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/oldlady/OldLady_AttackTypo_03", CreatureOutsideTextLayout.CENTER_BOTTOM, 1.5f, 6.5f)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/oldlady/OldLady_AttackTypo_04", CreatureOutsideTextLayout.CENTER_BOTTOM, 2.0f, 6.0f)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/oldlady/OldLady_AttackTypo_05", CreatureOutsideTextLayout.CENTER_BOTTOM, 2.5f, 5.5f)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/oldlady/OldLady_AttackTypo_06", CreatureOutsideTextLayout.CENTER_BOTTOM, 3.0f, 5.0f)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/oldlady/OldLady_AttackTypo_07", CreatureOutsideTextLayout.CENTER_BOTTOM, 3.5f, 4.5f)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/oldlady/OldLady_AttackTypo_08", CreatureOutsideTextLayout.CENTER_BOTTOM, 4.0f, 4.0f)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/oldlady/OldLady_AttackTypo_09", CreatureOutsideTextLayout.CENTER_BOTTOM, 4.5f, 3.5f)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/oldlady/OldLady_AttackTypo_10", CreatureOutsideTextLayout.CENTER_BOTTOM, 5.0f, 3.0f)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/oldlady/OldLady_AttackTypo_11", CreatureOutsideTextLayout.CENTER_BOTTOM, 5.5f, 2.5f)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/oldlady/OldLady_AttackTypo_12", CreatureOutsideTextLayout.CENTER_BOTTOM, 6.0f, 2.0f)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);

        for(int i=1; i<=5; i++)
        {
            int copiedI = i;
            TimerCallback.Create(i*1.2f, delegate() {
                skill.targetCreature.ShowNarrationText("special_ability"+copiedI, skill.agent.name);
            });
        }
        TimerCallback.Create(7.2f, delegate() {
            model.ShowNarrationText("special_ability6", skill.agent.name);
            //skill.agent.TakeMentalDamage(skillDamage);
            CreatureFeelingState feelingState = model.GetFeelingState();
            if (feelingState == CreatureFeelingState.BAD)
            {
                skill.agent.TakeMentalDamage(30);
            }
            else if (feelingState == CreatureFeelingState.NORM)
            {
                skill.agent.TakeMentalDamage(20);
            }
            else if (feelingState == CreatureFeelingState.GOOD)
            {
                skill.agent.TakeMentalDamage(10);
            }
        });
        */
    }

    public override void OnEnterRoom(UseSkill skill)
    {
        AgentCheck(skill);

        if (specialSkillActivated) { 
            //애니메이션 재생 후 작업 취소
            this.currentSkill = skill;
        }

        if (skill.skillTypeInfo.id == 5) {
            this.violentWorked = true;
        }
		/*
        skill.PauseWorking();

        OutsideTextEffect effect = OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/oldlady/OldLady_EnterTypo_01", CreatureOutsideTextLayout.CENTER_BOTTOM, 0, 9);
        effect.transform.localScale = new Vector3(1.1f, 1.1f, 1);

        // skill이 이미 release 될 상황 고려 필요
        effect.GetComponent<DestroyHandler>().AddReceiver(delegate() { skill.ResumeWorking(); });


        OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/oldlady/OldLady_EnterTypo_02", CreatureOutsideTextLayout.CENTER_BOTTOM, 1, 8)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/oldlady/OldLady_EnterTypo_03", CreatureOutsideTextLayout.CENTER_BOTTOM, 2, 7)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/oldlady/OldLady_EnterTypo_04", CreatureOutsideTextLayout.CENTER_BOTTOM, 3, 6)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/oldlady/OldLady_EnterTypo_05", CreatureOutsideTextLayout.CENTER_BOTTOM, 4, 5)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/oldlady/OldLady_EnterTypo_06", CreatureOutsideTextLayout.CENTER_BOTTOM, 5, 4)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/oldlady/OldLady_EnterTypo_07", CreatureOutsideTextLayout.CENTER_BOTTOM, 6, 3)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/oldlady/OldLady_EnterTypo_08", CreatureOutsideTextLayout.CENTER_BOTTOM, 7, 2)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        */
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

                if (this.currentAnimPlayingAnimator.GetInteger("AttackEnd") == 2 
                    && this.model.GetAnimScript().animator.GetInteger("AttackEnd") == 2) {
                    AnimatorManager.instance.ResetAnimatorTransform(currentAnimPlayingAgent.instanceId);
                    currentAnimPlayingAgent.ResetAnimator();
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
        int value = (int)(skill.agent.maxMental * 0.8f);
        skill.agent.TakeMentalDamage(value);
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
