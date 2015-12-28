using UnityEngine;
using System.Collections;

public class PassageObject : MonoBehaviour, IObserver {

    public GameObject[] mapObjectPoint;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void AddMapObject(MapObjectModel obj)
    {
    }

    public void OnNotice(string notice, params object[] param)
    {
        //if(
    }
}
