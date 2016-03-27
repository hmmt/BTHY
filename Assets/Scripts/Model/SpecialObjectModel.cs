using UnityEngine;
using System.Collections;

[System.Serializable]
public class SpecialObjectModel : ObjectModelBase {

	public int instanceId;

	MovableObjectNode movableNode;

	public SpecialObjectModel()
	{
		movableNode = new MovableObjectNode(this);
	}

	public void OnFixedUpdate()
	{
		movableNode.ProcessMoveNode(4);
	}
}
