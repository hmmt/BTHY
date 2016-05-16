using UnityEngine;
using System.Collections;

public class ElevatorPassageObject : MonoBehaviour {

	[HideInInspector]
	public ElevatorPassageModel model;

	public ElevatorObject elevator;

	public SpriteRenderer elevatorLong;
	public SpriteRenderer elevatorShort;


	public SpriteRenderer elevatorFrameLong;
	public SpriteRenderer elevatorFrameShort;

	// Use this for initialization
	void Start () {
		if (model.GetElevatorType () == ElevatorType.LONG) {
			elevatorLong.gameObject.SetActive (true);
			elevatorFrameLong.gameObject.SetActive (true);

			elevatorShort.gameObject.SetActive (false);
			elevatorFrameShort.gameObject.SetActive (false);
		} else {
			elevatorLong.gameObject.SetActive (false);
			elevatorFrameLong.gameObject.SetActive (false);

			elevatorShort.gameObject.SetActive (true);
			elevatorFrameShort.gameObject.SetActive (true);
		}
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
