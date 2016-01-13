using UnityEngine;
using System.Collections;

public class PassageDoor : MonoBehaviour {

    public Animator animator;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void FixedUpdate()
    {
    }

    public void CloseDoor()
    {
        if (animator != null)
        {
            animator.SetBool("opened", false);
        }
    }

    public void OpenDoor()
    {
        if (animator != null)
        {
            animator.SetBool("opened", true);
        }
    }
}
