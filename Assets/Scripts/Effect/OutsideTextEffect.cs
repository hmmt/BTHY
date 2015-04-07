﻿using UnityEngine;
using System.Collections;

public class OutsideTextEffect : MonoBehaviour {

	private float elapsedTime = 0;

	private float startTime = 0;
	private float goalTime = 5;

    private bool fadeEffect = true;
	
	// Use this for initialization
	void Start () {
		elapsedTime = 0;
	}
	
	void FixedUpdate()
	{
		elapsedTime += Time.deltaTime;

		UpdateState ();
	}

	private void UpdateState()
	{
		Color color = GetComponent<SpriteRenderer> ().color;

		float elapsed = elapsedTime - startTime;
		elapsed = elapsed > 0 ? elapsed : 0;

        if (fadeEffect)
        {
            color.a = MathUtil.UnitStep(1 - elapsed) * (elapsed)
                + MathUtil.UnitStep(elapsed - 1) * MathUtil.UnitStep(goalTime - 1 - elapsed)
                    + MathUtil.UnitStep(elapsed - (goalTime - 1)) * (goalTime - elapsed);
        }
        else
        {
            color.a = elapsed > 0 ? 1f : 0;
        }
		
		GetComponent<SpriteRenderer> ().color = color;
		
		if (elapsed > goalTime)
			Destroy (gameObject);
	}

    public OutsideTextEffect DisableFade()
    {
        fadeEffect = false;
        return this;
    }


	public static OutsideTextEffect Create(IsolateRoom room, string typoKey, CreatureOutsideTextLayout layout)
	{
		return Create(room, typoKey, layout, 0, 1);
	}

	public static OutsideTextEffect Create(IsolateRoom room, string typoKey, CreatureOutsideTextLayout layout, float start, float time)
	{
		return Create(room, typoKey, layout, start, time, null);
	}
	public static OutsideTextEffect Create(IsolateRoom room, string typoKey, CreatureOutsideTextLayout layout, float start, float time, Callback callback)
	{
		GameObject newEffect = Prefab.LoadPrefab ("OutsideTextEffect");
		
		newEffect.transform.SetParent (room.transform);
		
		//if(layout == CreatureOutsideTextLayout.CENTER_BOTTOM)
		{
			newEffect.transform.localPosition = new Vector3(0, -4, 0);
		}
		
		OutsideTextEffect effect = newEffect.GetComponent<OutsideTextEffect> ();
		
		Texture2D tex = Resources.Load<Texture2D> ("Sprites/"+typoKey);
		effect.GetComponent<SpriteRenderer>().sprite = Sprite.Create(tex, new Rect(0,0,tex.width, tex.height), new Vector2(0.5f, 0.5f));

		effect.startTime = start;
		effect.goalTime = time;
		
		effect.UpdateState ();
		
		return effect;
	}

}
