﻿using UnityEngine;
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
	public static WorkerCommand MakeReturnCreature(CreatureModel target)
	{
		ReturnCreatureWorkerCommand cmd = new ReturnCreatureWorkerCommand(target);
		cmd.type = AgentCmdType.RETURN_CREATURE;
		return cmd;
	}

	public static WorkerCommand MakeOpenRoom(CreatureModel targetCreature)
	{
		WorkerCommand cmd = new OpenIsolateWorkerCommand(targetCreature);
		cmd.type = AgentCmdType.OPEN_ROOM;
		return cmd;
	}

	public static WorkerCommand MakeSuppressWorking(WorkerModel targetWorker)
	{
		//WorkerCommand cmd = new WorkerCommand();
		SuppressWorkerCommand cmd = new SuppressWorkerCommand(targetWorker);
		cmd.type = AgentCmdType.SUPPRESS_WORKING;
		cmd.targetAgent = targetWorker;
		return cmd;
	}
	public static WorkerCommand MakeSuppressCreature(CreatureModel targetCreature)
	{
		SuppressWorkerCommand cmd = new SuppressWorkerCommand(targetCreature);
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

	public static WorkerCommand MakeMove(MapNode node)
	{
		MoveWorkerCommand cmd = new MoveWorkerCommand(node);
		cmd.type = AgentCmdType.MOVE;
		return cmd;
	}

	public static WorkerCommand MakeMove(MovableObjectNode movable)
	{
		MoveWorkerCommand cmd = new MoveWorkerCommand(movable);
		cmd.type = AgentCmdType.MOVE;
		return cmd;
	}

	public static WorkerCommand MakePanicPursueAgent(AgentModel targetAgent)
	{
		PanicPursueWorkerCommand cmd = new PanicPursueWorkerCommand (targetAgent);
		cmd.type = AgentCmdType.PANIC_VIOLENCE;
		return cmd;
	}

	public static WorkerCommand MakeUnconPursueAgent(WorkerModel targetAgent)
	{
		UnconPursueWorkerCommand cmd = new UnconPursueWorkerCommand (targetAgent);
		cmd.type = AgentCmdType.PANIC_VIOLENCE;
		return cmd;
	}

	public static WorkerCommand MakeFollowAgent(MovableObjectNode targetNode)
	{
		FollowWorkerCommand cmd = new FollowWorkerCommand (targetNode);
		cmd.type = AgentCmdType.FOLLOW;
		return cmd;
	}

	//public static AgentCommand MakePanic
}

public class MoveWorkerCommand : WorkerCommand
{
	//public MovableObjectNode targetNode;
	public MapNode targetNode;
	public MovableObjectNode targetMovable;

	public MoveWorkerCommand(MapNode targetNode)
	{
		this.targetNode = targetNode;
	}
	public MoveWorkerCommand(MovableObjectNode movableNode)
	{
		this.targetMovable = movableNode;
	}

	public override void OnStart(WorkerModel agent)
	{
		base.OnStart(agent);
		MovableObjectNode movable = agent.GetMovableNode();
		if (targetNode != null)
		{
			movable.MoveToNode (targetNode);
		}
		else
		{
			movable.MoveToMovableNode (targetMovable);
		}
			
	}
	public override void Execute(WorkerModel agent)
	{
		base.Execute(agent);
		MovableObjectNode movable = agent.GetMovableNode();

		if(targetNode != null)
		{
			if (!movable.IsMoving())
			{
				movable.MoveToNode(targetNode);
			}

			if (movable.GetCurrentNode() != null && movable.GetCurrentNode().GetId() == targetNode.GetId())
			{
				Finish(); 
			}
		}
		else
		{
			if (!movable.IsMoving())
			{
				movable.MoveToMovableNode (targetMovable);
			}

			if ((movable.GetCurrentViewPosition() - targetMovable.GetCurrentViewPosition()).sqrMagnitude < 1)
			{
				Finish(); 
			}
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