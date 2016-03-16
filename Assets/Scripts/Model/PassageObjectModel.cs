using UnityEngine;
using System.Collections.Generic;

public class PassageObjectModel : ObjectModelBase
{
	// 메타데이터
	//private PassageObjectTypeInfo metaInfo;

	private string id;
	private string src;
	private string sefiraName;
	private List<MapObjectModel> mapObjectList;
	private Dictionary<string, MapNode> mapNodeTable;
	private List<DoorObjectModel> doorObjectList;

	private List<MovableObjectNode> enteredUnitList;

	private float scaleFactor = 1.0f;
	//private

	public PassageObjectModel(string id, string sefiraName, string prefabSrc)
	{
		this.id = id;
		this.sefiraName = sefiraName;
		//this.metaInfo = metaInfo;
		this.src = prefabSrc;
		mapObjectList = new List<MapObjectModel>();
		mapNodeTable = new Dictionary<string, MapNode>();
		doorObjectList = new List<DoorObjectModel>();
		enteredUnitList = new List<MovableObjectNode> ();
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
		mapNodeTable.Add(node.GetId(), node);
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
		foreach (MapNode node in mapNodeTable.Values)
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

	public void FixedUpdate()
	{
		foreach (DoorObjectModel door in doorObjectList)
		{
			door.FixedUpdate();
		}
	}
}