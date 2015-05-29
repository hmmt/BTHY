using UnityEngine;
using System.Collections;

public class ResetSlide : MonoBehaviour {

    public SlideDoor slideDoor;
    public OpenRightDoor rightDoor;
    public OpenLeftDoor leftDoor;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	
	}

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Player")
        {


            slideDoor.leftDoorOpen = false;
            slideDoor.rightDoorOpen = false;
            slideDoor.onceCheck = false;
            SlideDoor.leftDoorClose = false;

            rightDoor.playerThroguh = false;
            rightDoor.checkOnce = false;

            leftDoor.openLeftDoor = false;
            leftDoor.closeRightDoor = false;
            leftDoor.onceCheck = false;

            GlobalFunction.finishWork = false;
        }
    }
}
