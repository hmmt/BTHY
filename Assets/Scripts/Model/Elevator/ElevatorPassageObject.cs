using UnityEngine;
using System.Collections;

public class ElevatorPassageObject : MonoBehaviour {

	[HideInInspector]
	public ElevatorPassageModel model;

	public ElevatorObject elevator;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate()
	{
		if (model == null)
			return;
		
		elevator.transform.localPosition = model.GetElevatorPosition() - transform.localPosition;
	}
}
