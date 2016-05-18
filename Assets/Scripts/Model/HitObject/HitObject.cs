using UnityEngine;
using System.Collections;

public class HitObject : MonoBehaviour {

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
		Vector3 pos = transform.position;

		foreach (AgentModel agent in AgentManager.instance.GetAgentList()) {
			Vector3 agentPos = agent.GetCurrentViewPosition ();

			if (pos.x - width / 2 < agentPos.x && pos.x + width / 2 > agentPos.x
				&& pos.y - height / 2 < agentPos.y && pos.y + height / 2 > agentPos.y) {

				//Debug.Log ("Hit");

				agent.TakePhysicalDamageByCreature (1);
				//agent.Stun (1.0f);
			}
		}

		//GetComponent<LineRenderer>().SetColors(


	}
}
