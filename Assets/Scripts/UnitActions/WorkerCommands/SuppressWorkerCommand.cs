using UnityEngine;
using System.Collections;

public class SuppressWorkerCommand : WorkerCommand {


	private static float preDelayGun = 0.5f;
	private float preDelay;

	SuppressAction suppressAction;

	public SuppressWorkerCommand(AgentModel targetAgent, SuppressAction suppressAction)
	{
		this.targetAgent = targetAgent;
		this.suppressAction = suppressAction;
	}

	public SuppressWorkerCommand(CreatureModel targetCreature, SuppressAction suppressAction)
	{
		this.targetCreature = targetCreature;
		this.suppressAction = suppressAction;
	}

	public override void OnInit(WorkerModel agent)
	{
		base.OnInit (agent);
	}

	public override void OnStart(WorkerModel agent)
	{
		base.OnStart (agent);
	}
	public override void Execute(WorkerModel agent)
	{
		base.Execute(agent);

		if (targetAgent != null)
		{
			if(targetAgent.IsPanic() == false)
			{
				Finish ();
				return;
			}
			MovableObjectNode movable = actor.GetMovableNode();

			if (!movable.IsMoving())
			{
				movable.MoveToMovableNode(targetAgent.GetMovableNode());
			}
			this.isMoving = movable.IsMoving ();
		}
		else if (targetCreature != null)
		{
			if (targetCreature.state != CreatureState.ESCAPE && targetCreature.state != CreatureState.ESCAPE_PURSUE)
			{
				Finish ();
				return;
			}
			MovableObjectNode movable = actor.GetMovableNode();

			if (!movable.IsMoving())
			{
				movable.MoveToMovableNode(targetCreature.GetMovableNode());
			}
			this.isMoving = movable.IsMoving ();
		}

		CheckRange ();
	}
	public override void OnDestroy(WorkerModel agent)
	{
		base.OnDestroy (agent);

		((AgentModel)actor).FinishSuppress();
	}

	void CheckRange()
	{
		AgentModel agentActor = (AgentModel)actor;
		if (agentActor.attackDelay > 0)
			return;
		
		if (targetAgent != null)
		{
			if (targetAgent.GetMovableNode ().GetPassage () == actor.GetMovableNode ().GetPassage ())
			{
				float sqrDistance = (actor.GetCurrentViewPosition () - targetAgent.GetCurrentViewPosition ()).sqrMagnitude;
				if (suppressAction.weapon == SuppressAction.Weapon.STICK)
				{
					if (sqrDistance < 1)
					{
						HitObjectManager.AddSuppressWorkerStickHitbox ((AgentModel) targetAgent);
						agentActor.SetAttackDelay (4.0f);
					}
				}
				else if (suppressAction.weapon == SuppressAction.Weapon.GUN)
				{
					if (sqrDistance < 5)
					{
						// 일단 딜레이 없이
						// TODO: call motion method
						Debug.Log("Shot!");
						agentActor.SetAttackDelay(4.0f);
						targetAgent.TakePhysicalDamage (1);
						targetAgent.TakePanicDamage (1);

					}
				}
			}
		}
		else if (targetCreature != null)
		{
			if (targetCreature.GetMovableNode ().GetPassage () == actor.GetMovableNode ().GetPassage ())
			{
				float sqrDistance = (actor.GetCurrentViewPosition () - targetCreature.GetCurrentViewPosition ()).sqrMagnitude;
				if (suppressAction.weapon == SuppressAction.Weapon.STICK)
				{
					if (sqrDistance < 1)
					{
						Debug.Log ("hit!");
						agentActor.SetAttackDelay(4.0f);
						targetCreature.TakeSuppressDamage (1);
					}
				}
				else if (suppressAction.weapon == SuppressAction.Weapon.GUN)
				{
					if (sqrDistance < 5)
					{
						Debug.Log ("Shoot!");
						agentActor.SetAttackDelay(4.0f);
						targetCreature.TakeSuppressDamage (1);
					}
				}
			}
		}
		else
		{
			Debug.Log ("unexpected condition");
		}
	}
}
