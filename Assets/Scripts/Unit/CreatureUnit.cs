using UnityEngine;
using System.Collections.Generic;

public class CreatureUnit : MonoBehaviour, IObserver {
   
    //환상체 나레이션 저장 List
    public List<string> narrationList;

	public CreatureTypeInfo metaInfo;

	public CreatureState state = CreatureState.WAIT;

    //환상체 도감 완성도
    public int observeProgress = 0;

	//
	public SpriteRenderer spriteRenderer;
	//

	public int feeling { get; private set; }

	public IsolateRoom room;

	public CreatureBase script;

	public SkillTypeInfo specialSkill; // 이 환상체를 대상으로 할 수 있는 특수스킬

	// path finding2
	private MapNode currentNode;

	private MapEdge currentEdge;
	private int edgeDirection;
	private float edgePosRate; // 0~1

	private MapEdge[] pathList;
	private int pathIndex;

	// graph
	private MapNode workspaceNode;

	private Vector2 GetCurrentViewPosition()
	{
		Vector2 output = transform.localPosition;
		if(currentNode != null)
		{
			Vector2 pos = currentNode.GetPosition();
			output.x = pos.x;
			output.y = pos.y;
		}
		else if(currentEdge != null)
		{
			MapNode node1 = currentEdge.node1;
			MapNode node2 = currentEdge.node2;
			Vector2 pos1 = node1.GetPosition();
			Vector2 pos2 = node2.GetPosition();

			if(edgeDirection == 1)
			{
				output.x = Mathf.Lerp(pos1.x, pos2.x, edgePosRate);
				output.y = Mathf.Lerp(pos1.y, pos2.y, edgePosRate);
			}
			else
			{
				output.x = Mathf.Lerp(pos1.x, pos2.x, 1-edgePosRate);
				output.y = Mathf.Lerp(pos1.y, pos2.y, 1-edgePosRate);
			}
		}
		return output;
	}

	private void UpdateViewPosition()
	{
		transform.localPosition = GetCurrentViewPosition();
	}

	void Awake()
	{

	}

	void Start () {
	}

	void OnEnable()
	{
		Notice.instance.Observe ("CreatureFeelingUpdateTimer", this);
	}
	void OnDiable()
	{
		Notice.instance.Remove ("CreatureFeelingUpdateTimer", this);
	}

	public void SetNode(MapNode node)
	{
		currentNode = node;
	}

	public MapNode GetNode()
	{
		return currentNode;
	}

	public void SetWorkspaceNode(MapNode node)
	{
        workspaceNode = node;
	}

	public MapNode GetWorkspaceNode()
	{
		return workspaceNode;
	}

	public void UpdateFeeling()
	{
		if (Random.value < metaInfo.feelingDownProb)
		{
			//feeling -= metaInfo.feelingDownValue;
			SubFeeling(metaInfo.feelingDownValue);

			//Notice.instance.Send("UpdateCreatureState_" + gameObject.GetInstanceID());
		}
	}

	public void OnNotice(string notice, params object[] param)
	{
		if(notice == "CreatureFeelingUpdateTimer")
		{
			UpdateFeeling();
		}
	}

	void FixedUpdate()
	{
        if (script != null)
        {
            script.FixedUpdate(this);
        }
		UpdateViewPosition();
	}

	public void ShowTextOutside(CreatureOutsideTextLayout layoutType, string textKey)
	{

	}

	public void ShowNarrationText(string narrationKey, params string[] param)
	{
		string narrationFormat;
		if(metaInfo.narrationTable.TryGetValue (narrationKey, out narrationFormat))
		{
			string narration = TextConverter.GetTextFromFormatText(narrationFormat, param);
            narrationList.Add(narration);
			Notice.instance.Send("AddNarrationLog", narration,this);
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

            else if (observeProgress <=100)
            {
                selected = narration[3];
            }
             narrationList.Add(selected);
            Notice.instance.Send("AddNarrationLog", selected, this);
        }
    }

    public void PlaySound(string soundKey)
    {
        string soundFilename;
        if (metaInfo.soundTable.TryGetValue(soundKey, out soundFilename))
        {
            SoundEffectPlayer.PlayOnce(soundFilename, transform.position);
        }
    }

	public void AddFeeling(int value)
	{
		feeling += value;
		if (feeling > metaInfo.feelingMax)
			feeling = metaInfo.feelingMax;
        Notice.instance.Send("UpdateCreatureState_" + gameObject.GetInstanceID());
	}

	public void SubFeeling(int value)
	{
		feeling = Mathf.Max(feeling - value, 0);
        Notice.instance.Send("UpdateCreatureState_" + gameObject.GetInstanceID());
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

    public string GetObserveText()
    {
        string output = "";
        int level = Mathf.Clamp(observeProgress, 0, metaInfo.observeList.Length-1);
        for(int i=0; i<=level; i++)
        {
            output += metaInfo.observeList[i];
        }
        return output;
    }

	public void OnClicked()
	{
		if(state == CreatureState.WAIT)
		{
			//SelectWorkAgentWindow.CreateWindow(this);
            SelectWorkAgentWindow.CreateWindow(room);
			//IsolateRoomStatus.CreateWindow(this);
		}
	}
}
