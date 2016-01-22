using UnityEngine;
using System.Collections;

public class TransitionScript : MonoBehaviour {
    public Animator anim;
    public AgentPanelScript script;
    private bool dir = true;

    public void Start() {
        anim = gameObject.GetComponent<Animator>();
        script = gameObject.GetComponent<AgentPanelScript>();

    }

    public void Update() { 
        
        if(anim.GetCurrentAnimatorStateInfo(0).IsName("Stay") || anim.GetCurrentAnimatorStateInfo(0).IsName("Init")){
            script.ChangePrefab();
        }
    }

    public void SetAnimation() {
        anim.SetBool("AnimFlag", dir);
        //script.ChangePrefab();
        dir = !dir;
    }
}
