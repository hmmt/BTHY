using UnityEngine;
using System.Collections;

public class OneBadManyGood : CreatureBase {

    private int skillDamage = 20;

    public override void OnInit()
    {
        this.skill = new OneBadManyGoodSkill(this.model);
    }

    // temporary
    public override void OnSkillFailWorkTick(UseSkill skill)
    {
        return;
    }

    public override void OnSkillSucceedWorkTick(UseSkill skill)
    {
        return;
        skill.targetCreature.AddFeeling(100);
    }
    //

    public override void OnEnterRoom(UseSkill skill)
    {
		// If GetSpecialSkill() is null
        if (skill.skillTypeInfo == GetSpecialSkill()) {
            //SpecialSkill
            Debug.Log("special skill");

            this.skill.SkillActivate(skill.agent);
            return;
        }
        //this.skill.SkillActivate(skill.agent);

		/*
        skill.PauseWorking();

        OutsideTextEffect effect = OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/goodbad/oneGoodManyBad_EnterTypo_01", CreatureOutsideTextLayout.CENTER_BOTTOM, 0, 4);
        effect.transform.localScale = new Vector3(1.1f, 1.1f, 1);

        // skill이 이미 release 될 상황 고려 필요
        effect.GetComponent<DestroyHandler>().AddReceiver(delegate() { skill.ResumeWorking(); });


        OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/goodbad/oneGoodManyBad_EnterTypo_02", CreatureOutsideTextLayout.CENTER_BOTTOM, 1, 3)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/goodbad/oneGoodManyBad_EnterTypo_03", CreatureOutsideTextLayout.CENTER_BOTTOM, 2, 2)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        */
    }

    public override void OnFixedUpdate(CreatureModel creature)
    {

        
        if (creature.GetFeelingPercent() < 50f)
        {
            (this.skill as OneBadManyGoodSkill).ReadySkill(false);
        }

        if (creature.GetFeelingPercent() >= 50f
            && !(this.skill as OneBadManyGoodSkill).GetSkillState()
            ) {
                (this.skill as OneBadManyGoodSkill).ReadySkill(true);
        }

    }

    public override SkillTypeInfo GetSpecialSkill()
    {
        OneBadManyGoodSkill currentSkill = this.skill as OneBadManyGoodSkill;

        if (currentSkill.GetSkillState())
        {
            return SkillTypeList.instance.GetData(40003);
        }
        else {
            return null;
        }
    }

    public override void OnRelease(UseSkill skill)
    {
        /*
        OneBadManyGoodSkill currentSkill = this.skill as OneBadManyGoodSkill;

        if (currentSkill.GetSuccessState()) {
            
        }*/
        if (skill.agent.isDead()) {

            return;
        }

    }
}
