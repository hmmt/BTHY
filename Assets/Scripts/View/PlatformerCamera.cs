using UnityEngine;
using System.Collections;

public class PlatformerCamera : MonoBehaviour
{

    public GameObject player;
    public Camera lightCamera;
    public Camera frontCamera;

    public GameObject escapeButton;

    public static bool introWalk = true;

    bool paused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (paused)
            {
                escapeButton.SetActive(true);
                Time.timeScale = 0;
                paused = false;
            }

            else
            {
                escapeButton.SetActive(false);
                Time.timeScale = 1;
                paused = true;
            }
        }
    }

    public void introCameraWalk()
    {
        Vector3 newPos = Camera.main.transform.localPosition;
        if (newPos.x <= -55.6)
        {
            newPos.x += 0.5f;
            Camera.main.transform.localPosition = newPos;
        }
        else
        {
            //Time.deltaTime
            Camera.main.orthographicSize -= 0.1f;
            newPos.x += 0.1f;
            Camera.main.transform.localPosition = newPos;
            Debug.Log("사이즈 : " + Camera.main.orthographicSize);
            if (Camera.main.orthographicSize <= 10)
            {
                if (Camera.main.transform.localPosition.x >= -27.7f)
                {
                    introWalk = false;
                }
            }
        }
    }

    void FixedUpdate()
    {
        //float a = Camera.main.aspect * Camera.main.orthographicSize;
        Vector3 pos = Input.mousePosition;

        if (introWalk)
        {
            introCameraWalk();
        }

        else
        {

            if (Input.GetKey(KeyCode.P))
            {
                Application.LoadLevel("Menu");
            }

            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                Vector3 newPos = Camera.main.transform.localPosition;
                newPos.x -= 0.1f;
                Camera.main.transform.localPosition = newPos;
            }


            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                Vector3 newPos = Camera.main.transform.localPosition;
                newPos.x += 0.1f;
                Camera.main.transform.localPosition = newPos;
            }

            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                Vector3 newPos = Camera.main.transform.localPosition;
                newPos.y -= 0.1f;
                Camera.main.transform.localPosition = newPos;
            }


            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                Vector3 newPos = Camera.main.transform.localPosition;
                newPos.y += 0.1f;
                Camera.main.transform.localPosition = newPos;
            }

            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - 0.1f, 1.5f, 16.5f);
                lightCamera.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - 0.1f, 1.5f, 16.5f);
                frontCamera.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - 0.1f, 1.5f, 16.5f);
             }
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
                Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize + 0.1f, 1.5f, 16.5f);
                 lightCamera.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize + 0.1f, 1.5f, 16.5f);
            frontCamera.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize + 0.1f, 1.5f, 16.5f);
            }
    }

    public void cameraZoomOut(float zoomOut)
    {
        Camera.main.orthographicSize += zoomOut;
        lightCamera.orthographicSize += zoomOut;
        frontCamera.orthographicSize += zoomOut;
    }

    public void cameraZoomIn(float zoomIn)
    {
        Camera.main.orthographicSize -= zoomIn;
        lightCamera.orthographicSize -= zoomIn;
        frontCamera.orthographicSize -= zoomIn;
    }
}


