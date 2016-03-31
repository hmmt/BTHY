using UnityEngine;
using System.Collections;

public enum SuppressType
{
	PANIC,
	UNCONTROLLABLE
}

public class SuppressWorkerCommand : WorkerCommand {


	private static float preDelayGun = 0.5f;
	private float preDelay;
	private SuppressType supType;

	SuppressAction suppressAction;

	public SuppressWorkerCommand(WorkerModel targetAgent, SuppressAction suppressAction, SuppressType supType)
	{
		this.targetAgent = targetAgent;
		this.suppressAction = suppressAction;
		this.supType = supType;
	}

	public SuppressWorkerCommand(CreatureModel targetCreature, SuppressAction suppressAction)
	{
		this.targetCreature = targetCreature;
		this.suppressAction = suppressAction;
	}

	public override void OnInit(WorkerModel agent)
	{
		base.OnInit (agent);

		AgentUnit agentView = AgentLayer.currentLayer.GetAgent (agent.instanceId);
		if(suppressAction.weapon == SuppressAction.Weapon.STICK)
			agentView.puppetAnim.SetInteger("Suppress", 1);
		else
			agentView.puppetAnim.SetInteger("Suppress", 2);
		agentView.puppetAnim.SetInteger ("SuppressAction", 0);
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
			if (targetAgent.isDead ()) {
				Finish ();
				return;
			}
			if (supType == SuppressType.PANIC) {
				if (targetAgent.IsPanic () == false) {
					Finish ();
					return;
				}
			} else {
				if (targetAgent is AgentModel) {
					if (((AgentModel)targetAgent).GetState () != AgentAIState.CANNOT_CONTROLL) {
						Finish ();
						return;
					}
				}
				else
				{
					if (((OfficerModel)targetAgent).GetState () != OfficerAIState.CANNOT_CONTROLL) {
						Finish ();
						return;
					}
				}
			}

			if (CheckRange ())
				return;
			
			MovableObjectNode movable = actor.GetMovableNode();

			if (!movable.IsMoving() && ((AgentModel)actor).moveDelay <= 0)
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

			if (CheckRange ())
				return;

			MovableObjectNode movable = actor.GetMovableNode();

			if (!movable.IsMoving() && ((AgentModel)actor).moveDelay <= 0)
			{
				movable.MoveToMovableNode(targetCreature.GetMovableNode());
			}
			this.isMoving = movable.IsMoving ();
		}
	}
	public override void OnDestroy(WorkerModel agent)
	{
		base.OnDestroy (agent);

		AgentUnit au = AgentLayer.currentLayer.GetAgent (agent.instanceId);
		au.puppetAnim.SetInteger ("Suppress", 0);
		au.SetParameterForSecond ("Return", true, 0.3f);

		((AgentModel)actor).FinishSuppress();
	}

	bool CheckRange()
	{
		AgentModel agentActor = (AgentModel)actor;
		
		if (targetAgent != null)
		{
			if (targetAgent.GetMovableNode ().GetPassage () == actor.GetMovableNode ().GetPassage ())
			{
				float sqrDistance = (actor.GetCurrentViewPosition () - targetAgent.GetCurrentViewPosition ()).sqrMagnitude;
				if (suppressAction.weapon == SuppressAction.Weapon.STICK)
				{
					if (sqrDistance < 1)
					{
						float actorX = actor.GetCurrentViewPosition ().x;
						float targetX = targetAgent.GetCurrentViewPosition ().x;
						if (actorX > targetX)
						{
							actor.GetMovableNode ().SetDirection (UnitDirection.LEFT);
						}
						if (actorX < targetX)
						{
							actor.GetMovableNode ().SetDirection (UnitDirection.RIGHT);
						}

						if (agentActor.attackDelay > 0)
							return true;
						
						//HitObjectManager.AddSuppressWorkerStickHitbox ((AgentModel) targetAgent);

						AgentUnit au = AgentLayer.currentLayer.GetAgent (agentActor.instanceId);
						au.SetParameterForSecond ("SuppressAction", Random.Range(1, 4), 0.3f);

						agentActor.SetAttackDelay (4.0f);
						agentActor.SetMoveDelay (1.0f);
						targetAgent.TakePhysicalDamage (1, DamageType.NORMAL);
						if(supType == SuppressType.PANIC)
							targetAgent.TakePanicDamage (1);


						agentActor.GetMovableNode ().StopMoving ();
						return true;
					}
				}
				else if (suppressAction.weapon == SuppressAction.Weapon.GUN)
				{
					if (sqrDistance < 5)
					{
						float actorX = actor.GetCurrentViewPosition ().x;
						float targetX = targetAgent.GetCurrentViewPosition ().x;
						if (actorX > targetX)
						{
							actor.GetMovableNode ().SetDirection (UnitDirection.LEFT);
						}
						if (actorX < targetX)
						{
							actor.GetMovableNode ().SetDirection (UnitDirection.RIGHT);
						}

						if (agentActor.attackDelay > 0)
							return true;
						// 일단 딜레이 없이
						// TODO: call motion method
						AgentUnit au = AgentLayer.currentLayer.GetAgent (agentActor.instanceId);
						au.SetParameterForSecond ("SuppressAction", Random.Range(1, 3), 0.3f);

						agentActor.SetAttackDelay(4.0f);
						agentActor.SetMoveDelay (1f);
						targetAgent.TakePhysicalDamage (1, DamageType.NORMAL);
						if(supType == SuppressType.PANIC)
							targetAgent.TakePanicDamage (1);
						agentActor.GetMovableNode ().StopMoving ();
						return true;
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
						float actorX = actor.GetCurrentViewPosition ().x;
						float targetX = targetCreature.GetCurrentViewPosition ().x;
						if (actorX > targetX)
						{
							actor.GetMovableNode ().SetDirection (UnitDirection.LEFT);
						}
						if (actorX < targetX)
						{
							actor.GetMovableNode ().SetDirection (UnitDirection.RIGHT);
						}

						if (agentActor.attackDelay > 0)
							return true;
						
						AgentUnit au = AgentLayer.currentLayer.GetAgent (agentActor.instanceId);
						au.SetParameterForSecond ("SuppressAction", Random.Range(1, 4), 0.3f);

						agentActor.SetAttackDelay(4.0f);
						agentActor.SetMoveDelay (1f);
						targetCreature.TakeSuppressDamage (1);
						agentActor.GetMovableNode ().StopMoving ();
						return true;
					}
				}
				else if (suppressAction.weapon == SuppressAction.Weapon.GUN)
				{
					if (sqrDistance < 5)
					{
						float actorX = actor.GetCurrentViewPosition ().x;
						float targetX = targetCreature.GetCurrentViewPosition ().x;
						if (actorX > targetX)
						{
							actor.GetMovableNode ().SetDirection (UnitDirection.LEFT);
						}
						if (actorX < targetX)
						{
							actor.GetMovableNode ().SetDirection (UnitDirection.RIGHT);
						}

						if (agentActor.attackDelay > 0)
							return true;

						AgentUnit au = AgentLayer.currentLayer.GetAgent (agentActor.instanceId);
						au.SetParameterForSecond ("SuppressAction", Random.Range(1, 3), 0.3f);

						agentActor.SetAttackDelay(4.0f);
						agentActor.SetMoveDelay (1f);
						targetCreature.TakeSuppressDamage (1);
						agentActor.GetMovableNode ().StopMoving ();

						GameObject ge = Prefab.LoadPrefab ("Effect/HitEffectGun");
						ge.transform.localPosition = targetCreature.GetCurrentViewPosition ();
						return true;
					}
				}
			}
		}
		else
		{
			Debug.Log ("unexpected condition");
		}
		return false;
	}
}
