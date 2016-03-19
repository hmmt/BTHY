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

	private List<MapNode> innerNodes;
	private List<EnteredUnit> enteredList;
	private ElevatorDirection currentDirection;
	//private Vector3 position;
	private List<FloorInfo> floorList;

	private float currentPos; // 0~1
	private int destinationFloor = 0;

	public ElevatorPassageModel()
	{
		innerNodes = new List<MapNode> ();
		enteredList = new List<EnteredUnit> ();
		floorList = new List<FloorInfo> ();
	}

	public void AddNode(MapNode node)
	{
		innerNodes.Add (node);
	}

	public void AddFloorInfo(MapNode node, Vector3 position)
	{
		floorList.Add (new FloorInfo (node, position));
	}

	public void OnUnitEnter(WorkerModel unit, MapNode destination)
	{
		enteredList.Add (new EnteredUnit (unit, destination));
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

	public void OnFixedUpdate()
	{
		if (currentDirection == ElevatorDirection.UP)
		{
			if (currentPos < destinationFloor)
			{
				currentPos += Time.deltaTime;
				if (currentPos >= destinationFloor)
				{
					currentPos = destinationFloor;
					currentDirection = ElevatorDirection.STOP;
					FinishMove (destinationFloor);
				}
			}
		}
		else if (currentDirection == ElevatorDirection.DOWN)
		{
			if (currentPos > destinationFloor)
			{
				currentPos -= Time.deltaTime;
				if (currentPos <= destinationFloor)
				{
					currentPos = destinationFloor;
					currentDirection = ElevatorDirection.STOP;
					FinishMove (destinationFloor);
				}
			}
		}
	}
}
