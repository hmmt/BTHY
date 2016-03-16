using UnityEngine;
using System.Collections.Generic;

public class HitObjectManager {
	/*
	private List<HitObject> hitList;

	public HitObjectManager()
	{
		hitList = new List<HitObject> ();
	}
*/

	public static HitObject AddHitbox(Vector2 position, float hitTime, float width, float height)
	{
		//HitObject model = new HitObject ();
		GameObject g = new GameObject("hitbox");
		HitObject hit = g.AddComponent<HitObject> ();

		g.transform.localPosition = new Vector3 (position.x, position.y, 0);
		hit.width = width;
		hit.height = height;

		hit.hitTime = hitTime;
		hit.endTime = hitTime + 1;

		AddLineComponentForDebug (g, width, height);

		//sr.sprite = Resources.Load<Sprite> ();

		return hit;
	}

	public static HitObjectPanicAttack AddPanicHitbox(Vector2 position, AgentModel actor, AgentModel target)
	{
		//HitObject model = new HitObject ();
		GameObject g = new GameObject("hitbox");
		HitObjectPanicAttack hit = g.AddComponent<HitObjectPanicAttack> ();

		float hitTime = 0.5f;
		float width = 1;
		float height = 1;

		hit.actor = actor;
		hit.target = target;

		g.transform.localPosition = new Vector3 (position.x, position.y, 0);
		hit.width = width;
		hit.height = height;

		hit.hitTime = hitTime;
		hit.endTime = hitTime + 1;

		AddLineComponentForDebug (g, width, height);
		//SpriteRenderer sr = g.AddComponent<SpriteRenderer> ();

		//sr.sprite = Resources.Load<Sprite> ();

		return hit;
	}

	public static HitObjectSuppressWorker AddSuppressWorkerStickHitbox(AgentModel targetAgent)
	{
		//HitObject model = new HitObject ();
		GameObject g = new GameObject("hitbox");
		HitObjectSuppressWorker hit = g.AddComponent<HitObjectSuppressWorker> ();

		float hitTime = 0.5f;
		float width = 1;
		float height = 1;

		hit.targetAgent = targetAgent;

		g.transform.localPosition = new Vector3 (targetAgent.GetCurrentViewPosition().x, targetAgent.GetCurrentViewPosition().y, 0);
		hit.width = width;
		hit.height = height;

		hit.hitTime = hitTime;
		hit.endTime = hitTime + 1;

		AddLineComponentForDebug (g, width, height);
		//SpriteRenderer sr = g.AddComponent<SpriteRenderer> ();

		//sr.sprite = Resources.Load<Sprite> ();

		return hit;
	}

	private static void AddLineComponentForDebug(GameObject g, float width, float height)
	{
		LineRenderer lr = g.AddComponent<LineRenderer> ();

		lr.useWorldSpace = false;
		lr.SetWidth (0.05f, 0.05f);
		lr.SetVertexCount (8);
		//lr.SetPositions (new Vector3[]{ 
		lr.SetPosition(0,new Vector3(-width/2, -height/2, -2));
		lr.SetPosition(1,new Vector3(width/2, -height/2, -2));

		lr.SetPosition(2,new Vector3(width/2, -height/2, -2));
		lr.SetPosition(3,new Vector3(width/2, height/2, -2));

		lr.SetPosition(4,new Vector3(width/2, height/2, -2));
		lr.SetPosition(5,new Vector3(-width/2, height/2, -2));

		lr.SetPosition(6,new Vector3(-width/2, height/2, -2));
		lr.SetPosition(7, new Vector3(-width / 2, -height / 2, -2));
	}
}
