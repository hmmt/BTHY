using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class SiningMachineSkill : CreatureSpecialSkill, IObserver {
    public AgentModel attractTargetAgent;
    List<AgentModel> targetList;
    const float frequency = 5f;
    float elapsed = 0f;
    bool Attracted = false;

    List<AgentModel> workedList;

    public SiningMachineSkill(CreatureModel model)
    {
        this.model = model;

        Notice.instance.Observe(NoticeName.FixedUpdate, this);
    }

    private List<AgentModel> GetTargetList() {
        List<AgentModel> output = new List<AgentModel>();

        foreach (AgentModel am in this.workedList) {
            if (am.isDead() == true) {
                output.Add(am);
            }
        }

        if (output.Count == 0) {
            Debug.Log("No list");
        }
        return output;
    }

    public override void FixedUpdate()
    {
        
    }

    public override void OnStageStart()
    {
        base.OnStageStart();
    }

    public override void SkillActivate()
    {
        base.SkillActivate();
    }

    private void TryAttract() { 
        
    }

    public void SetSuppressed() { 
        
    }



    void IObserver.OnNotice(string notice, params object[] param)
    {
        base.OnNotice(notice, param);
    }
}
