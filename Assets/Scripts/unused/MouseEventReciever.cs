using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class MouseEventReciever : MonoBehaviour {

	public virtual void OnClick()
	{
	}

	public virtual void OnHover(bool isOver)
	{
	}

	public virtual void OnUnhover()
	{
	}
}
