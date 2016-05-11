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
}
