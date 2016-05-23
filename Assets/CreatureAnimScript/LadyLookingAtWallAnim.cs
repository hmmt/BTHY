using UnityEngine;
using System.Collections;

public class LadyLookingAtWallAnim : CreatureAnimScript, IAnimatorEventCalled {

    public GameObject horrorFace;
    public Animator horrorAnim;

    LadyLookingAtWall script;

    public void StartEffect() {
        horrorFace.gameObject.SetActive(true);
        horrorAnim.SetBool("Start", true);
    }

    public void Init(LadyLookingAtWall script) {
        if (horrorFace.activeInHierarchy) {
            horrorFace.gameObject.SetActive(false);
        }
        this.script = script;
    }

    public void OnCalled()
    {
        horrorAnim.SetBool("Start", false);
        horrorFace.gameObject.SetActive(false);
        script.RestartSensing();
    }

    public void OnCalled(int i)
    {
        throw new System.NotImplementedException();
    }

    public void AgentReset()
    {
        throw new System.NotImplementedException();
    }

    public void AnimatorEventInit()
    {
        throw new System.NotImplementedException();
    }

    public void CreatureAnimCall(int i, CreatureBase script)
    {
        throw new System.NotImplementedException();
    }

    public void TakeDamageAnim(int isPhysical)
    {
        throw new System.NotImplementedException();
    }

    public void AttackCalled(int i)
    {
        throw new System.NotImplementedException();
    }

    public void SoundMake(string src)
    {
        throw new System.NotImplementedException();
    }
}
