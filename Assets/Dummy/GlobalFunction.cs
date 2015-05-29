using UnityEngine;
using System.Collections;

public class GlobalFunction : MonoBehaviour {

    public TextMesh dDay;

    public static bool finishWork = false;

	// Use this for initialization
	void Start () {


        dDay.text = "Day - " + PlayerModel.instnace.GetDay();
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
