using UnityEngine;
using System.Collections;

public class PanicReady : PanicAction {

    private AgentModel targetAgent;

    private float elapsedTime;
    private float waitTime;

    public PanicReady(AgentModel target)
    {
        targetAgent = target;
        elapsedTime = 0;
    }

    public void Execute()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime > waitTime)
        {
            elapsedTime -= waitTime;

            //TrySuicide();
            StartPanicAction();
        }
    }

    public void StartPanicAction()
    {
        targetAgent.PanicReadyComplete();
    }
}
