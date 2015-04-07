using UnityEngine;
using System.Collections;

[System.AttributeUsage(System.AttributeTargets.Method)]
public class TimerAttr : System.Attribute
{
	private float timer;
	public TimerAttr()
	{
	}

}
