using UnityEngine;
using System.Collections.Generic;

public class MapObjectTypeList {

    private static MapObjectTypeList _instance;
    public static MapObjectTypeList instance
    {
        get
        {
            if (_instance == null)
                _instance = new MapObjectTypeList();
            return _instance;
        }
    }

    private List<MapObjectTypeInfo> _list;
    private bool _loaded = false;

    public bool loaded
    {
        get { return _loaded; }
    }

    private MapObjectTypeList()
	{
        _list = new List<MapObjectTypeInfo>();

        // temp

        MapObjectTypeInfo info = new MapObjectTypeInfo();
        info.id = 1;
        info.horrorPoint = 10;
        info.src = "Map/MapObject";
        _list.Add(info);
        _loaded = true;
	}

    public void Init(MapObjectTypeInfo[] list)
	{
        _list = new List<MapObjectTypeInfo>(list);
        _loaded = true;
	}

    public MapObjectTypeInfo[] GetList()
	{
		return _list.ToArray ();
	}

    public MapObjectTypeInfo GetData(long id)
	{
        foreach (MapObjectTypeInfo info in _list)
		{
			if(info.id == id)
			{
				return info;
			}
		}
		return null;
	}}
