using UnityEngine;
using System.Collections;

public class CameraMover : MonoBehaviour
{

    public GameObject player;
    public GameObject escapeButton;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (escapeButton.activeSelf)
            {
                escapeButton.SetActive(false);
                Time.timeScale = 1;
            }

            else
            {
                escapeButton.SetActive(true);
                Time.timeScale = 0;
            }
        }
    }

    void FixedUpdate()
    {
        //float a = Camera.main.aspect * Camera.main.orthographicSize;
        Vector3 pos = Input.mousePosition;

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
            //Camera.allCameras. = Mathf.Clamp(Camera.main.orthographicSize - 0.1f, 1.5f, 16.5f);
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize + 0.1f, 1.5f, 16.5f);
    }
}

