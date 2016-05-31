using UnityEngine;
using System.Collections;

public class MachineAnim : CreatureAnimScript, IAnimatorEventCalled {


	SingingMachine script;

	public void Init(SingingMachine script) {
		this.script = script;
	}

	public void OnCalled()
	{
		throw new System.NotImplementedException();
	}

	public void OnCalled(int i)
	{
		throw new System.NotImplementedException();
	}

	public void AgentReset()
	{
		throw new System.NotImplementedException();
	}

	public void AnimatorEventInit()
	{
		throw new System.NotImplementedException();
	}

	public void CreatureAnimCall(int i, CreatureBase script)
	{
		throw new System.NotImplementedException();
	}

	public void TakeDamageAnim(int isPhysical)
	{
		//throw new System.NotImplementedException();
	}

	public void AttackCalled(int i)
	{
		throw new System.NotImplementedException();
	}

	public void SoundMake(string src)
	{
		CreatureUnit unit = script.GetCurrentCreatureUnit ();
		unit.PlaySound(src);
	}


	public void PlayAttackEffect()
	{
		//script.get
		script.PlayAttackEffect();
	}
}
