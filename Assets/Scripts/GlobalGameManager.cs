using UnityEngine;
using System.Collections;

public class GlobalGameManager : MonoBehaviour {

	private static GlobalGameManager _instance = null;
	public static GlobalGameManager instance
	{
		get
		{
			return _instance;
		}
	}

    public string language = "en";

	void Awake()
	{
		if (_instance != null) {
            Debug.Log(gameObject.name);
			Destroy (gameObject);
			return;
		}

		_instance = this;

		DontDestroyOnLoad (gameObject);

		GameStaticDataLoader.LoadStaticData();
		MapGraph.instance.LoadMap ();
	}

    void Start() {

    }

	void Update()
	{
        /*
        foreach (AudioListener listener in GameObject.FindObjectsOfType<AudioListener>())
        {
            if (listener.gameObject == Camera.main.gameObject) {
                Debug.Log("not this");
                continue;
            }
            Destroy(listener.gameObject);
        }*/

		if (Application.loadedLevelName == "StartScene")
		{
			Debug.Log ("load....");
			Application.LoadLevel ("Main");
		}
	}
}
