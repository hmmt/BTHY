using UnityEngine;
using System.Collections;

public class Prefab
{
	public static GameObject LoadPrefab(string name)
	{
        string path = "Prefabs/" + name ;
        GameObject obj = Resources.Load<GameObject>(path);

		return GameObject.Instantiate(obj) as GameObject;
	}
}
