using UnityEngine;
using System.Collections;

public class MouseEventManager : MonoBehaviour {

	private GameObject lastHover = null;

	void Update ()
	{
		Collider2D col = Physics2D.OverlapPoint (Camera.main.ScreenToWorldPoint (Input.mousePosition), 1 << LayerMask.NameToLayer("UI"));

		if(col != null)
		{
			GameObject target = col.gameObject;

			if(target != lastHover)
			{
				if(lastHover != null)
				{
					lastHover.SendMessage("OnHover", false, SendMessageOptions.DontRequireReceiver);
				}
				target.SendMessage("OnHover", true, SendMessageOptions.DontRequireReceiver);

				lastHover = target;
			}
			if( Input.GetMouseButtonDown(0))
			{
				target.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			if(lastHover != null)
			{
				lastHover.SendMessage("OnHover", false, SendMessageOptions.DontRequireReceiver);
				lastHover = null;
			}
		}
	}
}
