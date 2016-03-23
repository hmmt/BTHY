using UnityEngine;
using System.Collections.Generic;

public class ElevatorPassageModel {

	private enum ElevatorDirection
	{
		STOP,
		UP,
		DOWN
	}

	private class EnteredUnit
	{
		public WorkerModel unit;
		public MapNode destination;

		public EnteredUnit()
		{
		}

		public EnteredUnit(WorkerModel unit, MapNode destination)
		{
			this.unit = unit;
			this.destination = destination;
		}
	}
	private class FloorInfo
	{
		public MapNode exitNode;
		public Vector3 position;
		public FloorInfo(MapNode exitNode, Vector3 position)
		{
			this.exitNode = exitNode;
			this.position = position;
		}
	}

	private MapNode elevatorNode;

	private List<MapNode> innerNodes;
	private List<Vector3> nodeOrigin;

	private List<EnteredUnit> enteredList;
	private ElevatorDirection currentDirection;
	//private Vector3 position;
	private List<FloorInfo> floorList;
	private List<bool> buttonClicked;

	private float currentPos; // 0~1
	private int destinationFloor = 0;

	private float waitTimer = 2;

	public ElevatorPassageModel(MapNode elevatorNode)
	{
		this.elevatorNode = elevatorNode;
		
		innerNodes = new List<MapNode> ();
		nodeOrigin = new List<Vector3> ();

		enteredList = new List<EnteredUnit> ();
		floorList = new List<FloorInfo> ();

		buttonClicked = new List<bool> ();
	}

	public MapNode GetNode()
	{
		return elevatorNode;
	}

	public float GetCurrentPos()
	{
		return currentPos;
	}

	public void AddNode(MapNode node)
	{
		innerNodes.Add (node);
		nodeOrigin.Add (node.GetPosition ());
	}

	public void AddFloorInfo(MapNode node, Vector3 position)
	{
		floorList.Add (new FloorInfo (node, position));
		buttonClicked.Add (false);
	}

	public MapNode GetCurrentFloorNode()
	{
		if (currentPos <= 0) {
			return floorList [0].exitNode;
		} else if (currentPos >= 1) {
			return floorList [1].exitNode;
		}
		return null;
	}

	public Vector3 GetElevatorPosition()
	{
		Vector3 vp0 = floorList [0].position;
		Vector3 vp1 = floorList [1].position;

		 return vp0 + (vp1 - vp0) * currentPos;
	}



	public void OnUnitEnter(WorkerModel unit, MapNode destination)
	{
		enteredList.Add (new EnteredUnit (unit, destination));
		unit.SetCurrentNode (innerNodes [Random.Range(0,innerNodes.Count)]);
	}

	public void OnUnitExit(WorkerModel unit)
	{
		for (int i = 0; i < enteredList.Count; i++) {
			if (enteredList [i].unit == unit) {
				//int floor = enteredList [i].destination;
				//unit.GetMovableNode ().SetCurrentNode (floorList [floor]);
				unit.GetMovableNode().ExitElevator(enteredList [i].destination);

				enteredList.RemoveAt (i);
				break;
			}
		}
	}

	public void StartMove()
	{
		if (currentPos <= 0) {
			currentDirection = ElevatorDirection.UP;
			destinationFloor = 1;
		} else if (currentPos >= 1) {
			currentDirection = ElevatorDirection.DOWN;
			destinationFloor = 0;
		}
	}


	public void FinishMove(int floor)
	{
		MapNode floorNode = floorList [floor].exitNode;
		List<EnteredUnit> outList = new List<EnteredUnit> ();
		foreach (EnteredUnit u in enteredList)
		{
			if (u.destination == floorNode) {
				outList.Add (u);
			}
		}

		foreach (EnteredUnit o in outList) {
			OnUnitExit (o.unit);
		}
	}

	private void UpdateMapNodePosition()
	{
		Vector3 vp0 = floorList [0].position;
		Vector3 vp1 = floorList [1].position;

		Vector3 addPos = vp0 + (vp1 - vp0) * currentPos;

		for (int i = 0; i < innerNodes.Count; i++) {
			innerNodes [i].SetPosition (nodeOrigin [i] + addPos);
		}
	}

	public void ClickButton(MapNode callNode)
	{
		if (floorList [0].exitNode == callNode)
		{
			buttonClicked [0] = true;
		}
		else if (floorList [1].exitNode == callNode)
		{
			buttonClicked [1] = true;
		}
	}

	public void OnFixedUpdate()
	{
		if (currentDirection == ElevatorDirection.UP)
		{
			waitTimer = 2;
			if (currentPos < destinationFloor)
			{
				currentPos += Time.deltaTime;
			}
			if (currentPos >= destinationFloor)
			{
				currentPos = destinationFloor;
				currentDirection = ElevatorDirection.STOP;
				FinishMove (destinationFloor);

				buttonClicked [1] = false;
			}
		}
		else if (currentDirection == ElevatorDirection.DOWN)
		{
			waitTimer = 2;
			if (currentPos > destinationFloor)
			{
				currentPos -= Time.deltaTime;
			}
			if (currentPos <= destinationFloor)
			{
				currentPos = destinationFloor;
				currentDirection = ElevatorDirection.STOP;
				FinishMove (destinationFloor);

				buttonClicked [0] = false;
			}
		}
		else // STOP
		{
			if (enteredList.Count > 0 || buttonClicked [0] || buttonClicked [1]) {
				if (waitTimer > 0) {
					waitTimer -= Time.deltaTime; 
				} else {
					StartMove ();
					waitTimer = 2;
				}
			}
		}

		UpdateMapNodePosition ();
	}
}
