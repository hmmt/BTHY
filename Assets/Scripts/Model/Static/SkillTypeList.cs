using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkillTypeList
{
	private static SkillTypeList _instance;
	public static SkillTypeList instance
	{
		get{
			if(_instance == null)
				_instance = new SkillTypeList();
			return _instance;
		}
	}
	
	private List<SkillTypeInfo> _list;
    private bool _loaded = false;

    public bool loaded
    {
        get { return _loaded; }
    }

	private SkillTypeList()
	{
		_list = new List<SkillTypeInfo> ();
	}
	
	public void Init(SkillTypeInfo[] list)
	{
		_list = new List<SkillTypeInfo> (list);
        _loaded = true;
	}
	
	public SkillTypeInfo[] GetList()
	{
		return _list.ToArray ();
	}

	public SkillTypeInfo GetData(long id)
	{
		foreach(SkillTypeInfo info in _list)
		{
			if(info.id == id)
			{
				return info;
			}
		}
		return null;
	}


    public SkillTypeInfo GetNextSkill(SkillTypeInfo typeInfo)
    {
        if (typeInfo.nextSkillIdList.Length == 0)
        {
            return null;
        }

        SkillTypeInfo skill = GetData(typeInfo.nextSkillIdList[0]);

        return skill;
    }
}
