using UnityEngine;
using System.Collections;

public class StartController : MonoBehaviour {

    public GameObject player;

    public static bool Check=false;

	// Use this for initialization
	void Start () {
        Debug.Log("Check :"+Check);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Player" && Input.GetKey(KeyCode.E))
        {
            Application.LoadLevel("Main");
            PlayerModel.instnace.playerSpot = player.transform.localPosition;
        }
    }

    void OnTriggerStay2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Player" && Input.GetKey(KeyCode.E))
        {
            Check = true;
            Application.LoadLevel("Main");
            PlayerModel.instnace.playerSpot = player.transform.localPosition;

        }
    }
}
