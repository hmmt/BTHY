using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NullCreature : CreatureBase, IAnimatorEventCalled {
    public enum NullState { 
        CREATURE,
        BROKEN,
        WORKER
    }

    public class NullWorkingEffTable {
        public class TableElement {
            public long id;
            public int index;

            public TableElement(long id, int index) {
                this.id = id;
                this.index = index;
            }
        }

        public static TableElement[] Dtype = new TableElement[5] { 
                new TableElement(1,1),
                new TableElement(2,3),
                new TableElement(3,4),
                new TableElement(4,2),
                new TableElement(5,3)
            };
        public static TableElement[] Itype = { 
                new TableElement(1,2),
                new TableElement(2,4),
                new TableElement(3,1),
                new TableElement(4,3),
                new TableElement(5,3)
            };
        public static TableElement[] Stype = { 
                new TableElement(1,3),
                new TableElement(2,3),
                new TableElement(3,2),
                new TableElement(4,1),
                new TableElement(5,5)
            };
        public static TableElement[] Ctype = { 
                new TableElement(1,2),
                new TableElement(2,1),
                new TableElement(3,4),
                new TableElement(4,4),
                new TableElement(5,3)
            };
        public static TableElement[] Default = { 
                new TableElement(1,1),
                new TableElement(2,5),
                new TableElement(3,4),
                new TableElement(4,4),
                new TableElement(5,2)
            };

    }

    UseSkill currentWorkingSkill = null;

    private static int changeCondition = 25;
    private static string transImage = "Unit/creature/NullThing";

    private bool changed = false;
    public NullState currentNullState = NullState.WORKER;
    private WorkerModel _currentWorker = null;
    public WorkerModel currentWorker {
        get {
            return _currentWorker;
        }
    }

    public List<WorkerModel> attackingList = new List<WorkerModel>();
    NullthingLevelController levelControl = null;
    NullthingAnim animScript = null;
    bool isEscaped = false;
    PassageObjectModel currentCreaturePassage = null;
    public bool timerHalt = false;
    CreatureTimer escapeTimer = new CreatureTimer(10f);
    public bool isChanging = false;
    CreatureTimer delayedChangeTimer = new CreatureTimer();
    CreatureTimer escapeCoolTimer = new CreatureTimer(60f);
    bool isEscapableState = true;

	public override void OnInit()
	{
        if (currentNullState != NullState.WORKER) {
            currentNullState = NullState.WORKER;
        }
        model.escapeType = CreatureEscapeType.WANDER;
        model.AddFeeling(1000);//make Max feeling

		//model.SetCurrentNode (MapGraph.instance.GetNodeById("sefira-malkuth-4"));
	}

    public void ActivateSkillRoom(UseSkill skill) {
        levelControl.Good.gameObject.SetActive(false);
        levelControl.Good = AgentLayer.currentLayer.GetAgent(skill.agent.instanceId).gameObject;
        levelControl.Good.gameObject.SetActive(true);
        levelControl.Change(skill.agent);
        this.ChangeToWoreker(skill.agent);
        skill.agent.SetUncontrollableAction(new Uncontrollable_Nullthing(skill.agent, this));
        model.AddFeeling(200f);
        this.model.GetMovableNode().Assign(skill.agent.GetMovableNode());
        (model.GetAnimScript() as NullthingAnim).isDisguised = true;
        SuppressWindow.currentWindow.nullEscapedList.Add(this);
        
        isEscapableState = false;
        escapeCoolTimer.TimerStop();
        escapeCoolTimer.TimerStart(true);
    }

    private void ChangeBody()
    {
        /*
        CreatureUnit unit = CreatureLayer.currentLayer.GetCreature(model.instanceId);
        unit.spriteRenderer.sprite = ResourceCache.instance.GetSprite("Sprites/" + transImage);
        unit.spriteRenderer.gameObject.transform.localScale = new Vector3(1, 1, 1);
         */
    }
    private void ChangeNormal()
    {
        /*
        CreatureUnit unit = CreatureLayer.currentLayer.GetCreature(model.instanceId);
        unit.spriteRenderer.sprite = ResourceCache.instance.GetSprite("Sprites/" + model.metaInfo.imgsrc);
        Texture2D tex = unit.spriteRenderer.sprite.texture;
        unit.spriteRenderer.gameObject.transform.localScale = new Vector3(200f / tex.width, 200f / tex.height, 1);
         */
    }

    private void ChangeToWoreker(WorkerModel targetModel) { 
        //currentWorker change
        //if escaped, make effect
        
        this._currentWorker = targetModel;
        targetModel.LoseControl();
        Debug.Log("Null Change to " + targetModel.name);
        model.AddFeeling(200f);
        targetModel.nullParasite = this;
        targetModel.GetControl();

        model.GetMovableNode().StopMoving();

        //null 검사 필요 
        model.GetMovableNode().SetCurrentNode(targetModel.GetMovableNode().currentNode);
        

        //원본 CreatureUnit 처리할 것
    }

    public override void OnEnterRoom(UseSkill skill)
    {
        this.currentWorkingSkill = skill;
        if (!isEscaped && currentNullState == NullState.CREATURE) {
            skill.PauseWorking();
            ActivateSkillRoom(skill);
            skill.agentView.puppetAnim.SetBool("Return", true);
            
            skill.agent.OnWorkEndFlag = true;

            //skill.agent.LoseControl();
        }
    }

    private void ChangeToCollapsed() {
        //smth effect
        float max = model.metaInfo.feelingMax;
        float current = model.feeling;
        float value = max * 0.6f;
        float sub = value - current;
        //escapeTimer.TimerStop();
        model.SubFeeling(-sub);
        
        if (_currentWorker != null)
        {
            _currentWorker.TakePhysicalDamage(100000, DamageType.NORMAL);
        }
    }

    public void ChangeToCreature() { 
        //worker (collapsed) -> creature
        (model.GetAnimScript() as NullthingAnim).isDisguised = false;
        float max = model.metaInfo.feelingMax;
        float current = model.feeling;
        float value = max * 0.3f;
        float sub = value - current;
        escapeTimer.TimerStop();
        model.SubFeeling(-sub);
    }

    public void DelayedChangeToCollapsed(float time) {
        this.delayedChangeTimer.TimerStart(time, true);

        if (this.currentWorkingSkill != null) {
            this.currentWorkingSkill.PauseWorking();
            this.currentWorkingSkill.FinishForcely();
            this.currentWorkingSkill = null;
        }

    }

    public void SubFeeling(float value) { 
        model.SubFeeling(value);
    }

    public override bool hasUniqueEscape()
    {
        return true;
    }

    public override void OnViewInit(CreatureUnit unit)
    {
        levelControl = model.GetAnimScript().gameObject.GetComponent<NullthingLevelController>();
        animScript = model.GetAnimScript() as NullthingAnim;
        levelControl.Init(this.model);

        AnimatorEventInit();
    }

    public override void OnFixedUpdate(CreatureModel creature)
    {
        CheckCreatureFeeling();
        if (isChanging) return;

        if (!isEscaped && currentNullState == NullState.CREATURE 
            && currentWorkingSkill == null 
            && isEscapableState)
        { 
            Escape();
            isEscapableState = false;
            escapeCoolTimer.TimerStart(true);
        }

        if (!isEscaped 
            && currentNullState == NullState.CREATURE 
            && isEscapableState == false) {
            if (escapeCoolTimer.TimerRun())
            {
                isEscapableState = true;
            }
        }

        if (_currentWorker != null) {
            model.GetMovableNode().Assign(_currentWorker.GetMovableNode());
        }
    }

    public override void OnReturn()
    {
        SuppressWindow.currentWindow.nullEscapedList.Remove(this);
        isEscaped = false;
    }

    void CheckCreatureFeeling() {
        if (isChanging) return;
        if (model.GetFeelingPercent() < 33f && this.currentNullState != NullState.CREATURE) {
            currentNullState = NullState.CREATURE;
            isChanging = true;
            levelControl.ChangeTransform();
            this.model.GetMovableNode().StopMoving();
            //model.GetAnimScript().SendMessage("Transform");
            //make message for eat agent
            //this.levelControl.Disable(this.currentNullState);
            return;
        }

        if (model.GetFeelingPercent() >= 33f && model.GetFeelingPercent() < 66f && this.currentNullState != NullState.BROKEN) {
            currentNullState = NullState.BROKEN;
            isChanging = true;
            levelControl.ChangeTransform();
            this.model.GetMovableNode().StopMoving();
            //model.GetAnimScript().SendMessage("Transform");
            ChangeToCollapsed();
            //this.levelControl.Disable(this.currentNullState);
            return;
        }

        if (model.GetFeelingPercent() >= 66f && this.currentNullState != NullState.WORKER) {

            currentNullState = NullState.WORKER;
            isChanging = true;
            levelControl.ChangeTransform();
            this.model.GetMovableNode().StopMoving();
            //model.GetAnimScript().SendMessage("Transform");
            //this.levelControl.Disable(this.currentNullState);
            return;
        }
    }

    public override SkillTypeInfo GetSpecialSkill()
    {
        if (changed)
        {
            // 임시
            return null;
            //return SkillTypeList.instance.GetData(400010);
        }
        else
        {
            return null;
        }
    }
    // 변신
    /*
    public override void OnFixedUpdate(CreatureModel creature)
    {
		// 
		
        if (creature.feeling <= changeCondition)
        {
            if (!changed)
            {
                ChangeBody();
                changed = true;
            }
        }
        else
        {
            if (changed)
            {
                ChangeNormal();
                changed = false;
            }
        }

        if (creature.feeling <= 0)
        {
            creature.Escape();
        }
        
        
		if (creature.energyPoint < 80)
		{
			//creature.Escape ();
		}
        
		//creature.Escape();
        
		foreach (AgentModel agent in AgentManager.instance.GetAgentList())
		{
			if (model.GetMovableNode ().GetPassage ()	== agent.GetMovableNode ().GetPassage ())
			{
				//Debug.Log ("same passage");
			}
		}

		MovableObjectNode movable = model.GetMovableNode ();

		MapNode curNode = movable.GetCurrentNode ();
		MapEdge curEdge = movable.GetCurrentEdge ();

		if(model.state == CreatureState.ESCAPE)
		{
			model.MoveToNode(MapGraph.instance.GetNodeById("sefira-malkuth-1"));
		}
    }
    */

    public void ActivateSkillOut(WorkerModel wm) {
        if (Prob(100))
        {
            //UIEffectManager.instance.Noise(8f);
            Debug.Log("Escape Activate Skill " + wm.name);
            ActivateSkillEscaped(wm);
            attackingList.Clear();

            if (currentCreaturePassage != null)
            {
                Sefira sefira = SefiraManager.instance.getSefira(currentCreaturePassage.GetSefiraName());
                foreach (AgentModel am in sefira.agentList)
                {
                    if (am == wm) continue;
                    am.TakeMentalDamage(Random.Range(1, 3));
                }
                foreach (OfficerModel om in sefira.officerList)
                {
                    if (om == wm) continue;
                    om.TakeMentalDamage(Random.Range(1, 3));
                }
            }
        }
        else {
            UIEffectManager.instance.Noise(5f);
        }
    }

    public override void UniqueEscape()
    {
        CheckCreatureFeeling();
        if (isChanging) {
            model.GetMovableNode().StopMoving();
            if (_currentWorker != null) {
                _currentWorker.GetMovableNode().StopMoving();

            }
            return;
        }
        currentCreaturePassage = model.GetMovableNode().GetPassage();

        if (_currentWorker != null) {
            model.GetMovableNode().Assign(_currentWorker.GetMovableNode());
        }

        //조건 수정필요
        if (model.GetCreatureCurrentCmd() == null && this.attackingList.Count == 0) {
            MakeMovement();
        }

        if (this.delayedChangeTimer.TimerRun()) {
            delayedChangeTimer.TimerStop();
            ChangeToCollapsed();
            escapeTimer.TimerStop();
            escapeTimer.TimerStart(false);
        }

        if (currentNullState == NullState.CREATURE)
        {
            if (AttackNearAgent()) {
                Debug.Log("Null Attack " + this.attackingList[0].name);
                model.PursueWorker(attackingList[Random.Range(0, attackingList.Count)]);
            }

            /*
            if (attackingList.Count != 0)
            {
                WorkerModel target = null;
                foreach (WorkerModel wm in attackingList) {
                    if (wm.isDead()) {
                        target = wm;
                        break;
                    }
                }

                if (target != null) {
                    if (Prob(33)) {
                        ActivateSkillEscaped(target);
                        attackingList.Clear();
                    }
                }
            }
            else if (AttackNearAgent())
            {
                Debug.Log("Null Attack" + this.attackingList[0].name);
                model.PursueWorker(this.attackingList[0]);
                //ActivateSkillEscaped(this.attackingList[0]);
            }
            */
        }



        if (this.currentNullState == NullState.WORKER)
        {
            if (this._currentWorker is AgentModel)
            {
                if ((this._currentWorker as AgentModel).GetState() == AgentAIState.IDLE)
                {
                    if (escapeTimer.TimerRun()) {
                        if (Prob(100)) {
                            Debug.Log("Change Collapsed");
                            ChangeToCollapsed();
                            escapeTimer.TimerStart(false);
                        }
                    }
                }
            }
        }
        else if (this.currentNullState == NullState.BROKEN)
        {
            if (escapeTimer.TimerRun())
            {
                if (Prob(100))
                {
                    Debug.Log("Change NullCreature");
                    ChangeToCreature();
                }
                else
                {
                    Debug.Log("Fail to change");
                }
            }
        }

    }

    public void ActivateSkillEscaped(WorkerModel targetWorker)
    {
        //levelControl.Good.gameObject.SetActive(false);
        //levelControl.Good = AgentLayer.currentLayer.GetAgent(skill.agent.instanceId).gameObject;
        //levelControl.Good.gameObject.SetActive(true);
        //levelControl.Change(skill.agent);
        //this.ChangeToWoreker(skill.agent);
       // skill.agent.SetUncontrollableAction(new Uncontrollable_Nullthing(skill.agent, this));
        //model.AddFeeling(200f);

        levelControl.Good.gameObject.SetActive(false);
        if (targetWorker is AgentModel) {
            levelControl.Good = AgentLayer.currentLayer.GetAgent(targetWorker.instanceId).gameObject;
            
        }
        else if (targetWorker is OfficerModel) {
            levelControl.Good = OfficerLayer.currentLayer.GetOfficer(targetWorker.instanceId).gameObject;
        }
        ChangeToWoreker(targetWorker);
        levelControl.Change(targetWorker);
        model.AddFeeling(200f);
        model.GetMovableNode().Assign(targetWorker.GetMovableNode());
        (model.GetAnimScript() as NullthingAnim).isDisguised = true;
        escapeTimer.TimerStart(false);
    }

    public void Escape() {
        //Debug.Log("간다");
        model.StopEscapeWork();
        //Make movement
        MakeMovement();
        isEscaped = true;
        //escapeTimer.TimerStart(false);
        SuppressWindow.currentWindow.nullEscapedList.Add(this);
        //Debug.Log(SuppressWindow.currentWindow.nullEscapedList.Count);
    }

    int former = -1;
    public void MakeMovement()
    {
        if (_currentWorker != null) return;
        Debug.Log("MakeMovement >> ");
        int randVal;
        MapNode node;
        MapNode[] nodes = null;
        while (former == (randVal = UnityEngine.Random.Range(0, model.sefira.departmentNum + 1))) ;
        former = randVal;
        if (randVal == model.sefira.departmentNum)
        {
            nodes = MapGraph.instance.GetSefiraNodes(model.sefira);
        }
        else
        {
            nodes = MapGraph.instance.GetAdditionalSefira(model.sefira);
        }

        node = nodes[UnityEngine.Random.Range(0, nodes.Length)];
        model.MoveToNode(node);
    }

    public bool AttackNearAgent() {
        attackingList.Clear();
        List<WorkerModel> nearTarget = new List<WorkerModel>();
        /*
        OfficerModel[] detectedOfficer = OfficeManager.instance.GetNearOfficers(model.GetMovableNode());
        if (detectedOfficer.Length == 0)
        {
            AgentModel[] detectedAgent = AgentManager.instance.GetNearAgents(model.GetMovableNode());
            if (detectedAgent.Length == 0)
            {
                return false;
            }
            else
            {
                foreach (AgentModel am in detectedAgent)
                {
                    if (am.isDead()) continue;
                    nearTarget.Add(am);
                }
            }
        }
        else {
            foreach (OfficerModel om in detectedOfficer) {
                if (om.isDead()) continue;
                nearTarget.Add(om);
            }    
        }
        */

        AgentModel[] detectedAgent = AgentManager.instance.GetNearAgents(model.GetMovableNode());
        if (detectedAgent.Length == 0)
        {
            return false;
        }
        else
        {
            foreach (AgentModel am in detectedAgent)
            {
                if (am.isDead()) {
                    Debug.Log("this is dead " + am.name);
                }
                nearTarget.Add(am);
            }
        }

        foreach (WorkerModel wm in nearTarget) {
            this.attackingList.Add(wm);   
        }

        return true;
    }

    public CreatureModel GetModel()
    {
        return model;
    }

    public override bool hasUniqueFinish()
    {
        return true;
    }

    public override void UniqueFinish(UseSkill skill)
    {
        NullWorkingEffTable.TableElement[] currentTable = null;
        if (this._currentWorker is AgentModel)
        {
            switch ((this._currentWorker as AgentModel).agentLifeValue) {
                case PersonalityType.D:
                    currentTable = NullWorkingEffTable.Dtype;
                    break;
                case PersonalityType.I:
                    currentTable = NullWorkingEffTable.Itype;
                    break;
                case PersonalityType.S:
                    currentTable = NullWorkingEffTable.Stype;
                    break;
                case PersonalityType.C:
                    currentTable = NullWorkingEffTable.Ctype;
                    break;
                default:
                    currentTable = NullWorkingEffTable.Default;
                    break;
            }
        }
        else {
            currentTable = NullWorkingEffTable.Default;
        }
        if (skill.skillTypeInfo.id > 5) {
            Debug.Log("NullCreatre cannot has unique skill in current");
        }
        Debug.Log((int)skill.skillTypeInfo.id);
        NullWorkingEffTable.TableElement currentElement = currentTable[(int)skill.skillTypeInfo.id];
        //정확한 수치 계산식이 필요합니다
        float scale = 2 - (0.5f * (currentElement.index));
        
        if (currentElement.index == 3) {
            scale -= 0.5f;

        }
        else if (currentElement.index > 3) {
            scale -= 1f;
        }

        //float energyAdded = skill.agent.GetEnergyAbility(skill.skillTypeInfo) * skill.successCount / skill.totalTickNum;

        if (currentElement.index < 3) {
            SetCurrentSkillResult(0);
            model.AddFeeling(skill.skillTypeInfo.amount * skill.successCount / skill.totalTickNum * scale);
        }
        else if (currentElement.index > 3) {
            SetCurrentSkillResult(2);
            model.SubFeeling( skill.skillTypeInfo.amount * skill.successCount / skill.totalTickNum * scale);
        }
        MakeEffect(skill.targetCreatureView.room);
        ResetCurrentSkillResult();

        //model.SetFeelingChange(5, skill.skillTypeInfo.amount * skill.successCount / skill.totalTickNum * scale);
        
    }

    void EndTransform() {
        //levelControl.Appear(currentNullState);
        levelControl.Disable(currentNullState);
        if (_currentWorker != null)
        {
            if (currentNullState == NullState.WORKER) {
                _currentWorker.hp = _currentWorker.maxHp;
            }
            else if(currentNullState == NullState.BROKEN){
                _currentWorker.TakePhysicalDamage(100000, DamageType.NORMAL);
                _currentWorker = null;
            }
        }
    }

    void DeleteOlder() {
        isChanging = false;
        levelControl.EndTransform(); 
        
    }

    public void OnCalled()
    {
    }

    public void OnCalled(int i)
    {
        if (i == 0) {
            EndTransform();
        }
        else if (i == 1) {
            DeleteOlder();
        }
    }

    public void AnimatorEventInit()
    {
    }

    public void AgentReset() { }
}
