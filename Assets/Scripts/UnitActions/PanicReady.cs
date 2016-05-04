using UnityEngine;
using System.Collections;

/*
 *  1초간 기다린 뒤 새로운 패닉 상황으로 넘어간다
 *  이 기간 동안 직원의 대사, 표정의 변화가 필요하다.
 */
public class PanicReady : PanicAction {

	private WorkerModel actor;

    private float elapsedTime;
    private float waitTime = 5.0f;

    public PanicReady(WorkerModel target)
    {
        actor = target;
        elapsedTime = 0;
        actor.SetPanicState();
    }

	public void Init()
	{
		AgentUnit agentView = AgentLayer.currentLayer.GetAgent (actor.instanceId);
		agentView.puppetAnim.SetBool ("Panic", true);
		agentView.puppetAnim.SetInteger ("PanicType", 0);
		agentView.puppetAnim.SetBool ("PanicStart", true);
	}

    public void Execute()
    {
		PassageObjectModel passage = actor.GetMovableNode ().GetPassage ();
		if (passage == null || passage.IsIsolate ())
		{
			actor.MoveToNode (MapGraph.instance.GetSepiraNodeByRandom (actor.currentSefira));
			return;
		}

		if (actor is AgentModel)
		{
			((AgentModel)actor).StopAction ();
		}

        elapsedTime += Time.deltaTime;
        if (elapsedTime > waitTime)
        {
            StartPanicAction();
        }
    }

    public void StartPanicAction()
    {
		Debug.Log ("Panic  action start");
        actor.PanicReadyComplete();
    }
}
