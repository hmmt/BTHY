using UnityEngine;
using System.Collections;

public class ObserveAction : ActionClassBase {
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

	public ProgressBar progressBar;
	public AgentModel agent;
	public AgentUnit agentView;

	public CreatureModel targetCreature;
	public CreatureUnit targetCreatureView;
	public IsolateRoom room;

	/***
     * 현재 작업이 진행중인지 확인하는 변수.
     * 타이포 등에 의해서 작업이 일시정지되어 false가 될 수도 있다.
     * 
     ***/
	private bool workPlaying = true;


	//private bool narrationPart5 = false;

	private bool finished = false;

	void OnDisable()
	{
		if (finished == false)
		{
			Release();
		}
	}

	public void Init(AgentModel agent, int work, float speed)
	{
		workCount = 0;
		//workSpeed = speed;
		totalTickNum = 10;
		workSpeed = 0.5f;

		//tickInterval = totalWork / totalTickNum;
	}
	public void FixedUpdate()
	{
		CheckLive();
		if (finished)
			return;	

		ProgressWork();

		CheckLive();
		if (finished)
			return;


		//check work end and state changed to pile checking

		if (workCount >= totalTickNum && agent.OnWorkEndFlag)
		{
			targetCreature.ShowNarrationText("finish", agent.name);

			agent.OnWorkEndFlag = false;

			targetCreature.observeProgress++;

			/*
			float prob = 0.6f + agent.level * 0.05f - targetCreature.metaInfo.level * 0.05f
				+ targetCreature.GetObservationFailureCount() * 0.05f;

			Debug.Log ("agent level : " + agent.level + ", creature : " + targetCreature + ", prob : " + prob); 
			*/

			float prob = 0.6f + agent.level * 0.05f 
				+ targetCreature.GetObservationFailureCount() * 0.05f;

			Debug.Log ("agent level : " + agent.level + ", prob : " + prob); 

			switch(targetCreature.observeProgress)
			{
			case 0:
				prob = prob;
				break;
			case 1:
				prob = prob * 0.9f;
				break;
			case 2:
				prob = prob * 0.85f;
				break;
			case 3:
				prob = prob * 0.78f;
				break;
			case 4:
				prob = prob * 0.7f;
				break;
			}
			//prob = prob 

			if (Random.value <= prob)
				SucceedObservation ();
			else
				FailObservation ();

			return;
		}
		if (workPlaying && IsWorkingState())
		{	
			workProgress += Time.deltaTime * workSpeed;
		}
	}

	public bool IsWorkingState()
	{
		if (agent.GetCurrentCommandType() == AgentCmdType.OBSERVE_CREATURE)
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
	private void AddSkillTrait(long skillTypeId)
	{
		string traitNarration;
		TraitTypeInfo traitTypeInfo = TraitTypeList.instance.GetTraitWithId(skillTypeId);
		agent.applyTrait(traitTypeInfo);
		traitNarration = agent.name + "( 이)가 " + traitTypeInfo.name + " 특성을 획득하였습니다.";
		Notice.instance.Send("AddSystemLog", traitNarration);
	}


	public void PauseWorking()
	{
		workPlaying = false;
	}


	public void PauseWorking(float time)
	{
		workPlaying = false;
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
		agent.FinishWorking();
		if(targetCreature.state == CreatureState.OBSERVE)
			targetCreature.state = CreatureState.WAIT;

		targetCreature.currentSkill = null;
		//targetCreature.bufRemainingTime = 5f;
	}

	private void SucceedObservation()
	{
		targetCreature.AddObservationFailureCount ();
		FinishWork ();
	}
	 
	private void FailObservation()
	{
		targetCreature.ResetObservationFailureCount ();
		FinishWork ();
	}

	private void FinishWork()
	{
		finished = true;

		Release();
		Notice.instance.Send("UpdateCreatureState_" + targetCreature.instanceId);

		Destroy(gameObject);
		Destroy(progressBar.gameObject);
	}

	public void FinishForcely()
	{
		FinishWork ();
	}

	private void ProcessWorkTick()
	{
		//Debug.Log ("Tick... "+workCount);
		// 
		if (workPlaying)
		{
			//float workProb = agent.GetSuccessProb (skillTypeInfo);

			if (workCount == 5)
			{
				float attackProb = targetCreature.GetAttackProb ();

				if (Random.value <= attackProb) {
					targetCreature.AddAttackCount ();

					Debug.Log ("observe attack");

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
				else
				{
					Debug.Log ("fail attack");
				}
			}
			CheckLive ();
		}
	}

	public void CheckLive()
	{
		if (agent.mental <= 0)
		{
			//FinishWork();

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

			/*
			string narration = agent.name + " (이)가 공황에 빠져 " + skillTypeInfo.name + " 작업에 실패하였습니다.";
			Notice.instance.Send("AddSystemLog", narration);
			*/
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
			FinishWork();
			/*
			string narration = this.name + " (이)가 사망하여 안타깝게도 " + skillTypeInfo.name + " 작업에 실패하였습니다.";
			Notice.instance.Send("AddSystemLog", narration);
			*/
		}
	}

	public bool IsFinished()
	{
		return finished;
	}

	public static ObserveAction InitUseSkillAction(AgentModel agent, CreatureModel creature)
	{
		if (creature.state != CreatureState.WAIT)
		{
			return null;
		}
		GameObject newObject = new GameObject();
		newObject.name = "ObserveAction";

		ObserveAction inst = newObject.AddComponent<ObserveAction>();

		AgentUnit agentView = AgentLayer.currentLayer.GetAgent(agent.instanceId);
		CreatureUnit creatureView = CreatureLayer.currentLayer.GetCreature(creature.instanceId);

		agentView.showSkillIcon.turnOnDoingSkillIcon(true);
		//agentView.showSkillIcon.showDoingSkillIcon(skillInfo, agent);

		agentView.puppetAnim.speed = 6.0f;
		agentView.puppetAnim.SetBool("Memo", true);


		string speech;
		agent.speechTable.TryGetValue("work_start", out speech);
		Notice.instance.Send("AddSystemLog", agent.name + " : " + speech);
		agentView.showSpeech.showSpeech(speech);

		creature.ShowNarrationText("move", agent.name);

		inst.Init(agent, 10, agent.workSpeed);

		inst.agent = agent;
		inst.agentView = agentView;

		inst.targetCreature = creature;
		inst.targetCreatureView = creatureView;

		creature.state = CreatureState.OBSERVE;
		//creature.currentSkill = inst;

		creature.manageDelay = 21;


		GameObject progressObj = Instantiate(Resources.Load<GameObject>("Prefabs/EnergyBar")) as GameObject;
		//progressObj.transform.parent = creatureView.transform;
		//progressObj.transform.localPosition = new Vector3(0, -0.7f, 0);
		progressObj.transform.SetParent(inst.targetCreatureView.room.transform.GetChild(0).transform);
        progressObj.transform.localPosition = Vector3.zero;
		progressObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1.65f, 0.874f);

		inst.progressBar = progressObj.GetComponent<ProgressBar>();
        inst.progressBar.currentSprite = SefiraController.instance.GetSefiraSprite(creature.sefira).Energy;
		inst.progressBar.SetVisible(true);
		inst.progressBar.SetRate(0);
		inst.progressBar.transform.localScale = new Vector3(0.7692308f, 0.7692308f, 1f);
		float posy = inst.targetCreatureView.transform.localPosition.y;
		posy = 1 + posy;
        //Vector3 defPos = inst.progressBar.GetComponent<RectTransform>().localPosition = Vector3.zero;
        //defPos = new Vector3(-3.251f, -0.354f, -5f);
        //inst.progressBar.GetComponent<RectTransform>().localPosition = Vector3.zero;
		//inst.progressBar.GetComponent<RectTransform>().anchoredPosition = new Vector2(-3.251f, -0.354f);
		//inst.agent.currentSkill = inst;
		Notice.instance.Send("UpdateCreatureState_" + inst.targetCreature.instanceId);

		return inst;
	}
}
