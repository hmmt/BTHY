using UnityEngine;
using System.Collections;



public class Path {
	public enum AIDirection
	{
		NONE,
		LEFT,
		RIGHT,
		TOP,
		BOTTOM
	}

	private ArrayList directionList = new ArrayList();
	private int curIndex = 0;

	public void AddMoveDirection(AIDirection dir)
	{
		directionList.Add (dir);
	}

	public AIDirection PopDirection()
	{
		if(curIndex < directionList.Count )
		{
			return (AIDirection)directionList[curIndex++];
		}
		return AIDirection.NONE;
	}
}
