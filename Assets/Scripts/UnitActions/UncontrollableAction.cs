﻿using UnityEngine;
using System.Collections;

public class UncontrollableAction {

	public virtual void Init()
	{
	}

	// It is called when agent's state is CANNOT_CONTROL
	public virtual void Execute()
	{
		
	}

	public virtual void OnDestroy()
	{
		// ?
	}

	public virtual void OnDie()
	{
	}

	public virtual void OnClick()
	{
	}


	public virtual void UnderAttack()
	{

	}

	public virtual void OnTakePhysicalDamage(int damage)
	{
		
	}

	public virtual void OnTakeMentalDamage(int damage)
	{
	}
}
