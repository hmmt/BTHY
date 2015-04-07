using UnityEngine;
using System.Collections;

public class TextColorSetter : MonoBehaviour {

    public Color textColor = Color.white;

	// Use this for initialization
	void Start () {
        GetComponent<UnityEngine.UI.Text>().material.color = textColor;
	}
}
