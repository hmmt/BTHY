using UnityEngine;
using System.Collections.Generic;

public class PassageObjectLayer : MonoBehaviour, IObserver {

    public static PassageObjectLayer currentLayer { private set; get; }

    private List<PassageObject> creatureList;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnNotice(string notice, params object[] param)
    {
    }
}
