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

		//SpriteRenderer sr = g.AddComponent<SpriteRenderer> ();

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

		//sr.sprite = Resources.Load<Sprite> ();

		return hit;
	}
}
