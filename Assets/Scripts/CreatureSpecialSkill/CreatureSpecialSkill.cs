using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class CreatureSpecialSkill : IObserver{
    public CreatureModel model;
    public Sefira sefira;

    public bool Activated = false;

    public CreatureSpecialSkill() { }
    
    public CreatureSpecialSkill(CreatureModel model) {
        this.model = model;
        Notice.instance.Observe(NoticeName.FixedUpdate, this);
    }

    public virtual void SkillActivate() { 
        
    }

    public virtual void FixedUpdate() { 
        
    }

    public virtual void OnStageStart() { 
        
    }

    public virtual void Activate() {
        if (this.Activated == true) {
            return;
        }
        this.Activated = true;
    }

    public virtual void DeActivate() {
        if (this.Activated == false) {
            return;
        }
        this.Activated = false;
    }

    public void OnNotice(string notice, params object[] param)
    {
        if (this.Activated) {
            if (notice == NoticeName.FixedUpdate)
            {
                FixedUpdate();
            }
        }
    }
}

public class RedShoesSkill : CreatureSpecialSkill, IObserver{
	public WorkerModel attractTargetAgent;
	List<WorkerModel> targetList;
    const float frequencey = 5f;
    float elapsed = 0f;
	bool attracted = false;//직원이 환상체에 매혹당했는가
    bool isAcquired = false;//직원이 빨간구두를 습득하였는가

	// 나중에 분리


    public RedShoesSkill(CreatureModel model) {
        this.model = model;
        //this.targetList = GetTargetList();
        Notice.instance.Observe(NoticeName.FixedUpdate, this);
    }

	private List<WorkerModel> GetTargetList() {
		List<WorkerModel> output = new List<WorkerModel>();
        
		foreach (OfficerModel om in this.sefira.officerList) {
			if (om.gender == "Female") {
				output.Add(om);
			}
		}
        foreach (AgentModel am in this.sefira.agentList) {
            if (am.gender == "Female") {
                output.Add(am);
            }
        }
        if (output.Count == 0) {
            Debug.Log("Female is not exist");
        }
        return output;
    }
    
    public override void FixedUpdate()
    {
		//return;

		RedShoes shoes = (RedShoes)model.script;

		if (shoes.dropped && !this.attracted)
		{
			foreach (AgentModel agent in AgentManager.instance.GetAgentList())
			{
				if (agent.isDead ())
					continue;
				if (agent.gender == "Female")
				//if (agent.GetMovableNode ().GetPassage () == droppedPassage)
				{
					if ((agent.GetCurrentViewPosition () - shoes.droppedShoesPosition).sqrMagnitude < 2)
					{	
						Debug.Log ("infect!!!!");
						Attract (agent);
						GetRedShoes (2);

						shoes.dropped = false;
						break;
					}
				}
				else
				{
					//if()
					{
					}
				}
			}
		}
		else if (this.attracted)
        {
            //Call targeted Agent to Creature room and try 
			if (this.attractTargetAgent != null && !isAcquired)
			{
				if (this.attractTargetAgent is AgentModel)
				{
					AgentModel agent = (AgentModel)this.attractTargetAgent;
					if (agent.GetState() != AgentAIState.CANNOT_CONTROLL)
					{
						MapNode destination = model.GetWorkspaceNode ();
						this.attractTargetAgent.LoseControl ();
						this.attractTargetAgent.MoveToNode (destination);
					}

					if (this.attractTargetAgent.GetCurrentNode () == model.GetWorkspaceNode ())
					{
						GetRedShoes (1);
					}
				}
				else
				{
					OfficerModel officer = (OfficerModel)this.attractTargetAgent;
					if (officer.GetState() != OfficerAIState.CANNOT_CONTROLL)
					{
						MapNode destination = model.GetWorkspaceNode ();
						this.attractTargetAgent.LoseControl ();
						this.attractTargetAgent.MoveToNode (destination);
					}

					if (this.attractTargetAgent.GetCurrentNode () == model.GetWorkspaceNode ())
					{
						GetRedShoes (1);
					}
				}
			}
			else
			{
				
			}
        }
        else
		{
            elapsed += Time.deltaTime;
            if (elapsed > frequencey)
            {
                elapsed = 0f;
                TryAttract();
            }
        }
        
    }

	public void FreeAttractedAgent(WorkerModel target)
	{
		target.GetControl ();

		this.attracted = false;
		this.attractTargetAgent = null;
		this.isAcquired = false;

		if (target is AgentModel)
		{
			AgentUnit agentView = AgentLayer.currentLayer.GetAgent (attractTargetAgent.instanceId);

			AnimatorManager.instance.ChangeAnimatorByID (attractTargetAgent.instanceId, attractTargetAgent.instanceId,
				agentView.puppetAnim, false, false);

			//agentView.SetAnimatorChanged (false);
		}
		else
		{
			OfficerUnit officerView = OfficerLayer.currentLayer.GetOfficer (attractTargetAgent.instanceId);

			AnimatorManager.instance.ChangeAnimatorByID (attractTargetAgent.instanceId, attractTargetAgent.instanceId,
				officerView.puppetAnim, false, false);

			//officerView.SetAnimatorChanged (false);
		}


	}

	public void OnInfectedTargetTerminated()
	{
		if (!isAcquired)
		{
			Debug.Log ("OnInfectedTargetTerminated() : Invalid Access");
			return;
		}
		RedShoes shoes = (RedShoes)model.script;

		shoes.dropped = true;
		shoes.droppedPassage = attractTargetAgent.GetMovableNode ().GetPassage ();
		shoes.droppedShoesPosition = attractTargetAgent.GetCurrentViewPosition ();
		MovableObjectNode node = new MovableObjectNode ();
		node.Assign (attractTargetAgent.GetMovableNode ());
		shoes.droppedPositionNode = node;

		attracted = false;
		isAcquired = false;
	}

    public override void OnStageStart()
    {
        this.sefira = model.sefira;
        //Debug.Log(this.sefira.name);
        this.targetList = GetTargetList();
    }
    
    public override void SkillActivate()
    {
        Debug.Log("attracted " + this.attractTargetAgent.name);
    }

    private void TryAttract() {
        WorkerModel target = null;
        if (this.targetList.Count == 0) return;
        int randIndex = UnityEngine.Random.Range(0, this.targetList.Count);
        target = targetList[randIndex];
        /*
            20%확률로 매혹 판정
         */
        float randval = UnityEngine.Random.Range(0, 5);
        //if (randval == 0)
		if(true)
        {
            Debug.Log("걸림" + randval);
            this.attractTargetAgent = target;
            this.attracted = true;

			Attract (target);
            return;
        }
        else {

            Debug.Log("안걸림" + randval);
        }
    }

	private void Attract(WorkerModel target)
	{
		this.attractTargetAgent = target;
		this.attracted = true;

		if (target is AgentModel)
		{
			AgentUnit agentView = AgentLayer.currentLayer.GetAgent (attractTargetAgent.instanceId);

			AnimatorManager.instance.ResetAnimatorTransform (attractTargetAgent.instanceId);
			AnimatorManager.instance.ChangeAnimatorByName (attractTargetAgent.instanceId, AnimatorName.RedShoes_attract,
				agentView.puppetAnim, true, false);

			target.LoseControl ();
			target.SetUncontrollableAction (new Uncontrollable_RedShoesAttract (target, this));

			agentView.puppetAnim.SetBool ("Notice", true);
		}
		else
		{
			OfficerUnit officerView = OfficerLayer.currentLayer.GetOfficer (attractTargetAgent.instanceId);

			AnimatorManager.instance.ResetAnimatorTransform (attractTargetAgent.instanceId);
			AnimatorManager.instance.ChangeAnimatorByName (attractTargetAgent.instanceId, AnimatorName.RedShoes_attract,
				officerView.puppetAnim, true, false);

			target.LoseControl ();
			target.SetUncontrollableAction (new Uncontrollable_RedShoesAttract (target, this));

			officerView.puppetAnim.SetBool ("Notice", true);
		}
	}

	public void GetRedShoes(int startType) {
        if (isAcquired) {
            return;
        }
        isAcquired = true;
        //attractTargetAgent.MoveToNode(this.model.GetWorkspaceNode());
        //도착하면 animator change
		Debug.Log("Get Red Shoes");
		// TODO: motion 

		if (attractTargetAgent is AgentModel)
		{
			AgentUnit agentView = AgentLayer.currentLayer.GetAgent (attractTargetAgent.instanceId);

			AnimatorManager.instance.ResetAnimatorTransform (attractTargetAgent.instanceId);
			AnimatorManager.instance.ChangeAnimatorByName (attractTargetAgent.instanceId, AnimatorName.RedShoes_infected,
				agentView.puppetAnim, true, false);
		}
		else
		{
			OfficerUnit officerView = OfficerLayer.currentLayer.GetOfficer (attractTargetAgent.instanceId);

			AnimatorManager.instance.ResetAnimatorTransform (attractTargetAgent.instanceId);
			AnimatorManager.instance.ChangeAnimatorByName (attractTargetAgent.instanceId, AnimatorName.RedShoes_infected,
				officerView.puppetAnim, true, false);
		}
		//agentView.SetAnimatorChanged (true);

		attractTargetAgent.SetUncontrollableAction(new Uncontrollable_RedShoes(attractTargetAgent, this, startType));

		model.SendAnimMessage ("GetRedShoesAnim");
        //감염행동 시작 -> AgentModel에서 처리해야 할 듯?
        //행동은 DISC 타입에서 다른 직원을 살해하는 패닉 패턴을 이용하면 그럭저럭 작업을 줄일 수 있지 않을까
        //직원 처리는 패닉으로 처리하고, 해당 직원은 패닉 상태에서 제압될 경우 무조건 사망 판정으로 만들 것
        //StartAttractedBehaviour()->이런 식으로?
    }

    /*
        환상체에 지배당하는 직원을 제압하였을 때 호출
     */
    public void SetSuppressed(List<AgentModel> suppressors) {
        AgentModel nextTarget = null;

        foreach (AgentModel am in suppressors) {
            if (am.gender == "Female") {
                nextTarget = am;

                /*
                 여기서도 판정을 걸어야하는가? 무조건 감염되는 상태인가?
                 */
            }
        }

        if (nextTarget == null) {
            this.attracted = false;
            /*
                운반? 되는 도중에도 계속해서 유혹을 시도해야 하는가?
             */
        }

    }

    void IObserver.OnNotice(string notice, params object[] param)
    {
        base.OnNotice(notice, param);
    }
}
