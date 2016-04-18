﻿using UnityEngine;
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
        Animator agentAnim = null;
        if (!this.skillReady)
        {

            Debug.Log("넌 아직 준비가 안됐다");
            return;
        }

        if (target is AgentModel)
        {
            this.currentWorker = target as AgentModel;
            agentAnim =  AgentLayer.currentLayer.GetAgent(this.currentWorker.instanceId).puppetAnim;
            AnimatorManager.instance.ResetAnimatorTransform(this.currentWorker.instanceId);
            AnimatorManager.instance.ChangeAnimatorByName(this.currentWorker.instanceId, AnimatorName.OneBad,
                                                       agentAnim, true, false);

        }
        else {
            Debug.Log("Error in One Sin Skill");
            return;
        }
        
        Animator creatureAnimator = CreatureLayer.currentLayer.GetCreature(model.instanceId).creatureAnimator;
        //Feeling make zero
        this.ReadySkill(false);
        this.model.SubFeeling(1000);
        creatureAnimator.SetBool("Work", true);
        float recoveryValue= (float)this.currentWorker.maxMental / 2;
        int randVal = UnityEngine.Random.Range(0, 10);

        if (currentWorker.HasTrait(62)) {
            randVal = 0;
        }

        if (randVal == 0) { 
            //fail
            //FailWorkDamage();
            creatureAnimator.SetBool("Success", false);
            agentAnim.SetBool("Kill", true);
            this.currentWorker.TakeMentalDamage((int)(recoveryValue));
            Debug.Log("Mental Damage Taken");
            return;
        }
        creatureAnimator.SetBool("Success", true);
        this.currentWorker.RecoverMental((int)recoveryValue);
        
        Debug.Log("Mental recovered");
        this.currentWorker = null;
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


    void IObserver.OnNotice(string notice, params object[] param)
    {
        base.OnNotice(notice, param);
    }
}