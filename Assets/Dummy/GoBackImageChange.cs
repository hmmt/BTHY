using UnityEngine;
using System.Collections;

public class GoBackImageChange : MonoBehaviour {

    public GameObject goImage;
    public GameObject backImage;

    public ElevatorMover elevatorState;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        if ( ElevatorMover.currentState== ElevatorMover.STATE.RMOVE ||
            ElevatorMover.currentState == ElevatorMover.STATE.STOP)
        {
            goImage.SetActive(true);
            backImage.SetActive(false);
            //elevatorState.currentState
        }

        else if (ElevatorMover.currentState == ElevatorMover.STATE.MOVE
            || ElevatorMover.currentState == ElevatorMover.STATE.RSTOP)
        {
            backImage.SetActive(true);
            goImage.SetActive(false);
        }
	
	}
}
