using UnityEngine;
using System.Collections;

public class ElevatorNarratior : MonoBehaviour {

    public ElevatorMover elevator;

    public bool checkTime=false;
    public static float time = 0.0f;

    public TextMesh narration;

    public AudioSource citySound;

	// Use this for initialization
	void Start () {

        time = 0.0f;
        citySound.time = 12f;
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {

        if (elevator.isStop)
        {
            time += Time.deltaTime;
           narration.text = "오늘은 첫 출근~";

            if (time >= 5)
            {
                elevator.isStop = false;
                narration.text = "";
                citySound.Stop();
            }
        }
	
	}

    public void stopElevator()
    {
           
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Player" )
        {
            elevator.isStop = true;
            citySound.Play();
        }
    }

    void OnTriggerExit2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Player")
        {
            
        }
    }
}
