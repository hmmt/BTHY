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

public class SuppressCreatureSettingElement
{
	public CreatureModel creature;
	public List<SuppressAction> sup;

	public SuppressCreatureSettingElement(CreatureModel creature)
	{
		this.creature = creature;
		sup = new List<SuppressAction> ();
	}

	public void AddSuppress(SuppressAction action)
	{
		sup.Add (action);
	}

	public void RemoveSuppress(AgentModel agent)
	{
		SuppressAction rm = null;
		foreach(SuppressAction sa in sup)
		{
			if (sa.model == agent) {
				rm = sa;
				break;
			}
		}

		if (rm != null)
			sup.Remove (rm);
	}
}

public class SuppressAgentSettingElement
{
	public WorkerModel worker;
	public List<SuppressAction> sup;

	public SuppressAgentSettingElement(WorkerModel worker)
	{
		this.worker = worker;
		sup = new List<SuppressAction> ();
	}

	public void AddSuppress(SuppressAction action)
	{
		sup.Add (action);
	}

	public void RemoveSuppress(AgentModel agent)
	{
		SuppressAction rm = null;
		foreach(SuppressAction sa in sup)
		{
			if (sa.model == agent) {
				rm = sa;
				break;
			}
		}

		if (rm != null)
			sup.Remove (rm);
	}
}



public class AutoCommandManager : MonoBehaviour, IObserver {

	private static AutoCommandManager _instance;
	public static AutoCommandManager instance
	{
		get{ return _instance; }
	}

	Dictionary<int, WorkSettingElement> aiList;

	List<SuppressAgentSettingElement> supAgentSettings;
	List<SuppressCreatureSettingElement> supCreatureSettings;

	void Awake()
	{
		if (_instance != null) {
			Destroy (gameObject);
			return;
		}
		_instance = this;
		aiList = new Dictionary<int, WorkSettingElement> ();

		supAgentSettings = new List<SuppressAgentSettingElement> ();
		supCreatureSettings = new List<SuppressCreatureSettingElement> ();

		Notice.instance.Observe (NoticeName.AddCreature, this);
		Notice.instance.Observe (NoticeName.ChangeWorkSetting, this);

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
		/*
		List<SuppressAgentSettingElement> rmSupAgentList = new List<SuppressAgentSettingElement> ();
		foreach(SuppressAgentSettingElement setting in supAgentSettings)
		{
			if(setting.worker.IsSuppable())
			{
				rmSupAgentList.Add (setting);
				continue;
			}

			foreach(SuppressAction sa in setting.sup)
			{
				if (sa.model.GetState () != AgentAIState.SUPPRESS_WORKER &&
				   sa.model.GetState () != AgentAIState.SUPPRESS_CREATURE)
				{
					sa.model.StartSuppressAgent (setting.worker);
				}
			}
		}

		List<SuppressCreatureSettingElement> rmSupCreatureList = new List<SuppressCreatureSettingElement> ();
		foreach(SuppressCreatureSettingElement setting in supCreatureSettings)
		{
			if (setting.creature.state != CreatureState.ESCAPE &&
			   setting.creature.state != CreatureState.ESCAPE_ATTACK &&
			   setting.creature.state != CreatureState.ESCAPE_PURSUE &&
			   setting.creature.state != CreatureState.ESCAPE_RETURN)
			{
				rmSupCreatureList.Add (setting);
				continue;
			}
			foreach(SuppressAction sa in setting.sup)
			{
				if (sa.model.GetState () != AgentAIState.SUPPRESS_WORKER &&
					sa.model.GetState () != AgentAIState.SUPPRESS_CREATURE)
				{
					sa.model.SuppressCreature (setting.creature);
				}
			}
		}
		*/
	}

	public List<AgentModel> GetWaitingAgents()
	{
		AgentModel[] agents = AgentManager.instance.GetAgentList ();
		List<AgentModel> output = new List<AgentModel> ();
		foreach (AgentModel agent in agents)
		{
			if (agent.GetState () == AgentAIState.IDLE && !agent.isDead())
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

	private void OnChangeWorkSetting(CreatureModel targetCreature)
	{
		// TODO: 
		WorkSettingElement setting;
		if (aiList.TryGetValue (targetCreature.instanceId, out setting))
		{
			foreach (AgentModel agent in AgentManager.instance.GetAgentList()) {
				if (agent.GetState () == AgentAIState.MANAGE &&
				   agent.target != null && agent.target.instanceId == targetCreature.instanceId)
				{
					agent.FinishWorking();
				}
			}
		}
		Notice.instance.Send ("nico");
	}

	private void OnChangeWorkPriority()
	{
	}

	public SuppressCreatureSettingElement GetSuppressCreatureSetting(CreatureModel creature)
	{
		foreach (SuppressCreatureSettingElement setting in supCreatureSettings) {
			if (setting.creature == creature)
				return setting;
		}
		return null;
	}

	public SuppressAgentSettingElement GetSuppressAgentSetting(WorkerModel worker)
	{
		foreach (SuppressAgentSettingElement setting in supAgentSettings) {
			if (setting.worker == worker)
				return setting;
		}
		return null;
	}

	public void SetSuppressCreature(CreatureModel creature, SuppressAction sa)
	{
		SuppressCreatureSettingElement setting = GetSuppressCreatureSetting (creature);
		if (setting == null)
		{
			setting = new SuppressCreatureSettingElement (creature);
			supCreatureSettings.Add (setting);
		}

		setting.AddSuppress (sa);
	}

	public void SetSuppressAgent(WorkerModel worker, SuppressAction sa)
	{
		SuppressAgentSettingElement setting = GetSuppressAgentSetting(worker);
		if (setting == null)
		{
			setting = new SuppressAgentSettingElement (worker);
			supAgentSettings.Add (setting);
		}

		setting.AddSuppress (sa);
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

		if (name == NoticeName.ChangeWorkSetting)
		{
			OnChangeWorkSetting ((CreatureModel)param [0]);
		}
	}
}
