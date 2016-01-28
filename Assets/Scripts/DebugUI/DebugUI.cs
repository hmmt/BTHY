using UnityEngine;
using System.Collections;

public class DebugUI : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		//asdfasdgsda
	}

	public void OnClick()
	{
		if (GameManager.currentGameManager.state == GameState.PLAYING)
			GameManager.currentGameManager.Pause ();
		else if (GameManager.currentGameManager.state == GameState.PAUSE)
			GameManager.currentGameManager.Resume ();
	}
}
