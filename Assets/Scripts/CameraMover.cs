using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IScrollTarget {
    void Regist();
    void DeRegist();
    void AddTrigger();
}

[System.Serializable]
public class RecoilEffect {
    public Transform targetTransform;
    public float maxTime = 0.1f;
    public float scale = 1;
    public int recoilCount = 5;

    public RecoilEffect() { }

    public static List<RecoilArrow> MakeRecoilArrow(int level) {
        List<RecoilArrow> list = new List<RecoilArrow>();
        int former = -1;
        for (int i = 0; i < level; i++)
        {
            int randVal;
            while (former == (randVal = Random.Range(0, 8))) ;
            former = randVal;
            RecoilArrow arrow = GetArrow(randVal);

            list.Add(arrow);

        }
        return list;
    }

    public static RecoilArrow GetArrow(int index)
    {
        switch (index)
        {
            case 0: return RecoilArrow.LEFTUP;
            case 1: return RecoilArrow.UP;
            case 2: return RecoilArrow.RIGHTUP;
            case 3: return RecoilArrow.RIGHT;
            case 4: return RecoilArrow.RIGHTDOWN;
            case 5: return RecoilArrow.DOWN;
            case 6: return RecoilArrow.LEFTDOWN;
            case 7: return RecoilArrow.LEFT;
            default: return RecoilArrow.LEFTUP;
        }
    }

    public static Vector3 GetVector(RecoilArrow arrow, Vector3 initial, float scale)
    {
        float x;
        float y;
        int index = (int)arrow % 4;
        switch (index)
        {
            case 0:
                x = -1; y = 1;
                break;
            case 1:
                x = 0; y = 1;
                break;
            case 2:
                x = 1; y = 1;
                break;
            case 3:
                x = 1; y = 0;
                break;
            default:
                Debug.LogError("??");
                x = 0; y = 0;
                break;
        }
        int sign = 1;
        if ((int)arrow > 3)
        {
            sign = -1;
        }

        Vector3 output = new Vector3(initial.x + (x * sign) * scale,
                                     initial.y + (y * sign) * scale,
                                     initial.z
                                     );
        return output;
    }

    public static Vector2 GetVector(RecoilArrow arrow, Vector2 initial, float scale)
    {
        float x;
        float y;
        int index = (int)arrow % 4;
        switch (index)
        {
            case 0:
                x = -1; y = 1;
                break;
            case 1:
                x = 0; y = 1;
                break;
            case 2:
                x = 1; y = 1;
                break;
            case 3:
                x = 1; y = 0;
                break;
            default:
                Debug.LogError("??");
                x = 0; y = 0;
                break;
        }
        int sign = 1;
        if ((int)arrow > 3)
        {
            sign = -1;
        }

        Vector2 output = new Vector2(initial.x + (x * sign) * scale,
                                     initial.y + (y * sign) * scale);
        return output;
    }
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

    public RecoilEffect recoil;

    private IScrollTarget _target = null;

    private Vector3 ResetCamera;
    private Vector3 Origin;
    private Vector3 Diference;
    private bool Drag = false;
    bool movable = true;
    Vector3 dragedPos;

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

		if (Input.GetKeyDown (KeyCode.P)) {
			AgentModel model = AgentManager.instance.GetAgentList () [0];
			model.TakePhysicalDamageByCreature (1);
		}
		/*
    }

    void FixedUpdate()
    {
    */
        //float a = Camera.main.aspect * Camera.main.orthographicSize;
        Vector3 pos = Input.mousePosition;

        /*
        if (Input.GetKey(KeyCode.P))
        {
            Application.LoadLevel("Menu");
        }*/

		float unscaledDeltaTime = Time.unscaledDeltaTime;

		if (unscaledDeltaTime > 1)
			unscaledDeltaTime = 1;

        if (movable)
        {

            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                Vector3 newPos = Camera.main.transform.localPosition;
                newPos.x -= unscaledDeltaTime * 10;
                Camera.main.transform.localPosition = newPos;
            }


            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                Vector3 newPos = Camera.main.transform.localPosition;
                newPos.x += unscaledDeltaTime * 10;
                Camera.main.transform.localPosition = newPos;
            }

            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                Vector3 newPos = Camera.main.transform.localPosition;
                newPos.y -= Time.unscaledDeltaTime * 10;
                Camera.main.transform.localPosition = newPos;
            }


            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                Vector3 newPos = Camera.main.transform.localPosition;
                newPos.y += unscaledDeltaTime * 10;
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

            if (Input.GetKey(KeyCode.Alpha7)) {
                PlayerModel.instance.SetCurrentEmergencyLevel(2);
            }

            if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                //PlayerModel.instance.SetCurrentEmergencyLevel(2);
                foreach (CreatureModel creatureModel in CreatureManager.instance.GetCreatureList()) {
                    if (creatureModel.metadataId == 100005)
                    {
                        creatureModel.SubFeeling(10f);
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                //PlayerModel.instance.SetCurrentEmergencyLevel(2);
                foreach (CreatureModel creatureModel in CreatureManager.instance.GetCreatureList())
                {
                    if (creatureModel.metadataId == 100005)
                    {
                        creatureModel.AddFeeling(10f);
                    }
                }
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
                    CursorManager.instance.CursorSet(MouseCursorType.CLICK);
                    Origin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                }
            }
            else
            {
                if (Drag)
                {
                    CursorManager.instance.CursorSet(MouseCursorType.NORMAL);
                }
                Drag = false;
            }
            if (Drag == true)
            {
                Camera.main.transform.position = Origin - Diference;
                dragedPos = Camera.main.transform.position;
            }
        
        /*/RESET CAMERA TO STARTING POSITION WITH RIGHT CLICK
        if (Input.GetMouseButton(1))
        {
            Camera.main.transform.position = ResetCamera;
        }*/
    }

    public void Registration(IScrollTarget target) {
        this._target = target;
        CursorManager.instance.CursorSet(MouseCursorType.SCROLL);
    }

    public void DeRegistration() {
        this._target = null;
        CursorManager.instance.CursorSet(MouseCursorType.NORMAL);
    }

    public void Recoil(int level) {
        List<RecoilArrow> arrowList = RecoilEffect.MakeRecoilArrow(level * recoil.recoilCount);
        Vector3 initialPos = recoil.targetTransform.localPosition;
        movable = false;
        Queue<Vector3> queue = new Queue<Vector3>();
        foreach (RecoilArrow arrow in arrowList) { 
            queue.Enqueue(RecoilEffect.GetVector(arrow, initialPos, recoil.scale));
        }
        queue.Enqueue(initialPos);

        StartCoroutine(PlayRecoil(queue));
    }

    IEnumerator PlayRecoil(Queue<Vector3> queue) {
        
        int val = queue.Count;
        float unitStep = recoil.maxTime / val;

        while (queue.Count > 1) {
            yield return new WaitForSeconds(unitStep);
            Vector3 recoilUnit = queue.Dequeue();
            recoil.targetTransform.localPosition = new Vector3(recoilUnit.x ,
                                                               recoilUnit.y ,
                                                               recoilUnit.z);
            
        }
        if (Drag) {
            recoil.targetTransform.position = dragedPos;
        }
        else recoil.targetTransform.localPosition = queue.Dequeue();
        movable = true;
    }
}

