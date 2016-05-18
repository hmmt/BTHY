using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class ToolMapSefiraArea : MonoBehaviour {

	public string sefiraName;
	public int sub;


	void Awake()
	{
	}

	public static ToolMapSefiraArea AddSefiraArea(string name)
	{
		GameObject g = new GameObject ("MapSefiraArea-" + name);
		ToolMapSefiraArea mapSefiraArea = g.AddComponent<ToolMapSefiraArea> ();
		mapSefiraArea.sefiraName = name;

		ToolMapRoot root = ToolMapGraph_Editor.GetMapRoot ();

		g.transform.SetParent (root.transform);

		return mapSefiraArea;
	}
}
