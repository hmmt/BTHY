using UnityEngine;
using UnityEngine.SceneManagement;
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
			Destroy (gameObject);
			return;
		}

		_instance = this;

		DontDestroyOnLoad (gameObject);

		GameStaticDataLoader.LoadStaticData();
		MapGraph.instance.LoadMap ();
	}

	void Update()
	{
		//if (Application.loadedLevelName == "StartScene")
		if (SceneManager.GetActiveScene ().name == "StartScene")
		{
			Debug.Log ("load....");
			//Application.LoadLevel ("Main");
			SceneManager.LoadScene("StorySceneTemp");
		}
	}
}