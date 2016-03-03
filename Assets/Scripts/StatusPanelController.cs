using UnityEngine;
using System.Collections;

public class StatusPanelController : MonoBehaviour {

    public AgentList agentScript;
    public CollectionListScript creatureScript;

    public Animator controller;
    public Animation initial;
    public Animation middle;

    private static int state = 0;
    private bool check = false;
    private bool displayed = false;
    public static int opened = -1;

    public void WindowOpen(int index) {
        if (!displayed)
        {
            displayed = true;
            state = 0;
        }
        else {
            if (opened == index)
            {
                state = 3;
                displayed = false;
                check = false;
            }
            else
            {
                opened = index;
                if (!check) {
                    state = 1;
                    check = true;
                }
                else
                {
                    state = 2;
                    
                }
            }
        }
        //Debug.Log("State: " + state);
        controller.SetInteger("State", state);
        Debug.Log("State: " + controller.GetInteger("State"));
        switch (index)
        {
            case 0:
                agentScript.AgentListAnimButton();
                break;
            case 1:
                creatureScript.CollectionListAnimButton();
                break;
        }
        controller.SetBool("move", true);
    }
}
