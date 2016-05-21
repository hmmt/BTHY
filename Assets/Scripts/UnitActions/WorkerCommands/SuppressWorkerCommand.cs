using UnityEngine;
using System.Collections;

public class SuppressWorkerCommand : WorkerCommand {

	private float gunRange;

	private static float preDelayGun = 0.5f;
	private float preDelay;

	private float retreatDuration = 0;
	private float retreatDelay = 0;

	public SuppressWorkerCommand(WorkerModel targetAgent)
	{
		this.targetAgent = targetAgent;

		ResetGunRange ();
	}

	public SuppressWorkerCommand(CreatureModel targetCreature)
	{
		this.targetCreature = targetCreature;

		ResetGunRange ();
	}

	public override void OnInit(WorkerModel agent)
	{
		base.OnInit (agent);

		AgentModel agentActor = (AgentModel)actor;

		AgentUnit agentView = AgentLayer.currentLayer.GetAgent (agent.instanceId);
		/*
		if(suppressAction.weapon == SuppressAction.Weapon.STICK)
			agentView.puppetAnim.SetInteger("Suppress", 1);
		else
			agentView.puppetAnim.SetInteger("Suppress", 2);
		*/
		if(agentActor.weapon  == AgentWeapon.GUN)
			agentView.puppetAnim.SetInteger("Suppress", 2);
		else
			agentView.puppetAnim.SetInteger("Suppress", 1);
		agentView.puppetAnim.SetInteger ("SuppressAction", 0);

		ResetGunRange ();
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
			if (targetAgent is AgentModel) {
				if (((AgentModel)targetAgent).GetState () != AgentAIState.CANNOT_CONTROLL && targetAgent.IsPanic() == false) {
					Finish ();
					return;
				}
			}
			else
			{
				if (((OfficerModel)targetAgent).GetState () != OfficerAIState.CANNOT_CONTROLL && targetAgent.IsPanic() == false) {
					Finish ();
					return;
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
			MovableObjectNode movable = actor.GetMovableNode();

			if (targetCreature.state != CreatureState.ESCAPE && targetCreature.state != CreatureState.ESCAPE_PURSUE)
			{
				movable.StopMoving ();
				Finish ();
				return;
			}

			if(targetCreature.GetMovableNode().GetPassage() == null) // missing
			{
				Debug.Log (actor.name + " : missing...");
				movable.StopMoving ();
				Finish ();
				return;
			}

			if (retreatDelay > 0)
			{
				retreatDelay -= Time.deltaTime;
			}
			if (retreatDuration > 0)
			{
				retreatDuration -= Time.deltaTime;


				if(retreatDuration > 0 && ((AgentModel)actor).moveDelay <= 0)
				{
					Retreat ();
				}
				else
				{
					movable.StopMoving ();
				}
			}

			if(((AgentModel)actor).moveDelay > 0)
			{
				movable.StopMoving ();
			}

			if (CheckRange ())
				return;

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
		//au.SetParameterForSecond ("Return", true, 0.3f);
        au.puppetAnim.SetBool("Return", true);
		((AgentModel)actor).FinishSuppress();
	}


	void ResetGunRange()
	{
		gunRange = 5f + (0.5f - Random.value) / 2f;
	}

	void Retreat()
	{
		MovableObjectNode movable = actor.GetMovableNode();

		if (movable.GetCurrentViewPosition ().x > targetCreature.GetMovableNode ().GetCurrentViewPosition ().x)
		{
			movable.MoveBy (UnitDirection.RIGHT, 1);
		}
		else
		{
			movable.MoveBy (UnitDirection.LEFT, 1);
		}
	}

	bool CheckRange()
	{
		AgentModel agentActor = (AgentModel)actor;
		
		if (targetAgent != null)
		{
			if (targetAgent.GetMovableNode ().GetPassage () == actor.GetMovableNode ().GetPassage ())
			{
				float sqrDistance = (actor.GetCurrentViewPosition () - targetAgent.GetCurrentViewPosition ()).sqrMagnitude;
				if(agentActor.weapon == AgentWeapon.NORMAL || agentActor.weapon == AgentWeapon.SHIELD)
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
						if(targetAgent.IsPanic())
							targetAgent.TakePanicDamage (1);


						agentActor.GetMovableNode ().StopMoving ();
						return true;
					}
				}

				else if(agentActor.weapon == AgentWeapon.GUN)
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
						if(targetAgent.IsPanic())
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
				if(agentActor.weapon == AgentWeapon.NORMAL || agentActor.weapon == AgentWeapon.SHIELD)
				{
					if (sqrDistance < 5)
					{
						agentActor.GetMovableNode ().StopMoving ();

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

						GameObject ge = Prefab.LoadPrefab ("Effect/HitEffectGun");
						ge.transform.localPosition = targetCreature.GetCurrentViewPosition ();
						return true;
					}
				}
				else if(agentActor.weapon == AgentWeapon.GUN)
				{
					//Debug.Log (sqrDistance);
					if (sqrDistance >= gunRange*gunRange)
					{
						retreatDuration = 0;
					}
					if(sqrDistance <= 5 && retreatDelay <= 0 && agentActor.moveDelay <= 0)
					{
						retreatDuration = 2f;
						retreatDelay = 4f;
					}
					else if (sqrDistance <= 25 && retreatDuration <= 0)
					{
						agentActor.GetMovableNode ().StopMoving ();

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

						GameObject ge = Prefab.LoadPrefab ("Effect/HitEffectGun");
						ge.transform.localPosition = targetCreature.GetCurrentViewPosition ();

						//Debug.Log ("SHOT");
					}

					if (sqrDistance <= 25)
						return true;
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
