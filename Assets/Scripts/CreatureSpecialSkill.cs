using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class CreatureSpecialSkill : IObserver{
    public CreatureModel model;
    public Sefira sefira;

    public bool Activated = false;

    public CreatureSpecialSkill() { }
    
    public CreatureSpecialSkill(CreatureModel model) {
        this.model = model;
        Notice.instance.Observe(NoticeName.FixedUpdate, this);
    }

    public virtual void SkillActivate() { 
        
    }

    public virtual void FixedUpdate() { 
        
    }

    public virtual void OnStageStart() { 
        
    }

    public virtual void Activate() {
        if (this.Activated == true) {
            return;
        }
        this.Activated = true;
    }

    public virtual void DeActivate() {
        if (this.Activated == false) {
            return;
        }
        this.Activated = false;
    }

    public void OnNotice(string notice, params object[] param)
    {
        if (this.Activated) {
            if (notice == NoticeName.FixedUpdate)
            {
                FixedUpdate();
            }
        }
    }
}

public class RedShoesSkill : CreatureSpecialSkill, IObserver{
    public AgentModel attractTargetAgent;
    List<AgentModel> targetList;
    const float frequencey = 5f;
    float elapsed = 0f;
    bool Attracted = false;

    public RedShoesSkill(CreatureModel model) {
        this.model = model;
        //this.targetList = GetTargetList();
        Notice.instance.Observe(NoticeName.FixedUpdate, this);
    }

    private List<AgentModel> GetTargetList() {
        List<AgentModel> output = new List<AgentModel>();
        
        foreach (AgentModel am in this.sefira.agentList) {
            if (am.gender == "Female") {
                output.Add(am);
            }
        }
        if (output.Count == 0) {
            Debug.Log("Female is not exist");
        }
        return output;
    }
    
    public override void FixedUpdate()
    {
        
        if (this.Attracted)
        {
            //Call targeted Agent to Creature room and try 

        }
        else {

            elapsed += Time.deltaTime;
            if (elapsed > frequencey)
            {
                
                elapsed = 0f;
                TryAttract();
            }
        }
        
    }

    public override void OnStageStart()
    {
        this.sefira = model.sefira;
        //Debug.Log(this.sefira.name);
        this.targetList = GetTargetList();
    }
    
    public override void SkillActivate()
    {
        Debug.Log("attracted " + this.attractTargetAgent.name);
    }

    private void TryAttract() {
        AgentModel target = null;
        if (this.targetList.Count == 0) return;
        int randIndex = UnityEngine.Random.Range(0, this.targetList.Count);
        target = targetList[randIndex];
        /*
            20%확률로 매혹 판정
         */
        float randval = UnityEngine.Random.Range(0, 5);
        if (randval == 0)
        {
            Debug.Log("걸림" + randval);
            this.attractTargetAgent = target;
            this.Attracted = true;
            return;
        }
        else {

            Debug.Log("안걸림" + randval);
        }
    }


    void IObserver.OnNotice(string notice, params object[] param)
    {
        base.OnNotice(notice, param);
    }
}
