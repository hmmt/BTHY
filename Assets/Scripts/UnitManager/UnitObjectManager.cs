using UnityEngine;
using System.Collections.Generic;

public class UnitObjectManager {

	private static UnitObjectManager _instance;

	private List<ObjectModelBase> unitList;

	void Init()
	{
		unitList = new List<ObjectModelBase> ();
	}
}
