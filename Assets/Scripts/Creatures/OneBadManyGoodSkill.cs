using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class OneBadManyGoodSkill : CreatureSpecialSkill, IObserver {
    AgentModel currentWorker;
    bool skillReady;

    public OneBadManyGoodSkill(CreatureModel model) {
        this.model = model;
        Notice.instance.Observe(NoticeName.FixedUpdate, this);
        
    }

    public override void FixedUpdate()
    {
        //base.FixedUpdate();
    }

    public override void SkillActivate(WorkerModel target)
    {
        if (target is AgentModel)
        {
            this.currentWorker = target as AgentModel;
        }
        else {
            Debug.Log("Error in One Sin Skill");
            return;
        }

    }

    public override void SkillActivate()
    {
        if (!this.skillReady) {
            return;
        }
    }

    public void ReadySkill(bool state) {
        this.skillReady = state;
    }

    public bool GetSkillState() {
        return this.skillReady;
    }

}