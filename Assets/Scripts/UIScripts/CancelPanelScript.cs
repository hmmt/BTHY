using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CancelPanelScript : MonoBehaviour {
    public Image cancel;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void openCancel() {
        if (cancel.gameObject.activeInHierarchy == false)
        {
            cancel.gameObject.SetActive(true);
        }
    }

}
