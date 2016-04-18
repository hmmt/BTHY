using UnityEngine;
using System.Collections;

public class HappyTeddy  : CreatureBase {

    private bool b = false;

	AgentModel lastAgent;
	int teddyWorkNum;
	int noHugNum;
	int normalWorkCount;

	bool isKilling = false;
	AgentModel killTarget = null;

	public override void OnViewInit (CreatureUnit unit)
	{
	}
    
	public override void OnFixedUpdate (CreatureModel creature)
	{
		base.OnFixedUpdate (creature);

		if (model.energyPoint < 80)
		{
			//model.Escape ();
		}
		//anim.animator

		if (isKilling)
		{
			if (killTarget.GetCurrentNode () == model.GetCustomNode())
			{
				model.SendAnimMessage ("SpecialAttack");
				AgentUnit agentView = AgentLayer.currentLayer.GetAgent (killTarget.instanceId);

				AnimatorManager.instance.ResetAnimatorTransform (killTarget.instanceId);
				AnimatorManager.instance.ChangeAnimatorByName (killTarget.instanceId, AnimatorName.Teddy_agent,
					agentView.puppetAnim, true, false);

				agentView.puppetAnim.SetBool ("Work", false);
				agentView.puppetAnim.SetBool ("Dead", true);
				//agentView.transform.localPosition = new Vector3 (agentView.transform.localPosition.x, agentView.transform.localPosition.y, -1.1f);
				agentView.zValue = -1.1f;

				killTarget.Die ();

				isKilling = false;
				killTarget = null;
			}
			/*
			CreatureAnimScript anim = model.GetAnimScript ();
			int killMoment = anim.animator.GetInteger ("KillMoment");
			if (killMoment == 1) {
				anim.animator.SetInteger ("KillMoment", 0);

				if (model.currentSkill == null)
				{
					Debug.Log ("INVALID STATE : the creature must have UseSkill");
					return;
				}

				//model.currentSkill.agent.Die ();
				isKilling = false;
			}
			*/
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
			hugProb = 0.1f + agentProb + 0.8f;
		}
		else
		{
			hugProb = 0.2f * noHugNum + agentProb + 0.8f;
		}

		if (Random.value < hugProb)
		{
			ActivateSkillInWork (skill);
		}
		else if (skill.skillTypeInfo == GetSpecialSkill ())
		{
			HugSkill (skill);
		}
    }

	public override void OnRelease (UseSkill skill)
	{
		if(skill.agent.isDead() == false)
			skill.agent.ResetAnimator ();
	}

	void HugSkill(UseSkill skill)
	{
		//model.SendAnimMessage ("SpecialAttack");

		AgentUnit agentView = AgentLayer.currentLayer.GetAgent (skill.agent.instanceId);

		AnimatorManager.instance.ResetAnimatorTransform (skill.agent.instanceId);
		AnimatorManager.instance.ChangeAnimatorByName (skill.agent.instanceId, AnimatorName.Teddy_agent,
			agentView.puppetAnim, true, false);

		agentView.puppetAnim.SetBool ("Work", true);
		agentView.puppetAnim.SetBool ("Dead", false);

		noHugNum = 0;
	}

	void ActivateSkillInWork(UseSkill skill)
	{
		// play animator

		//CreatureUnit unit = CreatureLayer.currentLayer.GetCreature (model.instanceId);
		//uni
		//skill.agent.LoseControl ();

		//skill.PauseWorking ();
		isKilling = true;

		//skill.agent.Die ();

		killTarget = skill.agent;

		skill.agent.LoseControl ();
		skill.agent.MoveToNode (model.GetCustomNode ());
	}

    public override SkillTypeInfo GetSpecialSkill()
    {
        //return null;
        return SkillTypeList.instance.GetData(40002);
    }
}
