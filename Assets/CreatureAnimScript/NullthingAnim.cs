using UnityEngine;
using System.Collections;

public class NullthingAnim : CreatureAnimScript {

	private float attackMotionDelay = 0;
    public bool isDisguised = false;
    public NullCreature.NullState currentState = NullCreature.NullState.WORKER;

    

    public void SetState(NullCreature.NullState state) {
        this.currentState = state;
    }
	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
        if (isDisguised) return;
		if (attackMotionDelay > 0)
			attackMotionDelay -= Time.deltaTime;
	}

	void Attack(){
        if (isDisguised) return;
		Debug.Log ("attack");
		attackMotionDelay = 3.0f;
		animator.SetBool ("Move", false);
		//animator.SetBool ("Attack", true);
		animator.SetInteger("Physical", Random.Range(1, 4));
	}

	void Move()
	{
        if (isDisguised) return;

		if (attackMotionDelay > 0)
			return;
		//animator.SetBool ("Attack", false);
		animator.SetInteger("Physical", 0);
		animator.SetBool ("Move", true);
	}

	void Idle()
	{
        if (isDisguised) return;
		if (attackMotionDelay > 0)
			return;
		//animator.SetBool ("Attack", false);
		animator.SetInteger("Physical", 0);
		animator.SetBool ("Move", false);
	}

    public void Transform() {
        animator.SetBool("Transform", true);
        animator.SetBool("Transform", false);
    }
}
