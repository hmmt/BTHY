using UnityEngine;
using System.Collections;

public class cameraZoomEnd : MonoBehaviour {

    public CameraElevator cameraElevator;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Player" && CameraElevator.firstFlag)
        {
            CameraElevator.cameraZoomOut = false;
            CameraElevator.firstFlag = false;
            CameraElevator.secondFlag = true;
        }

        else if (coll.gameObject.tag == "Player" && CameraElevator.secondFlag)
        {
            CameraElevator.cameraZoomOut = false;
            CameraElevator.firstFlag = false;
        }
    }
}
