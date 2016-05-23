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
		slots = new Slot[5];
		slots [0] = new Slot ();
		slots [1] = new Slot ();
		slots [2] = new Slot ();
		slots [3] = new Slot ();
		slots [4] = new Slot ();
	}
}

/*
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
*/

public class SuppressSetting
{
	public AgentModel actor;
	public UnitModel target; // UnitModel

	public SuppressSetting(AgentModel actor, UnitModel target)
	{
		this.actor = actor;
		this.target = target;
	}
}


public class AutoCommandManager : MonoBehaviour, IObserver {

	private static AutoCommandManager _instance;
	public static AutoCommandManager instance
	{
		get{ return _instance; }
	}

	Dictionary<int, WorkSettingElement> aiList;

	Dictionary<int, SuppressSetting> suppressList;

	//List<SuppressAgentSettingElement> supAgentSettings;
	//List<SuppressCreatureSettingElement> supCreatureSettings;

	void Awake()
	{
		if (_instance != null) {
			Destroy (gameObject);
			return;
		}
		_instance = this;
		aiList = new Dictionary<int, WorkSettingElement> ();
		suppressList = new Dictionary<int, SuppressSetting> ();

		//supAgentSettings = new List<SuppressAgentSettingElement> ();
		//supCreatureSettings = new List<SuppressCreatureSettingElement> ();

		Notice.instance.Observe (NoticeName.AddCreature, this);
		Notice.instance.Observe (NoticeName.ChangeWorkSetting, this);
		Notice.instance.Observe (NoticeName.ClearCreature, this);

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

		List<int> rmSuppressList = new List<int> ();
		foreach (SuppressSetting ai in suppressList.Values)
		{
			if (ai.actor.isDead () || ai.actor.IsPanic () || ai.actor.GetState () == AgentAIState.CANNOT_CONTROLL)
			{
				rmSuppressList.Add (ai.actor.instanceId);
				continue;
			}

			if(ai.target is CreatureModel)
			{
				CreatureModel creatureTarget = (CreatureModel)ai.target;
				if (creatureTarget.state != CreatureState.ESCAPE && creatureTarget.state != CreatureState.ESCAPE_PURSUE)
				{
					rmSuppressList.Add (ai.actor.instanceId);
					continue;
				}
				if(creatureTarget.GetMovableNode().GetPassage() == null) // missing
					continue;

				if (ai.actor.GetState () != AgentAIState.SUPPRESS_CREATURE || ai.actor.target != ai.target)
				{
					ai.actor.SuppressCreature (creatureTarget);
				}
			}
			else if(ai.target is AgentModel)
			{
				AgentModel agentTarget = (AgentModel)ai.target;

				if (agentTarget.IsPanic () == false && agentTarget.GetState () != AgentAIState.CANNOT_CONTROLL)
				{
					rmSuppressList.Add (ai.actor.instanceId);
					continue;
				}
				if (agentTarget.isDead ())
				{
					rmSuppressList.Add (ai.actor.instanceId);
					continue;
				}
				if(agentTarget.GetMovableNode().GetPassage() == null) // missing
					continue;

				if (ai.actor.GetState () != AgentAIState.SUPPRESS_WORKER || ai.actor.targetWorker != ai.target)
				{
					ai.actor.SuppressAgent (agentTarget);
				}
			}
		}

		foreach (int rmTargetId in rmSuppressList)
		{
			suppressList.Remove (rmTargetId);
		}
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

	/*
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
	*/

	public void SetSuppressAction(AgentModel actor, UnitModel target)
	{
		if(target != null)
		{
			SuppressSetting setting = null;
			if (suppressList.TryGetValue (actor.instanceId, out setting))
			{
				setting.target = target;
			}
			else
			{
				setting = new SuppressSetting (actor, target);
				suppressList.Add (actor.instanceId, setting);
			}
		}
		else
		{
			SuppressSetting setting = null;
			if (suppressList.TryGetValue (actor.instanceId, out setting))
			{
				if (actor.GetState () == AgentAIState.SUPPRESS_CREATURE || actor.GetState() == AgentAIState.SUPPRESS_WORKER)
				{
					actor.StopAction ();
				}
				suppressList.Remove (actor.instanceId);
			}
		}
		
	}

	public UnitModel GetSuppressActionTarget(AgentModel actor)
	{
		SuppressSetting setting = null;
		if (suppressList.TryGetValue (actor.instanceId, out setting))
		{
			return setting.target;
		}
		return null;
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

		else if (name == NoticeName.ChangeWorkSetting)
		{
			OnChangeWorkSetting ((CreatureModel)param [0]);
		}
		else if(name == NoticeName.ClearCreature)
		{
			aiList.Clear ();
		}
	}
}
