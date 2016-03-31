 using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public enum AgentMotion
{
	ATTACK_MOTION,
	PANIC_ATTACK_MOTION
}


// 직원 데이터
[System.Serializable]
public class AgentModel : WorkerModel
{

    // 초기화 이외에는 사용하지 않고 있다.
    //public AgentTypeInfo metadata;

    public List<TraitTypeInfo> traitList;

	// motion variables



	//

    public int level;
    public int workDays;

    public int expFail = 0;
    public int expSuccess = 0;
    public int expHpDamage = 0;
    public int expMentalDamage = 0;

    public int defaultMaxHp;
    public int traitMaxHp;

    public int defaultMaxMental;
    public int traitMaxmental;

    public int defaultMovement;
    public float traitmovement;

    public int defaultWork;
    public float traitWork;
    public int workSpeed; //

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

    public AgentHistory history;
	/*
    public SkillTypeInfo directSkill;
    public SkillTypeInfo indirectSkill;
    public SkillTypeInfo blockSkill;
*/
	private List<SkillTypeInfo> skillList;
    private List<SkillCategory> skills;

    //

    //활성화된 직원인가 체크
    public bool activated;

    // 이하 save 되지 않는 데이터들
    private ValueInfo levelSetting;
    private AgentAIState state = AgentAIState.IDLE;
    

    public Sprite[] StatusSprites = new Sprite[4];
    public Sprite[] WorklistSprites = new Sprite[3];
    public Sprite tempHairSprite;
    public Sprite tempFaceSprite;

    /// <summary>
    /// 임시 값, 성공확률
    /// </summary>
    public float successPercent;

    /*
     * state; MOVE, WORKING
     * 이동하거나 작업할 때 대상 환상체
     */

    /// <summary>
    /// 특정 행동의 대상 직원
    /// (SUPPRESS)
    /// </summary>

    // panic action을 실행하는 클래스

    // path finding2

    public AgentModel(int id, string area)
    {
        movableNode = new MovableObjectNode(this);
        commandQueue = new WorkerCommandQueue(this);

        traitList = new List<TraitTypeInfo>();
		skillList = new List<SkillTypeInfo> ();

        skills = new List<SkillCategory>();

        instanceId = id;
        //currentSefira = area;
        currentSefira = "0";
        SetCurrentSefira(area);
        movableNode.SetCurrentNode(MapGraph.instance.GetSepiraNodeByRandom(area));
        history = new AgentHistory();

        tempHairSprite = AgentLayer.currentLayer.GetAgentHair();
        tempFaceSprite = AgentLayer.currentLayer.GetAgentFace();

        successPercent = Random.Range(0, 90f);

        this.AddNewCategory(1);//initial Skill
    }

    public override Dictionary<string, object> GetSaveData()
    {
        Dictionary<string, object> output = base.GetSaveData();
        output = history.GetSaveData(output);
        //output.Add("traitList", 
        
        output.Add("level", level);
        output.Add("workDays", workDays);

        output.Add("expFail", expFail);
        output.Add("expSuccess", expSuccess);
        output.Add("expHpDamage", expHpDamage);
        output.Add("expMentalDamage", expMentalDamage);

        output.Add("prefer", prefer);
        output.Add("preferBonus", preferBonus);
        output.Add("reject", reject);
        output.Add("rejectBonus", rejectBonus);

		output.Add("workSpeed", workSpeed);
    /*
        output.Add("directSkillId", directSkill.id);
        output.Add("indirectSkillId", indirectSkill.id);
        output.Add("blockSkillId", blockSkill.id);
*/
        /*
        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream stream = new MemoryStream();
        bf.Serialize(stream, output);
        return stream.ToArray();
        */

        return output; 
    }

    public override void LoadData(Dictionary<string, object> dic)
    {
        //BinaryFormatter bf = new BinaryFormatter();
        //Dictionary<string, object> dic = (Dictionary<string, object>)bf.Deserialize(stream);
        base.LoadData(dic);

        //output.Add("traitList", 
        history.LoadData(dic);
        TryGetValue(dic, "level", ref level);
        TryGetValue(dic, "workDays", ref workDays);

        TryGetValue(dic, "expFail", ref expFail);
        TryGetValue(dic, "expSuccess", ref expSuccess);
        TryGetValue(dic, "expHpDamage", ref expHpDamage);
        TryGetValue(dic, "expMentalDamage", ref expMentalDamage);

        TryGetValue(dic, "prefer", ref prefer);
        TryGetValue(dic, "preferBonus", ref preferBonus);
        TryGetValue(dic, "reject", ref reject);
        TryGetValue(dic, "rejectBonus", ref rejectBonus);

		TryGetValue(dic, "workSpeed", ref workSpeed);
		/*
        long id = 0;
        TryGetValue(dic, "directSkillId", ref id);
        directSkill = SkillTypeList.instance.GetData(id);
        id = 0;
        TryGetValue(dic, "indirectSkillId", ref id);
        indirectSkill = SkillTypeList.instance.GetData(id);
        id = 0;
        TryGetValue(dic, "blockSkillId", ref id);
        blockSkill = SkillTypeList.instance.GetData(id);
        */
    }

    private static bool tempPanic = false;
    // notice로 호출됨
    public override void OnFixedUpdate()
    {
        if (isDead())
            return;

		if(attackDelay > 0)
			attackDelay -= Time.deltaTime;
		if (moveDelay > 0)
			moveDelay -= Time.deltaTime;
		/*
        if (!tempPanic)
        {
            tempPanic = true;
            Panic();
        }
        */

		if (stunTime > 0) {
			stunTime -= Time.deltaTime;
			return;
		}

        ProcessAction();

		if(moveDelay > 0)
			movableNode.ProcessMoveNode(0);
		else
			movableNode.ProcessMoveNode((int)(movement * movementMul));
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
            traitmovement += traitList[i].move;
            traitWork += traitList[i].work;
        }

        maxHp = defaultMaxHp + traitMaxHp;
        maxMental = defaultMaxMental + traitMaxmental;
        movement = (int)(defaultMovement + traitmovement);
        workSpeed = (int)(defaultWork + traitWork);

        hp += addTrait.hp;
        mental += addTrait.mental;

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

        if(workSpeed <= 0)
        {
            workSpeed = 1;
        }

        /*
        Debug.Log("변경후 체력" + maxHp);
        Debug.Log("변경후 멘탈" + maxMental);
        Debug.Log("변경후 속도" + movement);
        Debug.Log("변경후 작업속도" + workSpeed);
         */

    }

	// skill start

    //레벨업 선처리
    public void PrePromotion() {
        switch (level) { 
            case 2:
                SkillCategory newSkill = SkillManager.instance.GetRandomCategory(2, this.skills).GetCopy();
                this.skills.Add(newSkill);
                break;
            default:
                return;
        }
    }

    public void Promotion(SkillCategory target) {
        SkillCategory temp = null;

        if (CheckSkillContains(target, ref temp))
        {
            //레벨업
            temp.SkillLevelUp();
            return;
        }
        //새 스킬
        temp = target.GetCopy();
        skills.Add(temp);
        /*
        switch (this.level) { 
            case 1://1->2
                
                break;
            case 2://2->3
                if (CheckSkillContains(target, ref temp)) { 
                    //레벨업
                    temp.SkillLevelUp();
                }
                //새 스킬
                temp = target.GetCopy();
                skills.Add(temp);
                break;
            case 3://3->4
                return;
            case 4://4->5
                return;
            default:
                return;
        }*/
        //level up
    }

    private bool CheckSkillContains(SkillCategory check, ref SkillCategory outTarget) {
        bool output = false;
        foreach (SkillCategory skill in skills) {
            if (skill.name.Equals(check.name)) {
                outTarget = skill;
                output = true;
                break;
            }
        }

        return output;
    }

    public SkillCategory[] GetSkillCategories() {
        return this.skills.ToArray();
    }

    public SkillCategory GetUniqueSkillCategory(string target) {
        SkillCategory output = null;
        foreach (SkillCategory cat in this.skills) {
            if (cat.name.Equals(target)) {
                output = cat;
                break;
            }
        }
        return output;
    }

    public void AddNewCategory(int tier) {
        SkillCategory newCat = SkillManager.instance.GetRandomCategory(tier, skills);
        if (newCat == null) {
            Debug.Log("Add new Skill category Error");
        }
        this.skills.Add(newCat);
        
    }

    public void AddNewCategory(SkillCategory target) {
        SkillCategory newCat = SkillManager.instance.GetCategoryByName(target.name);
        if (newCat == null)
        {
            Debug.Log("Add new Skill category Error");
        }
        this.skills.Add(newCat);
    }

    public void SkillLevelUp(string name) {
        SkillCategory target = null;

        foreach (SkillCategory item in skills) {
            if (item.name.Equals(name))
            {
                target = item;
                break;
            }
        }

        if (target == null) {
            Debug.Log("CannotFindTargetSkillCategory");
            return;
        }

        if (!target.SkillLevelUp()) {
            Debug.Log("Level Max");
            return;
        }
    }

    public void promoteSkill(int skillClass)
    {
		/*
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

        }*/

    }
	/*
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
    */

	public void AddSkill(SkillTypeInfo skill)
	{
		skillList.Add (skill);
	}

	public SkillTypeInfo[] GetSkillList()
	{
		return skillList.ToArray ();
	}

    /*
	private SkillTypeInfo[] GetSkillListByType(string type)
	{
		List<SkillTypeInfo> output = new List<SkillTypeInfo>();
		foreach(SkillTypeInfo skill in skillList)
		{
			if (skill.type == type)
				output.Add (skill);
		}
		return output.ToArray ();
	}

	public SkillTypeInfo[] GetDirectSkillList()
	{
		return GetSkillListByType ("direct");
	}
	public SkillTypeInfo[] GetIndirectSkillList()
	{
		return GetSkillListByType ("indirect");
	}
	public SkillTypeInfo[] GetBlockSkillList()
	{
		return GetSkillListByType ("block");
	}
    */

	public bool HasSkill(SkillTypeInfo skill)
	{
		//return directSkill.id == skill.id || indirectSkill.id == skill.id || blockSkill.id == skill.id;
        foreach (SkillCategory cat in skills) { 
            if (cat.list.Contains(skill)){
                return true;
            }
        }
        return false;
	}

	// skill end

    public override void ProcessAction()
    {
        commandQueue.Execute(this);

        if (CurrentPanicAction != null)
        {
            if (state != AgentAIState.PANIC_SUPPRESS_TARGET)
                CurrentPanicAction.Execute();
        }
        else if (state == AgentAIState.IDLE)
        {
            if (waitTimer <= 0)
            {
                //MovableNode.MoveToNode(MapGraph.instance.GetSepiraNodeByRandom(currentSefira));
				MoveToNode(MapGraph.instance.GetSepiraNodeByRandom(currentSefira));
                waitTimer = 1.5f + Random.value;
            }
        }
            /*
        else if (state == AgentAIState.MANAGE)
        {
            if (MovableNode.GetCurrentEdge() == null && MovableNode.GetCurrentNode() != target.GetWorkspaceNode())
            {
                //MoveToCreatureRoom(target);
                //commandQueue.SetAgentCommand(AgentCommand.MakeMove(target.GetWorkspaceNode()));
            }
        }
             */
        else if (state == AgentAIState.SUPPRESS_CREATURE)
        {
            // WorkEscapedCreature에서 실행
        }
        else if (state == AgentAIState.SUPPRESS_WORKER)
        {
        }
        else if (state == AgentAIState.DEAD)
        {

        }
		else if(state == AgentAIState.CANNOT_CONTROLL)
		{
			if (unconAction != null)
			{
				unconAction.Execute ();
			}
		}
        waitTimer -= Time.deltaTime;
    }


    /**
     * state 관련 함수들
     **/
    public AgentAIState GetState()
    {
        return state;
    }

	public WorkerCommand GetCurrentCommand()
	{
		return commandQueue.GetCurrentCmd ();
	}
    public AgentCmdType GetCurrentCommandType()
    {
        WorkerCommand cmd = commandQueue.GetCurrentCmd();
        if (cmd == null)
            return AgentCmdType.NONE;
        return cmd.type;
    }

	/*
	public void MoveToNode(MapNode node)
	{
		commandQueue.SetAgentCommand(WorkerCommand.MakeMove(node));
	}

	public void MoveToMovable(MovableObjectNode node)
	{
		commandQueue.SetAgentCommand(WorkerCommand.MakeMove(node));
	}
	*/

	public virtual void ClearUnconCommand()
	{
		if (state == AgentAIState.CANNOT_CONTROLL)
		{
			commandQueue.Clear ();
		}
	}

	public void FollowMovable(MovableObjectNode node)
	{
		commandQueue.SetAgentCommand (WorkerCommand.MakeFollowAgent (node));
	}

	public void PursueAgent(AgentModel agent)
	{
		state = AgentAIState.PANIC_VIOLENCE;
		commandQueue.SetAgentCommand (WorkerCommand.MakePanicPursueAgent (agent));
	}

	public void PursueUnconAgent(WorkerModel agent)
	{
		commandQueue.SetAgentCommand (WorkerCommand.MakeUnconPursueAgent (agent));
	}

    public void AttackedByCreature()
    {
        state = AgentAIState.CAPTURE_BY_CREATURE;

        commandQueue.SetAgentCommand(WorkerCommand.MakeCaptureByCreatue());
        movableNode.StopMoving();
        Notice.instance.Send(NoticeName.MakeName(NoticeName.ChangeAgentState, instanceId.ToString()));
    }
    //public void Working(CreatureModel target, UseSkill action)

	public void ManageCreature(CreatureModel target, SkillTypeInfo skill)
	{
		if (HasSkill (skill) == false)
		{
			Debug.LogError ("ManagerCreature >> invalid skill");
		}
		state = AgentAIState.MANAGE;
		this.target = target;
		commandQueue.Clear ();
		commandQueue.AddFirst(WorkerCommand.MakeMove(target.GetWorkspaceNode()));
		commandQueue.AddLast(WorkerCommand.MakeManageCreature(target, this, skill));

        //send Message to work slot(SelectWorkAgentWindow)
        object[] sendMessage = new object[3];
        sendMessage[0] = this;
        sendMessage[1] = target;
        sendMessage[2] = skill;
        Notice.instance.Send(NoticeName.ReportAgentSuccess, sendMessage);

	}
	public void ObserveCreature(CreatureModel target)
	{
		state = AgentAIState.OBSERVE;

		this.target = target;
		commandQueue.Clear ();
		commandQueue.AddFirst(WorkerCommand.MakeMove(target.GetWorkspaceNode()));
		commandQueue.AddLast(WorkerCommand.MakeObserveCreature(target));
	}


    public void Working(CreatureModel target)
    {
        state = AgentAIState.MANAGE;
        commandQueue.Clear();
        commandQueue.AddFirst(WorkerCommand.MakeMove(target.GetWorkspaceNode()));
        commandQueue.AddLast(WorkerCommand.MakeWorking(target));
        this.target = target;
        //base.MoveToCreatureRoom(target);
        Notice.instance.Send(NoticeName.MakeName(NoticeName.ChangeAgentState, instanceId.ToString()));
    }

	public void ReturnCreature(CreatureModel target)
    {
        state = AgentAIState.RETURN_CREATURE;
        commandQueue.SetAgentCommand(WorkerCommand.MakeReturnCreature(target));
        Notice.instance.Send(NoticeName.MakeName(NoticeName.ChangeAgentState, instanceId.ToString()));
    }
    public void FinishWorking()
    {
        //UI AgentWorkPanel 초상화 지우기
		if (state != AgentAIState.MANAGE && state != AgentAIState.OBSERVE)
			return;
        Notice.instance.Send(NoticeName.MakeName(NoticeName.WorkEndReport, instanceId.ToString()));
		StopAction ();
    }
	public void StopAction()
	{
		state = AgentAIState.IDLE;
		commandQueue.Clear();
		//AgentCommand cmd = GetCurrentCommand();
		this.target = null;
		Notice.instance.Send(NoticeName.MakeName(NoticeName.ChangeAgentState, instanceId.ToString()));
	}
    public void UpdateStateIdle()
    {
        state = AgentAIState.IDLE;
        commandQueue.Clear();
        this.target = null;
        Notice.instance.Send(NoticeName.MakeName(NoticeName.ChangeAgentState, instanceId.ToString()));
    }

    // 패닉 관련  start

    /// <summary>
    /// 다른 직원을 공격하던 것을 중지합니다.
    /// AttackAgentByAgent.cs 에서 사용합니다.
    /// </summary>
    public void StopPanicAttackAgent()
    {
        state = AgentAIState.IDLE;
        commandQueue.Clear();
        Notice.instance.Send(NoticeName.MakeName(NoticeName.ChangeAgentState, instanceId.ToString()));
    }

	/*
    public void StopSuppress()
    {
        state = AgentAIState.IDLE;
        commandQueue.Clear();
        Notice.instance.Send(NoticeName.MakeName(NoticeName.ChangeAgentState, instanceId.ToString()));
    }
    */

	public void FinishSuppress()
	{
		if (state == AgentAIState.SUPPRESS_WORKER || state == AgentAIState.SUPPRESS_CREATURE)
		{
			state = AgentAIState.IDLE;
		}
	}


	public void OpenIsolateRoom(CreatureModel targetCreature)
    {
        state = AgentAIState.OPEN_ISOLATE;
		commandQueue.SetAgentCommand(WorkerCommand.MakeOpenRoom(targetCreature));
        Notice.instance.Send(NoticeName.MakeName(NoticeName.ChangeAgentState, instanceId.ToString()));
    }

	public void SuppressCreature(CreatureModel target, SuppressAction suppressAction)
	{
		state = AgentAIState.SUPPRESS_CREATURE;

		commandQueue.SetAgentCommand(WorkerCommand.MakeSuppressCreature(target, suppressAction));
		this.target = target;
		//MoveToCreture(target);
		Notice.instance.Send(NoticeName.MakeName(NoticeName.ChangeAgentState, instanceId.ToString()));
	}

	public void StartSuppressAgent(WorkerModel targetWorker, SuppressAction suppressAction, SuppressType supType)
    {
        state = AgentAIState.SUPPRESS_WORKER;
		commandQueue.SetAgentCommand(WorkerCommand.MakeSuppressWorking(targetWorker, suppressAction, supType));
        this.targetWorker = targetWorker;
        Notice.instance.Send(NoticeName.MakeName(NoticeName.ChangeAgentState, instanceId.ToString()));
    }

    public void PanicSuppressed()
    {
        //state = AgentCmdState.PANIC_SUPPRESS_TARGET;
        movableNode.StopMoving();
        Notice.instance.Send(NoticeName.MakeName(NoticeName.ChangeAgentState, instanceId.ToString()));
    }

	public override void OnHitByWorker(WorkerModel worker)
	{
		//if(agentLifeValue == 1)
		{
			if (state == AgentAIState.MANAGE || state == AgentAIState.OBSERVE)
			{
				FinishWorking ();
			}
			else if(state == AgentAIState.IDLE)
			{
				SuppressAction sa = new SuppressAction (this);
				sa.weapon = SuppressAction.Weapon.STICK;
				if(worker.IsPanic())
					StartSuppressAgent (worker, sa, SuppressType.PANIC);
				else
					StartSuppressAgent (worker, sa, SuppressType.UNCONTROLLABLE);
			}
		}
	}
	public override void OnHitByCreature(CreatureModel creature)
	{
		
	}

	public override void LoseControl()
	{
		state = AgentAIState.CANNOT_CONTROLL;
		commandQueue.Clear ();
	}

	public override void GetControl()
	{
		if (state == AgentAIState.CANNOT_CONTROLL)
		{
			Debug.Log ("get Control....");
			state = AgentAIState.IDLE;
			commandQueue.Clear ();
		}
	}

	public override void SetUncontrollableAction(UncontrollableAction uncon)
	{
		unconAction = uncon;

		if (unconAction != null)
			unconAction.Init ();
	}

	public void OnClick()
	{
		if (unconAction != null) {
			unconAction.OnClick ();
		}
	}

	public override void OnDie()
	{
		if (unconAction != null) {
			unconAction.OnDie ();
		}
	}

    /// <summary>
    /// Set AgentAIState to IDLE
    /// </summary>
    public void FinishOpenIolateRoom()
    {
        if (state == AgentAIState.OPEN_ISOLATE)
        {
            state = AgentAIState.IDLE;
        }
    }

	public void FinishPursueAgent()
	{
		if (state == AgentAIState.PANIC_VIOLENCE)
		{
			state = AgentAIState.IDLE;
		}
	}

	public void FinishReturnCreature()
	{
		if(state == AgentAIState.RETURN_CREATURE)
		{
			state = AgentAIState.IDLE;
		}
	}

    // panic 관련 end

    // state 관련 함수들 end


	/*
	public void ResetAttackDelay()
	{
		attackDelay = 4.0f;
	}
	*/

	// method about managing
	public float GetSuccessProb(SkillTypeInfo skill)
	{



		// 가치관, 등급 고려
		return 0.5f + 0.2f;
	}

	public float GetEvasionProb()
	{
		return 0.1f;
	}

	public float GetEnergyAbility(SkillTypeInfo skill)
	{
		return 30f;
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
        /*
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
        */
        waitTimer = 0;
        Notice.instance.Send(NoticeName.ChangeAgentSefira, this, old);
    }

    public void Panic()
    {
		panicValue = 4;

        CurrentPanicAction = new PanicReady(this);
    }
    
    public override void PanicReadyComplete()
    {
		//CurrentPanicAction = new PanicRoaming (this);
		//CurrentPanicAction = new PanicOpenIsolate(this);
		CurrentPanicAction = new PanicViolence(this);
		return;
        // 바꿔야 함
        switch (agentLifeValue)
        {
        case 1:
            CurrentPanicAction = new PanicRoaming(this);
            break;
        case 2:
            CurrentPanicAction = new PanicSuicideExecutor(this);
            break;
        case 3:
            CurrentPanicAction = new PanicViolence(this);
            break;
		case 4:
			CurrentPanicAction = new PanicOpenIsolate (this);
            break;
        }
    }
    
    public override void StopPanic()
    {
		state = AgentAIState.IDLE;
        CurrentPanicAction = null;
    }

	public override bool IsPanic()
	{
		return CurrentPanicAction != null;
	}

	public override void EncounterCreature()
	{
		if(state != AgentAIState.SUPPRESS_CREATURE && state != AgentAIState.ENCOUNTER_PANIC_WORKER)
			state = AgentAIState.ENCOUNTER_CREATURE;
	}

	// motion


    public void Die()
    {
        string narration = this.name + " (이)가 사망했습니다.";

        Debug.Log("사망");

        Notice.instance.Send("AddSystemLog", narration);
        Notice.instance.Send("AgentDie", this);

        this.hp = 0;
        //this.state = AgentCmdState.DEAD;
        
        //AgentManager.instance.RemoveAgent(this);
        //AgentLayer.currentLayer.GetAgent(this.instanceId).DeadAgent();
    }

    public string LifeStyle() {
        string temp = null;
        /*
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
        */

        switch (agentLifeValue)
        {
            case 1:
                temp = "Rationalist";
                break;
            case 2:
                temp = "Optimist";
                break;
            case 3:
                temp = "Principlist";
                break;
            case 4:
                temp = "Pacifist";
                break;
        }
        return temp;
    }

    public void calcLevel() {
        int healthPoint, mentalPoint, workPoint, speedPoint;
        ValueInfo average = ValueInfo.getAverage();
        healthPoint = calc(hp, average.hp);
        mentalPoint = calc(mental, average.mental);
        workPoint = calc(workSpeed, average.workSpeed);
        speedPoint = calc(movement, average.movementSpeed);

        levelSetting = new ValueInfo(healthPoint, mentalPoint, workPoint, speedPoint);
        setSprite();
    }

    public void MakeAccessoryByTraits() {
        List<TraitTypeInfo> accessoryList = new List<TraitTypeInfo>();
        foreach (TraitTypeInfo trait in this.traitList) {
            if (trait.haveImg)
            {
                accessoryList.Add(trait);
            }
            else continue;
        }
        //Debug.Log(AgentLayer.currentLayer.GetAgent(this.instanceId));
        AgentLayer.currentLayer.GetAgent(this.instanceId).MakeAccessory(accessoryList);
        //call agentUnit to make accsseories
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
        string loc = "UIResource/Icons/Icons";
        Sprite[] icons = Resources.LoadAll<Sprite>(loc);

        for (int i = 0; i < StatusSprites.Length; i++) {
            Debug.Log((i * 3 + levelSetting.stats[i]).ToString());
            StatusSprites[i] = icons[i * 3 + levelSetting.stats[i]];
        }
        /*

        for (int i = 0; i < StatusSprites.Length; i++) {
            string fullpath = loc + i + levelSetting.stats[i];
            StatusSprites[i] = ResourceCache.instance.GetSprite(fullpath);
        }
        */
        for (int i = 0; i < WorklistSprites.Length; i++) {
            string fullpath = loc + "Work_" + i;
            WorklistSprites[i] = ResourceCache.instance.GetSprite(fullpath);
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

    public static void SetPortraitSprite(AgentModel target, Sprite face, Sprite hair)
    {
        Debug.Log("In PortraitSetting");
        face = target.tempFaceSprite;
        hair = target.tempHairSprite;
    }

    public static string GetLevelGradeText(AgentModel target) {
        switch (target.level) { 
            case 1:
                return "I";
            case 2:
                return "II";
            case 3:
                return "IIV";
            case 4:
                return "IV";
            case 5:
                return "V";
            default:
                return "I";
        }
    }
}
