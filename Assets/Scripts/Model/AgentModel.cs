using UnityEngine;
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

    public int agentLifeValue;

    public int internalTrait=0;
    public int externalTrait=0;
    public int thinkTrait=0;
    public int emotionalTrait=0;

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


    public Sprite[] StatusSprites = new Sprite[4];
    public Sprite[] WorklistSprites = new Sprite[3];
    /*
     * state; MOVE, WORKING
     * 이동하거나 작업할 때 대상 환상체
     */
    public CreatureModel target;

    /// <summary>
    /// 특정 행동의 대상 직원
    /// (SUPPRESS)
    /// </summary>
    public AgentModel targetAgent;

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

    private ValueInfo levelSetting;

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

    public void checkAgentLifeValue(TraitTypeInfo addTrait)
    {
        if (addTrait.discType == 1)
        {
            externalTrait++;
        }

        else if (addTrait.discType == 2)
        {
            internalTrait++;
        }

        else if (addTrait.discType == 3)
        {
            thinkTrait++;
        }

        else if (addTrait.discType == 4)
        {
            emotionalTrait++;
        }

        int externalFlag = 0;
        int thinkFlag = 0;

        if(externalTrait > internalTrait)
        {
            externalFlag = 1;
        }

        else if(externalTrait < internalTrait)
        {
            externalFlag = 0;
        }

        else
        {
            if (Random.Range(0, 1) == 1)
            {
                externalFlag = 1;
            }
            else
            {
                externalFlag = 0;
            }
        }

        if (thinkTrait > emotionalTrait)
        {
            thinkFlag = 1;
        }

        else if (thinkTrait < emotionalTrait)
        {
            thinkFlag = 0;
        }

        else
        {
            if (Random.Range(0, 1) == 1)
            {
                thinkFlag = 1;
            }
            else
            {
                thinkFlag = 0;
            }
        }

        if(externalFlag == 1 && thinkFlag == 1)
        {
            agentLifeValue = 1; // 합리주의자
        }

        else if(externalFlag == 1 && thinkFlag == 0)
        {
            agentLifeValue = 2; // 낙천주의자
        }

        else if (externalFlag == 0 && thinkFlag == 1)
        {
            agentLifeValue = 3; // 원칙주의자
        }

        else if (externalFlag == 0 && thinkFlag == 0)
        {
            agentLifeValue = 4; // 평화주의자
        }

    }

    public void applyTrait(TraitTypeInfo addTrait)
    {
        traitList.Add(addTrait);

        traitMaxHp = 0;
        traitMaxmental = 0;
        traitmovement = 0;
        traitWork = 0;

        if (addTrait.discType != 0) 
            checkAgentLifeValue(addTrait);

        for (int i=0; i<traitList.Count; i++)
        {
            traitMaxHp += traitList[i].hp;
            traitMaxmental += traitList[i].mental;
            traitmovement += traitList[i].moveSpeed;
            traitWork += traitList[i].workSpeed;
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

        /*
        Debug.Log("변경후 체력" + maxHp);
        Debug.Log("변경후 멘탈" + maxMental);
        Debug.Log("변경후 속도" + movement);
        Debug.Log("변경후 작업속도" + work);
         */
    }

    public void promoteSkill(int skillClass)
    {
        int randomSkillFlag;

        if(skillClass == 1)
        {
            randomSkillFlag = Random.Range(0,2);

            directSkill = SkillTypeList.instance.GetData(directSkill.nextSkillIdList[randomSkillFlag]);  
        }

        else if(skillClass == 2)
        {
            randomSkillFlag = Random.Range(0, 2);

            indirectSkill = SkillTypeList.instance.GetData(indirectSkill.nextSkillIdList[randomSkillFlag]);

        }

        else if(skillClass == 3)
        {
            randomSkillFlag = Random.Range(0, 2);

            blockSkill = SkillTypeList.instance.GetData(blockSkill.nextSkillIdList[randomSkillFlag]);

        }

    }

    public void UpdateSkill(string skillType)
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
            if (state != AgentCmdState.PANIC_SUPPRESS_TARGET)
                currentPanicAction.Execute();
        }
        else if (state == AgentCmdState.IDLE)
        {
            if (waitTimer <= 0)
            {

                movableNode.MoveToNode(MapGraph.instance.GetSepiraNodeByRandom(currentSefira), Random.value);

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
        else if (state == AgentCmdState.ESCAPE_WORKING)
        {
            // WorkEscapedCreature에서 실행
        }
        else if (state == AgentCmdState.SUPPRESS_WORKING)
        {
            if (!movableNode.CheckInRange(targetAgent.movableNode) && waitTimer <= 0)
            {
                movableNode.MoveToMovableNode(targetAgent.movableNode);
                waitTimer = 1.5f + Random.value;
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
    public Vector3 GetCurrentViewPosition()
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

    /**
     * state 관련 함수들
     **/
    public AgentCmdState GetState()
    {
        return state;
    }

    public void AttackedByCreature()
    {
        state = AgentCmdState.CAPTURE_BY_CREATURE;
        movableNode.StopMoving();
        Notice.instance.Send(NoticeName.MakeName(NoticeName.ChangeAgentState, instanceId.ToString()));
    }
    public void WorkEscape(CreatureModel target)
    {
        state = AgentCmdState.ESCAPE_WORKING;
        this.target = target;
        //MoveToCreture(target);
        Notice.instance.Send(NoticeName.MakeName(NoticeName.ChangeAgentState, instanceId.ToString()));
    }
    public void Working(CreatureModel target)
    {
        state = AgentCmdState.WORKING;
        this.target = target;
        MoveToCretureRoom(target);
        Notice.instance.Send(NoticeName.MakeName(NoticeName.ChangeAgentState, instanceId.ToString()));
    }

    public void ReturnCreature()
    {
        state = AgentCmdState.RETURN_CREATURE;
        Notice.instance.Send(NoticeName.MakeName(NoticeName.ChangeAgentState, instanceId.ToString()));
    }
    public void FinishWorking()
    {
        state = AgentCmdState.IDLE;
        this.target = null;
        Notice.instance.Send(NoticeName.MakeName(NoticeName.ChangeAgentState, instanceId.ToString()));
    }
    public void UpdateStateIdle()
    {
        state = AgentCmdState.IDLE;
        this.target = null;
        Notice.instance.Send(NoticeName.MakeName(NoticeName.ChangeAgentState, instanceId.ToString()));
    }

    /// <summary>
    /// 다른 직원을 공격합니다.
    /// 현재 살인상태인 경우에만 사용해야 합니다.
    /// AttackAgentByAgent.cs 에서 사용합니다.
    /// </summary>
    public void StartPanicAttackAgent()
    {
        state = AgentCmdState.PANIC_VIOLENCE;
        Notice.instance.Send(NoticeName.MakeName(NoticeName.ChangeAgentState, instanceId.ToString()));
    }

    /// <summary>
    /// 다른 직원을 공격하던 것을 중지합니다.
    /// AttackAgentByAgent.cs 에서 사용합니다.
    /// </summary>
    public void StopPanicAttackAgent()
    {
        state = AgentCmdState.IDLE;
        Notice.instance.Send(NoticeName.MakeName(NoticeName.ChangeAgentState, instanceId.ToString()));
    }

    public void StopSuppress()
    {
        state = AgentCmdState.IDLE;
        Notice.instance.Send(NoticeName.MakeName(NoticeName.ChangeAgentState, instanceId.ToString()));
    }


    public void OpenIsolateRoom()
    {
        state = AgentCmdState.OPEN_ROOM;
        Notice.instance.Send(NoticeName.MakeName(NoticeName.ChangeAgentState, instanceId.ToString()));
    }

    public void StartSuppressAgent(AgentModel targetAgent)
    {
        state = AgentCmdState.SUPPRESS_WORKING;
        this.targetAgent = targetAgent;
        Notice.instance.Send(NoticeName.MakeName(NoticeName.ChangeAgentState, instanceId.ToString()));
    }

    public void PanicSuppressed()
    {
        state = AgentCmdState.PANIC_SUPPRESS_TARGET;
        movableNode.StopMoving();
        Notice.instance.Send(NoticeName.MakeName(NoticeName.ChangeAgentState, instanceId.ToString()));
    }

    // state 관련 함수들 end

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

    public void RecoverHP(int amount)
    {
        hp += amount;
        hp = hp > maxHp ? maxHp : hp;
    }
    public void RecoverMental(int amount)
    {
        mental += amount;
        mental = mental > maxMental ? maxMental : mental;
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
        string old = currentSefira;
        currentSefira = sefira;
        switch (currentSefira)
        {
            case "0":
                imgsrc = "Agent/Malkuth/0";
                break;
            case "1": 
                imgsrc = "Agent/Malkuth/0"; 
                break;
            case "2": 
                imgsrc = "Agent/Nezzach/00";
                break;
            case "3": 
                imgsrc = "Agent/Hodd/00"; 
                break;
            case "4": 
                imgsrc = "Agent/Yessod/00"; 
                break;
        }
        if (currentSefira == "0")
            bodyImgSrc = "Sprites/Agent/Body/Body_1_S_00";
        else
            bodyImgSrc = "Sprites/Agent/Body/Body_" + currentSefira + "_S_00";

        waitTimer = 0;
        Notice.instance.Send(NoticeName.ChangeAgentSefira, this, old);
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
        currentPanicAction = new PanicReady(this);
    }
    public void PanicReadyComplete()
    {
        // currentPanicAction = new PanicSuicideExecutor(this, 5);
        //currentPanicAction = new PanicViolence(this);
        //currentPanicAction = new PanicOpenRoom(this);
        currentPanicAction = new PanicRoaming(this);
        // 바꿔야 함
        /*
        switch (agentLifeValue)
        {
            case 1:
                currentPanicAction = new PanicRoaming(this);
                break;
            case 2:
                currentPanicAction = new PanicSuicideExecutor(this);
                break;
            case 3:
                currentPanicAction = new PanicViolence(this);
                break;
            case 4:
                break;
        }
        */
    }

    public void StopPanic()
    {
        currentPanicAction = null;
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

    public string LifeStyle() {
        string temp = null;
        switch (agentLifeValue) { 
            case 1:
                temp = "합리주의자";
                break;
            case 2:
                temp = "낙천주의자";
                break;
            case 3:
                temp = "원칙주의자";
                break;
            case 4:
                temp = "평화주의자";
                break;
        }

        return temp;
    }

    public void calcLevel() {
        int healthPoint, mentalPoint, workPoint, speedPoint;
        ValueInfo average = ValueInfo.getAverage();
        healthPoint = calc(hp, average.hp);
        mentalPoint = calc(mental, average.mental);
        workPoint = calc(work, average.workSpeed);
        speedPoint = calc(movement, average.movementSpeed);

        levelSetting = new ValueInfo(healthPoint, mentalPoint, workPoint, speedPoint);
        setSprite();
    }

    public int calc(int value, int standard)
    {
        if (value < standard)
        {
            return 0;
        }
        else if (value >= standard && value < 2 * standard)
        {
            return 1;
        }
        else return 2;
    }

    public void setSprite() {
        string loc = "UIResource/Icons/";
        
        for (int i = 0; i < StatusSprites.Length; i++) {
            string fullpath = loc + i + levelSetting.stats[i];
            StatusSprites[i] = ResourceCache.instance.GetSprite(fullpath);
        }

        for (int i = 0; i < WorklistSprites.Length; i++) {
            string fullpath = loc + "Work_" + i;
            WorklistSprites[i] = ResourceCache.instance.GetSprite(fullpath);
        }
    }

    public static int CompareByName(AgentModel x, AgentModel y) {
        if (x == null || y == null) {
            Debug.Log("Errror in comparison by name");
            return 0;
        }
        if (x.name == null)
        {
            if (y.name == null) return 0;
            else return -1;
        }
        else {
            if (y.name == null) return 1;
            else {
                return x.name.CompareTo(y.name);
            }
        }
    }

    public static int CompareByID(AgentModel x, AgentModel y)
    {
        if (x == null || y == null)
        {
            Debug.Log("Errror in comparison by sefira");
            return 0;
        }

        
        return x.instanceId.CompareTo(y.instanceId);
    }

    public static int CompareBySefira(AgentModel x, AgentModel y) {
        if (x == null || y == null)
        {
            Debug.Log("Errror in comparison by sefira");
            return 0;
        }
        int xInt, yInt;

        xInt = int.Parse(x.currentSefira);
        yInt = int.Parse(y.currentSefira);

        return xInt.CompareTo(yInt);
    }

    public static int CompareByLevel(AgentModel x, AgentModel y) {
        if (x == null || y == null)
        {
            Debug.Log("Errror in comparison by level");
            return 0;
        }
        int xInt, yInt;

        xInt = x.level;
        yInt = y.level;

        return xInt.CompareTo(yInt);
    }

    public static int CompareByLifestyle(AgentModel x, AgentModel y) {
        if (x == null || y == null)
        {
            Debug.Log("Errror in comparison by LifeStyle");
            return 0;
        }

        return x.agentLifeValue.CompareTo(y.agentLifeValue);
    }

    public Sprite getCurrentSefiraSprite() {
        Sprite s = null;

        switch (currentSefira) {
            case "0":
                s = ResourceCache.instance.GetSprite("Sprites/UI/StageUI/None_Icon");
                break;
            case "1":
                s = ResourceCache.instance.GetSprite("Sprites/UI/StageUI/Malkuth_Icon");
                break;
            case "2":
                s = ResourceCache.instance.GetSprite("Sprites/UI/StageUI/Netzzach_Icon");
                break;
            case "3":
                s = ResourceCache.instance.GetSprite("Sprites/UI/StageUI/Hod_Icon");
                break;
            case "4":
                s = ResourceCache.instance.GetSprite("Sprites/UI/StageUI/Yessod_Icon");
                break;
            default:
                 s = ResourceCache.instance.GetSprite("Sprites/UI/StageUI/None_Icon");
                break;
        }

        return s;
    }

}
