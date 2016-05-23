using UnityEngine;
using System.Collections;

public class RedShoes : CreatureBase {
	

	public bool dropFinished = false;
	public bool dropped = false;

	public Vector3 droppedShoesPosition;
	public MovableObjectNode droppedPositionNode;
	public PassageObjectModel droppedPassage;


	public AgentModel owner = null;
	public int returnTargetCount = 0;

	// 

    public override void OnInit()
    {
		model.canBeSuppressed = false;
        this.skill = new RedShoesSkill(this.model);
		//model.SubFeeling (100);
    }

    // temporary
    public override void OnSkillFailWorkTick(UseSkill skill)
    {
        ActivateSkill(skill);
    }

    public override void OnFixedUpdate(CreatureModel creature)
    {
		//return;
		RedShoesSkill redShoesSkill = (RedShoesSkill)this.skill;

		if (creature.GetFeelingPercent() <= 30f && this.skill.Activated == false)
        {
            this.skill.Activate();
        }
        else if (creature.GetFeelingPercent() > 30f && this.skill.Activated == true)
		{
			if (this.skill.Activated == true && !redShoesSkill.IsAtivatedForcely() )
			{
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
				/*
				if (returnTargetCount > 0)
					break;
				*/
				if (agent.gender == "Female")
					continue;
				{
					if (agent.isDead () || agent.IsPanic () || agent.GetState () == AgentAIState.CANNOT_CONTROLL)
						continue;
					if (agent.GetState () == AgentAIState.RETURN_CREATURE)
						continue;
					if ((agent.GetCurrentViewPosition () - droppedShoesPosition).sqrMagnitude < 36)
					{
						//dropped = false;
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
		model.state = CreatureState.WAIT;
		
		dropped = false;
		dropFinished = false;
		owner = null;

		model.AddFeeling (100f);
		this.skill.DeActivate();

		model.SendAnimMessage ("ReturnRedShoesAnim");
	}

	public void OnDropFinished()
	{
		if (dropped)
		{
			dropFinished = true;
		}
	}

    public void ActivateSkill(UseSkill skill)
    {
        // show effect
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
    }

	public override void AgentAnimCalled(int i, WorkerModel actor)
	{
		switch (i)
		{
		case 0:
			OnDropFinished ();
			break;
		}
	}

	public override bool IsEscapable()
	{
		return false;
	}

    
}
