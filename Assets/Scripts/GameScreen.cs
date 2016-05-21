using UnityEngine;
using System.Collections;

public class GameScreen : MonoBehaviour {

	private static GameScreen _instance;
	public static GameScreen instance
	{
		get
		{
			return _instance;
		}
	}

	void Awake()
	{
		/*
		if (_instance != null)
		{
			Destroy (gameObject);
			return;
		}
		_instance = this;

		DontDestroyOnLoad (this);
		*/
	}
}
