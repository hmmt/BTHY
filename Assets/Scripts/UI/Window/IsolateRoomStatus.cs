using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IsolateRoomStatus : MonoBehaviour {

	public UnityEngine.UI.Text NameText;
	public UnityEngine.UI.Text FeelingText;

	public Transform anchor;


	private IsolateRoom target = null;


	public static IsolateRoomStatus CreateWindow(IsolateRoom room)
	{
		GameObject newObj = Instantiate(Resources.Load<GameObject> ("Prefabs/IsolateRoomStatus")) as GameObject;

		IsolateRoomStatus inst = newObj.GetComponent<IsolateRoomStatus> ();
		inst.target = room;
		inst.UpdateCreatureStatus ();
		inst.UpdatePosition ();

		return inst;
	}

	void FixedUpdate()
	{
		UpdatePosition ();
	}

	public void OnOpen()
	{

	}

	private void UpdatePosition()
	{
		if(target != null)
		{
			Vector3 targetPos = target.transform.position;

			anchor.position = Camera.main.WorldToScreenPoint(targetPos);
		}
	}

	public void UpdateCreatureStatus()
	{
		NameText.text = "name : " + target.targetUnit.metaInfo.name;
		FeelingText.text = "feel : " + target.targetUnit.feeling;
	}

	public void OnClickClose()
	{
		CloseWindow ();
	}

	public void CloseWindow()
	{
		Destroy (gameObject);
	}

}
