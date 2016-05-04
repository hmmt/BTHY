using UnityEngine;
using System.Collections;

public class PanicRoaming : PanicAction {

	private AgentModel actor;

	private float horrorDelay = 5;

	private float horrorElapsedTime;

    public PanicRoaming(AgentModel target)
	{
		actor = target;
	}

	public void Init()
	{
		AgentUnit agentView = AgentLayer.currentLayer.GetAgent (actor.instanceId);
		agentView.puppetAnim.SetBool ("Panic", true);
		agentView.puppetAnim.SetInteger ("PanicType", 3);
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

		if (actor.GetMovableNode().IsMoving() == false && actor.GetMovableNode().InElevator() == false) {
			actor.MoveToNode (MapGraph.instance.GetRoamingNodeByRandom (actor.currentSefira));
		}
	}

	private void SpreadHorror()
	{
		Debug.Log ("SPREAD....");
		foreach (AgentModel agent in AgentManager.instance.GetAgentList())
		{
			if (agent.GetMovableNode ().GetPassage () == actor.GetMovableNode ().GetPassage ()) {
				if (agent == actor)
					break;

				agent.TakeMentalDamage (5);
			}
		}
		foreach(OfficerModel officer in OfficeManager.instance.GetOfficerList())
		{
			if (officer.GetMovableNode ().GetPassage () == actor.GetMovableNode ().GetPassage ()) {
				officer.TakeMentalDamage (5);
			}
		}
	}
}
