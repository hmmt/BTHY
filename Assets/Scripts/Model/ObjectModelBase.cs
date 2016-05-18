using UnityEngine;
using System.Collections.Generic;

public class ObjectModelBase
{
    public Vector3 position;
}

public interface MovableObjectModel
{
    MovableObjectNode GetMovableObjectNode();
    void InteractWithDoor(DoorObjectModel door);
	void OnStopMovableByShield (AgentModel shield);
}