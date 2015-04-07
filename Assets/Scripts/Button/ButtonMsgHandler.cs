using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class ButtonMsgHandler : MonoBehaviour
{
    /*
	public enum Trigger
	{
		OnClick,
		OnMouseOver,
		OnMouseOut,
		OnPress,
		OnRelease,
		OnDoubleClick,
	}
	
	public GameObject target;
	public string functionName;
	public Trigger trigger = Trigger.OnClick;
	public bool includeChildren = false;
	
	bool mStarted = false;
	bool mHighlighted = false;

	void Start () {if (LayerMask.LayerToName (gameObject.layer) != "UI") Debug.LogWarning("layer must be 'UI'"); mStarted = true; }
	
	void OnEnable () { if (mStarted && mHighlighted) OnHover(UICamera.IsHighlighted(gameObject)); }
	
	void OnHover (bool isOver)
	{
		if (enabled)
		{
			if (((isOver && trigger == Trigger.OnMouseOver) ||
			     (!isOver && trigger == Trigger.OnMouseOut))) Send();
			mHighlighted = isOver;
		}
	}
	
	void OnPress (bool isPressed)
	{
		if (enabled)
		{
			if (((isPressed && trigger == Trigger.OnPress) ||
			     (!isPressed && trigger == Trigger.OnRelease))) Send();
		}
	}
	
	void OnClick () { if (enabled && trigger == Trigger.OnClick) Send(); }
	
	void OnDoubleClick () { if (enabled && trigger == Trigger.OnDoubleClick) Send(); }
	
	void Send ()
	{
		if (string.IsNullOrEmpty(functionName)) return;
		if (target == null) target = gameObject;
		
		if (includeChildren)
		{
			Transform[] transforms = target.GetComponentsInChildren<Transform>();
			
			for (int i = 0, imax = transforms.Length; i < imax; ++i)
			{
				Transform t = transforms[i];
				t.gameObject.SendMessage(functionName, gameObject, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			target.SendMessage(functionName, gameObject, SendMessageOptions.DontRequireReceiver);
		}
	}
     */
}