using UnityEngine;
using System.Collections;

public class PanicRoaming : PanicAction {

    private AgentModel targetAgent;

	private float horrorDelay = 5;

	private float horrorElapsedTime;

	private float waitTimer = 0;

    public PanicRoaming(AgentModel target)
	{
		targetAgent = target;
	}

	public void Execute()
	{
		float deltaTime = Time.deltaTime;

		horrorElapsedTime += deltaTime;
		if(horrorElapsedTime > horrorDelay)
		{
			horrorElapsedTime -= horrorDelay;

			SpreadHorror();
		}

		if (waitTimer <= 0) {
			targetAgent.MoveToNode (MapGraph.instance.GetSepiraNodeByRandom (targetAgent.currentSefira));
			waitTimer = 1.5f + Random.value;
		}

		waitTimer -= Time.deltaTime;
	}

	private void SpreadHorror()
	{
		Debug.Log ("SPREAD....");
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
}
