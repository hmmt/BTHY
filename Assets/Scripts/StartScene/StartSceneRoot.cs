using UnityEngine;
using System.Collections;

public class StartSceneRoot : MonoBehaviour {

	private static StartSceneRoot _instance = null;
	public static StartSceneRoot instance
	{
		get
		{
			return _instance;
		}
	}

	void Awake()
	{
		if (_instance != null) {
			Destroy (gameObject);
			return;
		}

		_instance = this;

		DontDestroyOnLoad (gameObject);

		GameStaticDataLoader.LoadStaticData();
		MapGraph.instance.LoadMap ();
	}
}
