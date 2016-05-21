using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class OneBadManyGoodSkill : CreatureSpecialSkill, IObserver, IAnimatorEventCalled {
    AgentModel currentWorker;
    bool skillReady;
    bool success = true;
    Animator creatureAnimator = null;



    Vector3 effectPos;
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
        /*
        if (!this.skillReady)
        {

            Debug.Log("넌 아직 준비가 안됐다");
            return;
        }*/

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

        if (creatureAnimator == null)
        {
            creatureAnimator = CreatureLayer.currentLayer.GetCreature(model.instanceId).creatureAnimator;
            this.AnimatorEventInit();
        }
        
        //Feeling make zero
        this.ReadySkill(false);
        this.model.SubFeeling(1000);
        creatureAnimator.SetBool("Work", true);
        float recoveryValue= (float)this.currentWorker.maxMental / 2;
        int randVal = UnityEngine.Random.Range(0, 10);
        effectPos = this.currentWorker.GetCurrentViewPosition();
        /*
        if (currentWorker.HasTrait(62)) {
            randVal = 0;
        }*/

        if (randVal == 0) { 
            //fail
            //FailWorkDamage();
            this.success = false;
            creatureAnimator.SetBool("Kill", true);
            agentAnim.SetBool("Success", false);
            this.currentWorker.TakeMentalDamage((int)(recoveryValue));
            return;
        }

        creatureAnimator.SetBool("Success", true);
        this.currentWorker.RecoverMental((int)recoveryValue);
        
        //this.currentWorker = null;
        
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

    public bool GetSuccessState() {
        return this.success;
    }

    public void InitSuccessState() {
        this.success = true;
    }

    public void ResetAnimator() { 
        
    }


    public void OnCalled()
    {
        ParticleDestroy des = null;
        GameObject target = null;
        Vector3 pos;
        if (this.success)
        {
            target = Prefab.LoadPrefab("Effect/Creature/OneBad/Ray");
            target.transform.position = new Vector3(effectPos.x, effectPos.y-1f, effectPos.z - 0.5f);
        }
        else {
            target = Prefab.LoadPrefab("Effect/Creature/OneBad/Thunder");
            target.transform.position = new Vector3(effectPos.x, effectPos.y-0.7f, effectPos.z - 0.5f);
        }
        des = target.GetComponent<ParticleDestroy>();
        target.SetActive(true);
        //set Position;

        if (this.success) {

            des.DelayedDestroy(10);
        }
        des.DelayedDestroy(5);

        InitSuccessState();
    }

    public void AnimatorEventInit()
    {
        AnimatorEventScript eventScript = this.creatureAnimator.GetComponent<AnimatorEventScript>();
        eventScript.SetTarget(this);
    }

    public void OnCalled(int i) {
        
    }

    public void AgentReset() { }

    public void CreatureAnimCall(int i, CreatureBase script)
    { 
        
    }

    public void TakeDamageAnim(int isPhysical) { }

    public void AttackCalled(int i)
    {

    }
}