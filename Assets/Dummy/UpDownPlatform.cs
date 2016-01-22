using UnityEngine;
using System.Collections;

public class UpDownPlatform : MonoBehaviour {

    public bool downPlatform;
    public bool platformFlag;

    public Rigidbody2D playerBody;


	// Use this for initialization
	void Start () {

        downPlatform = false;
        platformFlag = false;

	}
	
	// Update is called once per frame
	void Update () {

        if (downPlatform && !platformFlag)
        {
            //playerBody.gravityScale = 99999;
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y-0.01f, transform.localPosition.z);
        }
        
	}

    void OnCollisionStay2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "Player")
        {
            downPlatform = true;
        }

        else
        {
            downPlatform = false;
        }

    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "PlatFormFlag")
        {
            platformFlag = true;
            Debug.Log("멈춰 좀");
        }
            
        else
        {
            platformFlag = false;
        }
    }



    /*
     * 
     * 
    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "PlatformFlag")
        {
            platformFlag = true;
            Debug.Log("멈춰 좀");
        }

        else
        {
            platformFlag = false;
        }
    }
    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Player")
        {
            Debug.Log("platform1");
        }
    }

    void OnTriggerStay2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Player")
        {
            transform.localPosition = new Vector3(0,-2f,0);
            Debug.Log("platform");
        }
    }*/
}
