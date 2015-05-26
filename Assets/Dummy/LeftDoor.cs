using UnityEngine;
using System.Collections;

public class LeftDoor : MonoBehaviour {

    public bool leftDoorOpen = true;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "SecurityHall")
        {
            Debug.Log("leftdoor" + leftDoorOpen);
            leftDoorOpen = false;
        }
    }
}
