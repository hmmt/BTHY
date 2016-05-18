using UnityEngine;
using System.Collections.Generic;

public class ParamSetterBehavior : StateMachineBehaviour {

	[System.Serializable]
	public class ParamData
	{
		public string paramName;
		public int value;
	}

    [System.Serializable]
    public class BoolParamData {
        public string paramName;
        public bool value;
    }


	public List<ParamData> onStateEnter;
	public List<ParamData> onStateExit;
	public List<ParamData> onStateMove;

    public List<BoolParamData> onStateEnterBool;


	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		foreach (ParamData param in onStateEnter) {
			animator.SetInteger (param.paramName, param.value);
		}

        foreach (BoolParamData param in onStateEnterBool) {
            animator.SetBool(param.paramName, param.value);
        }
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		
	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		foreach (ParamData param in onStateExit) {
			animator.SetInteger (param.paramName, param.value);
		}
	}

	// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
	//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
	//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}
}
