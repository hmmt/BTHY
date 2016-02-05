using UnityEngine;
using System.Collections;

public class CreatureAttackInfo
{
    public float physicsProb;
    public float mentalProb;
    public int physicsDmg;
    public int mentalDmg;

    public CreatureAttackInfo() { }
    public CreatureAttackInfo(float physicsProb, float mentalProb, int physicsDmg, int mentalDmg)
    {
        this.physicsProb = physicsProb;
        this.mentalProb = mentalProb;
        this.physicsDmg = physicsDmg;
        this.mentalDmg = mentalDmg;
    }
}

public class CreatureBase {

    protected CreatureModel model;

    public void SetModel(CreatureModel model)
    {
        this.model = model;
    }

    public virtual void OnInit()
    {
    }

    public virtual void OnFixedUpdate(CreatureModel creature)
	{

	}

    public virtual void OnIdleFixedUpdate(CreatureModel creature)
	{
		
	}

    public virtual void OnFeelingUpdate(CreatureModel creature)
    {
    }
	
	public virtual void OnSkillStart(UseSkill skill)
	{
	}

    public virtual void OnSkillNormalAttack(UseSkill skill)
    {
    }

    public virtual void OnSkillSucceedWorkTick(UseSkill skill)
    {
    }

    public virtual void OnSkillFailWorkTick(UseSkill skill)
    {
    }

    // UseSkill의 ProccessWorkTick 이 실행될 때
	public virtual void OnSkillTickUpdate(UseSkill skill)
	{

	}

    public virtual void OnSkillGoalComplete(UseSkill skill)
    {
    }
	
	public virtual void OnSkillFinishUpdate(UseSkill skill)
	{
	}

	public virtual void OnEnterRoom(UseSkill skill)
	{
		
	}
	/*
    public virtual CreatureAttackInfo GetAttackInfo(UseSkill skill)
    {
        return new CreatureAttackInfo(
            model.metaInfo.physicsProb,
            model.metaInfo.mentalProb,
            model.metaInfo.physicsDmg,
            model.metaInfo.mentalDmg
            );
    }*/


    public virtual SkillTypeInfo GetSpecialSkill()
    {
        return null;
    }
}
