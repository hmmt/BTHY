using UnityEngine;

using System.Collections;
using System.Collections.Generic;

public class LadyLookingAtWall : CreatureBase {
    //private const long[] bannedTrait = new long[2]{10015, 10019};
    const string lookback = "lookback";
    const string surprise1 = "danger";
    const string surprise2 = "scream1";
    const string surprise3 = "scream2";
    const string surprise4 = "scream3";
    const string breath = "breathing";
    public const string beep = "beep";

    SensingModule sensing = new SensingModule();
    Camera sensor;
    private const long bannedTrait1 = 10015;
    private const long bannedTrait2 = 10019;
    private const int Damage = 3;
    private const int low = 30;
    private const int high = 50;
    private const int max = 100;
    private int sensingMax = 7;

    public CreatureUnit currentCreatureUnit = null;
    bool isWorking = false;
    bool shouldActivate = false;
    bool activatedSpecialEffect = false;

    List<AgentModel> workedAgentList = new List<AgentModel>();
    LadyLookingAtWallAnim animScript = null;
    int currentSkillPercent = 0;

    int sensingStack = 0;

    CreatureTimer cryTimer = new CreatureTimer();
    SoundEffectPlayer currentCry = null;

    public override void OnFixedUpdate(CreatureModel creature)
    {
        if (this.currentCreatureUnit == null) {
            this.currentCreatureUnit = CreatureLayer.currentLayer.GetCreature(model.instanceId);
            RectTransform rect = currentCreatureUnit.cameraSensingArea.GetComponent<RectTransform>();
            //sensing.SetEnabled(true);
            sensing.Set(rect.position.x - ((rect.rect.width / 2) * currentCreatureUnit.transform.localScale.x),
                        rect.position.x + ((rect.rect.width / 2) * currentCreatureUnit.transform.localScale.x),
                        rect.position.y - ((rect.rect.height / 2) * currentCreatureUnit.transform.localScale.y),
                        rect.position.y + ((rect.rect.height / 2) * currentCreatureUnit.transform.localScale.y));
            sensor = currentCreatureUnit.currentCreatureCanvas.worldCamera;
            sensing.SetEnabled(false);
            cryTimer.TimerStart(10f, true);
        }

        if (!sensing.GetEnabled()) {
            if (cryTimer.TimerRun()) {
                //Debug.Log("Timer out " + sensing.GetEnabled());
                //SoundMake
                //MakingEffect(null, null, breath, null, 0);
                if (currentCry == null)
                {
                    currentCry = currentCreatureUnit.PlaySound(breath);
                    cryTimer.TimerStart(Random.Range(10f, 30f), true);
                }
            }
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
        Debug.Log(sensingStack+" " + sensing.GetEnabled());
        if (sensingStack >= sensingMax) {
            SpecialEffect(skill);
        }
    }

    public override void OnRelease(UseSkill skill)
    {
        sensing.SetEnabled(false);
        cryTimer.TimerStart(Random.Range(10f, 30f), true);
        /*애니메이션 재생을 한 뒤 나가야 한다*/
       
        if (Prob(this.currentSkillPercent))
        {
            if (activatedSpecialEffect) return;
            //Debug.Log("Activated");
            ActivateSkill(skill);
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
        Debug.Log(this.activatedSpecialEffect);
        if (this.activatedSpecialEffect) return;
        skill.agent.HaltUpdate();
        this.animScript.animator.SetBool("Attack", true);

        //AgentAnimator Change
        AnimatorManager.instance.ResetAnimatorTransform(skill.agent.instanceId);
        AnimatorManager.instance.ChangeAnimatorByID(skill.agent.instanceId,
            AnimatorName.id_LadyLooking_AgentCTRL, skill.agentView.puppetAnim, true, false);
        skill.agentView.puppetAnim.SetInteger("Flag", 1);

        skill.agent.TakeMentalDamage(Damage);
        /*
        Debug.Log("LadyLookingAtWall ActivateSkill()");
        // 스킬: 뒤를 돌아보지 마
		skill.agent.TakePhysicalDamage(skillPhysicsDmg, DamageType.CUSTOM);
        skill.agent.TakeMentalDamage(skillMentalDmg);
         */
    }

    private void SpecialEffect(UseSkill skill) {
        sensingMax = Random.Range(4,8);
        sensing.SetEnabled(false);
        shouldActivate = false;
        skill.PauseWorking();
        skill.FinishForcely();
        UIEffectManager.instance.NoiseScreen(1f, 30);
        //꺄아아아아아아아아악
        //skill.agent.HaltUpdate();
        skill.agent.TakeMentalDamage(50);
        //sound block
        currentCreatureUnit.PlaySound(beep);
        currentCreatureUnit.PlaySound(surprise4, AudioRolloffMode.Linear);
        CameraMover.instance.Recoil(3);
        //sound block end1
        /*
        AnimatorManager.instance.ResetAnimatorTransform(skill.agent.instanceId);
        AnimatorManager.instance.ChangeAnimatorByID(skill.agent.instanceId,
            AnimatorName.id_LadyLooking_AgentCTRL, skill.agentView.puppetAnim, true, false);
        skill.agentView.puppetAnim.SetInteger("Flag", 2);
        */
        animScript.StartEffect();

        activatedSpecialEffect = true;
        //Debug.Log("KIYAAAAAAAAA");
    }

    public override void OnEnterRoom(UseSkill skill)
    {
        activatedSpecialEffect = false; 
        RestartSensing();
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
            //shouldActivate = true;
            //SetPercentage(high);
            SetPercentage(max);
            return;
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
        cryTimer.TimerStop();
        sensingStack = 0;
    }
}
