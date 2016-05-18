using UnityEngine;
using System.Collections;

public class EnergyModel : IObserver {
    
	private static EnergyModel _instance = null;

	public static EnergyModel instance
	{
		get{
			if(_instance == null)
			{
				_instance = new EnergyModel();
			}
			return _instance;
		}
	}

	private float energy = 0;
    private float leftEnergy = 40;
    private float stageLeftEnergy = 0;

    public EnergyModel()
    {
        Init();
    }
	public void Init()
	{
		energy = 0;
	}

	public void AddEnergy(float added)
	{
		this.energy += added;
	}

    public void SubEnergy(float sub)
    {
        this.energy -= sub;
    }

	public float GetEnergy()
	{
		return energy;
	}

    public void SetStageLeftEnergy(float energy)
    {
        stageLeftEnergy = energy;
    }

    public int GetStageLeftEnergy()
    {
        return (int)stageLeftEnergy;
    }

    public void SetLeftEnergy(float left)
    {
        leftEnergy = left;
    }

    public float GetLeftEnergy()
    {
        return leftEnergy;
    }

	private void UpdateEnergy()
	{
		CreatureModel[] units = CreatureManager.instance.GetCreatureList ();
		
		foreach(CreatureModel unit in units)
		{
			float addedEnergy = 1;
            /*
			int feelingTick = unit.metaInfo.feelingMax / unit.metaInfo.genEnergy.Length;
			addedEnergy = unit.metaInfo.genEnergy[Mathf.Clamp((int)(unit.feeling)/feelingTick, 0, unit.metaInfo.genEnergy.Length-1)];
            */
            if(unit.sefiraEmpty)
            {
                addedEnergy = (unit.GetEnergyTick()) / 2;
                if(addedEnergy < 0)
                {
                    addedEnergy = addedEnergy * 2;
                }
            }
            else
            {
                addedEnergy = unit.GetEnergyTick();
            }

			// temp for proto

			/*
			if (unit.energyPoint < 90)
				addedEnergy = (unit.energyPoint - 90) / 5f;
			else if (unit.energyPoint >= 110)
				addedEnergy = (unit.energyPoint - 110) / 5f;
			else
				addedEnergy = 0;
			*/

			addedEnergy = 0;

			//if(addedEnergy

			AddEnergy(addedEnergy);

			/*
			if(addedEnergy > 0)
			{
				TextAppearEffect.Create((Vector2)unit.GetCurrentViewPosition(), "+" + addedEnergy.ToString(), Color.white);
			}
			else if(addedEnergy < 0)
			{
                TextAppearEffect.Create((Vector2)unit.GetCurrentViewPosition(), addedEnergy.ToString(), Color.white);
			}
			*/
		}
		
		Notice.instance.Send("UpdateEnergy");
	}

	public void OnNotice(string notice, params object[] param)
	{
		if(notice == "EnergyTimer")
		{
			UpdateEnergy();
		}
	}
}
