using UnityEngine;
using System.Collections;

public class TeddyAnim : CreatureAnimScript {

	void SpecialAttack()
	{
		animator.SetInteger ("AttackType", 2);
	}
}
