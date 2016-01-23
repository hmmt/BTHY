using UnityEngine;
using System.Collections.Generic;

public class WorkSettingElement
{
	public class Slot
	{
		public SkillTypeInfo skill;
		public int agentCnt;
	}
	public CreatureModel creature;

	public Slot[] slots;

	public WorkSettingElement()
	{
		slots = new Slot[3];
		slots [0] = new Slot ();
		slots [1] = new Slot ();
		slots [2] = new Slot ();
	}
}


public class TempAgentAI : MonoBehaviour, IObserver {

	private static TempAgentAI _instance;
	public static TempAgentAI instance
	{
		get{ return _instance; }
	}

	Dictionary<int, WorkSettingElement> aiList;

	void Awake()
	{
		_instance = this;
		aiList = new Dictionary<int, WorkSettingElement> ();

		Notice.instance.Observe (NoticeName.AddCreature, this);

		DontDestroyOnLoad (gameObject);
	}


	public void FixedUpdate()
	{
		foreach (WorkSettingElement ai in aiList.Values)
		{
			if (ai.slots[0].skill == null)
				continue;
			if (ai.creature.state == CreatureState.WAIT && ai.creature.IsTargeted() == false && ai.creature.IsReady())
			{
				List<AgentModel> agents = GetWaitingAgents ();

				if (agents.Count != 0)
				{
					AgentModel agent = agents [Random.Range(0, agents.Count)];
					if (agent.HasSkill (ai.slots[0].skill)) {
						agent.ManageCreature(ai.creature, ai.slots[0].skill);
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

	public WorkSettingElement GetWorkSetting(CreatureModel targetCreature)
	{
		WorkSettingElement setting;
		if (aiList.TryGetValue (targetCreature.instanceId, out setting))
		{
			return setting;
		}
		else
		{
			Debug.Log ("GetWorkSetting >> creature work setting not found");
			return null;
		}

	}

	public void OnNotice(string name, params object[] param)
	{
		if (name == NoticeName.AddCreature)
		{
			CreatureModel creature = (CreatureModel)param [0];

			WorkSettingElement ai = new WorkSettingElement ();
			ai.creature = creature;

			aiList.Add (ai.creature.instanceId, ai);
		}
	}
}
