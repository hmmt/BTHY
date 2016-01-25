using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CancelScript : MonoBehaviour {
    public Image backGround;

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(1)) {
            CancelFunc();
        }
	}

    public void CancelFunc(){
        backGround.gameObject.SetActive(false);
    }
}
