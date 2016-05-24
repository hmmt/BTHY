using UnityEngine;
using System.Collections;

public interface IAnimatorEventCalled {
    void OnCalled();
    void OnCalled(int i);
    void AgentReset();
    void AnimatorEventInit();
    void CreatureAnimCall(int i, CreatureBase script);
    void TakeDamageAnim(int isPhysical);
    void AttackCalled(int i);
    void SoundMake(string src);
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

    public void AttackAnimCalled(int i)
    {
        target.AttackCalled(i);
    }

    public void AgentResetCommand() {
        target.AgentReset();
    }

    public void CreatureAnimCommand(int i) {
        if (target is AgentAnim){
            
            target.CreatureAnimCall(i, (target as AgentAnim).Model.animationMessageRecevied);
        }
    }

    public void TakeDamageAnim(int isPhysical) {
        target.TakeDamageAnim(isPhysical);
    }

    public void SoundMake(string src) {
        target.SoundMake(src);
    }

}
