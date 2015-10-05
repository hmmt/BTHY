﻿using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


// 직원 데이터
public class AgentModel : IObserver
{

    public int instanceId;

    // 초기화 이외에는 사용하지 않고 있다.
    //public AgentTypeInfo metadata;

    public string name;
    public int hp;
    public int mental;

    public List<TraitTypeInfo> traitList;

    public string gender;
    public int level;
    public int workDays;

    public int expFail = 0;
    public int expSuccess = 0;
    public int expHpDamage = 0;
    public int expMentalDamage = 0;

    public int defaultMaxHp;
    public int traitMaxHp;
    public int maxHp;

    public int defaultMaxMental;
    public int traitMaxmental;
    public int maxMental; //

    public int defaultMovement;
    public int traitmovement;
    public int movement; //

    public int defaultWork;
    public int traitWork;
    public int work; //

    public string prefer;
    public int preferBonus;
    public string reject;
    public int rejectBonus;

    public int directBonus;
    public int inDirectBonus;
    public int blockBonus;

    public SkillTypeInfo directSkill;
    public SkillTypeInfo indirectSkill;
    public SkillTypeInfo blockSkill;

    public string imgsrc;

    public Dictionary<string, string> speechTable = new Dictionary<string, string>();

    public string panicType;
    //

    // 현재 소속된 세피라
    public string currentSefira;

    //활성화된 직원인가 체크
    public bool activated;

    // 스프라이트 고유 번호
    public string faceSpriteName;
    public string hairSpriteName;
    public string bodySpriteName;
    public string panicSpriteName;

    public string hairImgSrc;
    public string faceImgSrc;
    public string bodyImgSrc;



    // 이하 save 되지 않는 데이터들

    private AgentCmdState state = AgentCmdState.IDLE;

    /*
     * state; MOVE, WORKING
     * 이동하거나 작업할 때 대상 환상체
     */
    public CreatureModel target; 

    // panic action을 실행하는 클래스
    private PanicAction currentPanicAction;

    // path finding2
    MovableObjectNode movableNode;

    public AgentModel(int instanceId, string area)
    {
        movableNode = new MovableObjectNode();

        traitList = new List<TraitTypeInfo>();
        this.instanceId = instanceId;
        //currentSefira = area;
        currentSefira = "0";
        SetCurrentSefira(area);
        movableNode.SetCurrentNode(MapGraph.instance.GetSepiraNodeByRandom(area));
    }

    private bool visible = true;
    private float oldZ;

    ////

    private float waitTimer = 0;

    public Dictionary<string, object> GetSaveData()
    {
        Dictionary<string, object> output = new Dictionary<string, object>();

        output.Add("instanceId", instanceId);
        output.Add("currentSefira", currentSefira);

        output.Add("name", name);
        output.Add("hp", hp);
        //output.Add("traitList", 

        output.Add("gender", gender);
        output.Add("level", level);
        output.Add("workDays", workDays);

        output.Add("expFail", expFail);
        output.Add("expSuccess", expSuccess);
        output.Add("expHpDamage", expHpDamage);
        output.Add("expMentalDamage", expMentalDamage);

        output.Add("maxHp", maxHp);
        output.Add("maxMental", maxMental);

        output.Add("mental", mental);
        output.Add("movement", movement);
        output.Add("work", work);

        output.Add("prefer", prefer);
        output.Add("preferBonus", preferBonus);
        output.Add("reject", reject);
        output.Add("rejectBonus", rejectBonus);
    
        output.Add("directSkillId", directSkill.id);
        output.Add("indirectSkillId", indirectSkill.id);
        output.Add("blockSkillId", blockSkill.id);

        output.Add("imgsrc", imgsrc);
        output.Add("speechTable", speechTable);
        output.Add("panicType", panicType);
        
        /*
        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream stream = new MemoryStream();
        bf.Serialize(stream, output);
        return stream.ToArray();
        */

        return output;
    }

    private static bool TryGetValue<T>(Dictionary<string, object> dic, string name, ref T field)
    {
        object output;
        if (dic.TryGetValue(name, out output))
        {
            field = (T)output;
            return true;
        }
        return false;
    }

    //랜덤으로 만들어진 직원 초상화 스프라이트 합쳐주는 함수

    public void AgentPortrait(string parts, string key)
    {
        if (parts == "hair")
        {
            hairImgSrc = "Sprites/Agent/Hair/Hair_M_"+key+"_00";
        }

        else if (parts == "face")
        {
            faceImgSrc = "Sprites/Agent/Face/Face_" + key + "_00";
        }

        else if (parts == "body")
        {
            if(currentSefira == "0")
                bodyImgSrc = "Sprites/Agent/Body/Body_1_S_00";
            else
            bodyImgSrc = "Sprites/Agent/Body/Body_" + currentSefira + "_S_00";
        }

    }

    public void LoadData(Dictionary<string, object> dic)
    {
        //BinaryFormatter bf = new BinaryFormatter();
        //Dictionary<string, object> dic = (Dictionary<string, object>)bf.Deserialize(stream);
        TryGetValue(dic, "instanceId", ref instanceId);

        TryGetValue(dic, "name", ref name);
        TryGetValue(dic, "hp", ref hp);
        //output.Add("traitList", 

        TryGetValue(dic, "gender", ref gender);
        TryGetValue(dic, "level", ref level);
        TryGetValue(dic, "workDays", ref workDays);

        TryGetValue(dic, "expFail", ref expFail);
        TryGetValue(dic, "expSuccess", ref expSuccess);
        TryGetValue(dic, "expHpDamage", ref expHpDamage);
        TryGetValue(dic, "expMentalDamage", ref expMentalDamage);

        TryGetValue(dic, "maxHp", ref maxHp);
        TryGetValue(dic, "maxMental", ref maxMental);

        TryGetValue(dic, "mental", ref mental);
        TryGetValue(dic, "movement", ref movement);
        TryGetValue(dic, "work", ref work);

        TryGetValue(dic, "prefer", ref prefer);
        TryGetValue(dic, "preferBonus", ref preferBonus);
        TryGetValue(dic, "reject", ref reject);
        TryGetValue(dic, "rejectBonus", ref rejectBonus);

        long id = 0;
        TryGetValue(dic, "directSkillId", ref id);
        directSkill = SkillTypeList.instance.GetData(id);
        id = 0;
        TryGetValue(dic, "indirectSkillId", ref id);
        indirectSkill = SkillTypeList.instance.GetData(id);
        id = 0;
        TryGetValue(dic, "blockSkillId", ref id);
        blockSkill = SkillTypeList.instance.GetData(id);

        TryGetValue(dic, "imgsrc", ref imgsrc);
        TryGetValue(dic, "speechTable", ref speechTable);
        TryGetValue(dic, "panicType", ref panicType);

        TryGetValue(dic, "currentSefira", ref currentSefira);
    }

    // notice로 호출됨
    public void OnFixedUpdate()
    {
        ProcessAction();

        movableNode.ProcessMoveNode(movement);
    }

    public void applyTrait(TraitTypeInfo addTrait)
    {
        Debug.Log("이름"+name);
        Debug.Log("체력" + defaultMaxHp);
        Debug.Log("멘탈" + defaultMaxMental);
        Debug.Log("이동속도" + defaultMovement);
        Debug.Log("작업속도" + defaultWork);


        traitList.Add(addTrait);

        traitMaxHp = 0;
        traitMaxmental = 0;
        traitmovement = 0;
        traitWork = 0;

        for(int i=0; i<traitList.Count; i++)
        {
            traitMaxHp += traitList[i].hp;
            traitMaxmental += traitList[i].mental;
            traitmovement += traitList[i].moveSpeed;
            traitWork += traitList[i].workSpeed;

            Debug.Log("특성 이름" + traitList[i].name);
            Debug.Log("특성 체력" + traitList[i].hp);
            Debug.Log("특성 멘탈" + traitList[i].mental);
            Debug.Log("특성 이동속도" + traitList[i].moveSpeed);
            Debug.Log("특성 작업속도" + traitList[i].workSpeed);
        }

        maxHp = defaultMaxHp + traitMaxHp;
        maxMental = defaultMaxMental + traitMaxmental;
        movement = defaultMovement + traitmovement;
        work = defaultWork + traitWork;

        hp += addTrait.hp;
        mental += addTrait.mental;

        directBonus += (int)addTrait.directWork;
        inDirectBonus += (int)addTrait.inDirectWork;
        blockBonus += (int)addTrait.blockWork;

        if (maxHp <= 0)
        {
            maxHp = 1;
            hp = 1;
        }

        if (maxMental <= 0)
        {
            maxMental = 1;
            mental = 1;
        }

        if(movement <= 0)
        {
            movement = 1;
        }

        if(work <= 0)
        {
            work = 1;
        }


        Debug.Log("변경후 체력" + maxHp);
        Debug.Log("변경후 멘탈" + maxMental);
        Debug.Log("변경후 속도" + movement);
        Debug.Log("변경후 작업속도" + work);
    }

    public void UpdateSKill(string skillType)
    {
        SkillTypeInfo newSkill = null;

        if (skillType == "direct")
        {
            newSkill = SkillTypeList.instance.GetNextSkill(directSkill);
            if (newSkill == null)
                return;
            directSkill = newSkill;
        }
        else if (skillType == "indirect")
        {
            newSkill = SkillTypeList.instance.GetNextSkill(indirectSkill);
            if (newSkill == null)
                return;
            indirectSkill = newSkill;
        }
        else if (skillType == "block")
        {
            newSkill = SkillTypeList.instance.GetNextSkill(blockSkill);
            if (newSkill == null)
                return;
            blockSkill = newSkill;
        }
        else
        {
            return;
        }
    }

    private void ProcessAction()
    {
        if (currentPanicAction != null)
        {
            currentPanicAction.Execute();
        }
        else if (state == AgentCmdState.IDLE)
        {
            if (waitTimer <= 0)
            {

                movableNode.MoveToNode(MapGraph.instance.GetSepiraNodeByRandom(currentSefira));

                waitTimer = 1.5f + Random.value;
            }
        }
        else if (state == AgentCmdState.WORKING)
        {
            if (movableNode.GetCurrentEdge() == null && movableNode.GetCurrentNode() != target.GetWorkspaceNode())
            {
                MoveToCretureRoom(target);
            }
        }

        else if (state == AgentCmdState.DEAD)
        {

        }
        waitTimer -= Time.deltaTime;
    }


    public MovableObjectNode GetMovableNode()
    {
        return movableNode;
    }
    public Vector2 GetCurrentViewPosition()
    {
        return movableNode.GetCurrentViewPosition();
    }
    // edge 위에 있을 때도 통합할 수 있는 타입 필요
    public MapNode GetCurrentNode()
    {
        return movableNode.GetCurrentNode();
    }
    public void SetCurrentNode(MapNode node)
    {
        movableNode.SetCurrentNode(node);
    }
    public MapEdge GetCurrentEdge()
    {
        return movableNode.GetCurrentEdge();
    }
    public int GetEdgeDirection()
    {
        return movableNode.GetEdgeDirection();
    }

    public AgentCmdState GetState()
    {
        return state;
    }
    public void ReturnToSefira()
    {
        SetCurrentNode(MapGraph.instance.GetSepiraNodeByRandom(currentSefira));
    }

    public void MoveToNode(string targetNodeId)
    {
        movableNode.MoveToNode(MapGraph.instance.GetNodeById(targetNodeId));
    }

    public void MoveToCreture(CreatureModel target)
    {
        movableNode.MoveToMovableNode(target.GetMovableNode());
    }
    public void MoveToCretureRoom(CreatureModel target)
    {
        movableNode.MoveToNode(target.GetWorkspaceNode());
    }

    public bool isDead()
    {
        return hp <= 0;
    }

    public void Attacked()
    {
        state = AgentCmdState.CAPTURE;
        movableNode.StopMoving();
    }
    public void WorkEscape(CreatureModel target)
    {
        state = AgentCmdState.ESCAPE_WORKING;
        this.target = target;
        MoveToCreture(target);
    }
    public void Working(CreatureModel target)
    {
        state = AgentCmdState.WORKING;
        this.target = target;
        MoveToCretureRoom(target);
    }
    public void FinishWorking()
    {
        state = AgentCmdState.IDLE;
        this.target = null;
    }
    public void TakePhysicalDamage(int damage)
    {
        Debug.Log(name + " takes PHYSICAL dmg " + damage);
        hp -= damage;

        if (hp <= 0)
        {
            Die();
        }
    }

    public void TakeMentalDamage(int damage)
    {
        Debug.Log(name + " takes MENTAL dmg " + damage);
        mental -= damage;

        if (mental <= 0)
        {
        }
    }

    public bool HasTrait(long id)
    {
        foreach (TraitTypeInfo info in traitList)
        {
            if (info.id == id)
                return true;
        }
        return false;
    }

    public void SetCurrentSefira(string sefira)
    {
        currentSefira = sefira;
        switch (currentSefira)
        {
            case "0": imgsrc = "Agent/Malkuth/0"; break;
            case "1": imgsrc = "Agent/Malkuth/0"; break;
            case "2": imgsrc = "Agent/Nezzach/00"; break;
            case "3": imgsrc = "Agent/Hodd/00"; break;
            case "4": imgsrc = "Agent/Yessod/00"; break;
        }
        waitTimer = 0;
    }

    public void Panic()
    {
        /*
        if (panicType == "default")
        {
            currentPanicAction = new PanicDefaultAction();
            string narration = this.name + " (이)가 공황에 빠져 우두커니 서있습니다.";
            Notice.instance.Send("AddSystemLog", narration);
        }
        else if (panicType == "roaming")
        {
            currentPanicAction = new PanicRoaming(this);
            string narration = this.name + " (이)가 공황에 빠져 방향을 잃고 배회합니다.";
            Notice.instance.Send("AddSystemLog", narration);
        }
         * */
    }

    public void Die()
    {
        string narration = this.name + " (이)가 사망했습니다.";

        Debug.Log("사망");

        Notice.instance.Send("AddSystemLog", narration);
        Notice.instance.Send("AgentDie", this);

        this.hp = 0;
        this.state = AgentCmdState.DEAD;

        //AgentManager.instance.RemoveAgent(this);
        //AgentLayer.currentLayer.GetAgent(this.instanceId).DeadAgent();
    }



    public void OnNotice(string notice, params object[] param)
    {
        if (notice == NoticeName.FixedUpdate)
        {
            OnFixedUpdate();
        }
    }
}
