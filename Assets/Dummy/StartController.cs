using UnityEngine;
using System.Collections;

public class StartController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter2D(Collider2D coll)
    {
        Debug.Log("sibal");
        if (coll.gameObject.tag == "Player" && Input.GetKey(KeyCode.E))
        {
            Application.LoadLevel("Main");
        }
    }

    void OnTriggerStay2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Player" && Input.GetKey(KeyCode.E))
        {
            Application.LoadLevel("Main");
        }
    }
}
