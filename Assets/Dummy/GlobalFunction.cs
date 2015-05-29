using UnityEngine;
using System.Collections;

public class GlobalFunction : MonoBehaviour {

    public TextMesh dDay;

    public static bool finishWork = false;

    public static int currentDay=0;

    int tempDay = 0;

	// Use this for initialization
	void Start () {

        tempDay = currentDay + 1;
        dDay.text = "Day - " + tempDay;
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
