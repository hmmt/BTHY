using UnityEngine;
using System.Collections;

public class PanicSuicideExecutor : PanicAction
{
	private AgentModel targetAgent;

	private float cooldown = 5;

	private float elapsedTime;

    private int shield;

    public PanicSuicideExecutor(AgentModel target, int shield)
    {
        targetAgent = target;
        this.shield = shield;
    }

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
        if (shield > 0)
        {
            shield--;
            Debug.Log("TrySuicide : stamina decrease by 1");
        }
        else
        {
            targetAgent.TakePhysicalDamage(1);
            Debug.Log("TrySuicide : ü�� decrease by 1");

            if (targetAgent.isDead())
            {

            }
        }
	}
}