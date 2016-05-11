using UnityEngine;
using System.Collections.Generic;

public class PassageObject : MonoBehaviour {

    public PassageObjectModel model;
    //public Transform mapObjectParent;

    public Transform[] mapObjectPoint;

    public GameObject fogObject;
    public GameObject shaderObject;
    private SpriteRenderer shader;
    private List<PassageDoor> doorList;

    void Awake()
    {
        doorList = new List<PassageDoor>();
    }
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        UpdateViewPosition();
	}

    public void Init(PassageObjectModel model)
    {
        this.model = model;
        SetShader(255);
    }

    private void UpdateViewPosition()
    {
        transform.localPosition = model.position;
    }

    public void AddMapObject(MapObjectModel model)
    {
        GameObject mapObj = Prefab.LoadPrefab("Map/MapObject");
        MapObject mapScript = mapObj.GetComponent<MapObject>();

        mapScript.model = model;

        foreach (Transform point in mapObjectPoint)
        {
            if (point.childCount == 0)
            {
                mapObj.transform.SetParent(point, false);
                break;
            }
        }

        //mapObj.transform.SetParent(mapObjectParent, false);
    }

	public void AddBloodMapObject(BloodMapObjectModel model)
	{
		GameObject mapObj = Prefab.LoadPrefab("Map/BloodMapObject");
		BloodMapObject bloodScript = mapObj.GetComponent<BloodMapObject>();

		bloodScript.model = model;

		bloodScript.transform.SetParent (this.transform);
		bloodScript.transform.position = model.position;

		bloodScript.Init ();
	}
    /*
    public void OpenDoor(DoorObjectModel doorModel)
    {
        foreach(PassageDoor door in doorList)
        {
            if(door.model.GetId() == doorModel.GetId())
                door.OpenDoor();
        }
    }

    public void CloseDoor(DoorObjectModel doorModel)
    {
        foreach (PassageDoor door in doorList)
        {
            if (door.model.GetId() == doorModel.GetId())
                door.CloseDoor();
        }
    }
    */

    /// <summary>
    /// Set Object's alpha include child object
    /// </summary>
    /// <param name="value"> alpha value, 0 ~ 255 </param>
    public void SetAplphaRecursive(float value) {
        float scaledValue = value / 255f;
        SetShader(value);
        foreach (Transform tr in this.transform) {
            if (!tr.gameObject.activeSelf) continue;

            SpriteRenderer spriteRenderer = tr.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null) {
                Color color = spriteRenderer.color;
                color.a = scaledValue;
                spriteRenderer.color = color;
            }
        }
    }

    public void SetBlackOut() {
        this.SetAplphaRecursive(0);
        SetShader(0);
    }

    public void SetWhite() {
        this.SetAplphaRecursive(255);
        SetShader(255);
    }

    public void SetShader(float value) {
        if (shaderObject == null) return;
        if (shader == null) shader = shaderObject.GetComponent<SpriteRenderer>();
        Color color = shader.color;
        float refinedValue = 255f - value;
        color.a = refinedValue;
        shader.color = color;
    }
}
