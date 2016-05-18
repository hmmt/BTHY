using UnityEngine;
using System.Collections;

public class CameraElevator : MonoBehaviour {

    public PlayerController player;
    public PlatformerCamera camera;
    //public GameObject playerBody;

    public static bool cameraZoomOut=false;
    public static bool cameraZoomIn = false;

    public static  bool firstFlag = true;
    public static bool secondFlag = false;

    public float yPlus=2;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        if(player.cameraWalk)
        Camera.main.transform.localPosition = new Vector3(player.transform.localPosition.x, player.transform.localPosition.y +yPlus, Camera.main.transform.localPosition.z);

        //first ZoomOut
        if (cameraZoomOut && firstFlag)
            camera.cameraZoomOut(1f * Time.deltaTime);

        //secondZoomOut
        if (cameraZoomOut && secondFlag)
        {
                yPlus += 4f * Time.deltaTime;
                camera.cameraZoomOut(1.3f * Time.deltaTime);
                if (Camera.main.orthographicSize>=5)
                {
                    secondFlag = false;
                    Debug.Log(Camera.main.transform.localPosition.y);
                    Debug.Log(player.cameraWalk);
                }
        }

        if (cameraZoomIn)
            camera.cameraZoomIn(-1f * Time.deltaTime);
	}

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Player" && firstFlag)
        {
            player.cameraWalk = true;
            cameraZoomOut = true;
           // cameraZoomIn = true;
            Camera.main.orthographicSize = 1.6f;
        }

        else if (coll.gameObject.tag == "Player" && secondFlag)
        {
            //yPlus = 3.5f;
            //player.cameraWalk = false;
            cameraZoomOut = true;
        }
    }

    void OnTriggerExit2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Player")
        {
          //  player.cameraWalk = false;
        }
    }
}
