﻿using UnityEngine;
using System.Collections;

public class ManageAnimScirpt : StateMachineBehaviour {

	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	//override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	//override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	//override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
	//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
	//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        /*
        if (animator.GetInteger("Manage") != 0) {
            animator.SetInteger("Manage", 0);
        }

        if (animator.GetBool("Memo") == true) {
            animator.SetBool("Memo", false);
        }*/

        if (animator.GetBool("Manage") != false) {
            animator.SetBool("Manage", false);
        }

        if (animator.GetInteger("Food") != 0){
            animator.SetInteger("Food", 0);
        }

        if (animator.GetInteger("Clean") != 0) {
            animator.SetInteger("Clean", 0);
        }

        if (animator.GetInteger("Communication") != 0) {
            animator.SetInteger("Communication", 0);
        }

        if (animator.GetInteger("Recept") != 0) {
            animator.SetInteger("Recept", 0);
        }

        if (animator.GetInteger("Restrick") != 0) {
            animator.SetInteger("Restrick", 0);
        }

        if (animator.GetInteger("Treat") != 0) {
            animator.SetInteger("Treat", 0);
        }



        if (animator.GetBool("Memo") == true)
        {
            animator.SetBool("Memo", false);
        }
    }
}
