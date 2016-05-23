using UnityEngine;

using System.Collections;
using System.Collections.Generic;

public class LadyLookingAtWall : CreatureBase {
    //private const long[] bannedTrait = new long[2]{10015, 10019};
    
    SensingModule sensing = new SensingModule();
    Camera sensor;
    private const long bannedTrait1 = 10015;
    private const long bannedTrait2 = 10019;
    private const int Damage = 3;
    private const int low = 30;
    private const int high = 50;
    private int sensingMax = 10;

    CreatureUnit currentCreatureUnit = null;
    bool isWorking = false;
    bool shouldActivate = false;
    bool activatedSpecialEffect = false;

    List<AgentModel> workedAgentList = new List<AgentModel>();
    LadyLookingAtWallAnim animScript = null;
    int currentSkillPercent = 0;

    int sensingStack = 0;

    public override void OnFixedUpdate(CreatureModel creature)
    {
        if (this.currentCreatureUnit == null) {
            this.currentCreatureUnit = CreatureLayer.currentLayer.GetCreature(model.instanceId);
            RectTransform rect = currentCreatureUnit.cameraSensingArea.GetComponent<RectTransform>();
            
            sensing.Set(rect.position.x - ((rect.rect.width / 2) * currentCreatureUnit.transform.localScale.x),
                        rect.position.x + ((rect.rect.width / 2) * currentCreatureUnit.transform.localScale.x),
                        rect.position.y - ((rect.rect.height / 2) * currentCreatureUnit.transform.localScale.y),
                        rect.position.y + ((rect.rect.height / 2) * currentCreatureUnit.transform.localScale.y));
            sensor = currentCreatureUnit.currentCreatureCanvas.worldCamera;
            
        }

    }

    public override void OnSkillTickUpdate(UseSkill skill)
    {
        if (sensor.orthographicSize < 6f)
        {
            if (sensing.Check(sensor.transform.position))
            {
                CameraChecked(skill);
            }
        }

        if (shouldActivate && skill.workCount > 1) {
            Debug.Log("이작업 하면 안돼!");
            SpecialEffect(skill);
            //작업 중지
            
        }
    }

    void CameraChecked(UseSkill skill) {
        sensingStack++;
        Debug.Log(sensingStack);
        if (sensingStack >= sensingMax) {
            SpecialEffect(skill);
        }
    }

    public override void OnRelease(UseSkill skill)
    {
        /*애니메이션 재생을 한 뒤 나가야 한다*/
        if (activatedSpecialEffect == true) {
            activatedSpecialEffect = false;
            return;
        }

        if (Prob(this.currentSkillPercent))
        {
            Debug.Log("Activated");
            ActivateSkill(skill);
        }
        else {
            Debug.Log("Not activated");
        }
    }

    private void ActivateSkill(UseSkill skill)
    {
        //직원 뒤돌아보는 모션
        //환상체 공격 모션
        /*
        if (Random.Range(0, 2) == 0)
        {
            skill.agent.TakeMentalDamage(Damage);
        }
        else {
            skill.agent.TakePhysicalDamageByCreature(Damage);
        }*/
        skill.agent.HaltUpdate();
        skill.agent.TakeMentalDamage(Damage);
        this.animScript.animator.SetBool("Attack", true);

        //AgentAnimator Change
        AnimatorManager.instance.ResetAnimatorTransform(skill.agent.instanceId);
        AnimatorManager.instance.ChangeAnimatorByID(skill.agent.instanceId,
            AnimatorName.id_LadyLooking_AgentCTRL, skill.agentView.puppetAnim, true, false);
        
        /*
        Debug.Log("LadyLookingAtWall ActivateSkill()");
        // 스킬: 뒤를 돌아보지 마
		skill.agent.TakePhysicalDamage(skillPhysicsDmg, DamageType.CUSTOM);
        skill.agent.TakeMentalDamage(skillMentalDmg);
         */
    }

    private void SpecialEffect(UseSkill skill) {
        sensingMax = Random.Range(7, 13);
        sensing.SetEnabled(false);
        shouldActivate = false;
        skill.PauseWorking();
        skill.FinishForcely();

        //꺄아아아아아아아아악
        skill.agent.TakeMentalDamage(50);
        
        animScript.StartEffect();

        activatedSpecialEffect = true;
        //Debug.Log("KIYAAAAAAAAA");
    }

    public override void OnEnterRoom(UseSkill skill)
    {
        CheckActivatePercentage(skill);
        animScript.animator.SetBool("Work", true);
        //skill.PauseWorking();
        //SoundEffectPlayer.PlayOnce("match_strike_1.wav", skill.targetCreature.transform.position);
    }

    void CheckActivatePercentage(UseSkill skill) {
        AgentModel model = skill.agent;

        Debug.Log("worked" + this.workedAgentList.Contains(model));
        Debug.Log(model.HasTrait(bannedTrait1) + " " + model.HasTrait(bannedTrait2));
        Debug.Log("lifevalue " + model.agentLifeValue);
        Debug.Log("current work " + skill.skillTypeInfo.id);

        if (skill.skillTypeInfo.id == 3)
        {
            shouldActivate = true;
            //SetPercentage(high);
        }

        if (!this.workedAgentList.Contains(model)) {
            SetPercentage(low);
            this.workedAgentList.Add(model);
            return;
        }
        if (model.HasTrait(bannedTrait1) || model.HasTrait(bannedTrait2)) {
            SetPercentage(high);
            return;
        }
		if (model.agentLifeValue == PersonalityType.I) {
            SetPercentage(high);
            return;
        }
        InitPercentage();
        
    }

    void SetPercentage(int value) {
        Debug.Log("percentage setted as " + value);
        this.currentSkillPercent = value;
    }

    void InitPercentage() {
        SetPercentage(0);
    }

    public override bool isAttackInWorkProcess()
    {
        return false;
    }

    public override void OnViewInit(CreatureUnit unit)
    {
        this.animScript = unit.animTarget as LadyLookingAtWallAnim;
        this.animScript.Init(this);
    }

    public void RestartSensing() {
        this.sensing.SetEnabled(true);
        sensingStack = 0;
    }
}
