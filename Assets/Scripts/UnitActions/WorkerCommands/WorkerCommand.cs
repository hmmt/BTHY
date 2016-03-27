using UnityEngine;
using System.Collections;



public class WorkerCommand
{
	public WorkerModel actor;

	public AgentCmdType type;

	// parameters
	//public ObjectModelBase targetObject;
	public CreatureModel targetCreature;
	public WorkerModel targetAgent;

	public DoorObjectModel targetDoor;

	public bool isFinished = false;

	public bool isMoving = false;

	/// <summary>
	/// Raises the init event.
	/// </summary>
	/// <param name="agent">Agent.</param>
	public virtual void OnInit(WorkerModel agent)
	{
		actor = agent;
	}

	/// <summary>
	/// 秦寸 command啊 角青登扁 矫累且 锭 龋免邓聪促.
	/// </summary>
	/// <param name="agent"></param>
	public virtual void OnStart(WorkerModel agent)
	{
	}

	/// <summary>
	/// 秦寸 command啊 角青 吝老 锭 概 橇饭烙 付促 龋免邓聪促.
	/// agent啊 概 橇饭烙付促 且 青悼阑 沥狼钦聪促.
	/// agent狼 青悼阑 捞 皋辑靛俊辑 沥狼窍瘤 臼绊 ActionClassBase俊辑 沥狼窍绰 版快档 乐嚼聪促. ex) UseSkill
	/// </summary>
	/// <param name="agent"></param>
	public virtual void Execute(WorkerModel agent)
	{
	}

	/// <summary>
	/// This method is called after removing from queue
	/// </summary>
	/// <param name="agent">Agent.</param>
	public virtual void OnStop(WorkerModel agent)
	{
	}

	/// <summary>
	/// Raises the destroy event.
	/// </summary>
	public virtual void OnDestroy(WorkerModel agent)
	{
	}

	public void Finish()
	{
		isFinished = true;
	}


	public static WorkerCommand MakeWorking(CreatureModel targetCreature)
	{
		WorkerCommand cmd = new WorkerCommand();
		cmd.type = AgentCmdType.MANAGE_CREATURE;
		cmd.targetCreature = targetCreature;
		//cmd.action = action;
		return cmd;
	}

	public static WorkerCommand MakeObserveCreature(CreatureModel targetCreature)
	{
		ObserveCreatureAgentCommand cmd = new ObserveCreatureAgentCommand (targetCreature);
		cmd.type = AgentCmdType.OBSERVE_CREATURE;

		return cmd;
	}
	public static WorkerCommand MakeManageCreature(CreatureModel targetCreature, AgentModel agent, SkillTypeInfo skill)
	{
		ManageCreatureAgentCommand cmd = new ManageCreatureAgentCommand (targetCreature, agent, skill);
		cmd.type = AgentCmdType.MANAGE_CREATURE;

		return cmd;
	}
	public static WorkerCommand MakeReturnCreature()
	{
		WorkerCommand cmd = new WorkerCommand();
		cmd.type = AgentCmdType.RETURN_CREATURE;
		return cmd;
	}

	public static WorkerCommand MakeOpenRoom(CreatureModel targetCreature)
	{
		WorkerCommand cmd = new OpenIsolateWorkerCommand(targetCreature);
		cmd.type = AgentCmdType.OPEN_ROOM;
		return cmd;
	}

	public static WorkerCommand MakeSuppressWorking(AgentModel targetAgent, SuppressAction suppressAction, SuppressType supType)
	{
		//WorkerCommand cmd = new WorkerCommand();
		SuppressWorkerCommand cmd = new SuppressWorkerCommand(targetAgent, suppressAction, supType);
		cmd.type = AgentCmdType.SUPPRESS_WORKING;
		cmd.targetAgent = targetAgent;
		return cmd;
	}
	public static WorkerCommand MakeSuppressCreature(CreatureModel targetCreature, SuppressAction suppressAction)
	{
		SuppressWorkerCommand cmd = new SuppressWorkerCommand(targetCreature, suppressAction);
		cmd.type = AgentCmdType.SUPPRESS_CREATURE;
		cmd.targetCreature = targetCreature;
		return cmd;
	}

	public static WorkerCommand MakeOpenDoor(DoorObjectModel door)
	{
		OpenDoorAgnetCommand cmd = new OpenDoorAgnetCommand(door);
		cmd.type = AgentCmdType.OPEN_DOOR;
		return cmd;
	}

	public static WorkerCommand MakeCaptureByCreatue()
	{
		WorkerCommand cmd = new WorkerCommand();
		cmd.type = AgentCmdType.CAPTURE_BY_CREATURE;
		return cmd;
	}

	public static WorkerCommand MakeMove(MapNode node)
	{
		MoveWorkerCommand cmd = new MoveWorkerCommand(node);
		cmd.type = AgentCmdType.MOVE;
		return cmd;
	}

	public static WorkerCommand MakePanicPursueAgent(AgentModel targetAgent)
	{
		PanicPursueWorkerCommand cmd = new PanicPursueWorkerCommand (targetAgent);
		cmd.type = AgentCmdType.PANIC_VIOLENCE;
		return cmd;
	}

	public static WorkerCommand MakeUnconPursueAgent(AgentModel targetAgent)
	{
		UnconPursueWorkerCommand cmd = new UnconPursueWorkerCommand (targetAgent);
		cmd.type = AgentCmdType.PANIC_VIOLENCE;
		return cmd;
	}
	//public static AgentCommand MakePanic
}

public class MoveWorkerCommand : WorkerCommand
{
	//public MovableObjectNode targetNode;
	public MapNode targetNode;

	public MoveWorkerCommand(MapNode targetNode)
	{
		this.targetNode = targetNode;
	}
	public override void OnStart(WorkerModel agent)
	{
		base.OnStart(agent);
		MovableObjectNode movable = agent.GetMovableNode();
		movable.MoveToNode(targetNode);
	}
	public override void Execute(WorkerModel agent)
	{
		base.Execute(agent);
		MovableObjectNode movable = agent.GetMovableNode();

		if (!movable.IsMoving())
		{
			movable.MoveToNode(targetNode);
		}

		if (movable.GetCurrentNode() != null && movable.GetCurrentNode().GetId() == targetNode.GetId())
		{
			Finish(); 
		}
	}
	public override void OnStop(WorkerModel agent)
	{
		base.OnStop(agent);
		MovableObjectNode movable = agent.GetMovableNode();
		movable.StopMoving();
	}
}

public class OpenDoorAgnetCommand : WorkerCommand
{
	private DoorObjectModel door;
	private float elapsedTime;

	public OpenDoorAgnetCommand(DoorObjectModel door)
	{
		this.door = door;
		elapsedTime = 0;
	}

	public override void OnStart(WorkerModel agent)
	{
		base.OnStart(agent);
	}
	public override void Execute(WorkerModel agent)
	{
		base.Execute(agent);

		elapsedTime += Time.deltaTime;

		//if (elapsedTime >= 0.5f)
		if (elapsedTime >= 0.0f)
		{
			door.Open();
			Finish();
		}
	}
	public override void OnStop(WorkerModel agent)
	{
		base.OnStop(agent);
	}
}

public class ManageCreatureAgentCommand : WorkerCommand
{
	private AgentModel[] coopAgents;
	private SkillTypeInfo skill;

	private UseSkill useSkill;

	private bool waiting = true;

	public ManageCreatureAgentCommand(CreatureModel targetCreature, AgentModel self, SkillTypeInfo skill)
	{
		this.targetCreature = targetCreature;
		this.skill = skill;
		this.coopAgents = new AgentModel[]{ self };
	}

	public ManageCreatureAgentCommand(CreatureModel targetCreature, AgentModel[] coopAgents, SkillTypeInfo skill)
	{
		this.targetCreature = targetCreature;
		this.skill = skill;
		this.coopAgents = coopAgents;
	}

	public override void OnInit(WorkerModel agent)
	{
		base.OnInit (agent);
		targetCreature.AddTargetedCount ();
	}
	public override void Execute(WorkerModel agent)
	{
		base.Execute(agent);
		if(useSkill == null)
			CheckStarting ((AgentModel)agent);
	}
	public override void OnDestroy(WorkerModel agent)
	{
		targetCreature.ReleaseTargetedCount ();
	}

	public void Cancle()
	{
	}

	private void CheckStarting(AgentModel agent)
	{
		if (!waiting)
			return;
		int count = 0;
		/*
		foreach (AgentModel otherAgent in coopAgents)
		{
			if (otherAgent.GetCurrentCommandType() != AgentCmdType.MANAGE_CREATURE)
			{
				count++;
			}
		}*/

		if (count == 0)
		{
			/*
			foreach (AgentModel otherAgent in coopAgents)
			{
				AgentCommand otherCmd = otherAgent.GetCurrentCommand ();
				ManageCreatureAgentCommand cmd = (ManageCreatureAgentCommand)otherCmd;

				cmd.waiting = false;
			}*/


			useSkill = UseSkill.InitUseSkillAction (skill, agent, targetCreature);

			if (useSkill == null)
				Finish ();
			waiting = false;

			//Finish ();
		}
	}
}

public class ObserveCreatureAgentCommand : WorkerCommand
{
	public ObserveCreatureAgentCommand(CreatureModel targetCreature)
	{
		this.targetCreature = targetCreature;
	}

	public override void OnInit(WorkerModel agent)
	{
		base.OnInit (agent);
		targetCreature.AddTargetedCount ();
	}

	public override void OnStart(WorkerModel agent)
	{
		ObserveCreature.Create ((AgentModel)agent, targetCreature);
	}
	public override void Execute(WorkerModel agent)
	{
		base.Execute(agent);
	}
	public override void OnDestroy(WorkerModel agent)
	{
		targetCreature.ReleaseTargetedCount ();
	}

	public void Cancle()
	{
	}

}