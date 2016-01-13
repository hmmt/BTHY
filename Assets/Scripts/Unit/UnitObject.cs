using UnityEngine;
using System.Collections.Generic;

// 맵 오브젝트를 만들 주체
// 맵 오브젝트를 누가 관리?
public class MapObjectCreator
{
}

public class ObjectModelBase
{
    public Vector3 position;
    public string GetType()
    {
        return "unknown";
    }

    public bool CanOpenDoor()
    {
        return true;
    }

    public virtual void InteractWithDoor(DoorObjectModel door)
    {
    }
}


public class PassageObjectModel : ObjectModelBase
{
    // 메타데이터
    private PassageObjectTypeInfo metaInfo;
    
    private string id;
    private string sefiraName;
    private List<MapObjectModel> mapObjectList;
    private Dictionary<string, MapNode> mapNodeTable;
    private List<DoorObjectModel> doorObjectList;
    //private 

    private bool closed;

    public PassageObjectModel(string id, string sefiraName, PassageObjectTypeInfo metaInfo)
    {
        this.closed = false;

        this.id = id;
        this.sefiraName = sefiraName;
        this.metaInfo = metaInfo;
        mapObjectList = new List<MapObjectModel>();
        mapNodeTable = new Dictionary<string, MapNode>();
        doorObjectList = new List<DoorObjectModel>();
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
    private void AddMapObject(MapObjectModel mapObject)
    {
        mapObjectList.Add(mapObject);
        Notice.instance.Send(NoticeName.AddMapObject, mapObject);
    }

    public void AddNode(MapNode node)
    {
        mapNodeTable.Add(node.GetId(), node);
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

    public bool IsClosed()
    {
        return closed;
    }

    public void UpdatePassageDoor()
    {
        foreach (MapNode node in mapNodeTable.Values)
        {
            if (node.IsClosable())
            {
                if (closed)
                {
                    node.closed = true;
                    Notice.instance.Send(NoticeName.ClosePassageDoor, this);
                }
                else
                {
                    node.closed = false;
                    Notice.instance.Send(NoticeName.OpenPassageDoor, this);
                }
                break;
            }
        }
    }

    public void ClosePassage()
    {
        closed = true;
        UpdatePassageDoor();
    }

    public void OpenPassage()
    {
        closed = false;
        UpdatePassageDoor();
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

    public PassageObjectTypeInfo GetMetaInfo()
    {
        return metaInfo;
    }
    public string GetId()
    {
        return id;
    }

    public string GetSefiraName()
    {
        return sefiraName;
    }
}

public class DoorObjectModel : ObjectModelBase
{
    public int hp;
    private bool closed;
    
    public MapNode node;

    public void Open()
    {
        closed = false;
        node.closed = false;
    }

    public void Close()
    {
        closed = true;
        node.closed = true;
    }
}

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