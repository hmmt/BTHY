using UnityEngine;
using System.Collections;

public class WorkerAnim : MonoBehaviour {

	//private string moti;

	public string[] motions;

	public bool ContainsMotion(string motionName)
	{
		foreach (string m in motions) {
			if (m == motionName)
				return true;
		}
		return false;
	}

	public void SetMotion(string motionName)
	{
		
	}
}
