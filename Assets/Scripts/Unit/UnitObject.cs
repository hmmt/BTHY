using UnityEngine;
using System.Collections.Generic;

public class ObjectModelBase
{
    public Vector3 position;

    public virtual bool CanOpenDoor()
    {
        return true;
    }

    public virtual void InteractWithDoor(DoorObjectModel door)
    {
    }
}

public interface MovableObjectModel
{
    MovableObjectNode GetMovableObjectNode();
    void InteractWithDoor(DoorObjectModel door);
}

public class UnitObject
{
}


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

public class DoorObjectModel : ObjectModelBase
{
    private string id;

    private PassageObjectModel passage;
    public int hp;
    private bool closed;
    
    // TypeInfo로 변경필요
    public string type;
    public MapNode node;

	private DoorObjectModel connectedDoor = null;

    private float autoCloseCount;

    public DoorObjectModel(string id, string type, PassageObjectModel passage, MapNode node)
    {
        this.id = id;
        //openProgress = 0;
        this.type = type;
        this.passage = passage;
        this.node = node;
    }

    public string GetId()
    {
        return id;
    }

    public bool IsClosed()
    {
        return closed;
    }

	public void Connect(DoorObjectModel door)
	{
		connectedDoor = door;
		door.connectedDoor = this;
	}

    public void Open()
    {
        closed = false;
        node.closed = false;
		if(connectedDoor != null && connectedDoor.closed == true)
		{
			connectedDoor.Open ();
		}
        //Notice.instance.Send(NoticeName.OpenPassageDoor, passage, this);
    }

    public void Close()
    {
        autoCloseCount = 0;
        closed = true;
        node.closed = true;
		if(connectedDoor != null && connectedDoor.closed == false)
		{
			connectedDoor.Close ();
		}
        //Notice.instance.Send(NoticeName.ClosePassageDoor, passage, this);
    }

    public void OnObjectPassed()
    {
        autoCloseCount = 0;
		if(connectedDoor != null)
		{
			connectedDoor.autoCloseCount = 0;
		}
    }
    public void FixedUpdate()
    {
        if (!closed)
        {
            autoCloseCount += Time.deltaTime;
            if (autoCloseCount > 1)
            {
                Close();
            }
        }
    }
}
/*
public class DoorBlockModel : DoorObjectModel
{

}*/

public class MapObjectModel : ObjectModelBase
{
    public MapObjectTypeInfo metaInfo;
    
    // mapObject가 있는 passage
    public PassageObjectModel passage;
    public int horrorPoint;
}

public class MapObjectBloodModel : MapObjectModel
{
    public string bloodEffectPreb;
}