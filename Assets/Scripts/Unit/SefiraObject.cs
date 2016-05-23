using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class SefiraObject : MonoBehaviour {

    public string sefiraName;
    public SefiraCoreRoom sefiraCore;
    public GameObject sefira; // 직원 공간
    public GameObject sepfiraFog;

    public GameObject way1Fog;
    public GameObject way2Fog;
    public GameObject way3Fog;
    public GameObject way4Fog;

    public GameObject elevatorFog1; 
    public GameObject elevatorFog2;

	[HideInInspector]
    public List<GameObject> fogs;

    private List<PassageObject> passageList;

	private List<ElevatorPassageObject> elevatorList;


    void Awake()
    {
        fogs = new List<GameObject>();
        passageList = new List<PassageObject>();
		elevatorList = new List<ElevatorPassageObject> ();
    }
    public void OnClick()
    {
        SelectSefiraAgentWindow.CreateWindow(sefira, sefiraName);
    }

	public void Clear()
	{
		foreach (PassageObject passage in passageList)
		{
			Destroy (passage.gameObject);
		}

		foreach (ElevatorPassageObject elevator in elevatorList)
		{
			Destroy (elevator.gameObject);
		}

		passageList.Clear ();
		elevatorList.Clear ();
	}

    public PassageObject GetPassageObject(string id)
    {
        foreach (PassageObject obj in passageList)
        {
            if(obj.model.GetId() == id)
                return obj;
        }
        return null;
    }

    public void InitPassageObject(PassageObjectModel model)
    {
        AddPassageObject(model);

        foreach (DoorObjectModel door in model.GetDoorList())
        {
            AddPassageDoor(model, door);
        }
    }

    public void AddPassageObject(PassageObjectModel model)
    {
        //passageObj.
		GameObject passageObj = Prefab.LoadPrefab(model.GetSrc());

        PassageObject passageScript = passageObj.GetComponent<PassageObject>();

        
        if (passageScript.fogObject != null)
        {
            fogs.Add(passageScript.fogObject);
        }
        passageList.Add(passageScript);
        if (passageScript.shouldCheckSefira) {
            passageScript.SetSefiraFrame(SefiraManager.instance.GetSefira(this.sefiraName));
        }

        passageObj.transform.SetParent(transform);

        passageScript.Init(model);
    }

    public void AddPassageDoor(PassageObjectModel model, DoorObjectModel doorModel)
    {
        PassageObject passage = GetPassageObject(model.GetId());

		//Debug.Log ("AddPassageDoor >>> " + doorModel.type);

        GameObject doorObj = Prefab.LoadPrefab("Map/Door/"+doorModel.type);
        PassageDoor doorScript = doorObj.GetComponent<PassageDoor>();

        doorScript.model = doorModel;

        Vector3 doorPos = doorModel.position;

        doorObj.transform.localPosition = doorPos - model.position;
        doorObj.transform.SetParent(passage.transform, false);
    }

	public void AddElevatorPassage(ElevatorPassageModel model)
	{
		//GameObject g = new GameObject ("ElevatorPassage");

		GameObject g = Prefab.LoadPrefab ("Map/Passage/ElevatorPassage");

		ElevatorPassageObject e = g.GetComponent<ElevatorPassageObject> ();

		e.model = model;

        if (e.shouldSefiraCheck) {
            e.SetSprite(SefiraManager.instance.GetSefira(this.sefiraName));
        }

		g.transform.localPosition = model.GetNode ().GetPosition ();
		g.transform.SetParent(transform, false);

		elevatorList.Add (e);
	}

    public void AddMapObject(MapObjectModel mapObjModel)
    {
        foreach (PassageObject passage in passageList)
        {
            if (passage.model.GetId() == mapObjModel.passage.GetId())
            {
                passage.AddMapObject(mapObjModel);
            }
        }
    }
	public void AddBloodMapObject(BloodMapObjectModel mapObjModel)
	{
		foreach (PassageObject passage in passageList)
		{
			if (passage.model.GetId() == mapObjModel.passage.GetId())
			{
				passage.AddBloodMapObject(mapObjModel);
			}
		}
	}
    /*
    public void ClosePassage(PassageObjectModel model, DoorObjectModel doorModel)
    {
        Debug.Log("SefiraObject -> CALL ClosePassage");
        foreach (PassageObject passage in passageList)
        {
            if (passage.model.GetId() == model.GetId())
            {
                passage.CloseDoor(doorModel);
            }
        }
    }

    public void OpenPassage(PassageObjectModel model, DoorObjectModel doorModel)
    {
        foreach (PassageObject passage in passageList)
        {
            if (passage.model.GetId() == model.GetId())
            {
                passage.OpenDoor(doorModel);
            }
        }
    }*/
}