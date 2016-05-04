using UnityEngine;
using System.Collections;

public class PanicWander : PanicAction {

    private WorkerModel worker;
    private int movementSpeed;
    private MapNode[] sefiraNode;

    public PanicWander(WorkerModel target) {
        worker = target;
        movementSpeed = target.movement;//may be changed to double
        sefiraNode = MapGraph.instance.GetSefiraNodes(target.currentSefira);
    }
    
    public MapNode GetRandomNodeByRandom() {
        int randIndex = -1;

        randIndex = Random.Range(0, sefiraNode.Length);

        return sefiraNode[randIndex];
    }

	public void Init()
	{
	}

    public void Execute()
    {
		if (worker.GetMovableNode().IsMoving() == false) { 
            Debug.Log("PanicAction");
			//worker.GetMovableNode().MoveToNode(GetRandomNodeByRandom());
			worker.MoveToNode(GetRandomNodeByRandom());
        }
    }
}

public class PanicStay : PanicAction {

    private WorkerModel worker;
    private float stayTime = 10.0f;
    private float elapsedTime;

    public PanicStay(WorkerModel target) {
        worker = target;
        elapsedTime = 0.0f;
    }

	public void Init()
	{
	}

    public void Execute()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime > stayTime)
        {
            Debug.Log("시간지남");
            //worker.ReturnToSefira();

        }
    }

    

}

public class PanicReturn : PanicAction {

    private WorkerModel worker;
    private MapNode node;

    public PanicReturn(WorkerModel target) {
        this.worker = target;

        node = GetRandomSefira();
        worker.MoveToNode(node.GetId());
    }

	public void Init()
	{
	}

    public void Execute() {
        
        //
    }

    public MapNode GetRandomSefira()
    {
        return MapGraph.instance.GetSepiraNodeByRandom(worker.currentSefira);
    }
}

/*
 *  1초간 기다린 뒤 새로운 패닉 상황으로 넘어간다
 *  이 기간 동안 직원의 대사, 표정의 변화가 필요하다.
 */
public class PanicReadyAlter : PanicAction {

    private WorkerModel worker;

    private float elapsedTime;
    private float waitTime = 3.0f;

    public PanicReadyAlter(WorkerModel target) {
        worker = target;
        elapsedTime = 0.0f;
        
    }

	public void Init()
	{
	}

    public void Execute()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime > waitTime) { 
            //새로운 패닉으로 넘어감
        }
    }
}