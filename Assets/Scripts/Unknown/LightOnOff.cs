using UnityEngine;
using System.Collections;

public class LightOnOff : MonoBehaviour {

    //public Rigidbody2D homeDoor;

    public bool goOut;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {

      //  if(goOut)
      //      homeDoor.MovePosition(new Vector2(homeDoor.position.x, homeDoor.position.y+0.03f));
   //     else
//homeDoor.MovePosition(new Vector2(homeDoor.position.x, homeDoor.position.y - 0.03f));

	
	}

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Player2")
        {

            if (GlobalFunction.HomeLight)
            {
                GlobalFunction.HomeLight = false;
            }
            else
            {
                GlobalFunction.HomeLight = true;
            }
        }
    }
}
