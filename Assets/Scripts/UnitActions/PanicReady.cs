using UnityEngine;
using System.Collections;

/*
 *  1초간 기다린 뒤 새로운 패닉 상황으로 넘어간다
 *  이 기간 동안 직원의 대사, 표정의 변화가 필요하다.
 */
public class PanicReady : PanicAction {

    private WorkerModel targetAgent;

    private float elapsedTime;
    private float waitTime = 1.0f;

    public PanicReady(WorkerModel target)
    {
        targetAgent = target;
        elapsedTime = 0;
        targetAgent.SetPanicState();
    }

    public void Execute()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime > waitTime)
        {
            //elapsedTime -= waitTime;

            //TrySuicide();
            StartPanicAction();
        }
    }

    public void StartPanicAction()
    {
        targetAgent.PanicReadyComplete();
    }
}
