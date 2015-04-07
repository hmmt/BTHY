using UnityEngine;
using System.Collections;

public class PanicRoaming : PanicAction {

	private AgentUnit targetAgent;

	public PanicRoaming(AgentUnit target)
	{
		targetAgent = target;
	}

	public void Execute()
	{
		/*
		if(targetAgent.isMoving() == false)
		{
			Vector2 roamingPoint = CreatureRoom.instance.GetRandomRoamingPoint();
			targetAgent.MoveToTilePos((int)roamingPoint.x, (int)roamingPoint.y);
		}
		*/
	}
}
