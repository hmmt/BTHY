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
    private bool _loaded = false;

    public bool loaded
    {
        get { return _loaded; }
    }

    private TraitTypeList()
    {
        _list = new List<TraitTypeInfo>();
    }

    public void Init(TraitTypeInfo[] list)
    {
        _list = new List<TraitTypeInfo>(list);
        _loaded = true;
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

        if (_list[i].randomFlag != 1)
        {
            while (true)
            {
                i = Random.Range(0, _list.Count);
                if (_list[i].randomFlag == 1)
                    return _list[i];
            }
        }

        return _list[i];
    }

    public TraitTypeInfo GetTraitWithId(long id)
    {
        for(int i=0; i<_list.Count; i++)
        {
            if(id == _list[i].id)
                return _list[i];
        }

        return null;
    }
}

