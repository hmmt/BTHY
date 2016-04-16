using UnityEngine;
using System.Collections;

public class HappyTeddy  : CreatureBase {

    private bool b = false;

	AgentModel lastAgent;
	int teddyWorkNum;
	int noHugNum;
	int normalWorkCount;
    
	public override void OnFixedUpdate (CreatureModel creature)
	{
		base.OnFixedUpdate (creature);

		if (model.energyPoint < 80)
		{
			//model.Escape ();
		}
	}

	public override void OnReturn ()
	{
		model.energyPoint = 130;
	}

	public override void OnSkillStart(UseSkill skill)
	{
		/*
		if(skill.skillTypeInfo.id == 40002) // 
		{
			if(skill.targetCreature.feeling <= 50)
			{
				if(Random.value <= 0.65)
				{
					//skill.agent.hp -= 1; // temp
				}
			}
		}
		*/
	}

    public override void OnSkillTickUpdate(UseSkill skill)
	{
        bool isSpecialSkill = skill.skillTypeInfo.id == 40002;
        float prob = 0;
        int feelingUp = 0;
		/*
        CreatureFeelingState feelingState = model.GetFeelingState();
        if (feelingState == CreatureFeelingState.BAD)
        {
            prob = isSpecialSkill ? 0.4f : 0.9f;
            feelingUp = 50;
        }
        else
        {
            prob = isSpecialSkill ? 0.1f : 0.3f;
            feelingUp = 100;
        }
        if (Random.Range(0,100) < prob*100)
        {
            ActivateSkill(skill, feelingUp);
        }
        */
	}

    //

    public override void OnEnterRoom(UseSkill skill)
    {
		ActivateSkillInWork (skill);
		return;


		if (skill.skillTypeInfo != GetSpecialSkill ())
		{
			normalWorkCount++;

			if (normalWorkCount >= 2)
			{
				normalWorkCount -= 2;
				noHugNum++;
			}
		}

		if (lastAgent == skill.agent)
		{
			teddyWorkNum++;
		}

		float hugProb = 0;
		float agentProb = 0.1f * teddyWorkNum;

		if (skill.skillTypeInfo == GetSpecialSkill ())
		{
			hugProb = 0.1f + agentProb;
		}
		else
		{
			hugProb = 0.2f * noHugNum + agentProb;
		}

		if (Random.value < hugProb)
		{
			ActivateSkillInWork (skill);
		}
    }

	void ActivateSkillInWork(UseSkill skill)
	{
		// play animator

		//CreatureUnit unit = CreatureLayer.currentLayer.GetCreature (model.instanceId);
		//uni

		model.SendAnimMessage ("SpecialAttack");
		AgentUnit agentView = AgentLayer.currentLayer.GetAgent (skill.agent.instanceId);

		AnimatorManager.instance.ResetAnimatorTransform (skill.agent.instanceId);
		AnimatorManager.instance.ChangeAnimatorByName (skill.agent.instanceId, AnimatorName.Teddy_agent,
			agentView.puppetAnim, true, false);
	}

    public override SkillTypeInfo GetSpecialSkill()
    {
        //return null;
        return SkillTypeList.instance.GetData(40002);
    }
}
