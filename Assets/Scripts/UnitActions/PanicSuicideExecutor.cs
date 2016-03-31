using UnityEngine;
using System.Collections.Generic;

public class PanicSuicideExecutor : PanicAction
{
	private AgentModel targetAgent;

	private float suicideDelay = 7;
	private float horrorDelay = 5;

	private float elapsedTime;
	private float horrorElapsedTime;

    public PanicSuicideExecutor(AgentModel target)
    {
        targetAgent = target;
    }

	public void Execute()
	{
		float deltaTime = Time.deltaTime;

		elapsedTime += deltaTime;
		if(elapsedTime > suicideDelay)
		{
			elapsedTime -= suicideDelay;

			TrySuicide();
		}

		horrorElapsedTime += deltaTime;
		if(horrorElapsedTime > horrorDelay)
		{
			horrorElapsedTime -= horrorDelay;

			SpreadHorror();
		}
	}

	private void SpreadHorror()
	{
		foreach (AgentModel agent in AgentManager.instance.GetAgentList())
		{
			if (agent.GetMovableNode ().GetPassage () == targetAgent.GetMovableNode ().GetPassage ()) {
				if (agent == targetAgent)
					break;

				agent.TakeMentalDamage (5);
			}
		}
		foreach(OfficerModel officer in OfficeManager.instance.GetOfficerList())
		{
			if (officer.GetMovableNode ().GetPassage () == targetAgent.GetMovableNode ().GetPassage ()) {
				officer.TakeMentalDamage (5);
			}
		}
	}

	private void TrySuicide()
	{
		/*
        targetAgent.TakePhysicalDamage(1);

        if (targetAgent.isDead())
        {

        }
        */
		int r = Random.Range (0, 10);

		if (r < 7)
		{
			targetAgent.TakePhysicalDamage (targetAgent.hp, DamageType.CUSTOM);
			// set motion
		}
	}
}