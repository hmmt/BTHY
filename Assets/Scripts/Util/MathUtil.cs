using UnityEngine;
using System;
using System.Collections.Generic;

public class MathUtil
{
	public static int UnitStep(float n)
	{
		return n > 0 ? 1 : 0;
	}
}

public class GameUtil
{
    public static bool TryGetValue<T>(Dictionary<string, object> dic, string name, ref T field)
    {
        object output;
        if (dic.TryGetValue(name, out output))
        {
            field = (T)output;
            return true;
        }
        return false;
    }
}


public class AnimatorUtil
{
	public static bool HasParameter(Animator animator, string name)
	{
		foreach (AnimatorControllerParameter param in animator.parameters) {
			if (param.name == name)
				return true;
		}
		return false;
	}
}