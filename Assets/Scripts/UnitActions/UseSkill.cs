using UnityEngine;
using System.Collections;

public class UseSkill : ActionClassBase
{

    public int totalTickNum;
    public float tickInterval = 1.0f;
    public float workSpeed;
    public float workProgress;
    //public float totalFeeling;
	/*
    public int goalWork;
    public float elapsedWorkingTime;
    public int currentWork;
    public int updateTick = 5;
    */

	public int successCount;
    public int workCount;

    // skill info
    public SkillTypeInfo skillTypeInfo;

    public ProgressBar progressBar;
    public AgentModel agent;
    public AgentUnit agentView;

    public CreatureModel targetCreature;
    public CreatureUnit targetCreatureView;
    public IsolateRoom room;

    //private int mentalReduce; // 작업 후 정신력 감소량
    //private int mentalTick; // 틱 당 정신력 변화량

    private bool alreadyHit = false;

    /***
     * 현재 작업이 진행중인지 확인하는 변수.
     * 타이포 등에 의해서 작업이 일시정지되어 false가 될 수도 있다.
     * 
     ***/
    private bool workPlaying = true;

    private bool faceCreature = false;
    private bool readyToFinish = false;

    private bool narrationPart1 = false;
    private bool narrationPart2 = false;
    private bool narrationPart3 = false;
    private bool narrationPart4 = false;
    //private bool narrationPart5 = false;

    private bool finished = false;

    void OnDisable()
    {
        if (finished == false)
        {
            Release();
        }
    }

    public void Init(SkillTypeInfo skill, AgentModel agent, int tickNum, int work, float speed, float feeling)
    {
        workCount = 0;
        totalTickNum = tickNum;
        //workSpeed = speed;
		workSpeed = 1f;
        //totalFeeling = feeling;

        // 성향에 따른 보너스
        switch (agent.agentLifeValue)
        {
        case 1:
            //totalWork *= skill.amountBonusD;
            //totalFeeling *= skill.feelingBonusD;
            //mentalReduce = skill.mentalReduceD;
            //mentalTick = skill.mentalTickD;
            break;
        case 2:
            //totalWork *= skill.amountBonusI;
            //totalFeeling  *= skill.feelingBonusI;
            //mentalReduce = skill.mentalReduceI;
            //mentalTick = skill.mentalTickI;
            break;
        case 3:
            //totalWork *= skill.amountBonusC;
            //totalFeeling *= skill.feelingBonusC;
            //mentalReduce = skill.mentalReduceC;
            //mentalTick = skill.mentalTickC;
            break;
        case 4:
            //totalWork *= skill.amountBonusS;
            //totalFeeling *= skill.feelingBonusS;
            //mentalReduce = skill.mentalReduceS;
            //mentalTick = skill.mentalTickS;
            break;
        }

        //tickInterval = totalWork / totalTickNum;
    }

    public void FixedUpdate()
    {
        ProcessWorkNarration();

        ProgressWork();

		if (workCount >= totalTickNum && !readyToFinish)
        {
            string speech;
            if (agent.speechTable.TryGetValue("work_complete", out speech))
            {
                Notice.instance.Send("AddPlayerLog", agent.name + " : " + speech);
                Notice.instance.Send("AddSystemLog", agent.name + " : " + speech);
                agentView.showSpeech.showSpeech(speech);
            }
            targetCreature.ShowNarrationText("finish", agent.name);

            targetCreature.script.OnSkillGoalComplete(this);

            //StatusView.instance.Hide ();

            readyToFinish = true;
            return;
        }
        if (workPlaying && readyToFinish)
        {
            FinshWork();

            ProcessTraitExp();
        }


        if (agent.GetCurrentNode() != null && agent.GetCurrentNode().GetId() == targetCreature.GetWorkspaceNode().GetId())
        {
            if (!faceCreature)
            {
                faceCreature = true;
                targetCreature.ShowProcessNarrationText("start", agent.name);
                targetCreatureView.PlaySound("enter");
                Debug.Log("Enter");
                targetCreature.script.OnEnterRoom(this);
            }
        }

        if (workPlaying && IsWorkingState())
        {
            workProgress += Time.deltaTime * workSpeed;
        }
    }
    public bool IsWorkingState()
    {
        if (agent.GetCurrentCommandType() == AgentCmdType.MANAGE_CREATURE)
            return true;
        return false;
    }
    private void ProgressWork()
    {
        if (workProgress >= tickInterval * (workCount + 1))
        {
            workCount++;
            ProcessWorkTick();
			progressBar.SetRate(workCount / (float)totalTickNum);
        }
    }
    private void ProcessWorkNarration()
    {
		if (!narrationPart1 && workCount >= (totalTickNum) / 5.0f)
        {
            targetCreature.ShowNarrationText("mid1", agent.name);
            narrationPart1 = true;
        }
		else if (!narrationPart2 && workCount >= (2 * totalTickNum) / 5.0f)
        {
            targetCreature.ShowNarrationText("mid2", agent.name);
            narrationPart2 = true;
        }
		else if (!narrationPart3 && workCount >= (3 * totalTickNum) / 5.0f)
        {
            targetCreature.ShowNarrationText("mid3", agent.name);
            narrationPart3 = true;
        }
		else if (!narrationPart4 && workCount >= (4 * totalTickNum) / 5.0f)
        {
            targetCreature.ShowNarrationText("mid4", agent.name);
            narrationPart4 = true;
        }
    }

    private void AddSkillTrait(long skillTypeId)
    {
        string traitNarration;
        TraitTypeInfo traitTypeInfo = TraitTypeList.instance.GetTraitWithId(skillTypeId);
        agent.applyTrait(traitTypeInfo);
        traitNarration = agent.name + "( 이)가 " + traitTypeInfo.name + " 특성을 획득하였습니다.";
        Notice.instance.Send("AddSystemLog", traitNarration);
    }

    private void ProcessTraitExp()
    {
        agent.expSuccess++;

        if (agent.expMentalDamage > 100)
        {
            int i = Random.Range(0, 6);
            if (i == 3)
            {
                AddSkillTrait(10011);
            }
        }

        if (agent.expHpDamage > 5)
        {
            int i = Random.Range(0, 6);
            if (i == 3)
            {
                AddSkillTrait(10010);
            }
        }

        if (agent.expSuccess > 5)
        {
            int i = Random.Range(0, 6);
            if (i == 3)
            {
                AddSkillTrait(10013);
            }
        }

        string narration = agent.name + "( 이)가 " + skillTypeInfo.name + " 작업을 완료하였습니다.";
        Notice.instance.Send("AddSystemLog", narration);
    }

    public void PauseWorking()
    {
        workPlaying = false;
    }

    public void ResumeWorking()
    {
        workPlaying = true;
    }

    private void Release()
    {
        agent.FinishWorking();
        targetCreature.state = CreatureState.WAIT;
		//targetCreature.bufRemainingTime = 5f;
    }

    private void FinshWork()
    {
        finished = true;
        /*
        tempView.Hide();
        tempCreView.Hide();
        */

		//workCount
		//successCount
		float energyAdd = agent.GetEnergyAbility(skillTypeInfo)*successCount/workCount;

		if (targetCreature.IsPreferSkill (skillTypeInfo)) {
			targetCreature.AddFeeling(skillTypeInfo.amount*successCount/workCount * targetCreature.GetWorkEfficient(skillTypeInfo));
		} else if (targetCreature.IsRejectSkill (skillTypeInfo)) {
			// unused
			targetCreature.SubFeeling (skillTypeInfo.amount*successCount/workCount);
		}

		// temp comment
		//targetCreature.SetEnergyChange (5, energyAdd);

		// temp for proto
		targetCreature.SetEnergyChange (5, skillTypeInfo.amount*successCount/workCount * targetCreature.GetWorkEfficient(skillTypeInfo));




        //agent.GetComponentInChildren<agentSkillDoing>().turnOnDoingSkillIcon(false);
        agentView.showSkillIcon.turnOnDoingSkillIcon(false);

		//targetCreature.SetFeelingBuf (bufTime, feelingBufAmount);

        Release();

        Notice.instance.Send("UpdateCreatureState_" + targetCreature.instanceId);

        Destroy(gameObject);
        Destroy(progressBar.gameObject);
    }

	public void FinishForcely()
	{
		FinshWork ();
	}

    private void ProcessWorkTick()
    {
        targetCreature.script.OnSkillTickUpdate(this);

        // 
        if (workPlaying)
        {
            bool success = true;
            bool agentUpdated = false;
            

			float workProb = agent.GetSuccessProb (skillTypeInfo);

            if (Random.value < workProb)
                success = true;
            else
                success = false;

            if (success)
            {
				successCount++;
            }
            else
            {
                targetCreature.script.OnSkillFailWorkTick(this);

                // It can be skipped when changed in SkillFailWorkTick
                if (workPlaying)
                {
					// current 20% + 30%
					float attackProb = targetCreature.GetAttackProb () - agent.GetEvasionProb() + 0.3f;

					if (Random.value <= attackProb) {
						if (targetCreature.GetAttackType () == CreatureAttackType.PHYSICS)
						{
							agent.TakePhysicalDamage(targetCreature.GetPhysicsDmg ());
						}
						else if (targetCreature.GetAttackType () == CreatureAttackType.MENTAL)
						{
							agent.TakeMentalDamage (targetCreature.GetMentalDmg ());
						}
						else // COMPLEX
						{
							agent.TakePhysicalDamage(targetCreature.GetPhysicsDmg ());
							agent.TakeMentalDamage (targetCreature.GetMentalDmg ());
						}
					}
                }
            }

            Notice.instance.Send("UpdateCreatureState_" + targetCreature.instanceId);
            if (agentUpdated)
            {
                Notice.instance.Send("UpdateAgentState_" + agent.instanceId);
            }
            CheckLive();
        }
    }

    public void CheckLive()
    {
        if (agent.mental <= 0)
        {
            string speech;
            if (agent.speechTable.TryGetValue("panic", out speech))
            {
                Notice.instance.Send("AddPlayerLog", agent.name + " : " + speech);
                Notice.instance.Send("AddSystemLog", agent.name + " : " + speech);
                agentView.showSpeech.showSpeech(speech);
            }

            targetCreature.ShowNarrationText("panic", agent.name);

            FinshWork();
            agent.Panic();

            agent.expFail++;


            if (agent.expMentalDamage > 100)
            {
                int i = Random.Range(0, 6);
                if (i == 3)
                {
                    AddSkillTrait(10012);
                }
            }

            if (agent.expHpDamage > 6)
            {
                int i = Random.Range(0, 6);
                if (i == 3)
                {
                    AddSkillTrait(10014);
                }
            }

            string narration = agent.name + " (이)가 공황에 빠져 " + skillTypeInfo.name + " 작업에 실패하였습니다.";
            Notice.instance.Send("AddSystemLog", narration);
        }
        if (agent.hp <= 0)
        {
            string speech;
            if (agent.speechTable.TryGetValue("dead", out speech))
            {
                Notice.instance.Send("AddPlayerLog", agent.name + " : " + speech);
                Notice.instance.Send("AddSystemLog", agent.name + " : " + speech);
                agentView.showSpeech.showSpeech(speech);
            }

            targetCreature.ShowNarrationText("dead", agent.name);
            FinshWork();
            agent.Die();
            string narration = this.name + " (이)가 사망하여 안타깝게도 " + skillTypeInfo.name + " 작업에 실패하였습니다.";
            Notice.instance.Send("AddSystemLog", narration);
        }
    }

	public bool IsFinished()
	{
		return finished;
	}

    public static UseSkill InitUseSkillAction(SkillTypeInfo skillInfo, AgentModel agent, CreatureModel creature)
    {
        if (creature.state != CreatureState.WAIT)
        {
            return null;
        }
        GameObject newObject = new GameObject();
        newObject.name = "UseSkill";

        string narration = agent.name + " (이)가 " + skillInfo.name + " 작업을 시작합니다.";
        Notice.instance.Send("AddSystemLog", narration);

        UseSkill inst = newObject.AddComponent<UseSkill>();

        AgentUnit agentView = AgentLayer.currentLayer.GetAgent(agent.instanceId);
        CreatureUnit creatureView = CreatureLayer.currentLayer.GetCreature(creature.instanceId);

        agentView.showSkillIcon.turnOnDoingSkillIcon(true);
        agentView.showSkillIcon.showDoingSkillIcon(skillInfo, agent);

		agentView.puppetAnim.speed = 6.0f;
        if (skillInfo.animID == 0)
            agentView.puppetAnim.SetBool("Memo", true);
        else {
            //Debug.Log(SkillCategoryName.GetCategoryName(skillInfo));
            agentView.puppetAnim.SetInteger( SkillCategoryName.GetCategoryName(skillInfo),
                skillInfo.animID);
        }

        string speech;
        agent.speechTable.TryGetValue("work_start", out speech);
        Notice.instance.Send("AddSystemLog", agent.name + " : " + speech);
        agentView.showSpeech.showSpeech(speech);

        creature.ShowNarrationText("move", agent.name);

        //agent.MoveToCreture(creature.gameObject);
        //agent.Working (creature.gameObject);
        //agent.Working(creature);
        //creature.ShowNarrationText("start", agent.name);

        //inst.Init(skillInfo, agent, 10, skillInfo.amount, agent.workSpeed, skillInfo.amount); // 임시
        inst.Init(skillInfo, agent, 10, (int)skillInfo.amount, agent.workSpeed, skillInfo.amount);

        inst.agent = agent;
        inst.agentView = agentView;

        inst.targetCreature = creature;
        inst.targetCreatureView = creatureView;

        inst.skillTypeInfo = skillInfo;

        creature.state = CreatureState.WORKING;

		agent.SetSkillDelay (skillInfo, 30);

		creature.manageDelay = 15;

        //관찰 조건을 위한 환상체 작업 횟수추가
        creature.workCount++;

        GameObject progressObj = Instantiate(Resources.Load<GameObject>("Prefabs/EnergyBar")) as GameObject;
        //progressObj.transform.parent = creatureView.transform;
        //progressObj.transform.localPosition = new Vector3(0, -0.7f, 0);
        progressObj.transform.SetParent(creatureView.transform.GetChild(0));
        progressObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1.65f, 0.874f);

        inst.progressBar = progressObj.GetComponent<ProgressBar>();
        inst.progressBar.SetVisible(true);
        inst.progressBar.SetRate(0);
        inst.progressBar.transform.localScale = new Vector3(0.7692308f, 0.7692308f, 1f);
        inst.progressBar.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1.763f, 0.6420423f);

        Notice.instance.Send("UpdateCreatureState_" + inst.targetCreature.instanceId);

        return inst;
    }
}
