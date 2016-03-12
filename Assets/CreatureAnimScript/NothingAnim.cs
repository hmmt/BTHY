using UnityEngine;
using System.Collections;

public class NothingAnim : CreatureAnimScript {

	private float attackMotionDelay = 0;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (attackMotionDelay > 0)
			attackMotionDelay -= Time.deltaTime;
	}

	void Attack(){
		Debug.Log ("attack");
		attackMotionDelay = 3.0f;
		animator.SetBool ("move", false);
		animator.SetBool ("attack", true);
	}

	void Move()
	{
		if (attackMotionDelay > 0)
			return;
		animator.SetBool ("attack", false);
		animator.SetBool ("move", true);
	}

	void Idle()
	{
		if (attackMotionDelay > 0)
			return;
		animator.SetBool ("attack", false);
		animator.SetBool ("move", false);
	}
}
