﻿using UnityEngine;
using System.Collections;

public class OpenRightDoor : MonoBehaviour {


    public bool playerThroguh=false;
    public bool checkOnce = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	}

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Player")
        {
            if (GlobalFunction.finishWork && !checkOnce)
            {
                checkOnce = true;
                playerThroguh = true;
                Debug.Log("ON");
            }

            if (GlobalFunction.currentDay + 1 == PlayerModel.instance.GetDay())
                GlobalFunction.finishWork = true;
        }

        if (coll.gameObject.tag == "SlideDoorRight")
        {
            playerThroguh = false;
        }
    }

    /*
    void OnTriggerExit2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Player")
        {
            if (GlobalFunction.currentDay+1 == PlayerModel.instance.GetDay())
                GlobalFunction.finishWork = true;
          }
    }*/
}
