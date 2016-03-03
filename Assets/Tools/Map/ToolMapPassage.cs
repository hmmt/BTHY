using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class ToolMapPassage : MonoBehaviour {

	public string src = "";

	public PassageObject passageObject;

	private string srcOld = "";

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
