using UnityEngine;
using System.Collections.Generic;

public class TempAgentAI : MonoBehaviour, IObserver {

	private class AIElement
	{
		public CreatureModel creature;
		public SkillTypeInfo skill;
	}

	List<AIElement> aiList;

	void Awake()
	{
		aiList = new List<AIElement> ();

		Notice.instance.Observe (NoticeName.AddCreature, this);
	}


	public void FixedUpdate()
	{

		//CreatureModel[] creatures = CreatureManager.instance.GetCreatureList ();
	
		/*
		foreach (CreatureModel creature in creatures)
		{
			if (creature.state == CreatureState.WAIT)
			{
				if (agents.Count != 0)
				{
					AgentModel agent = agents [0];
					UseSkill.InitUseSkillAction(skillInfo, agent, targetCreature);
				}
			}
		}*/

		foreach (AIElement ai in aiList)
		{
			if (ai.creature.state == CreatureState.WAIT && ai.creature.IsTargeted() == false && ai.creature.IsReady())
			{
				List<AgentModel> agents = GetWaitingAgents ();

				if (agents.Count != 0)
				{
					AgentModel agent = agents [Random.Range(0, agents.Count)];
					if (agent.HasSkill (ai.skill)) {
						//UseSkill.InitUseSkillAction (ai.skill, agent, ai.creature);
						agent.ManageCreature(ai.creature, ai.skill);
						Debug.Log ("AI :: ");
					}
					break;
				}
			}
		}
	}

	public List<AgentModel> GetWaitingAgents()
	{
		AgentModel[] agents = AgentManager.instance.GetAgentList ();
		List<AgentModel> output = new List<AgentModel> ();
		foreach (AgentModel agent in agents)
		{
			if (agent.GetState () == AgentCmdState.IDLE && !agent.isDead())
			{
				output.Add (agent);
			}
		}
		return output;
	}

	public void OnNotice(string name, params object[] param)
	{
		if (name == NoticeName.AddCreature)
		{
			CreatureModel creature = (CreatureModel)param [0];

			AIElement ai = new AIElement ();
			ai.creature = creature;
			ai.skill = SkillTypeList.instance.GetData (10001);

			aiList.Add (ai);
		}
	}


}
