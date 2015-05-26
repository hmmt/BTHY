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

        if (elevatorState.currentState == ElevatorMover.STATE.RMOVE ||
            elevatorState.currentState == ElevatorMover.STATE.STOP)
        {
            goImage.SetActive(true);
            backImage.SetActive(false);
        }

        else if (elevatorState.currentState == ElevatorMover.STATE.MOVE
            || elevatorState.currentState == ElevatorMover.STATE.RSTOP)
        {
            backImage.SetActive(true);
            goImage.SetActive(false);
        }
	
	}
}
