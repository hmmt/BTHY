using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ToolMapPassage_bloodPoint
{
	public bool use;
	public float height;
	public List<string> sprites;

	public ToolMapPassage_bloodPoint()
	{
		sprites = new List<string> ();
	}
}

[ExecuteInEditMode]
public class ToolMapPassage : MonoBehaviour {

	public string src = "";

	public string passageType = "";
	public PassageObject passageObject;

	private string srcOld = "";

	public ToolMapPassage_bloodPoint ground;
	public ToolMapPassage_bloodPoint wall;

	void Awake()
	{
	}


	void Update()
	{
		if (src != srcOld)
		{
			srcOld = src;
			string path = "Prefabs/" + src ;
			GameObject obj = Resources.Load<GameObject>(path);

			if (obj != null)
			{
				if(obj.GetComponent<PassageObject>())
				{
					GameObject g = GameObject.Instantiate (obj) as GameObject;

					PassageObject newPassageObj = g.GetComponent<PassageObject> ();
					if(newPassageObj != null)
					{
						if(passageObject != null)
						{
							DestroyImmediate(passageObject.gameObject);
						}
						passageObject = newPassageObj;
						passageObject.transform.localPosition = Vector3.zero;
						passageObject.SetWhite ();
						g.transform.SetParent (transform, false);
					}
				}
			}
		}

	}

	public static ToolMapPassage AddPassage(ToolMapSefiraArea sefira)
	{
		GameObject g = new GameObject ("passage");

		ToolMapPassage passage = g.AddComponent<ToolMapPassage> ();

		g.transform.SetParent (sefira.transform);

		return passage;
	}
}
