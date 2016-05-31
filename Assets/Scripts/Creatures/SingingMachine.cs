using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class SingingMachine : CreatureBase {

    public override void OnInit()
    {
        this.skill = new SingingMachineSkill(this.model);
    }

    public override void OnSkillFailWorkTick(UseSkill skill)
    {
        base.OnSkillFailWorkTick(skill);
    }

    public override void OnFixedUpdate(CreatureModel creature)
    {
		//if (creature.GetFeelingPercent() < 30f)
        if (creature.GetFeelingPercent() < 100f)
		//if (creature.GetFeelingPercent() < 110f )
        {
            this.skill.Activate();
        }
		else
		{
            this.skill.DeActivate();
        }
    }

    public override void OnEnterRoom(UseSkill skill)
    {
        if (this.skill.Activated) {
            Debug.Log("Found");
            (this.skill as SingingMachineSkill).SkillActivate(skill.agent);
        }
    }

	public override bool IsEscapable()
	{
		return false;
	}

	public override void AgentAnimCalled(int i, WorkerModel actor)
	{
		switch (i)
		{
		case 1:
			if (actor is AgentModel) {
				AgentUnit agentView = AgentLayer.currentLayer.GetAgent (actor.instanceId);
				agentView.MannualMovingCallWithTime (agentView.transform.localPosition + new Vector3 (-2f, 0, 0), 4f);
			}
			else if(actor is OfficerModel){
				OfficerUnit officerView = OfficerLayer.currentLayer.GetOfficer (actor.instanceId);
				officerView.MannualMovingCallWithTime (officerView.transform.localPosition + new Vector3 (-2f, 0, 0), 4f);
			}
			break;
		}
	}

	public override void OnViewInit(CreatureUnit unit)
	{
		MachineAnim animScript = unit.animTarget as MachineAnim;
		animScript.Init(this);
	}

	public CreatureUnit GetCurrentCreatureUnit()
	{
		return CreatureLayer.currentLayer.GetCreature (model.instanceId);
	}

	public void PlayAttackEffect()
	{

		GameObject ge = Prefab.LoadPrefab("Effect/MachineAttackBlood");
		Vector3 pos = this.model.GetMovableNode().GetCurrentViewPosition();

		ge.transform.localPosition = pos + new Vector3 (0, 0, 0.5f);
		//ge.transform.SetParent(this.body.gameObject.transform);
		//ge.transform.localPosition = Vector3.zero;
	}
}
