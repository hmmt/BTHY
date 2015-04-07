using UnityEngine;
using System.Collections;

public class CreatureBase {

	public virtual void FixedUpdate(CreatureUnit creature)
	{

	}

	public virtual void IdleFixedUpdate(CreatureUnit creature)
	{
		
	}
	
	public virtual void SkillStartUpdate(UseSkill skill)
	{
	}

    public virtual void SkillNormalAttack(UseSkill skill)
    {
    }

    public virtual void SkillSucceedWorkTick(UseSkill skill)
    {
    }

    public virtual void SkillFailWorkTick(UseSkill skill)
    {
    }

	public virtual void SkillTickUpdate(UseSkill skill)
	{

	}

    public virtual void SkillGoalComplete(UseSkill skill)
    {
    }
	
	public virtual void SkillFinishUpdate(UseSkill skill)
	{
	}

	public virtual void EnterRoom(UseSkill skill)
	{
		
	}
}
