using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Uncontrollable_Nullthing : UncontrollableAction
{
    WorkerModel currentModel;
    CreatureModel creatureModel;

    public Uncontrollable_Nullthing(WorkerModel model, NullCreature script) {
        creatureModel = script.GetModel();
        currentModel = model;
    }

    public override void Init()
    {
        //base.Init();
    }

    public override void Execute()
    {
        //Do likewise agent action
    }

    public override void OnDie()
    {
        //if change collapsed mode or make egg
    }

    public override void OnClick()
    {
        //current nothing to clicked. Just appear suppresswindow.
        if (currentModel is AgentModel) SuppressWindow.CreateWindow(currentModel as AgentModel);
        else if (currentModel is OfficerModel) SuppressWindow.CreateWindow(currentModel as OfficerModel);
    }

}