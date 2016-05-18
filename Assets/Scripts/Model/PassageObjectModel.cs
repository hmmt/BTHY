using UnityEngine;
using System.Collections.Generic;

public enum PassageType { 
    SEFIRA,//main sefira
    DEPARTMENT,//subsefira
    VERTICAL,//vertical Way
    HORIZONTAL,//horizontal Way
    NONE//for debug
}


public class PassageGroundInfo
{
	public float height;
	public List<Sprite> bloodSprites;

	public PassageGroundInfo()
	{
		bloodSprites = new List<Sprite> ();
	}
}
public class PassageWallInfo
{
	public float height;
	public List<Sprite> bloodSprites;

	public PassageWallInfo()
	{
		bloodSprites = new List<Sprite> ();
	}
}

public class PassageObjectModel : ObjectModelBase
{
	// 메타데이터
	//private PassageObjectTypeInfo metaInfo;
	private string id;
	private string src;
	private string sefiraName;
	private bool isIsolate = false;

	public PassageGroundInfo groundInfo = null;
	public PassageWallInfo wallInfo = null;


	private List<MapObjectModel> mapObjectList;
	private List<MapNode> mapNodeList;
	private List<DoorObjectModel> doorObjectList;

	private List<MovableObjectNode> enteredUnitList;

	private float scaleFactor = 1.0f;
    public PassageType type = PassageType.NONE;
	//private

    public static PassageType GetPassageTypeByString(string str) {
        switch (str) {
            case "sefira": return PassageType.SEFIRA;
            case "dept": return PassageType.DEPARTMENT;
            case "vertical": return PassageType.VERTICAL;
            case "horizontal": return PassageType.HORIZONTAL;
            default: return PassageType.NONE;
        }
    }

    public void SetPassageType(PassageType type) {
        this.type = type;
    }

    public PassageType GetPassageType() {
        return this.type;
    }

	public PassageObjectModel(string id, string sefiraName, string prefabSrc)
	{
		this.id = id;
		this.sefiraName = sefiraName;
		//this.metaInfo = metaInfo;
		this.src = prefabSrc;
		mapObjectList = new List<MapObjectModel>();
		mapNodeList = new List<MapNode>();
		doorObjectList = new List<DoorObjectModel>();
		enteredUnitList = new List<MovableObjectNode> ();
	}

	public void SetToIsolate()
	{
		isIsolate = true;
	}

	public bool IsIsolate()
	{
		return isIsolate;
	}
	public void CreateMapObject(long typeId)
	{
		MapObjectTypeInfo typeInfo =  MapObjectTypeList.instance.GetData(typeId);

		MapObjectModel model = new MapObjectModel();
		model.metaInfo = typeInfo;
		model.passage = this;

		AddMapObject(model);

		//Notice.instance.Send(NoticeName.AddMapObject, model);

		/*GameObject mapObj = Prefab.LoadPrefab(typeInfo.src);
        MapObject mapObjScript = mapObj.GetComponent<MapObject>();*/
	}
	public void AttachBloodObject(float posx)
	{
		bool isGroundBlood = Random.Range (0, 2) == 1 ? true : false;

		if (groundInfo != null && wallInfo == null)
			isGroundBlood = true;
		else if (groundInfo == null && wallInfo != null)
			isGroundBlood = false;


		if (isGroundBlood && groundInfo != null && groundInfo.bloodSprites.Count > 0)
		{
			BloodMapObjectModel bloodModel = new BloodMapObjectModel ();

			int randIndex = Random.Range (0, groundInfo.bloodSprites.Count);
			Sprite selectedSprite = groundInfo.bloodSprites[randIndex];

			bloodModel.position = new Vector3 (posx, groundInfo.height, position.z-0.01f);
			bloodModel.passage = this;
			bloodModel.bloodSprite = selectedSprite;

			AddBloodMapObject (bloodModel);
		}

		if (!isGroundBlood && wallInfo != null && wallInfo.bloodSprites.Count > 0)
		{
			BloodMapObjectModel bloodModel = new BloodMapObjectModel ();

			int randIndex = Random.Range (0, wallInfo.bloodSprites.Count);
			Sprite selectedSprite = wallInfo.bloodSprites[randIndex];

			bloodModel.position = new Vector3 (posx, wallInfo.height, position.z-0.01f);
			bloodModel.passage = this;
			bloodModel.bloodSprite = selectedSprite;

			AddBloodMapObject (bloodModel);
		}

		//bloodList.Add (bloodModel);
	}

	public DoorObjectModel[] GetDoorList()
	{
		return doorObjectList.ToArray();
	}

	public float GetScaleFactor()
	{
		return scaleFactor;
	}
	private void AddMapObject(MapObjectModel mapObject)
	{
		mapObjectList.Add(mapObject);
		Notice.instance.Send(NoticeName.AddMapObject, mapObject);
	}
	private void AddBloodMapObject(BloodMapObjectModel mapObject)
	{
		Notice.instance.Send(NoticeName.AddBloodMapObject, mapObject);
	}

	public void EnterUnit(MovableObjectNode unit)
	{
		enteredUnitList.Add (unit);
	}
	public void ExitUnit(MovableObjectNode unit)
	{
		enteredUnitList.Remove (unit);
	}

	public void AddNode(MapNode node)
	{
		mapNodeList.Add(node);
		/*
        if (node.IsClosable())
        {
            DoorObjectModel door = new DoorObjectModel(this, node);
            doorObjectList.Add(door);
            node.SetDoor(door);
        }*/
	}

	public void AddDoor(DoorObjectModel door)
	{
		doorObjectList.Add(door);
	}

	public bool IsClosable()
	{
		foreach (MapNode node in mapNodeList)
		{
			if (node.IsClosable())
			{
				return true;
			}
		}
		return false;
	}


	public void ClosePassage()
	{
		foreach (DoorObjectModel door in doorObjectList)
		{
			door.Close();
		}
	}

	public void OpenPassage()
	{
		foreach (DoorObjectModel door in doorObjectList)
		{
			door.Open();
		}
	}

	public int GetHorrorPointTotal()
	{
		int output = 0;
		foreach (MapObjectModel obj in mapObjectList)
		{
			output += obj.horrorPoint;
		}

		return output;
	}

	public string GetId()
	{
		return id;
	}

	public string GetSrc()
	{
		return src;
	}

	public string GetSefiraName()
	{
		return sefiraName;
	}

	public MapNode[] GetNodeList()
	{
		return mapNodeList.ToArray();
	}

	public virtual void FixedUpdate()
	{
		foreach (DoorObjectModel door in doorObjectList)
		{
			door.FixedUpdate();
		}
	}

}