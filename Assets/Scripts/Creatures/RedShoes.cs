using UnityEngine;
using System.Collections;

public class RedShoes : CreatureBase {
	

	public bool dropped = false;

	public Vector3 droppedShoesPosition;
	public MovableObjectNode droppedPositionNode;
	public PassageObjectModel droppedPassage;


	public AgentModel owner = null;

	// 

    public override void OnInit()
    {
        this.skill = new RedShoesSkill(this.model);
    }

    // temporary
    public override void OnSkillFailWorkTick(UseSkill skill)
    {
        ActivateSkill(skill);
    }

    public override void OnFixedUpdate(CreatureModel creature)
    {
		//return;
        if (creature.GetFeelingPercent() <= 30f && this.skill.Activated == false)
        {
            this.skill.Activate();
        }
        else if (creature.GetFeelingPercent() > 30f && this.skill.Activated == true){
            if (this.skill.Activated == true) {
                this.skill.DeActivate();
            }
        }

		if (dropped)
		{
			foreach (AgentModel agent in AgentManager.instance.GetAgentList())
			{
			}
				
			foreach (AgentModel agent in AgentManager.instance.GetAgentList())
			{
				if (agent.gender == "Female")
					continue;
				//if (agent.GetMovableNode ().GetPassage () == droppedPassage)
				{
					if (agent.GetState () == AgentAIState.RETURN_CREATURE)
						continue;
					if ((agent.GetCurrentViewPosition () - droppedShoesPosition).sqrMagnitude < 2)
					{
						dropped = false;
						agent.ReturnCreature (model);
						break;
					}
				}
			}
		}
    }

	public void ReturnShoesByAgent(AgentModel agent)
	{
		owner = agent;
	}

	public void ReturnFinish()
	{
		dropped = false;
		owner = null;

		model.AddFeeling (100f);
		this.skill.DeActivate();

		model.SendAnimMessage ("ReturnRedShoesAnim");
	}

    public void ActivateSkill(UseSkill skill)
    {
        // show effect
		/*
        skill.PauseWorking();
        ///SoundEffectPlayer.PlayOnce("creature/match_girl/matchgirl_ability.wav", skill.targetCreature.transform.position);

        OutsideTextEffect effect = OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/redshoes/redShoes_AttackTypo_01", CreatureOutsideTextLayout.CENTER_BOTTOM, 0, 4.0f).DisableFade();
        effect.transform.localScale = new Vector3(1.1f, 1.1f, 1);

        // skill이 이미 release 될 상황 고려 필요
        effect.GetComponent<DestroyHandler>().AddReceiver(delegate() { skill.ResumeWorking(); });

        OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/redshoes/redShoes_AttackTypo_02", CreatureOutsideTextLayout.CENTER_BOTTOM, 0.5f, 3.5f).DisableFade()
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/redshoes/redShoes_AttackTypo_03", CreatureOutsideTextLayout.CENTER_BOTTOM, 1.0f, 3.0f).DisableFade()
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/redshoes/redShoes_AttackTypo_04", CreatureOutsideTextLayout.CENTER_BOTTOM, 1.5f, 2.5f).DisableFade()
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/redshoes/redShoes_AttackTypo_05", CreatureOutsideTextLayout.CENTER_BOTTOM, 2.0f, 2.0f).DisableFade()
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        */
    }

    //

    public override void OnEnterRoom(UseSkill skill)
    {
        if (this.skill.Activated == false) {
            if (skill.agent.gender == "Female") {
                skill.PauseWorking();
                Debug.Log("Attract in Room");
                (this.skill as RedShoesSkill).AttractInIsolate(skill.agent);
            }
        }


		/*
        skill.PauseWorking();

        OutsideTextEffect effect = OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/redshoes/redShoes_EnterTypo_01", CreatureOutsideTextLayout.CENTER_BOTTOM, 0.5f, 5.0f);
        effect.transform.localScale = new Vector3(1.1f, 1.1f, 1);

        // skill이 이미 release 될 상황 고려 필요
        effect.GetComponent<DestroyHandler>().AddReceiver(delegate() { skill.ResumeWorking(); });

        OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/redshoes/redShoes_EnterTypo_02", CreatureOutsideTextLayout.CENTER_BOTTOM, 1.0f, 4.5f)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/redshoes/redShoes_EnterTypo_03", CreatureOutsideTextLayout.CENTER_BOTTOM, 1.5f, 4.0f)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/redshoes/redShoes_EnterTypo_04", CreatureOutsideTextLayout.CENTER_BOTTOM, 2.0f, 3.5f)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/redshoes/redShoes_EnterTypo_05", CreatureOutsideTextLayout.CENTER_BOTTOM, 2.5f, 3.0f)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/redshoes/redShoes_EnterTypo_06", CreatureOutsideTextLayout.CENTER_BOTTOM, 3.0f, 2.5f)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/redshoes/redShoes_EnterTypo_07", CreatureOutsideTextLayout.CENTER_BOTTOM, 3.5f, 2.0f)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        */
    }

	public override bool IsEscapable()
	{
		return false;
	}

    
}
