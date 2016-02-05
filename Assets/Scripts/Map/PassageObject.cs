using UnityEngine;
using System.Collections.Generic;

public class PassageObject : MonoBehaviour {

    public PassageObjectModel model;
    //public Transform mapObjectParent;

    public Transform[] mapObjectPoint;

    public GameObject fogObject;
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
}
