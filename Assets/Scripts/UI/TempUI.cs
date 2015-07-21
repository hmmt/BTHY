using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class TempUI : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Save()
    {
        GameManager.currentGameManager.SaveData();
    }

    public void Load()
    {
        GameManager.currentGameManager.LoadData();
    }
}
