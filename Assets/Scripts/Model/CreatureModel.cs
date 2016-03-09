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

// 
[System.Serializable]
public class CreatureModel : ObjectModelBase, IObserver
{
    public int instanceId;

	MovableObjectNode movableNode;
	CreatureCommandQueue commandQueue;

	public string escapeType = "attackWorker";

	// lock
	public int targetedCount = 0;

	// buf
	public float energyChangeTime;
	public float energyChangeAmount;
	public float energyChangeElapsedTime;
	//public float bufFeelingAddRate; // per second

    // 메타데이터
    public CreatureTypeInfo metaInfo;
    
    public long metadataId; // metaInfo.id

    public Vector2 basePosition;
    //public Vector2 position;

    public string entryNodeId;

    //?纂삐도감 완성도
    public int observeProgress = 0;

    public float feeling { get; private set; }

	public float energyPoint = 100;
	//public float feelingsPoint;

    //환상체 나레이션 저장 List
    public List<string> narrationList;

    // 이하 save 프錘않는 데이터들

    public CreatureState state = CreatureState.WAIT;

    public float escapeAttackWait = 0;

    public CreatureBase script;

    public MovableObjectNode lookAtTarget;

    // 관찰관련 조건 변수 추가
    public float genEnergyCount=0;
    public int workCount=0;
    public int observeCondition = 0;

    // 세피라 변수 (TODO: 변수명 이상함)
    public string sefiraNum;

    // 세피라에 직원 없을 시 발동됨
    public bool sefiraEmpty=false;


    // graph
    private MapNode workspaceNode;

    private MapNode roomNode;

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

        this.instanceId = instanceId;
        narrationList = new List<string>();
    }

    public CreatureFeelingState GetFeelingState()
    {
		return CreatureFeelingState.GOOD;
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
			if (energyPoint <= info.upperBound)
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

    public MovableObjectNode GetMovableNode()
    {
        return movableNode;
    }

    public void UpdateFeeling()
    {
        //if (Random.value < metaInfo.feelingDownProb)
        {
			energyPoint += metaInfo.energyPointChange;
            SubFeeling(metaInfo.feelingDownValue);

            Notice.instance.Send("UpdateCreatureState_" + instanceId);
        }
    }

    public void OnFixedUpdate()
    {
		commandQueue.Execute (this);

		if (energyChangeTime > energyChangeElapsedTime)
		{
			ProcessWorkingBuf ();
		}
		 
        if (escapeAttackWait > 0)
        {
            escapeAttackWait -= Time.deltaTime;
        }
        if (script != null)
        {
            script.OnFixedUpdate(this);
        }

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
        else if (state == CreatureState.ESCAPE)
        {
            OnEscapeUpdate();
        }
        else if (state == CreatureState.WAIT)
        {
        }
        movableNode.ProcessMoveNode(4);
    }

    public void OnEscapeUpdate()
    {
        if (escapeAttackWait > 0)
            return;
        //if (movableNode.IsMoving() == false)
		if (escapeType == "attackWorker")
		{
			if(commandQueue.GetCurrentCmd() == null)
			{
				//movableNode.MoveToNode(MapGraph.instance.GetCreatureRoamingPoint());
				MoveToNode(MapGraph.instance.GetCreatureRoamingPoint());
			}
			else
			{
				AgentModel[] detectedAgents = AgentManager.instance.GetNearAgents(movableNode);

				if (detectedAgents.Length > 0) {
					PursueWorker (detectedAgents [0]);
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

    public void DangerFeeling()
    {
        Debug.Log("세피라에 직원 없다" + instanceId);
    }

    public void StopEscapeAttack()
    {
        state = CreatureState.ESCAPE;
        escapeAttackWait = 2;
    }

    public void StartEscapeWork()
    {
        state = CreatureState.ESCAPE_WORK;
    }
	public void StopEscapeWork()
	{
		state = CreatureState.ESCAPE;
	}

    public void ReturnEscape()
    {
        state = CreatureState.ESCAPE_RETURN;
    }

	public float GetAttackProb()
	{
		return 0.3f;
	}

	public int GetPhysicsDmg()
	{
		return 1;
	}
	public int GetMentalDmg()
	{
		return 1;
	}
	public CreatureAttackType GetAttackType()
	{
		return CreatureAttackType.PHYSICS;
	}

	/*
	// unused
	public bool GetPreferSkillBonus(SkillTypeInfo skillTypeInfo, out float bonus)
	{
		FeelingSectionInfo info = GetCurrentFeelingSectionInfo();
		foreach (SkillBonusInfo bonusInfo in info.preferList)
		{
			if (bonusInfo.skillType == skillTypeInfo.type || bonusInfo.skillId == skillTypeInfo.id)
			{
				bonus = bonusInfo.bonus;
				return true;
			}
		}
		bonus = 0;
		return false;
	}

	// unused
    public bool GetRejectSkillBonus(SkillTypeInfo skillTypeInfo, out float bonus)
    {
        FeelingSectionInfo info = GetCurrentFeelingSectionInfo();
        foreach (SkillBonusInfo bonusInfo in info.rejectList)
        {
            if (bonusInfo.skillType == skillTypeInfo.type || bonusInfo.skillId == skillTypeInfo.id)
            {
                bonus = -bonusInfo.bonus;
                return true;
            }
        }
        bonus = 0;
        return false;
    }
	*/

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
            state = CreatureState.ESCAPE;
        }
    }

    public string GetObserveText()
    {
        string output = "";
        int level = Mathf.Clamp(observeProgress, 0, metaInfo.observeList.Length - 1);
        for (int i = 0; i <= level; i++)
        {
            output += metaInfo.observeList[i];
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
            Debug.Log("관찰 컨디션 4단계로 갱신");
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
            Debug.Log("관찰 컨디션 2단계로 갱신");
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

    // commands

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
		state = CreatureState.ESCAPE_PURSUE;
		commandQueue.SetAgentCommand (CreatureCommand.MakePursue (target));
	}

    public void AttackWorker(WorkerModel target)
    {
        
    }

	public override void InteractWithDoor(DoorObjectModel door)
	{
		base.InteractWithDoor(door);

		commandQueue.AddFirst(CreatureCommand.MakeOpenDoor(door));
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
}

