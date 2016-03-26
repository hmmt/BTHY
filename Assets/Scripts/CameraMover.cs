using UnityEngine;
using System.Collections;

public interface IScrollTarget {
    void Regist();
    void DeRegist();
    void AddTrigger();
}

public class CameraMover : MonoBehaviour
{
    private static CameraMover _instance;
    public static CameraMover instance {
        get {
            return _instance;
        }
    }
   
    public GameObject player;
    public GameObject escapeButton;
    public float scrollSpeed;

    private IScrollTarget _target = null;

    private Vector3 ResetCamera;
    private Vector3 Origin;
    private Vector3 Diference;
    private bool Drag = false;

    void Awake() {
        _instance = this;
    }

    void Start()
    {
        ResetCamera = Camera.main.transform.position;
    }


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

        /*
        if (Input.GetKey(KeyCode.P))
        {
            Application.LoadLevel("Menu");
        }*/

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

        if (this._target == null)
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - 0.1f * scrollSpeed, 1.5f, 50.5f);

            }
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize + 0.1f * scrollSpeed, 1.5f, 50.5f);
            }
        }
    }

    void LateUpdate()
    {
        if (Input.GetMouseButton(1))
        {
            Diference = (Camera.main.ScreenToWorldPoint(Input.mousePosition)) - Camera.main.transform.position;
            if (Drag == false)
            {
                Drag = true;
                Origin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
        }
        else
        {
            Drag = false;
        }
        if (Drag == true)
        {
            Camera.main.transform.position = Origin - Diference;
        }
        /*/RESET CAMERA TO STARTING POSITION WITH RIGHT CLICK
        if (Input.GetMouseButton(1))
        {
            Camera.main.transform.position = ResetCamera;
        }*/
    }

    public void Registration(IScrollTarget target) {
        this._target = target;
    }

    public void DeRegistration() {
        this._target = null;
    }
}

