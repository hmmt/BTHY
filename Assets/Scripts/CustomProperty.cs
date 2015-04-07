using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CustomProperty : MonoBehaviour {

	[System.Serializable]
	public class Property
	{
		public string key;
		public string value;
	}

	public Property[] properties;

	private Dictionary<string, string> _dic = null;

	void Awake()
	{
		if(_dic == null)
			initDictionary();
	}

	void initDictionary()
	{
		_dic = new Dictionary<string, string> ();
		foreach(Property p in properties)
		{
			_dic.Add(p.key, p.value);
		}
	}


	public string GetValue(string key)
	{
		if(_dic == null)
			initDictionary();

		string output = null;

		_dic.TryGetValue (key, out output);

		return output;
	}
}
