using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public delegate void Callback();

public class DestroyHandler : MonoBehaviour {

	private List<Callback> list = new List<Callback>();

	void OnDisable()
	{
		foreach(Callback e in list.ToArray())
		{
			e();
		}
		list.Clear ();
	}

	void OnDestroy()
	{
		foreach(Callback e in list)
		{
			e();
		}
		list.Clear ();
	}

	public Callback AddReceiver(Callback e)
	{
		list.Add (e);
		return () => { list.Remove(e); };
	}
}
