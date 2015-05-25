using UnityEngine;
using System.Collections;

public class OpenLeftDoor : MonoBehaviour {

    public bool openLeftDoor=false;
    public bool closeRightDoor = false;
    public bool onceCheck=false;

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
            if (GlobalFunction.finishWork && !onceCheck)
            {
                onceCheck = true;
                openLeftDoor = true;
                closeRightDoor = true;
            }
        }

        if (coll.gameObject.tag == "SlideDoorLeft")
        {
            if(GlobalFunction.finishWork)
                SlideDoor.leftDoorClose = true;
            openLeftDoor = false;
        }
    }
}
