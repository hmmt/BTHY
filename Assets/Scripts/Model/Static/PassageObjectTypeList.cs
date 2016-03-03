using UnityEngine;
using System.Collections.Generic;

public class PassageObjectTypeList {

    private static PassageObjectTypeList _instance;
    public static PassageObjectTypeList instance
    {
        get
        {
            if (_instance == null)
                _instance = new PassageObjectTypeList();
            return _instance;
        }
    }

    private List<PassageObjectTypeInfo> _list;
    private bool _loaded = false;

    public bool loaded
    {
        get { return _loaded; }
    }

    private PassageObjectTypeList()
	{
        _list = new List<PassageObjectTypeInfo>();
	}

    public void Init(PassageObjectTypeInfo[] list)
	{
        _list = new List<PassageObjectTypeInfo>(list);
        _loaded = true;
	}

    public PassageObjectTypeInfo[] GetList()
	{
		return _list.ToArray ();
	}

    public PassageObjectTypeInfo GetData(long id)
	{
        foreach (PassageObjectTypeInfo info in _list)
		{
			if(info.id == id)
			{
				return info;
			}
		}
		return null;
	}

	public PassageObjectTypeInfo GetVoidData()
	{
        PassageObjectTypeInfo info = new PassageObjectTypeInfo ();
		info.id = -1;
		info.prefabSrc = "Map/Passage/PassageEmpty";
		return info;
	}
}
