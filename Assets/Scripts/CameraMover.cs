using UnityEngine;
using System.Collections;

public class CameraMover : MonoBehaviour {

	void FixedUpdate ()
	{
		//float a = Camera.main.aspect * Camera.main.orthographicSize;
		Vector3 pos = Input.mousePosition;

		if(pos.x < 10)
		{
			Vector3 newPos = Camera.main.transform.localPosition;
			newPos.x -= 0.1f;
			Camera.main.transform.localPosition = newPos;
		}


		if(pos.x > Screen.width - 10)
		{
			Vector3 newPos = Camera.main.transform.localPosition;
			newPos.x += 0.1f;
			Camera.main.transform.localPosition = newPos;
		}

		if(pos.y < 10)
		{
			Vector3 newPos = Camera.main.transform.localPosition;
			newPos.y -= 0.1f;
			Camera.main.transform.localPosition = newPos;
		}
		
		
		if(pos.y > Screen.height - 10)
		{
			Vector3 newPos = Camera.main.transform.localPosition;
			newPos.y += 0.1f;
			Camera.main.transform.localPosition = newPos;
		}
		if(Input.GetKey("="))
			Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize-0.1f, 1.5f, 16.5f );
		if(Input.GetKey("-"))
			Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize+0.1f, 1.5f, 16.5f );
	}
}
