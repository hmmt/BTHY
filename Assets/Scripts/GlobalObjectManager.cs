using UnityEngine;
using System.Collections;

public class GlobalObjectManager : MonoBehaviour {

	public GameObject selectActionWindow;

	private static GlobalObjectManager _instance;

	public static GlobalObjectManager instance
	{
		get{ return _instance; }
	}

	void Awake()
	{
		_instance = this;
	}

	public SelectActionWindow GetSelectActionWindow()
	{
		return selectActionWindow.GetComponent<SelectActionWindow> ();
	}
}