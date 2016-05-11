using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class BigBird : CreatureBase, IAnimatorEventCalled{
    CreatureUnit birdCreatureUnit = null;
    List<WorkerModel> targetList;
    CreatureTimer timer;
    CreatureTimer attractCastingTimer;
    bool attract = false;
    bool escaped = false;
    bool attractCasting = false;
    bool isPlayingDeadScene = false;
    bool targetMoving = false;

    WorkerModel currentTarget = null;
    
    PassageObjectModel oldPassage = null;

    int former = -1;
    int currentPos = -1;

    bool animatorScriptInit = false;

    public override void OnInit()
    {
        timer = new CreatureTimer(10f);
        attractCastingTimer = new CreatureTimer(5f);
        model.escapeType = CreatureEscapeType.WANDER;
    }

    public override void OnFixedUpdate(CreatureModel creature)
    {
        if(!animatorScriptInit){
            AnimatorEventInit();
            animatorScriptInit = true;
        }
        if (birdCreatureUnit == null) {
            birdCreatureUnit = CreatureLayer.currentLayer.GetCreature(model.instanceId);
        }

        if (escaped)
        {
            return;
        }
        else {
            if (timer.isStarted()) {
                timer.TimerStop();
            }
        }

        if (model.GetFeelingPercent() == 0f ||
            (int)PlayerModel.instance.GetCurrentEmergencyLevel() >= (int)EmergencyLevel.LEVEL2)
        {
            if (!escaped)
            {
                Escape();
            }
        }

        
    }

    public override bool hasUniqueEscape()
    {
        return true;
    }

    public void SetPassageAlpha(int i) 
    {
        if (oldPassage == null) return;
        if (model.GetCurrentEdge() != null) {
            if (model.GetCurrentEdge().type == "door") return;
        }
        object[] param = new object[2];
        param[0] = oldPassage;
        param[1] = i;
        Notice.instance.Send(NoticeName.PassageAlpha, param);
    }

    public override void UniqueEscape()
    {
        // Debug.Log("head " + model.GetMovableNode().IsMoving());
        PassageObjectModel currentPassage = model.GetMovableNode().GetPassage();
        
        if (currentPassage != null)
        { 
            if (currentPassage.GetPassageType() == PassageType.HORIZONTAL ||
                currentPassage.GetPassageType() == PassageType.DEPARTMENT ||
                currentPassage.GetPassageType() == PassageType.SEFIRA )
            {
                SetPassageAlpha(255);
                oldPassage = currentPassage;
                
                //Notice.instance.Send(NoticeName.PassageBlackOut, currentPassage);
            }
        }
        else  {
            return;
        }

        if (model.GetMovableNode().currentEdge != null && model.GetMovableNode().currentEdge.type == "door") return;

        if (oldPassage != null && currentPassage != oldPassage) {
            //Notice.instance.Send(NoticeName.PassageWhitle, oldPassage);
            oldPassage = null;
        }

        //추가조건 필요
        if (model.GetCreatureCurrentCmd() == null && attract == false && currentTarget == null) {
            //Debug.Log("BirdStartMovement");
            MakeBirdMovement();
        }

        if (timer.TimerRun() && currentPassage != null) {
            StopBirdMovement();

            attractCasting = true;
            attractCastingTimer.TimerStart(true);
            timer.TimerStop();
        }

        if (attractCasting && currentPassage != null) {
            
            if (attractCastingTimer.isStarted())
            {
                if (attractCastingTimer.TimerRun() == false)
                {
                    if (AttractNearAgent())
                    {
                        attractCastingTimer.TimerStop();
                        attract = true;
                        attractCasting = false;
                        targetMoving = true;
                        //currentTarget.MoveToNode(model.GetMovableNode().currentNode);
                        /*
                        if (model.GetCurrentEdge() == null)
                        {
                            currentTarget.MoveToNode(model.GetCurrentNode().GetEdgeByNode(model.GetCurrentNode()).ConnectedNode(model.GetCurrentNode()));
                        }
                        else {
                            currentTarget.MoveToNode(model.GetCurrentEdge().node1);
                        }*/
                        
                        //currentTarget.movableNode.MoveToMovableNode(model.GetMovableNode());
                        StopBirdMovement();
                        if (currentTarget.GetMovableNode().GetCurrentViewPosition().x < model.GetMovableNode().GetCurrentViewPosition().x)
                        {
                            
                            
                            if (model.GetAnimScript().transform.localScale.x > 0)
                            {
                                Debug.Log("좌로본다");
                                birdCreatureUnit.scaleSetting = true;
                                model.GetAnimScript().transform.localScale = new Vector3(-1 * model.GetAnimScript().transform.localScale.x,
                                                                                         model.GetAnimScript().transform.localScale.y,
                                                                                         model.GetAnimScript().transform.localScale.z);
                            }
                        }
                        else {
                            
                            if (model.GetAnimScript().transform.localScale.x < 0)
                            {
                                Debug.Log("우로본다");
                                birdCreatureUnit.scaleSetting = true;
                                model.GetAnimScript().transform.localScale = new Vector3(-1 * model.GetAnimScript().transform.localScale.x,
                                                                                         model.GetAnimScript().transform.localScale.y,
                                                                                         model.GetAnimScript().transform.localScale.z);
                            }
                        }
                        
                        Debug.Log("성공");
                    }
                }
                else
                {
                    attract = false;
                    timer.TimerStart(false);
                    attractCasting = false;
                }
            }
        }

        if (attract) {
            //Try Attract when timer out this duration will cast attract motions
            
            
            if (currentTarget != null)
            {
                if (targetMoving != true)
                {
                    Debug.Log("도착한거같은데");

                    model.GetAnimScript().SendMessage("Kill");
                    AnimatorManager.instance.ResetAnimatorTransform(currentTarget.instanceId);
                    Debug.Log(currentTarget.instanceId);
                    Animator agentAnim = null;
                    if (currentTarget is AgentModel)
                    {
                        agentAnim = AgentLayer.currentLayer.GetAgent(currentTarget.instanceId).puppetAnim;
                    }
                    else if (currentTarget is OfficerModel)
                    {
                        Debug.Log("사무직 행동 멈춰야 함");
                        agentAnim = OfficerLayer.currentLayer.GetOfficer(currentTarget.instanceId).puppetAnim;
                    }

                    Debug.Log(agentAnim);
                    AnimatorManager.instance.ChangeAnimatorByName(currentTarget.instanceId, AnimatorName.BigBird_AgentCTRL,
                                      agentAnim, true, false);
                    currentTarget.TakePhysicalDamageByCreature(100);
                    isPlayingDeadScene = true;
                    currentTarget = null;
                    //timer.TimerStart(false);
                }
                else {
                    Vector3 pos = this.model.GetMovableNode().GetCurrentViewPosition();

                    if (model.GetAnimScript().transform.localScale.x < 0)
                    {
                        pos = new Vector3(pos.x - 1.8f, pos.y, pos.z);
                    }
                    else {
                        pos = new Vector3(pos.x + 1.8f, pos.y, pos.z);
                    }

                  
                    if (currentTarget is AgentModel) {
                        AgentUnit unit = AgentLayer.currentLayer.GetAgent(currentTarget.instanceId);
                        if (unit.MannualMovingCall(pos)) {
                            targetMoving = false;
                        }
                    }
                    else if (currentTarget is WorkerModel) {
                        OfficerUnit unit = OfficerLayer.currentLayer.GetOfficer(currentTarget.instanceId);
                        if (unit.MannualMovingCall(pos)) {

                            targetMoving = false;
                        }
                    }
                }
            }
            else {
                if (isPlayingDeadScene == true && model.GetAnimScript().animator.GetInteger("Kill") == 0) {
                    isPlayingDeadScene = false;
                    timer.TimerStart(false);
                    birdCreatureUnit.scaleSetting = false;
                    attract = false;
                }
            }
            
        }
        //Debug.Log("tail " + model.GetMovableNode().IsMoving());
    }

    bool AttractNearAgent() {
        List<WorkerModel> nearTarget = new List<WorkerModel>();

        OfficerModel[] detectedOfficer = OfficeManager.instance.GetNearOfficers(model.GetMovableNode());
        if (detectedOfficer.Length == 0)
        {
            AgentModel[] detectedAgent = AgentManager.instance.GetNearAgents(model.GetMovableNode());
            if (detectedAgent.Length == 0)
            {
                return false;
            }
            else
            {
                foreach (AgentModel am in detectedAgent)
                {
                    nearTarget.Add(am);
                }
            }
        }
        else {
            foreach (OfficerModel om in detectedOfficer) {
                nearTarget.Add(om);
            }
        }

        currentTarget = nearTarget[UnityEngine.Random.Range(0, nearTarget.Count)];
        Debug.Log(currentTarget.name);
        return true;
    /*
        AgentModel[] detectedAgent = AgentManager.instance.GetNearAgents(model.GetMovableNode());
        if (detectedAgent.Length == 0)
        {
            Debug.Log("매혹대상없음");
            return false;
        }
        else
        {
            foreach (AgentModel am in detectedAgent)
            {
                nearTarget.Add(am);
            }
        }

        

        return true;
     */
    }

    void MakeBirdMovement() {
        int randVal;
        MapNode node;
        MapNode[] nodes = null;
        while (former == (randVal = UnityEngine.Random.Range(0, model.sefira.departmentNum + 1))) ;
        former = randVal;
        if (randVal == model.sefira.departmentNum)
        {
            nodes = MapGraph.instance.GetSefiraNodes(model.sefira);
        }
        else
        {
            nodes = MapGraph.instance.GetAdditionalSefira(model.sefira);
        }
        node = nodes[UnityEngine.Random.Range(0, nodes.Length)];

        model.MoveToNode(node);
        
    }

    void StopBirdMovement()
    {
        Debug.Log("BirdStop");
        model.ClearCommand();
        model.GetMovableNode().StopMoving();
        
        
        model.GetAnimScript().SendMessage("Stop");
    }

    public void Escape() {
        Debug.Log("탈출해야지");
        model.StopEscapeWork();
        MakeBirdMovement();
        escaped = true;
        timer.TimerStart(false);
    }

    public void OnCalled() { 
        
    }

    public void OnCalled(int i)
    {
        this.SetPassageAlpha(i);
    }

    public void AnimatorEventInit()
    {
        AnimatorEventScript script = model.GetAnimScript().animator.GetComponent<AnimatorEventScript>();
        script.SetTarget(this);
    }

    public void AgentReset() { }


}