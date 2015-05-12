using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TraitTypeList
{
    private static TraitTypeList _instance;
    public static TraitTypeList instance
    {
        get
        {
            if (_instance == null)
                _instance = new TraitTypeList();
            return _instance;
        }
    }

    private List<TraitTypeInfo> _list;

    private TraitTypeList()
    {
        _list = new List<TraitTypeInfo>();
    }

    public void Init(TraitTypeInfo[] list)
    {
        _list = new List<TraitTypeInfo>(list);
    }

    public TraitTypeInfo[] GetList()
    {
        return _list.ToArray();
    }

    public TraitTypeInfo GetData(long id)
    {
        foreach (TraitTypeInfo info in _list)
        {
            if (info.id == id)
            {
                return info;
            }
        }
        return null;
    }

    public TraitTypeInfo GetRandomInitTrait()
    {
        int i = 0;

        i = Random.Range(0, _list.Count);

        return _list[i];
    }
}

