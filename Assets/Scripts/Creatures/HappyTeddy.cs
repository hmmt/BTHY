using UnityEngine;
using System.Collections;

public class HappyTeddy  : CreatureBase {

    private bool b = false;

	AgentModel lastAgent;
	int teddyWorkNum;
	int noHugNum;
	int normalWorkCount;


	bool huging = false;

	bool isPlayingDeadScene = false;
	bool isKilling = false;
	AgentModel killTarget = null;

	public override void OnViewInit (CreatureUnit unit)
	{
		//GameObject g = new GameObject ();
		//g.AddComponent<UnityEngine.UI.Text>()
	}
    
	public override void OnFixedUpdateInSkill (UseSkill skill)
	{
		base.OnFixedUpdateInSkill (skill);

		if (huging && skill.IsWorking() == false &&
			skill.agent.GetCurrentNode() == model.GetCustomNode())
		{
			skill.ResumeWorking ();

			AgentUnit agentView = AgentLayer.currentLayer.GetAgent (skill.agent.instanceId);

			AnimatorManager.instance.ResetAnimatorTransform (skill.agent.instanceId);
			AnimatorManager.instance.ChangeAnimatorByName (skill.agent.instanceId, AnimatorName.Teddy_agent,
				agentView.puppetAnim, true, false);

			agentView.puppetAnim.SetBool ("Work", true);
			agentView.puppetAnim.SetBool ("Dead", false);

			noHugNum = 0;
		}
	}
	public override void OnFixedUpdate (CreatureModel creature)
	{
		base.OnFixedUpdate (creature);

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
				isPlayingDeadScene = true;
			}
		}

		if (isPlayingDeadScene)
		{
			if (model.GetAnimScript ().animator.GetInteger ("KillMoment") == 1)
			{
				isPlayingDeadScene = false;
				if(model.state == CreatureState.WORKING_SCENE)
					model.state = CreatureState.WAIT;
			}
		}
	}

	public override void OnReturn ()
	{
		model.energyPoint = 130;
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
		else
		{
			teddyWorkNum = 0;
		}
		lastAgent = skill.agent;

		float hugProb = 0;
		float agentProb = 0.1f * teddyWorkNum;

		if (skill.skillTypeInfo == GetSpecialSkill ())
		{
			hugProb = 0.1f + agentProb;// + 0.8f;
		}
		else
		{
			hugProb = 0.2f * noHugNum + agentProb + 1.9f;// + 0.8f;
		}

		Debug.Log ("hug prob : " + hugProb + "(teddyWorkNum:"+teddyWorkNum+", noHugNum:"+noHugNum+")");
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
		if (skill.agent.isDead () == false && huging)
		{
			skill.agentView.ResetZValue ();
			skill.agent.ResetAnimator ();
		}

		huging = false;
	}

	void HugSkill(UseSkill skill)
	{
		//model.SendAnimMessage ("SpecialAttack");

		huging = true;
		skill.agentView.zValue = -1.1f;

		skill.PauseWorking ();

		skill.agent.MoveToNode (model.GetCustomNode (), false);
	}

	void ActivateSkillInWork(UseSkill skill)
	{
		isKilling = true;

		killTarget = skill.agent;

		model.state = CreatureState.WORKING_SCENE;
		skill.agent.LoseControl ();
		skill.agent.MoveToNode (model.GetCustomNode ());
	}

    public override SkillTypeInfo GetSpecialSkill()
    {
        //return null;
        return SkillTypeList.instance.GetData(40002);
    }

	public override string GetDebugText ()
	{
		//return base.GetDebugText ();
		return "teddyWorkNum:"+teddyWorkNum + ", " + "noHugNum:" + noHugNum;
	}
}
