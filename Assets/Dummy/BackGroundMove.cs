using UnityEngine;
using System.Collections;

public class BackGroundMove : MonoBehaviour {

    public GameObject player;
    public PlayerController playerCon;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        if (playerCon.playerMove && playerCon.facingRight)
        {
            transform.localPosition = new Vector3(transform.localPosition.x +0.02f, transform.localPosition.y, transform.localPosition.z);
           // transform.localPosition = new Vector3(transform.localPosition.x - player.transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
        }
        else if (playerCon.playerMove && !playerCon.facingRight)
        {
            transform.localPosition = new Vector3(transform.localPosition.x -0.02f, transform.localPosition.y, transform.localPosition.z);

        }
	}
}
