using UnityEngine;
using System.Collections.Generic;

interface ISerializablePlayData{

	Dictionary<string, object> GetSaveData ();
	void LoadData (Dictionary<string, object> dic);
}


/*
public class PlayData
{
	Dictionary<string, object> 
	object value;
}
*/