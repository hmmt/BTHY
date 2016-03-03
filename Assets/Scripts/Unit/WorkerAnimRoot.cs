using UnityEngine;
using System.Collections.Generic;

public class WorkerAnimRoot : MonoBehaviour {

	private List<WorkerAnim> _list;

	void Awake()
	{
		_list = new List<WorkerAnim> ();
	}

	public void AddAnim(GameObject animPrefab)
	{
		GameObject animObject = (GameObject)Instantiate (animPrefab);

		WorkerAnim animScript = animObject.GetComponent<WorkerAnim> ();

		animObject.transform.SetParent (transform, false);

		_list.Add (animScript);
	}

	public void SetCurrentAnim(string animName)
	{
		foreach (WorkerAnim anim in _list) {
			
		}
	}
}
