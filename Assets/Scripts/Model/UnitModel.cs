using UnityEngine;
using System.Collections;

public class UnitModel : ObjectModelBase {

	protected MovableObjectNode movableNode;

	public virtual bool CanOpenDoor()
	{
		return true;
	}

	public virtual void InteractWithDoor(DoorObjectModel door)
	{
	}

	public virtual void OnStopMovableByShield(AgentModel shielder)
	{
	}
}
