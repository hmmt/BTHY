using UnityEngine;
using System.Collections;

public class HitObjectSuppressWorker : MonoBehaviour {

	public AgentModel targetAgent;
	//public CreatureModel targetCreature;

	public float width;
	public float height;

	public float hitTime;
	public float endTime;


	private float elapsedTime = 0.0f;

	private bool hit = false;
	private bool end = false;

	void FixedUpdate()
	{
		elapsedTime += Time.deltaTime;

		if (!hit && elapsedTime > hitTime) {

			hit = true;

			DectectCollision ();
		}

		if (!end && elapsedTime > endTime)
			Destroy (gameObject);
	}

	void DectectCollision()
	{
		if (targetAgent.IsPanic () == false)
			return;
		Vector3 pos = transform.position;

		Vector3 agentPos = targetAgent.GetCurrentViewPosition ();

		if (pos.x - width / 2 < agentPos.x && pos.x + width / 2 > agentPos.x
			&& pos.y - height / 2 < agentPos.y && pos.y + height / 2 > agentPos.y)
		{
			//target.TakePhysicalDamage (1);
			targetAgent.TakePanicDamage(1);
			targetAgent.TakePhysicalDamage (1);
			targetAgent.Stun (1.0f);
		}

		//GetComponent<LineRenderer>().SetColors(


	}
}
