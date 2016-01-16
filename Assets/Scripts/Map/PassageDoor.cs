using UnityEngine;
using System.Collections;

public class PassageDoor : MonoBehaviour {

    public DoorObjectModel model;
    public Animator animator;

    private bool closed;

	// Use this for initialization
	void Start () {
        closed = model.IsClosed();
        UpdateDoor();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void FixedUpdate()
    {
        bool newClosed = model.IsClosed();
        if (newClosed != closed)
        {
            closed = newClosed;
            UpdateDoor();
        }
    }

    private void UpdateDoor()
    {
        if (closed)
        {
            CloseDoor();
        }
        else
        {
            OpenDoor();
        }
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
            Debug.Log("OPEND!!!");
            animator.SetBool("opened", true);
        }
    }
}
