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
		if (creature.GetFeelingPercent() < 30f && this.skill.Activated == false)
        {
            this.skill.Activate();
        }
		else if(this.skill.Activated == true)
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
}
