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
public class CreatureModel : IObserver
{
    public int instanceId;

    // 메타데이터
    public CreatureTypeInfo metaInfo;
    public long metadataId; // metaInfo.id

    public Vector2 basePosition;
    public Vector2 position;

    public string baseNodeId;

    //환상체 도감 완성도
    public int observeProgress = 0;

    public float feeling { get; private set; }

    //환상체 나레이션 저장 List
    public List<string> narrationList;

    // 이하 save 되지 않는 데이터들

    public CreatureState state = CreatureState.WAIT;

    public float escapeAttackWait = 0;

    public CreatureBase script;

    public SkillTypeInfo specialSkill; // 이 환상체를 대상으로 할 수 있는 특수스킬


    // 관찰관련 조건 변수 추가
    public float genEnergyCount=0;
    public int workCount=0;
    public int observeCondition = 0;

    // 세피라 변수
    public string sefiraNum;


    // graph
    private MapNode workspaceNode;

    private MapNode roomNode;
    MovableObjectNode movableNode;

    public Dictionary<string, object> GetSaveData()
    {
        Dictionary<string, object> output = new Dictionary<string, object>();

        output.Add("instanceId", instanceId);

        output.Add("metadataId", metadataId);
        output.Add("baseNodeId", baseNodeId);

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
        GameUtil.TryGetValue(dic, "baseNodeId", ref baseNodeId);

        GameUtil.TryGetValue(dic, "observeProgress", ref observeProgress);

        Vector2Serializer v2 = new Vector2Serializer();
        GameUtil.TryGetValue(dic, "basePosition", ref v2);
        position = basePosition = v2.V2;
    }

    public CreatureModel(int instanceId)
    {
        movableNode = new MovableObjectNode();

        this.instanceId = instanceId;
        narrationList = new List<string>();
    }

    public float GetEnergyTick()
    {
        int feelingTick = metaInfo.feelingMax / metaInfo.genEnergy.Length;
        
        float energyDummy = metaInfo.genEnergy[Mathf.Clamp((int)(feeling) / feelingTick, 0, metaInfo.genEnergy.Length - 1)];

        return energyDummy + energyDummy * 0.6f * ((float)(observeProgress)/ 5);
    }

    private static int counter = 0;

    public Vector2 GetCurrentViewPosition()
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

    public MovableObjectNode GetMovableNode()
    {
        return movableNode;
    }

    public void UpdateFeeling()
    {
        if (Random.value < metaInfo.feelingDownProb)
        {
            SubFeeling(metaInfo.feelingDownValue);

            Notice.instance.Send("UpdateCreatureState_" + instanceId);
        }
    }

    public void OnFixedUpdate()
    {
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
                if (movableNode.IsMoving() == false)
                {
                    movableNode.MoveToNode(workspaceNode);
                }
            }
        }
        else if (state == CreatureState.ESCAPE)
        {
            OnEscapeUpdate();
        }
        else if (state == CreatureState.WAIT)
        {
            if (feeling <= 0)
            {
                Escape();
            }
        }
        movableNode.ProcessMoveNode(4);
    }

    public void OnEscapeUpdate()
    {
        if (escapeAttackWait > 0)
            return;
        if (movableNode.IsMoving() == false)
        {
            movableNode.MoveToNode(MapGraph.instance.GetCreatureRoamingPoint());
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

    public bool GetPreferSkillBonus(string type, out float bonus)
    {
        foreach (SkillBonusInfo info in metaInfo.preferList)
        {
            if (info.skillType == type)
            {
                bonus = info.bonus;
                return true;
            }
        }
        bonus = 0;
        return false;
    }

    public bool GetRejectSkillBonus(string type, out float bonus)
    {
        foreach (SkillBonusInfo info in metaInfo.rejectList)
        {
            if (info.skillType == type)
            {
                bonus = info.bonus;
                return true;
            }
        }
        bonus = 0;
        return false;
    }

    public bool IsPreferSkill(string type)
    {
        float bonus;
        return GetPreferSkillBonus(type, out bonus);
    }

    public bool IsRejectSkill(string type)
    {
        float bonus;
        return GetRejectSkillBonus(type, out bonus);
    }

    public string GetArea()
    {
        return workspaceNode.GetAreaName();
    }

    public void Escape()
    {
        state = CreatureState.ESCAPE;
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

    //환상체 관찰 조건 갱신 함수
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
            Debug.Log("환상체 관찰조건 쪽 코드가 이상함");
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

