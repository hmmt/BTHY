using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class SingingMachineSkill : CreatureSpecialSkill, IObserver {
    public List<WorkerModel> attractTarget;
    List<WorkerModel> targetList;
    const float frequency = 5f;
    float elapsed = 0f;
    bool Attracted = false;
    MapNode passageNode;
    public bool PlayingMusic = false;
    float musicTimer = 0f;
    float musicDuration = 10f;
    bool Working = false;

    List<AgentModel> workedList;
    PassageObjectModel passageModel;
    Vector3 machinePos;
    string NoteEffect = "Effect/NoteEffect";
    GameObject machineNote;
    GameObject passageNote;

    public SingingMachineSkill(CreatureModel model)
    {
        this.model = model;

        Notice.instance.Observe(NoticeName.FixedUpdate, this);
        machineNote = ResourceCache.instance.LoadPrefab(NoteEffect);
        passageNote = ResourceCache.instance.LoadPrefab(NoteEffect);
        this.targetList = new List<WorkerModel>();
        this.attractTarget = new List<WorkerModel>();
        machineNote.SetActive(false);
        passageNote.SetActive(false);
    }

    public override void FixedUpdate()
    {
        if (PlayingMusic) { 
            //근처 직원들을 리스트에 넣어야 한다
            //위 리스트 중 음악이 끝날 때의 직원 하나를 Attracted상태로 변화
            //
            CheckAgentInRange();
            musicTimer += Time.deltaTime;
            if (musicTimer > musicDuration) {
                Debug.Log("끝");
                musicTimer = 0f;
                //MusicEnd;
                StopNote();

                Attract(targetList);
            }
        }

        if (Activated) { 
            
        }
    }

    //직원 제압 성공
    public void FreeAttractedAgent(WorkerModel target) {
        
        //패닉에서 돌아옴
        
        //agentView.SetAnimatorChanged(false);

    }

    public override void OnStageStart()
    {
        this.sefira = model.sefira;

        this.passageNode = MapGraph.instance.GetNodeById(model.entryNodeId);
        this.passageModel = this.passageNode.GetAttachedPassage();
        this.machinePos = this.model.GetCurrentViewPosition();
        Vector3 tempPos = this.passageNode.GetPosition();

        this.machineNote.transform.position = new Vector3(machinePos.x +0.9f, machinePos.y +1f, -10f);
        this.passageNote.transform.position = new Vector3(tempPos.x, tempPos.y, -10f);
        this.targetList.Clear();
    }

    public override void SkillActivate(WorkerModel agent)
    {
        Debug.Log("SkillActivate");
        if (agent is AgentModel) {
            (agent as AgentModel).LoseControl();
        }
        this.SpecialSkill(agent);
    }

    private void SpecialSkill(WorkerModel target) {
        Animator targetAnim;
        Debug.Log(target.name);
        if (target is AgentModel) {
            targetAnim = AgentLayer.currentLayer.GetAgent(target.instanceId).puppetAnim;
        }
        else if(target is OfficerModel){
            targetAnim = OfficerLayer.currentLayer.GetOfficer(target.instanceId).puppetAnim;
        }
        else{
            Debug.Log("Error");
            return;
        }

        //this animator play killing anim
        //targetAnimChangeAnimator
        //AnimatorManager.instance.ChangeAnimatorByID();
        CreatureLayer.currentLayer.GetCreature(model.instanceId).creatureAnimator.SetBool("Kill", true);
        AnimatorManager.instance.ChangeAnimatorByID(target.instanceId, AnimatorName.id_Machine_victim, targetAnim, true, false);
        MakeNote();
        //add feeling 80% 
        this.model.AddFeeling((float)this.model.metaInfo.feelingMax * 0.8f);
        //make Note in machine & Wall in passage
    }

    public void Attract(List<WorkerModel> list) {
        if (list.Count == 0) {
            return;
        }
        int index = UnityEngine.Random.Range(0, list.Count);
        this.attractTarget.Add(list[index]);
        Debug.Log(list[index].name);
        AttractInitialMovement(list[index]);
        this.targetList.Clear();
    }

    public void MakeNote() {
        this.PlayingMusic = true;
        //Instantiate Music Effect in Isolateroom & Passage door

        this.passageNote.SetActive(true);
        this.machineNote.SetActive(true);
    }

    public void StopNote() {
        this.passageNote.SetActive(false);
        this.machineNote.SetActive(false);
        this.PlayingMusic = false;
    }

    public void CheckAgentInRange() {
        foreach (AgentModel am in this.sefira.agentList) {
            if (this.targetList.Contains(am)) continue;
            if (am.movableNode.GetPassage() == this.passageModel) {
                this.targetList.Add(am);
                Debug.Log(am.name);
            }
            
        }
        foreach (OfficerModel om in this.sefira.officerList) {
            if (this.targetList.Contains(om)) continue;
            if (om.movableNode.GetPassage() == this.passageModel) {
                this.targetList.Add(om);
                Debug.Log(om.name);
            }
        }
    }

    public void SetSuppressed() { 
        
    }
    
    void IObserver.OnNotice(string notice, params object[] param)
    {
        base.OnNotice(notice, param);
    }

    public void AttractInitialMovement(WorkerModel target) {
        Animator targetAnim = null;
        if (target is OfficerModel) {
            targetAnim = OfficerLayer.currentLayer.GetOfficer(target.instanceId).puppetAnim;
        }
        else if (target is AgentModel) {
            (target as AgentModel).LoseControl();
            targetAnim = AgentLayer.currentLayer.GetAgent(target.instanceId).puppetAnim;
        }
        
        AnimatorManager.instance.ChangeAnimatorByName(target.instanceId, AnimatorName.Machine_attract, targetAnim, true, false);
    }
}
