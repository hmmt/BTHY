using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Vector2Serializer
{
    public float x;
    public float y;

    public Vector2Serializer()
    {
    }

    public Vector2Serializer(Vector2 v2)
    {
        Fill(v2);
    }

    public void Fill(Vector2 v2)
    {
        x = v2.x;
        y = v2.y;
    }

    public Vector3 V2 { get { return new Vector2(x, y); } set { Fill(value); } }
}

public enum CreatureEscapeType { 
    ATTACKWORKER,
    FACETOSEFIRA,
    WANDER
}

public class ObserveInfo
{
	public int successCount = 0;
	public int failureCount = 0;
	public int attackCount = 0;
	public int escapeCount = 0;
	public int specialAttackCount = 0;

	public int observationFailureCount = 0;
}

// 
[System.Serializable]
public class CreatureModel : UnitModel, IObserver
{
    
    public int instanceId;

	CreatureCommandQueue commandQueue;

	//public string escapeType = "attackWorker";
    public CreatureEscapeType escapeType = CreatureEscapeType.ATTACKWORKER;

	// temp for proto
	public float manageDelay = 0;

	// lock
	public int targetedCount = 0;

	// buf
	public float energyChangeTime;
	public float energyChangeAmount;
	public float energyChangeElapsedTime;
	//public float bufFeelingAddRate; // per second

	public float attackDelay = 0;

	// for escape
	public int hp;

    // 메타데이터
    public CreatureTypeInfo metaInfo;
    
    public long metadataId; // metaInfo.id

    public Vector2 basePosition;
    //public Vector2 position;

    //?纂삐도감 완성도
    public int observeProgress = 0;
	public ObserveInfo observeInfo;

	public float energyPoint = 100;
	//public float feelingsPoint;

    //환상체 나레이션 저장 List
    public List<string> narrationList;

    // 이하 save 프錘않는 데이터들

    public CreatureState state = CreatureState.WAIT;
    private UseSkill _currentSkill = null;
    public UseSkill currentSkill {
        get {
            if (_currentSkill != null)
            {
               // Debug.Log("Getting UseSKill" + _currentSkill.skillTypeInfo.id);
            }
            return _currentSkill;
        }
        set {
            //Debug.Log("Setting UseSkill" + value);
            _currentSkill = value;
        }
    }

    public CreatureBase script;

    public MovableObjectNode lookAtTarget;

    // 관찰관련 조건 변수 추가
    public float genEnergyCount=0;
    public int workCount=0;
    public int observeCondition = 0;

    // 세피라 변수 (TODO: 변수명 이상함)
    public string sefiraNum;
    public Sefira sefira;
    // 세피라에 직원 없을 시 발동됨
    public bool sefiraEmpty=false;

    //환상체 기분 수치 관련
    public float feeling { get; private set; }//currentFeeling

    // graph
	public string entryNodeId;

    private MapNode workspaceNode;

    private MapNode roomNode;
	private MapNode customNode;
    
    public Dictionary<string, object> GetSaveData()
    {
        Dictionary<string, object> output = new Dictionary<string, object>();

        output.Add("instanceId", instanceId);
        
        output.Add("metadataId", metadataId);
        output.Add("entryNodeId", entryNodeId);

        output.Add("observeProgress", observeProgress);

        output.Add("basePosition", new Vector2Serializer(basePosition));

        output.Add("narrationList", narrationList);

        return output;
    }

    /**
     * 환상체 데이터를 dictionary로부터 로드합니다.
     * 메타데이터 정보를 적용하려면 CreatureManager.BuildCreatureModel()를 사용해야 합니다.
     * 
     */
    public void LoadData(Dictionary<string, object> dic)
    {
        GameUtil.TryGetValue(dic, "instanceId", ref instanceId);

        GameUtil.TryGetValue(dic, "metadataId", ref metadataId);
        GameUtil.TryGetValue(dic, "entryNodeId", ref entryNodeId);

        GameUtil.TryGetValue(dic, "observeProgress", ref observeProgress);

        Vector2Serializer v2 = new Vector2Serializer();
        GameUtil.TryGetValue(dic, "basePosition", ref v2);
        position = basePosition = v2.V2;
    }

    public CreatureModel(int instanceId)
    {
        movableNode = new MovableObjectNode(this);
		commandQueue = new CreatureCommandQueue (this);

		observeInfo = new ObserveInfo ();

		movableNode.AddUnpassableType (PassType.SHIELDBEARER);

        this.instanceId = instanceId;
        narrationList = new List<string>();
    }

    public CreatureFeelingState GetFeelingState()
    {
		if (energyPoint >= 110)
			return CreatureFeelingState.GOOD;
		else if (energyPoint >= 90)
			return CreatureFeelingState.NORM;
		else
			return CreatureFeelingState.BAD;
		//return CreatureFeelingState.GOOD;
		/*
        CreatureFeelingState feelingState = CreatureFeelingState.BAD;
        float sectionMax = 0;

        foreach (FeelingSectionInfo info in metaInfo.feelingSectionInfo)
        {
            if (sectionMax <= info.section && feeling >= info.section)
            {
                sectionMax = info.section;
                feelingState = info.feelingState;
            }
        }

        return feelingState;
        */
    }
	/*
    public FeelingSectionInfo GetCurrentFeelingSectionInfo()
    {
        FeelingSectionInfo output = metaInfo.feelingSectionInfo[0];
        float sectionMax = 0;

        foreach (FeelingSectionInfo info in metaInfo.feelingSectionInfo)
        {
            if (sectionMax <= info.section && feeling >= info.section)
            {
                sectionMax = info.section;
                output = info;
            }
        }

        return output;
    }
    */

    public float GetEnergyTick()
    {
		EnergyGenInfo selected = metaInfo.energyGenInfo [0];
		foreach (EnergyGenInfo info in metaInfo.energyGenInfo)
		{
			//if (energyPoint <= info.upperBound)
			if(feeling <= info.upperBound)
			{
				selected = info;
			}
			else
			{
				break;
			}	
		}
		return selected.genValue;
		/*
        float energyDummy = 0;
        float sectionMax = 0;

        foreach (FeelingSectionInfo info in metaInfo.feelingSectionInfo)
        {
            if (sectionMax <= info.section && feeling >= info.section)
            {
                energyDummy = info.energyGenValue;
                sectionMax = info.section;
            }
        }

        return energyDummy + energyDummy * 0.6f * ((float)(observeProgress) / 5);
        */
    }

    private static int counter = 0;

    public Vector3 GetCurrentViewPosition()
    {
        return movableNode.GetCurrentViewPosition();
    }

    public void SetRoomNode(MapNode node)
    {
        roomNode = node;
    }

	public void SetCustomNode(MapNode node)
	{
		customNode = node;
	}

    public void SetCurrentNode(MapNode node)
    {
        movableNode.SetCurrentNode(node);
    }
    public MapNode GetCurrentNode()
    {
        return movableNode.GetCurrentNode();
    }
    public MapEdge GetCurrentEdge()
    {
        return movableNode.GetCurrentEdge();
    }

    public void SetWorkspaceNode(MapNode node)
    {
        workspaceNode = node;
    }

    public MapNode GetWorkspaceNode()
    {
        return workspaceNode;
    }

    public MapNode GetEntryNode()
    {
        return MapGraph.instance.GetNodeById(entryNodeId);
    }

	public MapNode GetCustomNode()
	{
		return customNode;
	}

    public MovableObjectNode GetMovableNode()
    {
        return movableNode;
    }

	public UnitDirection GetDirection()
	{
		return movableNode.GetDirection ();
	}

    public void UpdateFeeling()
    {
		if (IsWorkingState())
			return;
        if (script != null) {
            if (!script.AutoFeelingDown()) {
                return;
            }
        }
        //if (Random.value < metaInfo.feelingDownProb)
        {
			energyPoint += metaInfo.energyPointChange;
            SubFeeling(metaInfo.feelingDownValue);

            Notice.instance.Send("UpdateCreatureState_" + instanceId);
        }
    }

	public bool IsWorkingState()
	{
		return state == CreatureState.WORKING || state == CreatureState.WORKING_SCENE || state == CreatureState.OBSERVE;
	}

    public void OnFixedUpdate()
    {
		if(attackDelay > 0)
			attackDelay -= Time.deltaTime;
		if (manageDelay > 0)
			manageDelay -= Time.deltaTime;

		if (GetCurrentNode () == roomNode)
			movableNode.SetDirection (UnitDirection.RIGHT);

		commandQueue.Execute (this);

		CreatureCommand cmd = commandQueue.GetCurrentCmd ();
		if (cmd != null && cmd.isMoving) {
			SendAnimMessage ("Move");
		}

		if (energyChangeTime > energyChangeElapsedTime)
		{
			ProcessWorkingBuf ();
		}
		 
        if (script != null)
        {
            script.OnFixedUpdate(this);
        }
		/*
        if (state == CreatureState.ESCAPE_RETURN)
        {
            if (movableNode.GetCurrentNode() == workspaceNode)
            {
                state = CreatureState.WAIT;
            }
            else
            {
                //if (movableNode.IsMoving() == false)
				if(commandQueue.GetCurrentCmd() == null)
                {
                    MoveToNode(workspaceNode);
                }
            }
        }
        else
        */
        if (state == CreatureState.ESCAPE)
        {
            OnEscapeUpdate();
        }
		else if(state == CreatureState.SUPPRESSED)
		{
			
		}
        else if (state == CreatureState.WAIT)
        {
        }
        movableNode.ProcessMoveNode(metaInfo.speed);
    }

    public void OnEscapeUpdate()
    {
        //if (movableNode.IsMoving() == false)
        if (escapeType == CreatureEscapeType.ATTACKWORKER)
        {
            if (commandQueue.GetCurrentCmd() == null)
            {
                //movableNode.MoveToNode(MapGraph.instance.GetCreatureRoamingPoint());
                MoveToNode(MapGraph.instance.GetCreatureRoamingPoint());
            }
            else
            {
                AgentModel[] detectedAgents = AgentManager.instance.GetNearAgents(movableNode);

                if (detectedAgents.Length > 0)
                {
                    PursueWorker(detectedAgents[0]);
                }
            }
        }
        else {
            if (script != null && script.hasUniqueEscape()) {
                script.UniqueEscape();
            }
        }

		PassageObjectModel currentPassage = movableNode.GetPassage ();
		if(currentPassage != null)
		{
			foreach (AgentModel agent in AgentManager.instance.GetAgentList())
			{
				if (agent.GetMovableNode ().GetPassage () == currentPassage)
				{
					if (agent.GetState () != AgentAIState.ENCOUNTER_CREATURE)
					{
						agent.EncounterCreature ();
					}
				}
			}

			foreach (OfficerModel officer in OfficerManager.instance.GetOfficerList())
			{
				if (officer.GetMovableNode ().GetPassage () == currentPassage)
				{
					if (officer.GetState () != OfficerAIState.PANIC)
					{
						officer.EncounterCreature ();
					}
				}
			}
		}
		
		/*
		if(commandQueue.GetCurrentCmd() == null)
        {
            //movableNode.MoveToNode(MapGraph.instance.GetCreatureRoamingPoint());
			MoveToNode(MapGraph.instance.GetCreatureRoamingPoint());
        }
        else
        {
            AgentModel[] detectedAgents = AgentManager.instance.GetNearAgents(movableNode);

            if (detectedAgents.Length > 0)
            {
                movableNode.StopMoving();
                AttackAgent.Create(detectedAgents[0], this);
            }
        }
        */
    }

    public void OnNotice(string notice, params object[] param)
    {
        if (notice == NoticeName.CreatureFeelingUpdateTimer)
        {
            UpdateFeeling();
        }
        else if (notice == NoticeName.FixedUpdate)
        {
            OnFixedUpdate();
        }
    }

    public void ShowTextOutside(CreatureOutsideTextLayout layoutType, string textKey)
    {

    }

    public void ShowNarrationText(string narrationKey, params string[] param)
    {
        string narrationFormat;
        if (metaInfo.narrationTable.TryGetValue(narrationKey, out narrationFormat))
        {
            string narration = TextConverter.GetTextFromFormatText(narrationFormat, param);

            narrationList.Add(narration);
            Notice.instance.Send("AddNarrationLog", narration, this);
        }
    }

    public void ShowProcessNarrationText(string narrationKey, params string[] param)
    {
        string narrationFormat;
        if (metaInfo.narrationTable.TryGetValue(narrationKey, out narrationFormat))
        {
            string[] narration = TextConverter.GetTextFromFormatProcessText(narrationFormat, param);
            string selected = "";

            if (observeProgress < 40)
            {
                selected = narration[0];
            }

            else if (40 <= observeProgress && observeProgress < 80)
            {
                selected = narration[1];
            }

            else if (80 <= observeProgress && observeProgress < 100)
            {
                selected = narration[2];
            }

            else if (observeProgress <= 100)
            {
                selected = narration[3];
            }
            narrationList.Add(selected);
            Notice.instance.Send("AddNarrationLog", selected, this);
        }
    }

	private void ProcessWorkingBuf()
	{
		float delta = Time.deltaTime;
		if (energyChangeElapsedTime + Time.deltaTime > energyChangeTime)
			delta = energyChangeTime - energyChangeElapsedTime;
		energyChangeElapsedTime += delta;

		energyPoint += energyChangeAmount / (energyChangeTime / delta);
		Notice.instance.Send("UpdateCreatureState_" + instanceId);
	}
	private void GenerateEnergy()
	{
	}
	/*
	public void SetFeelingBuf(float time, float feelingAmount)
	{
		bufRemainingTime = time;
		bufFeelingAddRate = feelingAmount / time;
	}
	*/

	public void SetEnergyChange(float time, float energyChangeAmount)
	{
		energyChangeTime = time;
		this.energyChangeAmount = energyChangeAmount;
		energyChangeElapsedTime = 0;
	}

    public void AddFeeling(float value)
    {
        feeling += value;
        if (feeling > metaInfo.feelingMax)
            feeling = metaInfo.feelingMax;

        Notice.instance.Send("UpdateCreatureState_" + instanceId);
    }

    public void SubFeeling(float value)
    {
        feeling = Mathf.Max(feeling - value, 0);
        Notice.instance.Send("UpdateCreatureState_" + instanceId);
    }

	public void SubEnergyPoint(float value)
	{
		energyPoint = Mathf.Max(energyPoint - value, 0);
		Notice.instance.Send("UpdateCreatureState_" + instanceId);
	}

    public void DangerFeeling()
    {
        Debug.Log("세피璨졶직원 없다" + instanceId);
    }

    public void StartEscapeWork()
    {
        state = CreatureState.ESCAPE_WORK;
    }
	public void StopEscapeWork()
	{
		state = CreatureState.ESCAPE;
	}

	public float GetAttackProb()
	{
		return 0.3f;
	}

	public int GetPhysicsDmg()
	{
		return metaInfo.physicsDmg;
	}
	public int GetMentalDmg()
	{
		return metaInfo.mentalDmg;
	}
	public CreatureAttackType GetAttackType()
	{
		return metaInfo.attackType;
	}

	public void ResetAttackDelay()
	{
		attackDelay = 4.0f;
	}

	public void TakeSuppressDamage(int damage)
	{
		if (hp > 0) {
			hp -= damage;
           			//Debug.Log ("Creature  take suppress damage.. current HP : " + hp);

		}
		if ((state == CreatureState.ESCAPE || state == CreatureState.ESCAPE_PURSUE)
			&& hp <= 0)
		{
			state = CreatureState.SUPPRESSED;
			commandQueue.Clear ();
		}
	}

    public bool IsPreferSkill(SkillTypeInfo skillTypeInfo)
    {
		/*
        float bonus;
        return GetPreferSkillBonus(skillTypeInfo, out bonus);
        */
		return true;
    }

    public bool IsRejectSkill(SkillTypeInfo skillTypeInfo)
    {
		/*
        float bonus;
        return GetRejectSkillBonus(skillTypeInfo, out bonus);
        */
		return false;
    }

    public string GetArea()
    {
        return workspaceNode.GetAreaName();
    }

    /**
     * 환상체가 wait 상태일 때만 탈출됩니다.
     * 
     */
    public void Escape()
    {
        if (state == CreatureState.WAIT)
        {
			Debug.Log ("CreatureModel >>> Try Escape ");
			//hp = 5;
			hp = 500;
            state = CreatureState.ESCAPE;
        }
    }

    public string GetObserveText()
    {
        string output = "";
        int level = Mathf.Clamp(observeProgress, 0, metaInfo.observeRecord.Count-1);
        for (int i = 0; i <= level; i++)
        {
            output += metaInfo.observeRecord[i];
        }
        return output;
    }

    //환상체 관찰 조건 뻥콥함수
    /*
    public void CheckObserveCondition()
    {
        if (workCount ==1 && observeCondition == 0)
        {
            Debug.Log("관찰 컨디션 1단계로 갱신");
            observeCondition = 1;
        }

        else if (workCount == 5 && genEnergyCount >= 20 && observeCondition == 1)
        {
            Debug.Log("관찰 컨디션 2단계로 갱신");
            observeCondition = 2;
        }

        else if (workCount == 7 && genEnergyCount >= 40 && observeCondition == 2)
        {
            Debug.Log("관찰 컨디션 3단계로 갱신");
            observeCondition = 3;
        }

        else if (workCount == 10 && genEnergyCount >= 100 && observeCondition == 3)
        {
            Debug.Log("관찰 컨퉜계로 갱신");
            observeCondition = 4;
        }
        else
        {
            Debug.Log("환상체 관찰조건 쪽 코드가 이상함");
        }
    }
    */
    //관찰 가능하다고 알림을 보내는 함수
    public bool NoticeDoObserve()
    {

        if ( workCount < 5 && workCount >= 1 && observeCondition == 0)
        {
            Debug.Log("관찰 컨디션 1단계로 갱신");
            observeCondition = 1;
        }

        else if ( workCount < 7 && workCount >= 5 && genEnergyCount <40 &&genEnergyCount >= 20 && observeCondition == 1)
        {
            Debug.Log("관찰 좁퉜계로 갱신");
            observeCondition = 2;
        }
        else if (workCount <10 && workCount >= 7 && genEnergyCount <100 &&genEnergyCount >= 40 && observeCondition == 2)
        {
            Debug.Log("관찰 컨디션 3단계로 갱신");
            observeCondition = 3;
        }
        else if (workCount >= 10 && genEnergyCount >= 100 && observeCondition == 3)
        {
            Debug.Log("관찰 컨디션 4단계로 갱신");
            observeCondition = 4;
        }
        else
        {
            //Debug.Log("Not sufficient");
        }

        if (observeCondition >= observeProgress + 1)
        {
            return true;
        }

        else
        {
            return false;
        }
    }

	public bool IsTargeted()
	{
		return targetedCount > 0;
	}
	public void AddTargetedCount()
	{
		targetedCount++;
	}
	public void ReleaseTargetedCount()
	{
		targetedCount--;
	}

	public bool IsReady()
	{
		return energyChangeElapsedTime >= energyChangeTime;
	}

	public bool IsEscapable()
	{
		if (script != null)
			return script.IsEscapable ();
		return false;
	}


    // commands

    public void ClearCommand()
    {
       // if(state == CreatureState.ESCAPE_PURSUE)
        commandQueue.Clear();
    }

	public void MoveToNode(MapNode mapNode)
	{
		commandQueue.SetAgentCommand(CreatureCommand.MakeMove(mapNode));
	}

	public void MoveToNode(string targetNodeID)
	{
		commandQueue.SetAgentCommand(CreatureCommand.MakeMove(MapGraph.instance.GetNodeById(targetNodeID)));
	}

	public void PursueWorker(WorkerModel target)
	{
		Debug.Log ("Start pursue .. " + state);
		state = CreatureState.ESCAPE_PURSUE;
		commandQueue.SetAgentCommand (CreatureCommand.MakePursue (target));
	}

    public void AttackWorker(WorkerModel target)
    {
        
    }

	public void FinishReturn()
	{
		if (state == CreatureState.SUPPRESSED_RETURN)
		{
			state = CreatureState.WAIT;
			//MoveToNode (roomNode);
			if (script != null)
				script.OnReturn ();
			SetCurrentNode(roomNode);
		}
	}

	public void SendAnimMessage(string name)
	{
		CreatureUnit unit = CreatureLayer.currentLayer.GetCreature (instanceId);
		if(unit != null)
		{
			CreatureAnimScript animTarget = unit.animTarget;
			if (animTarget != null)
			{
				animTarget.SendMessage (name);
			}
		}
	}

	public CreatureAnimScript GetAnimScript()
	{
		CreatureUnit unit = CreatureLayer.currentLayer.GetCreature (instanceId);
		if(unit != null)
		{
			return unit.animTarget;
		}
		return null;
	}

	public override void InteractWithDoor(DoorObjectModel door)
	{
		base.InteractWithDoor(door);

		commandQueue.AddFirst(CreatureCommand.MakeOpenDoor(door));
	}

	public override void OnStopMovableByShield (AgentModel shielder)
	{
		//shielder


	}

    /*
    public void OnClicked()
    {
        if (state == CreatureState.WAIT)
        {
            //SelectWorkAgentWindow.CreateWindow(this);
            SelectWorkAgentWindow.CreateWindow(room);
            //IsolateRoomStatus.CreateWindow(this);
        }
    }
    */


	public float GetEnergyMax()
	{
		return 200;
	}

    public float GetFeelingPercent() {
        float max = this.metaInfo.feelingMax;
        float current = this.feeling;
        if (max == 0f) return 1f;
        float percent = (current / max) * 100f;

        return percent;
    }


	public void AddSuccessCount()
	{
		observeInfo.successCount++;
	}
	public void AddFailureCount()
	{
		observeInfo.failureCount++;
	}
	public void AddAttackCount()
	{
		observeInfo.attackCount++;
	}
	public void AddEscapeCount()
	{
		observeInfo.escapeCount++;
	}
	public void AddSpecialAttackCount()
	{
		observeInfo.specialAttackCount++;
	}
	public void AddObservationFailureCount()
	{
		observeInfo.observationFailureCount++;
	}

	public void ResetObservationFailureCount()
	{
		observeInfo.observationFailureCount = 0;
	}

	public int GetObservationFailureCount()
	{
		return observeInfo.observationFailureCount;
	}

	public int GetObservationConditionPoint()
	{
		int point = 0;
		point += observeInfo.successCount/30;
		point += observeInfo.failureCount/20;
		point += observeInfo.attackCount/10;
		point += observeInfo.escapeCount;
		point += observeInfo.specialAttackCount;

		return point;
	}

	public bool CanObserve()
	{
		return true;
		int point = GetObservationConditionPoint ();

		switch (observeProgress) {
		case 0:
			if (point >= 5)
				return true;
			break;
		case 1:
			if (point >= 15)
				return true;
			break;
		case 2:
			if (point >= 20)
				return true;
			break;
		case 3:
			if (point >= 30)
				return true;
			break;
		case 4:
			if (point >= 40)
				return true;
			break;
		}
		return false;
	}

	// temp for proto
	public float GetWorkEfficient(SkillTypeInfo skill)
	{
		float value ;
		if (metaInfo.workEfficiency.TryGetValue (skill.id, out value))
		{
			return value;
		}

		return 1;
	}

    public CreatureCommand GetCreatureCurrentCmd() {
        return this.commandQueue.GetCurrentCmd();
    } 
}

