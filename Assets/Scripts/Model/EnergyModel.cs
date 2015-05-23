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

	public void Init()
	{
		energy = 0;
	}

	public void AddEnergy(float added)
	{
		this.energy += added;
	}

	public float GetEnergy()
	{
		return energy;
	}

	public void UpdateEnergy()
	{
		CreatureUnit[] units = CreatureFacade.instance.GetCreatureList ();
		
		foreach(CreatureUnit unit in units)
		{
			float addedEnergy = 1;

			int feelingTick = unit.metaInfo.feelingMax / unit.metaInfo.genEnergy.Length;
			addedEnergy = unit.metaInfo.genEnergy[Mathf.Clamp(unit.feeling/feelingTick, 0, unit.metaInfo.genEnergy.Length-1)];
			AddEnergy(addedEnergy);
			if(addedEnergy > 0)
			{
				TextAppearEffect.Create((Vector2)unit.transform.position, "+" + addedEnergy.ToString(), Color.white);
			}
			else if(addedEnergy < 0)
			{
				TextAppearEffect.Create((Vector2)unit.transform.position, addedEnergy.ToString(), Color.white);
			}
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
