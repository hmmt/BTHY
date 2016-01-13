using UnityEngine;
using System.Collections.Generic;

public class PassageObject : MonoBehaviour {

    public PassageObjectModel model;
    //public Transform mapObjectParent;

    public Transform[] mapObjectPoint;

    public GameObject fogObject;
    public PassageDoor door;

	// Use this for initialization
	void Start () {
        if (model.IsClosed())
        {
            CloseDoor();
        }
        else
        {
            OpenDoor();
        }
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

    public void OpenDoor()
    {
        if (door != null)
        {
            door.OpenDoor();
        }
    }

    public void CloseDoor()
    {
        Debug.Log("PassageObject -> CALL CloseDoor");
        if (door != null)
        {
            door.CloseDoor();
        }
    }

    public void OnClick()
    {

    }
}
