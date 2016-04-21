using UnityEngine;
using System.Collections;

public interface IAnimatorEventCalled {
    void OnCalled();
    void AnimatorEventInit();
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
        print(target);
    }

    public void EventCalled() {
        target.OnCalled();
    }

}
