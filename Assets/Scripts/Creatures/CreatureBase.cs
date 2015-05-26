using UnityEngine;
using System.Collections;

public class CreatureBase {

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
}
