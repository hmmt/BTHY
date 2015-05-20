using UnityEngine;
using System.Collections;

public class UseSkill : MonoBehaviour {

    public TraitTypeList traitList;

	public int goalWork;
	public float elapsedWorkingTime;
	public int currentWork;
	public int updateTick = 5;
	public int workCount;
	public long skillId;
	public SkillTypeInfo skillTypeInfo;


	public ProgressBar progressBar;
	public AgentUnit agent;
	public CreatureUnit targetCreature;

	private bool alreadyHit = false;
	private bool workPlaying = true;

	private bool faceCreature = false;
    private bool readyToFinish = false;

	private bool narrationPart1 = false;
	private bool narrationPart2 = false;
	private bool narrationPart3 = false;
	private bool narrationPart4 = false;
	//private bool narrationPart5 = false;

	public void FixedUpdate()
	{
		if(!narrationPart1 && currentWork >= (goalWork)/5.0f)
		{
			targetCreature.ShowNarrationText("mid1", agent.name);
			narrationPart1 = true;
		}
		else if(!narrationPart2 && currentWork >= (2*goalWork)/5.0f)
		{
			targetCreature.ShowNarrationText("mid2", agent.name);
			narrationPart2 = true;
		}
		else if(!narrationPart3 && currentWork >= (3*goalWork)/5.0f)
		{
			targetCreature.ShowNarrationText("mid3", agent.name);
			narrationPart3 = true;
		}
		else if(!narrationPart4 && currentWork >= (4*goalWork)/5.0f)
		{
			targetCreature.ShowNarrationText("mid4", agent.name);
			narrationPart4 = true;
		}

		if(elapsedWorkingTime >= updateTick * (workCount+1))
		{
			workCount++;
			if(agent.work + currentWork > goalWork)
			{
				ProcessWorkTick(goalWork - currentWork);
				currentWork = goalWork;
			}
			else
			{
				ProcessWorkTick(agent.work);
				currentWork += agent.work;
			}
			progressBar.SetRate(currentWork/(float)goalWork);
		}
        if(currentWork >= goalWork && !readyToFinish)
		{
			/*
			if(currentWork < goalWork)
			{
				ProcessWorkTick(goalWork - currentWork);
				currentWork = goalWork;
			}
			*/
			string speech;
			if(agent.speechTable.TryGetValue ("work_complete", out speech))
			{
				Notice.instance.Send("AddPlayerLog", agent.name + " : " +  speech);
			}
			targetCreature.ShowNarrationText("finish", agent.name);

            targetCreature.script.SkillGoalComplete(this);

			//StatusView.instance.Hide ();

            readyToFinish = true;
			return;
		}
        if(workPlaying && readyToFinish)
        {
            FinshWork();

            agent.expSuccess++;

            string traitNarration;

            if (agent.expMentalDamage > 100)
            {
                int i = Random.Range(0, 6);
                if (i == 3)
                {
                    agent.traitList.Add(traitList.GetTraitWithId(10011));
                    agent.applyTrait(traitList.GetTraitWithId(10011));
                    traitNarration = agent.name + "( 이)가 " + traitList.GetTraitWithId(10011).name + " 특성을 획득하였습니다.";
                    Notice.instance.Send("AddSystemLog", traitNarration);
                }
            }

            if (agent.expHpDamage > 5)
            {
                int i = Random.Range(0, 6);
                if (i == 3)
                {
                    agent.traitList.Add(traitList.GetTraitWithId(10010));
                    agent.applyTrait(traitList.GetTraitWithId(10010));
                    traitNarration = agent.name + "( 이)가 " + traitList.GetTraitWithId(10010).name + " 특성을 획득하였습니다.";
                    Notice.instance.Send("AddSystemLog", traitNarration);
                }
            }

            if (agent.expSuccess > 5)
            {
                int i = Random.Range(0, 6);
                if (i == 3)
                {
                    agent.traitList.Add(traitList.GetTraitWithId(10013));
                    agent.applyTrait(traitList.GetTraitWithId(10013));
                    traitNarration = agent.name + "( 이)가 " + traitList.GetTraitWithId(10013).name + " 특성을 획득하였습니다.";
                    Notice.instance.Send("AddSystemLog", traitNarration);
                }
            }

			string narration = agent.name+"( 이)가 "+skillTypeInfo.name+" 작업을 완료하였습니다.";
			Notice.instance.Send("AddSystemLog", narration);
        }


		//if(Vector2.Distance((Vector2)agent.transform.position, (Vector2)targetCreature.transform.position) < 1)
		if(agent.GetCurrentNode() != null && agent.GetCurrentNode().GetId() == targetCreature.GetWorkspaceNode().GetId())
		{
			if(!faceCreature)
			{
				faceCreature = true;
				//OutsideTextEffect.Create(targetCreature.room, "typo/01_matchGirl_out_typo", CreatureOutsideTextLayout.CENTER_BOTTOM);
				//targetCreature.ShowNarrationText("start", agent.name);
                targetCreature.ShowProcessNarrationText("start",agent.name);
                targetCreature.PlaySound("enter");
				targetCreature.script.EnterRoom(this);
			}
			if(workPlaying)
			{
				elapsedWorkingTime += Time.deltaTime;
			}
		}
	}

	public void PauseWorking()
	{
		workPlaying = false;
	}

	public void ResumeWorking()
	{
		workPlaying = true;
	}


	private void FinshWork()
	{
        
		/*
		tempView.Hide();
		tempCreView.Hide();
		*/
		agent.FinishWorking();
		targetCreature.state = CreatureState.WAIT;

		Destroy(gameObject);
		Destroy(progressBar.gameObject);
	}

	private void ProcessWorkTick(int workValue)
	{
		//EnergyModel.instance.AddEnergy()

        targetCreature.script.SkillTickUpdate(this);

        // 
        if (workPlaying)
        {
            bool success = true;
            bool agentUpdated = false;
            float workProb = 0.6f;

            // agent prefer
            if (agent.prefer == skillTypeInfo.type)
            {
                workProb += agent.preferBonus;
            }
            else if (agent.reject == skillTypeInfo.type)
            {
                workProb += agent.rejectBonus;
            }
            else
            {
            }

            // creature prefer
            float bonus = 0;
            if (targetCreature.GetPreferSkillBonus(skillTypeInfo.type, out bonus))
            {
                // prob up
                workProb += bonus;
            }
            else if (targetCreature.GetRejectSkillBonus(skillTypeInfo.type, out bonus))
            {
                // prob down
                workProb += bonus;
            }
            else
            {
                // prob middle
            }

            if (Random.value < workProb)
            {
                success = true;
            }
            else
            {
                success = false;
            }

            if (success)
            {
                if (targetCreature.IsPreferSkill(skillTypeInfo.type))
                {
                    workValue = (int)(workValue * 1.5);
                }
                else if (targetCreature.IsRejectSkill(skillTypeInfo.type))
                {
                    workValue = (int)(workValue * 0.5);
                }
            }
            else
            {
                targetCreature.script.SkillFailWorkTick(this);

                // when changed in SkillFailWorkTick
                if (workPlaying)
                {
                    if (targetCreature.IsPreferSkill(skillTypeInfo.type))
                    {
                        workValue = (int)(workValue * 0.5);
                    }
                    else if (targetCreature.IsRejectSkill(skillTypeInfo.type))
                    {
                        workValue = (int)(workValue * 1.5);
                    }
                    workValue = -workValue;

                    bool physicsAtk = Random.value < targetCreature.metaInfo.physicsProb;
                    bool mentalAtk = Random.value < targetCreature.metaInfo.mentalProb;

                    if (physicsAtk || mentalAtk)
                    {
                        targetCreature.script.SkillNormalAttack(this);
                    }

                    if (physicsAtk)
                    {

                        //agent.agentAttackedAnimator.GetComponent<Animator>().SetBool("attackUp",true);
                       // Debug.Log("직원 애니메이터 1불 : "+agent.agentAttackedAnimator.GetComponent<Animator>().GetBool("attackUP"));

                        agent.hp -= targetCreature.metaInfo.physicsDmg;
                        agent.expHpDamage += targetCreature.metaInfo.physicsDmg;
        
                        agentUpdated = true;

                        AgentHitEffect.Create(agent);
                        targetCreature.PlaySound("attack");

                        string speech;
                        if (!alreadyHit && agent.speechTable.TryGetValue("work_hit", out speech))
                        {
                            Notice.instance.Send("AddPlayerLog", agent.name + " : " + speech);
                            alreadyHit = true;
                        }

                        //agent.agentAttackedAnimator.GetComponent<Animator>().SetBool("attackUp", false);
                      //  Debug.Log("직원 애니메이터 2불 : " + agent.agentAttackedAnimator.GetComponent<Animator>().GetBool("attackUP"));
                    }
                    if (mentalAtk)
                    {
                        agent.mental -= targetCreature.metaInfo.mentalDmg;
                        agent.expMentalDamage += targetCreature.metaInfo.mentalDmg;

                        agentUpdated = true;
                    }
                }
            }

            targetCreature.AddFeeling(workValue);

            Notice.instance.Send("UpdateCreatureState_" + targetCreature.gameObject.GetInstanceID());
            if (agentUpdated)
            {
                Notice.instance.Send("UpdateAgentState_" + agent.gameObject.GetInstanceID());
            }
            CheckLive();
        }
	}

    public void CheckLive()
    {
        string traitNarration;

        if (agent.mental <= 0)
        {
            string speech;
            if (agent.speechTable.TryGetValue("panic", out speech))
            {
                Notice.instance.Send("AddPlayerLog", agent.name + " : " + speech);
            }

            targetCreature.ShowNarrationText("panic", agent.name);

            FinshWork();
            agent.Panic();

            agent.expFail++;


            if (agent.expMentalDamage> 100)
            {
                int i = Random.Range(0, 6);
                if (i == 3)
                {
                    agent.traitList.Add(traitList.GetTraitWithId(10012));
                    agent.applyTrait(traitList.GetTraitWithId(10012));
                    traitNarration = agent.name + "( 이)가 " + traitList.GetTraitWithId(10012).name + " 특성을 획득하였습니다.";
                    Notice.instance.Send("AddSystemLog", traitNarration);
                }
            }

            if (agent.expHpDamage > 6)
            {
                int i = Random.Range(0, 6);
                if (i == 3)
                {
                    agent.traitList.Add(traitList.GetTraitWithId(10014));
                    agent.applyTrait(traitList.GetTraitWithId(10014));
                    traitNarration = agent.name + "( 이)가 " + traitList.GetTraitWithId(10014).name + " 특성을 획득하였습니다.";
                    Notice.instance.Send("AddSystemLog", traitNarration);
                }
            }

			string narration = this.name+" (이)가 공황에 빠져 "+skillTypeInfo.name+" 작업에 실패하였습니다.";
			Notice.instance.Send("AddSystemLog", narration);
        }
        if (agent.hp <= 0)
        {
            string speech;
            if (agent.speechTable.TryGetValue("dead", out speech))
            {
                Notice.instance.Send("AddPlayerLog", agent.name + " : " + speech);
            }

            targetCreature.ShowNarrationText("dead", agent.name);
            FinshWork();
            agent.Die();
			string narration = this.name+" (이)가 사망하여 안타깝게도 "+skillTypeInfo.name+" 작업에 실패하였습니다.";
			Notice.instance.Send("AddSystemLog", narration);
        }
    }

	public static UseSkill InitUseSkillAction(SkillTypeInfo skillInfo, AgentUnit agent, CreatureUnit creature)
	{
		if(agent.target != null || creature.state != CreatureState.WAIT)
		{
			return null;
		}
		GameObject newObject = new GameObject ();

		string narration = agent.name+" (이)가 "+skillInfo.name+" 작업을 시작합니다.";
		Notice.instance.Send("AddSystemLog", narration);

		UseSkill inst = newObject.AddComponent<UseSkill> ();

        inst.traitList = TraitTypeList.instance;

		agent.Speech ("work_start");
		creature.ShowNarrationText("move", agent.name);

		//agent.MoveToCreture(creature.gameObject);
		//agent.Working (creature.gameObject);
		agent.Working (creature);
		//creature.ShowNarrationText("start", agent.name);

		inst.currentWork = 0;
		inst.goalWork = skillInfo.amount;
		inst.workCount = 0;
		inst.agent = agent;
		inst.targetCreature = creature;

		inst.skillTypeInfo = skillInfo;
		inst.skillId = skillInfo.id;

		creature.state = CreatureState.WORKING;

		GameObject progressObj = Instantiate(Resources.Load<GameObject> ("Prefabs/ProgressBar")) as GameObject;
		progressObj.transform.parent = creature.transform;
		progressObj.transform.localPosition = new Vector3(0, -0.7f, 0);

		inst.progressBar = progressObj.GetComponent<ProgressBar> ();
		inst.progressBar.SetVisible (true);
		inst.progressBar.SetRate (0);

		return inst;
	}
}
