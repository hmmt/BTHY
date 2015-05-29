using UnityEngine;
using System.Collections;

public class SlideDoor : MonoBehaviour {

    public Rigidbody2D leftDoor;
    public Rigidbody2D rightDoor;

    public ElevatorMover elevatorState;

    public PlayerController playerBody;

    public bool leftDoorOpen = false;
    public bool rightDoorOpen = false;

    public OpenRightDoor openRightFlag;
    public OpenLeftDoor openLeftFlag;

    public bool onceCheck = false;
    public static bool leftDoorClose = false;

	// Use this for initialization
	void Awake () {
        leftDoor.isKinematic = true;
        rightDoor.isKinematic = true;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        // 왼쪽문 개방
        if (ElevatorMover.currentState == ElevatorMover.STATE.RSTOP)
        {
            leftDoor.MovePosition (new Vector2(leftDoor.position.x, leftDoor.position.y+0.03f));
        }

        // 왼쪽문 폐쇄
        if (leftDoorOpen)
        {
            leftDoor.MovePosition(new Vector2(leftDoor.position.x, leftDoor.position.y - 0.03f));
            rightDoor.MovePosition(new Vector2(rightDoor.position.x, rightDoor.position.y + 0.03f));
        }

        //오른쪽문 폐쇄 + 오른쪽문폐쇄#2
        if (rightDoorOpen || openLeftFlag.closeRightDoor)
        {
            rightDoor.MovePosition(new Vector2(rightDoor.position.x, rightDoor.position.y - 0.03f));
        }

        //오른쪽문 개방
        if (openRightFlag.playerThroguh)
        {
            rightDoor.MovePosition(new Vector2(rightDoor.position.x, rightDoor.position.y + 0.03f));
            //openRightFlag.playerThroguh = false;
        }

        // 왼쪽문 개방 #2
        if (openLeftFlag.openLeftDoor)
        {
            leftDoor.MovePosition(new Vector2(leftDoor.position.x, leftDoor.position.y + 0.03f));
        }

        //왼쪽문 폐쇄 #2
        if (leftDoorClose &&  elevatorState.playerOn)
        {
            leftDoor.MovePosition(new Vector2(leftDoor.position.x, leftDoor.position.y - 0.03f));
        }
	}

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            if (!GlobalFunction.finishWork)
                leftDoorOpen = true;
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.tag == "Player" && !onceCheck)
        {
            onceCheck = true;
            rightDoorOpen = true;
        }
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "SlideDoorLeft")
        {
            leftDoorOpen = false;
            if (leftDoorClose)
            {
                leftDoorClose = false;
            }
        }

        if (coll.gameObject.tag == "SlideDoorRight")
        {
            rightDoorOpen = false;
            openLeftFlag.closeRightDoor = false;
        }
    }
}
