using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AgentTypeList
{
	private static AgentTypeList _instance;
	public static AgentTypeList instance
	{
		get{
			if(_instance == null)
				_instance = new AgentTypeList();
			return _instance;
		}
	}

	private List<AgentTypeInfo> _list;

	private AgentTypeList()
	{
		_list = new List<AgentTypeInfo> ();
	}

	public void Init(AgentTypeInfo[] list)
	{
		_list = new List<AgentTypeInfo> (list);
	}

	public AgentTypeInfo[] GetList()
	{
		return _list.ToArray ();
	}

	public AgentTypeInfo GetData(long id)
	{
		foreach(AgentTypeInfo info in _list)
		{
			if(info.id == id)
			{
				return info;
			}
		}
		return null;
	}
}
