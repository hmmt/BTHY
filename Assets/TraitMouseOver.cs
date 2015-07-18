using UnityEngine;
using System.Collections;

public class TraitMouseOver : MonoBehaviour {

    private bool mouseOver = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        if (mouseOver)
        {
            Debug.Log("Trait Mouse Over");
        }

        else
        {

        }

	}

    void OnMouseOver()
    {
        mouseOver = true;
    }

    void OnMouseExit()
    {
        mouseOver = false;
    }
}
