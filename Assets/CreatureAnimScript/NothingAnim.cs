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
		animator.SetBool ("Move", false);
		//animator.SetBool ("Attack", true);
		animator.SetInteger("Physical", 1);
	}

	void Move()
	{
		if (attackMotionDelay > 0)
			return;
		//animator.SetBool ("Attack", false);
		animator.SetInteger("Physical", 0);
		animator.SetBool ("Move", true);
	}

	void Idle()
	{
		if (attackMotionDelay > 0)
			return;
		//animator.SetBool ("Attack", false);
		animator.SetInteger("Physical", 0);
		animator.SetBool ("Move", false);
	}
}
