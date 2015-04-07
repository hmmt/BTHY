using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CreatureTypeList {

	private static CreatureTypeList _instance;
	public static CreatureTypeList instance
	{
		get{
			if(_instance == null)
				_instance = new CreatureTypeList();
			return _instance;
		}
	}
	
	private List<CreatureTypeInfo> _list;
	
	private CreatureTypeList()
	{
		_list = new List<CreatureTypeInfo> ();
	}
	
	public void Init(CreatureTypeInfo[] list)
	{
		_list = new List<CreatureTypeInfo> (list);
	}
	
	public CreatureTypeInfo[] GetList()
	{
		return _list.ToArray ();
	}
	
	public CreatureTypeInfo GetData(long id)
	{
		foreach(CreatureTypeInfo info in _list)
		{
			if(info.id == id)
			{
				return info;
			}
		}
		return null;
	}
}
