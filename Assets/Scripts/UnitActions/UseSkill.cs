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

    //타이머 관련 변수
    float elapsed;
    float maxTime;
    bool timerStart = false;

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
		workSpeed = 0.5f;
        //totalFeeling = feeling;

        // 성향에 따른 보너스
        switch (agent.agentLifeValue)
        {
		case PersonalityType.D:
            //totalWork *= skill.amountBonusD;
            //totalFeeling *= skill.feelingBonusD;
            //mentalReduce = skill.mentalReduceD;
            //mentalTick = skill.mentalTickD;
            break;
		case PersonalityType.I:
            //totalWork *= skill.amountBonusI;
            //totalFeeling  *= skill.feelingBonusI;
            //mentalReduce = skill.mentalReduceI;
            //mentalTick = skill.mentalTickI;
            break;
		case PersonalityType.S:
            //totalWork *= skill.amountBonusC;
            //totalFeeling *= skill.feelingBonusC;
            //mentalReduce = skill.mentalReduceC;
            //mentalTick = skill.mentalTickC;
            break;
		case PersonalityType.C:
            //totalWork *= skill.amountBonusS;
            //totalFeeling *= skill.feelingBonusS;
            //mentalReduce = skill.mentalReduceS;
            //mentalTick = skill.mentalTickS;
            break;
        }

        //tickInterval = totalWork / totalTickNum;
    }

    public void MakeReaction() {
        string speech = null;
        int endLevel = GetEfficientLevel();
        AgentLyrics.CreatureReactionList reaction = null;
        if ((reaction = AgentLyrics.instance.GetCreatureReaction(this.targetCreature.metadataId)) != null)
        {

            string desc = reaction.GetDesc(endLevel);
            speech = "";
            if (desc == null)
            {
                agent.speechTable.TryGetValue("work_complete", out speech);
            }
            else
            {
                speech = desc;
            }
            SendAgentSpeechMessage(speech);
        }
        else if (agent.speechTable.TryGetValue("work_complete", out speech))
        {
            SendAgentSpeechMessage(speech);
        }
    }

    public void FixedUpdate()
    {
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


		targetCreature.script.OnFixedUpdateInSkill (this);

		CheckLive();
		if (finished)
			return;	

        ProcessWorkNarration();

        ProgressWork();

		CheckLive();
		if (finished)
			return;


        if (agent.workEndReaction) {
            agent.workEndReaction = false;
            MakeReaction();
        }
        //check work end and state changed to pile checking

		if (workCount >= totalTickNum && !readyToFinish && agent.OnWorkEndFlag)
        {
            //MakeReaction();

            //AngelaConversaion.instance.MakeCreatureReaction(this.targetCreature, endLevel);

            targetCreature.ShowNarrationText("finish", agent.name);

            targetCreature.script.OnSkillGoalComplete(this);

            //StatusView.instance.Hide ();

            readyToFinish = true;
            agent.OnWorkEndFlag = false;
  
			return;
        }
        if (workPlaying && readyToFinish)
		{
			FinshWork();

			ProcessTraitExp();
        }

        if (workPlaying && IsWorkingState())
        {
            workProgress += Time.deltaTime * workSpeed;
        }

        if (timerStart) {
            elapsed += Time.deltaTime;
            if (elapsed > maxTime) {
                EndTimer();
            }
        }
    }

    public void SendAgentSpeechMessage(string desc)
    {
        Notice.instance.Send("AddPlayerLog", agent.name + " : " + desc);
        Notice.instance.Send("AddSystemLog", agent.name + " : " + desc);
        agentView.showSpeech.showSpeech(desc, 10f);
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

    
    public void PauseWorking(float time) {
        workPlaying = false;
        SetTimer(time);
    }

    public void SetTimer(float duration) {
        timerStart = true;
        elapsed = 0f;
        maxTime = duration;
    }

    public void EndTimer() {
        timerStart = false;
        this.targetCreature.script.OnTimerEnd();
    }
    
    public void ResumeWorking()
    {
        workPlaying = true;
    }

	public bool IsWorking()
	{
		return workPlaying;
	}

    private void Release()
    {
		targetCreature.script.OnRelease (this);

        agent.FinishWorking();
		if(targetCreature.state == CreatureState.WORKING)
        	targetCreature.state = CreatureState.WAIT;
		
		targetCreature.currentSkill = null;
		//targetCreature.bufRemainingTime = 5f;
    }

    public int GetEfficientLevel()
    {
        float targetValue = targetCreature.GetWorkEfficient(targetCreature.currentSkill.skillTypeInfo);
        if (targetValue >= 1.5f)
        {
            return 1;
        }
        else if (targetValue < 1.5f && targetValue >= 1f) {
            return 2;
        }
        else if (targetValue < 1f && targetValue >= 0f) {
            return 3;
        }
        else if (targetValue < 0f && targetValue >= -1f)
        {
            return 4;
        }
        else {
            return 5;
        }
    }

    private void FinshWork()
    {
        finished = true;
        int result = -1;
        if (workPlaying && !targetCreature.script.hasUniqueFinish()) {
            
            float eff = targetCreature.GetWorkEfficient(targetCreature.currentSkill.skillTypeInfo);
            if (eff > 1)
            {
                result = 0;
            }
            else if (eff < 0)
            {
                result = 2;
            }
            else
            {
                result = 1;
            }
            if (targetCreature.script != null)
            {
                targetCreature.script.SetCurrentSkillResult(result);
                targetCreature.script.MakeEffect(targetCreatureView.room);
                targetCreature.script.ResetCurrentSkillResult();
            }
        }
        

        /*
        tempView.Hide();
        tempCreView.Hide();
        */

		//workCount
		//successCount

        /*

        
		float energyAdd = agent.GetEnergyAbility(skillTypeInfo)*successCount/totalTickNum;

		if (targetCreature.IsPreferSkill (skillTypeInfo)) {
			targetCreature.AddFeeling(skillTypeInfo.amount*successCount/totalTickNum * targetCreature.GetWorkEfficient(skillTypeInfo));
		} else if (targetCreature.IsRejectSkill (skillTypeInfo)) {
			// unused
			targetCreature.SubFeeling (skillTypeInfo.amount*successCount/totalTickNum);
		}
        targetCreature.SetEnergyChange (5, skillTypeInfo.amount*successCount/totalTickNum * targetCreature.GetWorkEfficient(skillTypeInfo) * 2);
		// temp comment
		//targetCreature.SetEnergyChange (5, energyAdd);

		// temp for proto
		//targetCreature.SetEnergyChange (5, skillTypeInfo.amount*successCount/totalTickNum * targetCreature.GetWorkEfficient(skillTypeInfo) * 2);
        */

        if (this.targetCreature.script != null && this.targetCreature.script.hasUniqueFinish())
        {
            this.targetCreature.script.UniqueFinish(this);
        }
        else {
            float energyAdd = agent.GetEnergyAbility(skillTypeInfo) * successCount / totalTickNum;

            if (targetCreature.IsPreferSkill(skillTypeInfo))
            {
                targetCreature.AddFeeling(skillTypeInfo.amount * successCount / totalTickNum * targetCreature.GetWorkEfficient(skillTypeInfo));
            }
            else if (targetCreature.IsRejectSkill(skillTypeInfo))
            {
                // unused
                targetCreature.SubFeeling(skillTypeInfo.amount * successCount / totalTickNum);
            }
            targetCreature.SetEnergyChange(5, skillTypeInfo.amount * successCount / totalTickNum * targetCreature.GetWorkEfficient(skillTypeInfo) * 2);
        }

        //agent.GetComponentInChildren<agentSkillDoing>().turnOnDoingSkillIcon(false);
        agentView.showSkillIcon.turnOnDoingSkillIcon(false);

		//targetCreature.SetFeelingBuf (bufTime, feelingBufAmount);

        Release();
        agent.currentSkill = null;
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

		//Debug.Log ("Tick... "+workCount);

        // 
        if (workPlaying)
        {
            bool success = true;
            bool agentUpdated = false;
            

			//float workProb = agent.GetSuccessProb (skillTypeInfo);

			float workProb = 0.7f;
			if (targetCreature.GetWorkEfficient (skillTypeInfo) > 1f)
				workProb += 0.2f;
			else if (targetCreature.GetWorkEfficient (skillTypeInfo) >= 0f)
				workProb += 0.1f;

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
                if (workPlaying && targetCreature.script.isAttackInWorkProcess())
                {
					// current 20% + 30%
					float attackProb = targetCreature.GetAttackProb () - agent.GetEvasionProb() + 0.2f;

                    
					if (Random.value <= attackProb) {
						if (targetCreature.GetAttackType () == CreatureAttackType.PHYSICS)
						{
							agent.TakePhysicalDamageByCreature(targetCreature.GetPhysicsDmg ());
						}
						else if (targetCreature.GetAttackType () == CreatureAttackType.MENTAL)
						{
							agent.TakeMentalDamage (targetCreature.GetMentalDmg ());
						}
						else // COMPLEX
						{
							agent.TakePhysicalDamageByCreature(targetCreature.GetPhysicsDmg ());
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

			CheckLive ();
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
        //if (agent.hp <= 0)
		if(agent.isDead())
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
            //agent.Die();
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
        if(skillInfo.animTarget != null){
            if (skillInfo.animTarget == "UniqueWork")
            {
                //some other process
                int value = (int)skillInfo.id % 100;
                agentView.puppetAnim.SetInteger(skillInfo.animTarget, value);
            }
            else {
                agentView.puppetAnim.SetInteger(skillInfo.animTarget, 1);
                agent.OnWorkEndFlag = false;
            }
            
            
        }
        else if (skillInfo.animID == 0)
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
		creature.currentSkill = inst;

		agent.SetSkillDelay (skillInfo, 40);

		creature.manageDelay = 21;

        //관찰 조건을 위한 환상체 작업 횟수추가
        creature.workCount++;

        GameObject progressObj = Instantiate(Resources.Load<GameObject>("Prefabs/EnergyBar")) as GameObject;
        //progressObj.transform.parent = creatureView.transform;
        //progressObj.transform.localPosition = new Vector3(0, -0.7f, 0);
        progressObj.transform.SetParent(inst.targetCreatureView.room.transform.GetChild(0).transform);
        progressObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1.65f, 0.874f);

        inst.progressBar = progressObj.GetComponent<ProgressBar>();
        inst.progressBar.SetVisible(true);
        inst.progressBar.SetRate(0);
        inst.progressBar.transform.localScale = new Vector3(0.7692308f, 0.7692308f, 1f);
        float posy = inst.targetCreatureView.transform.localPosition.y;
        posy = 1 + posy;
        inst.progressBar.GetComponent<RectTransform>().anchoredPosition = new Vector2(-3.251f, -0.354f);
        inst.agent.currentSkill = inst;
        Notice.instance.Send("UpdateCreatureState_" + inst.targetCreature.instanceId);

        return inst;
    }

}
