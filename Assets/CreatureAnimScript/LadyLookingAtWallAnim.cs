using UnityEngine;
using System.Collections;

public class LadyLookingAtWallAnim : CreatureAnimScript, IAnimatorEventCalled {
    const float MaxSize = 3f;
    const float MinSize = 1.5f;
    public float std = 1.35f;
    public GameObject horrorFace;
    public Animator horrorAnim;

    LadyLookingAtWall script;
    Camera cam;

    public void StartEffect() {
        horrorFace.gameObject.SetActive(true);
        horrorAnim.SetBool("Start", true);
        horrorFace.transform.SetParent(cam.gameObject.transform);
        horrorFace.transform.localPosition = new Vector3(0, 0, 5);
        //horrorFace.transform.localScale = new Vector3(cam.orthographicSize * std / MaxSize, cam.orthographicSize * std / MaxSize, 1f);
        horrorFace.transform.localScale = new Vector3(cam.orthographicSize * 0.333f, cam.orthographicSize * 0.333f, 1f);
        //Debug.Log(horrorFace.transform.localScale);
    }

    public void Init(LadyLookingAtWall script) {
        if (horrorFace.activeInHierarchy) {
            horrorFace.gameObject.SetActive(false);
        }
        cam = Camera.main;
        this.script = script;
    }

    public void OnCalled()
    {
        horrorFace.transform.SetParent(this.gameObject.transform);
        horrorAnim.SetBool("Start", false);
        horrorFace.gameObject.SetActive(false);
        //script.currentCreatureUnit.PlaySound("beep");
        //script.RestartSensing();
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
        CreatureUnit unit = script.currentCreatureUnit;
        unit.PlaySound(src, AudioRolloffMode.Linear);
    }
}
