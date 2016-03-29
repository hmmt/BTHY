using UnityEngine;
using System.Collections.Generic;

public class CreatureAnimScript : MonoBehaviour {
	private class ParameterInfo
	{
		public enum ParameterType
		{
			INT,
			FLOAT,
			BOOL
		}

		public string name;
		public int ivalue;
		public bool bvalue;
		public ParameterType type;

		public ParameterInfo(string name, int value)
		{
			this.name = name;
			this.ivalue = value;
			this.type = ParameterType.INT;
		}

		public ParameterInfo(string name, bool value)
		{
			this.name = name;
			this.bvalue = value;
			this.type = ParameterType.BOOL;
		}
	}


	public Animator animator;

	private Stack<ParameterInfo> updatedParameters;

	void Awake()
	{
		updatedParameters = new Stack<ParameterInfo> ();
	}

	public void SetParameterOnce(string pname, int value)
	{
		updatedParameters.Push (new ParameterInfo (pname, animator.GetInteger (pname)));
		animator.SetInteger (pname, value);
	}

	public void SetParameterOnce(string pname, bool value)
	{
		updatedParameters.Push (new ParameterInfo (pname, animator.GetBool (pname)));
		animator.SetBool (pname, value);
	}

	public void SetParameter(string pname, bool value)
	{
		animator.SetBool (pname, value);
	}

	void LateUpdate()
	{
		while (updatedParameters.Count > 0)
		{
			ParameterInfo info = updatedParameters.Pop ();
			if(info.type == ParameterInfo.ParameterType.INT)
				animator.SetInteger(info.name, info.ivalue);
			else
				animator.SetBool(info.name, info.bvalue);
		}
	}
}
