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

    public bool shouldSefiraCheck;
    [HideInInspector]
    public SpaceObjectType type;

	// Use this for initialization
	void Start () {
		if (model.GetElevatorType () == ElevatorType.LONG) {
            this.type = SpaceObjectType.ELEVATORLONG;
			elevatorLong.gameObject.SetActive (true);
			elevatorFrameLong.gameObject.SetActive (true);

			elevatorShort.gameObject.SetActive (false);
			elevatorFrameShort.gameObject.SetActive (false);
		} else {
            this.type = SpaceObjectType.ELEVATORSHORT;
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

    public void SetSprite(Sefira currentSefira) {
        elevatorFrameLong.sprite = SefiraController.instance.GetSefiraSprite(currentSefira).ElevatorLongFrame;
        elevatorFrameShort.sprite = SefiraController.instance.GetSefiraSprite(currentSefira).ElevatorShortFrame;
    }
}
