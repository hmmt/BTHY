using UnityEngine;
using System.Collections;

public class UseSkill : ActionClassBase
{

    public int totalTickNum;
    public float tickInterval;
    public float totalWork;
    public float curWork;
    public float workSpeed;
    public float workProgress;
    public float totalFeeling;

    public int goalWork;
    public float elapsedWorkingTime;
    public int currentWork;
    public int updateTick = 5;
    public int workCount;

    // skill info
    public SkillTypeInfo skillTypeInfo;

    public ProgressBar progressBar;
    public AgentModel agent;
    public AgentUnit agentView;

    public CreatureModel targetCreature;
    public CreatureUnit targetCreatureView;
    public IsolateRoom room;

    private int mentalReduce; // 작업 후 정신력 감소량
    private int mentalTick; // 틱 당 정신력 변화량

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
        totalWork = work;
        workSpeed = speed;
        totalFeeling = feeling;

        int maxHP = 0;
        int maxMental = 0;
        // skill 보너스
        if (maxHP > 100)
        {
            workSpeed *= 1.3f;
        }
        else if (maxMental > 200)
        {
            workSpeed *= 1.3f;
        }

        // 성향에 따른 보너스
        switch (agent.agentLifeValue)
        {
        case 1:
            totalWork *= skill.amountBonusD;
            totalFeeling *= skill.feelingBonusD;
            mentalReduce = skill.mentalReduceD;
            mentalTick = skill.mentalTickD;
            break;
        case 2:
            totalWork *= skill.amountBonusI;
            totalFeeling  *= skill.feelingBonusI;
            mentalReduce = skill.mentalReduceI;
            mentalTick = skill.mentalTickI;
            break;
        case 3:
            totalWork *= skill.amountBonusC;
            totalFeeling *= skill.feelingBonusC;
            mentalReduce = skill.mentalReduceC;
            mentalTick = skill.mentalTickC;
            break;
        case 4:
            totalWork *= skill.amountBonusS;
            totalFeeling *= skill.feelingBonusS;
            mentalReduce = skill.mentalReduceS;
            mentalTick = skill.mentalTickS;
            break;
        }

        tickInterval = totalWork / totalTickNum;
    }

    public void FixedUpdate()
    {
        ProcessWorkNarration();

        ProgressWork();

        if (curWork >= totalWork && !readyToFinish)
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
            //int addedWork = (int)(totalWork / totalTickNum);
            int addedFelling = (int)(totalFeeling / totalTickNum);
            ProcessWorkTick(addedFelling);
            curWork = workCount * totalWork / totalTickNum;
            progressBar.SetRate(curWork / (float)totalWork);
        }
    }
    private void ProcessWorkNarration()
    {
        if (!narrationPart1 && curWork >= (totalWork) / 5.0f)
        {
            targetCreature.ShowNarrationText("mid1", agent.name);
            narrationPart1 = true;
        }
        else if (!narrationPart2 && curWork >= (2 * totalWork) / 5.0f)
        {
            targetCreature.ShowNarrationText("mid2", agent.name);
            narrationPart2 = true;
        }
        else if (!narrationPart3 && curWork >= (3 * totalWork) / 5.0f)
        {
            targetCreature.ShowNarrationText("mid3", agent.name);
            narrationPart3 = true;
        }
        else if (!narrationPart4 && curWork >= (4 * totalWork) / 5.0f)
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
		targetCreature.bufRemainingTime = 5f;
    }

    private void FinshWork()
    {
        finished = true;
        /*
        tempView.Hide();
        tempCreView.Hide();
        */

        //agent.GetComponentInChildren<agentSkillDoing>().turnOnDoingSkillIcon(false);
        agentView.showSkillIcon.turnOnDoingSkillIcon(false);

        Release();

        Notice.instance.Send("UpdateCreatureState_" + targetCreature.instanceId);

        Destroy(gameObject);
        Destroy(progressBar.gameObject);
    }

    private void ProcessWorkTick(int workValue)
    {
        targetCreature.script.OnSkillTickUpdate(this);

        // 
        if (workPlaying)
        {
            bool success = true;
            bool agentUpdated = false;
            float workProb = 0.6f;

            float physicsProb = targetCreature.metaInfo.physicsProb;
            float mentalProb = targetCreature.metaInfo.mentalProb;
            int physicsDmg = targetCreature.metaInfo.physicsDmg;
            int mentalDmg = targetCreature.metaInfo.mentalDmg;

            if (targetCreature.script != null)
            {
                CreatureAttackInfo attackInfo = targetCreature.script.GetAttackInfo(this);

                physicsProb = attackInfo.physicsProb;
                mentalProb = attackInfo.mentalProb;
                physicsDmg = attackInfo.physicsDmg;
                mentalDmg = attackInfo.mentalDmg;
            }

            if (skillTypeInfo.type == "direct")
            {
                workProb += agent.directBonus;
            }
            else if (skillTypeInfo.type == "indirect")
            {
                workProb += agent.inDirectBonus;
            }
            else if (skillTypeInfo.type == "block")
            {
                workProb += agent.blockBonus;
            }

            // creature prefer
            float bonus = 0;
            if (targetCreature.GetPreferSkillBonus(skillTypeInfo, out bonus))
            {
                // prob up
                workProb += bonus;
            }
            else if (targetCreature.GetRejectSkillBonus(skillTypeInfo, out bonus))
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
                if (targetCreature.IsPreferSkill(skillTypeInfo))
                {
                    workValue = (int)(workValue * 1.5);
                }
                else if (targetCreature.IsRejectSkill(skillTypeInfo))
                {
                    workValue = (int)(workValue * 0.5);
                }
            }
            else
            {
                targetCreature.script.OnSkillFailWorkTick(this);

                // when changed in SkillFailWorkTick
                if (workPlaying)
                {
                    if (targetCreature.IsPreferSkill(skillTypeInfo))
                    {
                        workValue = (int)(workValue * 0.5);
                    }
                    else if (targetCreature.IsRejectSkill(skillTypeInfo))
                    {
                        workValue = (int)(workValue * 1.5);
                    }
                    workValue = -workValue;

                    bool physicsAtk = Random.value < physicsProb;
                    bool mentalAtk = Random.value < mentalProb;

                    if (physicsAtk || mentalAtk)
                    {
                        targetCreature.script.OnSkillNormalAttack(this);
                    }

                    if (physicsAtk)
                    {

                        //agent.agentAttackedAnimator.GetComponent<Animator>().SetBool("attackUp",true);
                        // Debug.Log("직원 애니메이터 1불 : "+agent.agentAttackedAnimator.GetComponent<Animator>().GetBool("attackUP"));

                        agent.TakePhysicalDamage(physicsDmg);
                        agent.expHpDamage += physicsDmg;

                        agentUpdated = true;

                        AgentHitEffect.Create(agent);
                        targetCreatureView.PlaySound("attack");

                        string speech;
                        if (!alreadyHit && agent.speechTable.TryGetValue("work_hit", out speech))
                        {
                            Notice.instance.Send("AddSystemLog", agent.name + " : " + speech);
                            Notice.instance.Send("AddPlayerLog", agent.name + " : " + speech);
                            alreadyHit = true;
                            agentView.showSpeech.showSpeech(speech);
                        }

                        //agent.agentAttackedAnimator.GetComponent<Animator>().SetBool("attackUp", false);
                        //  Debug.Log("직원 애니메이터 2불 : " + agent.agentAttackedAnimator.GetComponent<Animator>().GetBool("attackUP"));
                    }
                    if (mentalAtk)
                    {
                        agent.TakeMentalDamage(targetCreature.metaInfo.mentalDmg);
                        agent.expMentalDamage += targetCreature.metaInfo.mentalDmg;

                        agentUpdated = true;
                    }
                }
            }

            if (mentalTick > 0)
            {
                agent.RecoverMental(mentalTick);
            }
            else if (mentalTick < 0)
            {
                agent.TakeMentalDamage(mentalTick);
            }

            targetCreature.AddFeeling(workValue);

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

    public static UseSkill InitUseSkillAction(SkillTypeInfo skillInfo, AgentModel agent, CreatureModel creature)
    {
        if (agent.target != null || creature.state != CreatureState.WAIT)
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

        string speech;
        agent.speechTable.TryGetValue("work_start", out speech);
        Notice.instance.Send("AddSystemLog", agent.name + " : " + speech);
        agentView.showSpeech.showSpeech(speech);

        creature.ShowNarrationText("move", agent.name);

        //agent.MoveToCreture(creature.gameObject);
        //agent.Working (creature.gameObject);
        //agent.Working(creature);
        //creature.ShowNarrationText("start", agent.name);

        inst.Init(skillInfo, agent, 10, skillInfo.amount, agent.workSpeed, skillInfo.amount); // 임시
        
        inst.agent = agent;
        inst.agentView = agentView;

        inst.targetCreature = creature;
        inst.targetCreatureView = creatureView;

        inst.skillTypeInfo = skillInfo;

        creature.state = CreatureState.WORKING;

        //관찰 조건을 위한 환상체 작업 횟수추가
        creature.workCount++;

        GameObject progressObj = Instantiate(Resources.Load<GameObject>("Prefabs/ProgressBar")) as GameObject;
        progressObj.transform.parent = creatureView.transform;
        progressObj.transform.localPosition = new Vector3(0, -0.7f, 0);

        inst.progressBar = progressObj.GetComponent<ProgressBar>();
        inst.progressBar.SetVisible(true);
        inst.progressBar.SetRate(0);

        Notice.instance.Send("UpdateCreatureState_" + inst.targetCreature.instanceId);

        return inst;
    }
}
