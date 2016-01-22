using UnityEngine;
using System.Collections;

public class GlobalFunction : MonoBehaviour {

    public TextMesh dDay;

    public static bool finishWork = false;

    public static int currentDay=0;

    public GameObject HomeOn;
    public GameObject HomeOff;

    public static bool HomeLight=true;

    int tempDay = 0;

	// Use this for initialization
	void Start () {

	}

   public void Quit()
    {
        Debug.Log("Quit");
        Application.Quit();
    }


	
	// Update is called once per frame
	void FixedUpdate () {

        dDay.text = "Day - " + currentDay;

        TimerCallback.Create(1f, delegate()
        {
            if (HomeLight)
            {
                HomeOff.SetActive(false);
                HomeOn.SetActive(true);
            }
            else
            {
                HomeOn.SetActive(false);
                HomeOff.SetActive(true);
            }
        });


	
	}
}
