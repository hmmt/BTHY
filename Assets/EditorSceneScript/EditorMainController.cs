using UnityEngine;
using System.Collections;

public class EditorMainController : MonoBehaviour {

	// Use this for initialization
	void Start () {
        MapGraph.instance.LoadMap();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
