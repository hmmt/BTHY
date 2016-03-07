using UnityEngine;
using System.Collections.Generic;

public enum CreatureCmdType
{
	NONE,
	MOVE
}

public class CreatureCommand
{
	public CreatureCmdType type ;

	public bool isFinished = false;

	public virtual void OnInit(CreatureModel creature)
	{
	}

	public virtual void OnStart(CreatureModel creature)
	{
	}

	public virtual void Execute(CreatureModel creature)
	{
	}

	public virtual void OnStop(CreatureModel creature)
	{
	}

	public virtual void OnDestroy(CreatureModel creature)
	{
	}

	public void Finish()
	{
		isFinished = true;
	}

	public static CreatureCommand MakeMove(MapNode node)
	{
		MoveCreatureCommand cmd = new MoveCreatureCommand(node);
		cmd.type = CreatureCmdType.MOVE;
		return cmd;
	}

	public static CreatureCommand MakeOpenDoor(DoorObjectModel door)
	{
		OpenDoorCreatureCommand cmd = new OpenDoorCreatureCommand(door);
		return cmd;
	}
}


public class MoveCreatureCommand : CreatureCommand
{
	//public MovableObjectNode targetNode;
	public MapNode targetNode;

	public MoveCreatureCommand(MapNode targetNode)
	{
		this.targetNode = targetNode;
	}
	public override void OnStart(CreatureModel creature)
	{
		base.OnStart(creature);
		MovableObjectNode movable = creature.GetMovableNode();
		movable.MoveToNode(targetNode);
	}
	public override void Execute(CreatureModel creature)
	{
		base.Execute(creature);
		MovableObjectNode movable = creature.GetMovableNode();

		if (!movable.IsMoving())
		{
			movable.MoveToNode(targetNode);
		}

		if (movable.GetCurrentNode() != null && movable.GetCurrentNode().GetId() == targetNode.GetId())
		{
			Finish(); 
		}
	}
	public override void OnStop(CreatureModel creature)
	{
		base.OnStop(creature);

		MovableObjectNode movable = creature.GetMovableNode();
		movable.StopMoving();
	}
}



public class OpenDoorCreatureCommand : CreatureCommand
{
	private DoorObjectModel door;
	private float elapsedTime;

	public OpenDoorCreatureCommand(DoorObjectModel door)
	{
		this.door = door;
		elapsedTime = 0;
	}

	public override void OnStart(CreatureModel agent)
	{
		base.OnStart(agent);
	}
	public override void Execute(CreatureModel agent)
	{
		base.Execute(agent);

		elapsedTime += Time.deltaTime;

		if (elapsedTime >= 0.5f)
		{
			door.Open();
			Finish();
		}
	}
	public override void OnStop(CreatureModel agent)
	{
		base.OnStop(agent);
	}
}

public class AttackWorkerCreatureCommand : CreatureCommand
{
    private WorkerModel targetWorker;
    private float elapsedTime;

    public AttackWorkerCreatureCommand(WorkerModel target)
    {
    }

    public override void OnStart(CreatureModel creature)
    {
        base.OnStart(creature);
    }
    public override void Execute(CreatureModel creature)
    {
        base.Execute(creature);

        
    }
    public override void OnStop(CreatureModel creature)
    {
        base.OnStop(creature);
    }
}

public class PursueCreatureCommand : CreatureCommand
{
    private WorkerModel targetWorker;
    private float elapsedTime;

    public PursueCreatureCommand(WorkerModel target)
    {
        this.targetWorker = target;
    }

    public override void OnStart(CreatureModel creature)
    {
        base.OnStart(creature);
    }
    public override void Execute(CreatureModel creature)
    {
        base.Execute(creature);


    }
    public override void OnStop(CreatureModel creature)
    {
        base.OnStop(creature);
    }
}



public class CreatureCommandQueue
{
	private LinkedList<CreatureCommand> queue;
	private CreatureModel creature;

	public CreatureCommandQueue(CreatureModel creature)
	{
		queue = new LinkedList<CreatureCommand>();
		this.creature = creature;
	}

	public CreatureCommand GetCurrentCmd()
	{
		if (queue.Count <= 0)
		{
			return null;
		}
		return queue.First.Value;
	}

	/// <summary>
	/// command甫 角青茄促.
	/// </summary>
	public void Execute(CreatureModel creature)
	{
		if (queue.Count > 0)
		{
			CreatureCommand cmd = queue.First.Value;

			cmd.Execute(creature);

			if (cmd.isFinished)
			{
				queue.RemoveFirst();
				cmd.OnStop(creature);
				cmd.OnDestroy (creature);

				if (queue.Count > 0)
				{
					queue.First.Value.OnStart(creature);
				}
			}
		}
	}

	/// <summary>
	/// Idle 惑怕肺 父电促
	/// </summary>
	public void Clear()
	{
		foreach (CreatureCommand cmd in queue)
		{
			cmd.OnStop(creature);
			cmd.OnDestroy (creature);
		}
		queue.Clear();
	}

	public void SetAgentCommand(CreatureCommand cmd)
	{
		foreach (CreatureCommand oldCmd in queue)
		{
			oldCmd.OnStop(creature);
		}
		queue.Clear();
		queue.AddFirst(cmd);
		cmd.OnInit (creature);
		cmd.OnStart(creature);
	}

	public void AddFirst(CreatureCommand cmd)
	{
		if (queue.Count > 0)
		{
			queue.First.Value.OnStop(creature);
		}
		queue.AddFirst(cmd);
		cmd.OnInit (creature);
		cmd.OnStart(creature);
	}

	public void AddLast(CreatureCommand cmd)
	{
		queue.AddLast(cmd);
		cmd.OnInit (creature);
	}
}