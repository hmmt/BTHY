using UnityEngine;
using System.Collections;

public interface IAnimatorEventCalled {
    void OnCalled();
    void OnCalled(int i);
    void AgentReset();
    void AnimatorEventInit();
    void CreatureAnimCall(int i, CreatureBase script);
}

public class AnimatorEventScript : MonoBehaviour {

    public MonoBehaviour script = null;

    public IAnimatorEventCalled target;

    public void Start() {
        if (script != null)
        {
            if (script is IAnimatorEventCalled)
            {
                target = script as IAnimatorEventCalled;
            }
            else
            {
                Debug.LogError("Type not validate for IAnimatorEventCalled with " + script.name);
            }
        }
    }

    public void SetTarget(IAnimatorEventCalled target) {
        this.target = target;
    }

    public void EventCalled() {
        target.OnCalled();
    }
    
    public void EventCalledInt(int i) {
        target.OnCalled(i);
    }

    public void AgentResetCommand() {
        target.AgentReset();
    }

    public void CreatureAnimCommand(int i) {
        if (target is AgentAnim){
            
            target.CreatureAnimCall(i, (target as AgentAnim).Model.animationMessageRecevied);
        }
    }
    
}
