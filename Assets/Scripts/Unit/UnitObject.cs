using UnityEngine;
using System.Collections.Generic;

public class ObjectModelBase
{
    public string GetType()
    {
        return "unknown";
    }
}

public class UnitObjectModel : ObjectModelBase
{
    virtual public MovableObjectNode GetMovableNode()
    {
        return null;
    }
}

public class PassageObjectModel : ObjectModelBase
{
    private List<MapObjectModel> mapObjectList;
    private Dictionary<string, MapNode> mapNodeList;

    public PassageObjectModel()
    {
        mapObjectList = new List<MapObjectModel>();
    }

    public void AddMapObject(MapObjectBloodModel mapObject)
    {
        mapObjectList.Add(mapObject);
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
}

public class MapObjectModel : ObjectModelBase
{
    public int horrorPoint;
}

public class MapObjectBloodModel : MapObjectModel
{
    public string bloodEffectPreb;
}