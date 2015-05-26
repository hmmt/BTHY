using UnityEngine;
using System.Collections;

public class PanicSuicideExecutor : PanicAction
{
	private AgentModel targetAgent;

	private float cooldown;

	private int endurance; // 버틸 수 있는 양


	private float elapsedTime;

	public void Execute()
	{
		float deltaTime = Time.deltaTime;
		elapsedTime += deltaTime;
		if(elapsedTime > cooldown)
		{
			elapsedTime -= cooldown;

			TrySuicide();
		}
	}

	private void TrySuicide()
	{
		//
		if(Random.value < 0.25)
		{
			if(endurance > 0)
			{
				endurance--;
			}
			else
			{
				// suicide;
				targetAgent.Die();
			}
		}
	}
}