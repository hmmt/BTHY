using UnityEngine;
using System.Collections;

public class NullthingAnim : CreatureAnimScript {

	private float attackMotionDelay = 0;
    public bool isDisquised = false;
	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
        if (isDisquised) return;
		if (attackMotionDelay > 0)
			attackMotionDelay -= Time.deltaTime;
	}

	void Attack(){
        if (isDisquised) return;
		Debug.Log ("attack");
		attackMotionDelay = 3.0f;
		animator.SetBool ("Move", false);
		//animator.SetBool ("Attack", true);
		animator.SetInteger("Physical", 1);
	}

	void Move()
	{
        if (isDisquised) return;

		if (attackMotionDelay > 0)
			return;
		//animator.SetBool ("Attack", false);
		animator.SetInteger("Physical", 0);
		animator.SetBool ("Move", true);
	}

	void Idle()
	{
        if (isDisquised) return;
		if (attackMotionDelay > 0)
			return;
		//animator.SetBool ("Attack", false);
		animator.SetInteger("Physical", 0);
		animator.SetBool ("Move", false);
	}
}
