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
    bool isPlayingDeadScene = false;


    const string changeSoundKey0 = "change0";
    const string changeSoundKey1 = "change1";
    const string changeSoundKey2 = "change2";
    const string waitSoundKey0 = "wait0";
    const string waitSoundKey1 = "wait1";
    const string waitSoundKey2 = "wait2";
    const string waitSoundKey3 = "wait3";
    const string deadsceneSoundKey0 = "deadscene0";
    const string deadsceneSoundKey1 = "deadscene1";
    const string transformingSoundKey = "transforming";
    const string suppressedSoundKey = "suppressed";

    public NullState currentNullState = NullState.WORKER;
    private WorkerModel _currentWorker = null;
    public WorkerModel currentWorker {
        get {
            return _currentWorker;
        }
    }
    public PersonalityType currentLifeType = PersonalityType.C;
    WorkerModel recentWorker = null;

    public List<WorkerModel> attackingList = new List<WorkerModel>();
    List<WorkerModel> currentAttackTarget = new List<WorkerModel>();
    NullthingLevelController levelControl = null;
    NullthingAnim animScript = null;
    bool isEscaped = false;
    PassageObjectModel currentCreaturePassage = null;
    public bool timerHalt = false;
    CreatureTimer escapeTimer = new CreatureTimer(10f);
    public bool isChanging = false;
    CreatureTimer delayedChangeTimer = new CreatureTimer();
    CreatureTimer escapeCoolTimer = new CreatureTimer(60f);
    CreatureTimer staySoundTimer = new CreatureTimer();
    bool isEscapableState = true;

    CreatureUnit _nullUnit = null;

	public override void OnInit()
	{
        if (currentNullState != NullState.WORKER) {
            currentNullState = NullState.WORKER;
        }
        model.escapeType = CreatureEscapeType.WANDER;
        model.AddFeeling(1000);//make Max feeling
        staySoundTimer.TimerStart(Random.Range(10f, 30f), true);
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
        if (targetWorker is AgentModel)
        {
            levelControl.Good = AgentLayer.currentLayer.GetAgent(targetWorker.instanceId).gameObject;

        }
        else if (targetWorker is OfficerModel)
        {
            levelControl.Good = OfficerLayer.currentLayer.GetOfficer(targetWorker.instanceId).gameObject;
        }
        ChangeToWoreker(targetWorker);
        levelControl.Change(targetWorker);
        model.AddFeeling(200f);
        model.GetMovableNode().Assign(targetWorker.GetMovableNode());
        (model.GetAnimScript() as NullthingAnim).isDisguised = true;
        escapeTimer.TimerStart(false);
        UIEffectManager.instance.ActivateUIEffect("BlackOut", 1f, 0, false);

        _nullUnit.PlaySound(changeSoundKey0);
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
        recentWorker = targetModel;
        if (recentWorker is AgentModel) {
            currentLifeType = (recentWorker as AgentModel).agentLifeValue;
        }
        
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
        AnimatorEventInit();
        levelControl.Init(this.model);

        _nullUnit = CreatureLayer.currentLayer.GetCreature(this.model.instanceId);

        AnimatorEventInit();
    }

    public override void OnFixedUpdate(CreatureModel creature)
    {
        if (isPlayingDeadScene) return;
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

        if (!isEscaped) {
            if (staySoundTimer.TimerRun()) {
                int randVal = Random.Range(0, 4);
                switch (randVal) { 
                    case 0:
                        _nullUnit.PlaySound(waitSoundKey0);
                        break;
                    case 1:
                        _nullUnit.PlaySound(waitSoundKey1);
                        break;
                    case 2:
                        _nullUnit.PlaySound(waitSoundKey2);
                        break;
                    default:
                        _nullUnit.PlaySound(waitSoundKey3);
                        break;
                }

                staySoundTimer.TimerStart(Random.Range(10f, 30f), true);
            }
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
            levelControl.ChangeTransform(1f);
            this.model.GetMovableNode().StopMoving();
            //_currentWorker = null;
            //model.GetAnimScript().SendMessage("Transform");
            //make message for eat agent
            //this.levelControl.Disable(this.currentNullState);
            return;
        }

        if (model.GetFeelingPercent() >= 33f && model.GetFeelingPercent() < 66f && this.currentNullState != NullState.BROKEN) {
            currentNullState = NullState.BROKEN;
            isChanging = true;
            levelControl.ChangeTransform(1f);
            this.model.GetMovableNode().StopMoving();
            //model.GetAnimScript().SendMessage("Transform");
            ChangeToCollapsed();
            //this.levelControl.Disable(this.currentNullState);
            return;
        }

        if (model.GetFeelingPercent() >= 66f && this.currentNullState != NullState.WORKER) {

            currentNullState = NullState.WORKER;
            isChanging = true;
            levelControl.ChangeTransform(3f);
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
        if (Prob(50))
        {
            //UIEffectManager.instance.Noise(8f);
            Debug.Log("Escape Activate Skill " + wm.name);
            wm.GetMovableNode().StopMoving();
            this.model.GetMovableNode().StopMoving();
            wm.HaltUpdate();
            ActivateSkillEscaped(wm);
            attackingList.Clear();

            if (currentCreaturePassage != null)
            {
                Sefira sefira = SefiraManager.instance.GetSefira(currentCreaturePassage.GetSefiraName());
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
            //DeadScene

            this.model.GetMovableNode().StopMoving();
            wm.HaltUpdate();

            DeadScene(wm);

            //UIEffectManager.instance.Noise(5f);
           // UIEffectManager.instance.ActivateUIEffect("BlackOut", 1f, false);
            UIEffectManager.instance.NoiseScreen(2f, 30);
        }
    }

    void DeadScene(WorkerModel wm) {
        this.animScript.animator.SetBool("Killcam", true);
        this.animScript.animator.SetBool("Special", false);

        Animator animTarget = null;
        float TargetPos = wm.GetMovableNode().GetCurrentViewPosition().x;
        if (wm is AgentModel)
        {
            animTarget = AgentLayer.currentLayer.GetAgent(wm.instanceId).puppetAnim;
           
        }
        else {
            animTarget = OfficerLayer.currentLayer.GetOfficer(wm.instanceId).puppetAnim;
        }

        AnimatorManager.instance.ResetAnimatorTransform(wm.instanceId);
        AnimatorManager.instance.ChangeAnimatorByID(wm.instanceId, AnimatorName.id_Nullthing_AgentCTRL, animTarget, true, false);

        if (TargetPos < this.model.GetMovableNode().GetCurrentViewPosition().x)
        {
            this.model.GetMovableNode().SetDirection(UnitDirection.LEFT);
        }
        else {
            this.model.GetMovableNode().SetDirection(UnitDirection.RIGHT);        
        }

        this.isPlayingDeadScene = true;

    }

    public override void UniqueEscape()
    {
        CheckCreatureFeeling();
        if (isPlayingDeadScene) return;
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
            this.currentAttackTarget.Clear();
            if (AttackNearAgent()) {
                int index = Random.Range(0, attackingList.Count);
                Debug.Log("Null Attack " + this.attackingList[index].name);
                model.PursueWorker(attackingList[index]);
                //방향 뒤집자
                /*
                WorkerModel target = attackingList[index];
                float targetX = target.GetMovableNode().GetCurrentViewPosition().x;
                float modelX = this.model.GetMovableNode().GeftCurrentViewPosition().x;

                
                if (modelX < targetX)
                {
                    model.GetMovableNode().SetDirection(UnitDirection.RIGHT);
                }
                else {
                    model.GetMovableNode().SetDirection(UnitDirection.LEFT);
                }
                */
                
                this.currentAttackTarget.Add(attackingList[index]);
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
                if (am.isDead())
                {
                    Debug.Log("this is dead " + am.name);
                }
                else {
                    nearTarget.Add(am);
                }
                
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
        if (this.recentWorker is AgentModel && this.currentNullState != NullState.CREATURE)
        {
            Debug.Log("Current Type: " + currentLifeType);
            switch (currentLifeType)
            {
                case PersonalityType.D:
                    currentTable = NullWorkingEffTable.Dtype;
                    //Debug.Log("D");
                    break;
                case PersonalityType.I:
                    currentTable = NullWorkingEffTable.Itype;
                    //Debug.Log("I");
                    break;
                case PersonalityType.S:
                    currentTable = NullWorkingEffTable.Stype;
                    //Debug.Log("S");
                    break;
                case PersonalityType.C:
                    currentTable = NullWorkingEffTable.Ctype;
                    //Debug.Log("C");
                    break;
                default:
                    currentTable = NullWorkingEffTable.Default;
                    //Debug.Log("Default");
                    break;
            }
        }
        else {
            currentTable = NullWorkingEffTable.Default;
            Debug.Log("Default");
        }
        if (skill.skillTypeInfo.id > 5) {
            Debug.Log("NullCreatre cannot has unique skill in current");
        }
        
        NullWorkingEffTable.TableElement currentElement = currentTable[(int)skill.skillTypeInfo.id];
        //정확한 수치 계산식이 필요합니다
        float scale = 2 - (0.5f * (currentElement.index));
        Debug.Log("value "+currentElement.index);

        if (currentElement.index == 3) {
            scale -= 0.5f;

        }
        else if (currentElement.index > 3) {
            scale -= 1f;
        }

        //float energyAdded = skill.agent.GetEnergyAbility(skill.skillTypeInfo) * skill.successCount / skill.totalTickNum;
        int result = 0;
        if (currentElement.index < 3) {
            SetCurrentSkillResult(0);
            model.AddFeeling(skill.skillTypeInfo.amount * skill.successCount / skill.totalTickNum * scale);
            result = 0;
        }
        else if (currentElement.index > 3) {
            SetCurrentSkillResult(2);
            model.SubFeeling( skill.skillTypeInfo.amount * skill.successCount / skill.totalTickNum * scale);
            result = 2;
        }
        MakeEffectAlter(skill.targetCreatureView.room, result);
        ResetCurrentSkillResult();

        //model.SetFeelingChange(5, skill.skillTypeInfo.amount * skill.successCount / skill.totalTickNum * scale);
        
    }

    void EndTransform() {
        //levelControl.Appear(currentNullState);
        levelControl.Disable(currentNullState);
        _nullUnit.PlaySound(transformingSoundKey, AudioRolloffMode.Linear);
        if (_currentWorker != null)
        {
            if (currentNullState == NullState.WORKER) {
                // _currentWorker.hp = _currentWorker.maxHp * 10;
                float val = 0.2f;
                PassageObjectModel pom = model.GetMovableNode().GetPassage();
                if (pom != null) {
                    float avr = 0f;
                    int cnt = 0;
                    //List<AgentModel> passageAgent = new List<AgentModel>();
                    if (model.sefira != null) {
                        foreach (AgentModel am in model.sefira.agentList) {
                            //passageAgent.Add(am);
                            if (am.isDead()) continue;
                            if (am.GetMovableNode().GetPassage() != null) {
                                if (am.GetMovableNode().GetPassage() == pom) {
                                    //avr += (float)am.hp;
                                    avr += (float)am.hp / am.maxHp;
                                    cnt++;
                                }
                            }
                        }

                        val = avr / cnt;
                        Debug.Log(val);
                    }
                }

                _currentWorker.hp = (int)(_currentWorker.maxHp * val);
                _currentWorker.mental = _currentWorker.maxMental;
            }
            else if(currentNullState == NullState.BROKEN){
                _currentWorker.TakePhysicalDamage(100000, DamageType.NORMAL);
                //_currentWorker.TakePhysicalDamageByCreature(10000);
                //_currentWorker = null;
            }

            if (_currentWorker != null) {
                _currentWorker.ReleaseUpdate();
            }
            
        }
    }

    void DeleteOlder() {
        isChanging = false;
        levelControl.EndTransform();
    }

    public void OnCalled()
    {
        this.isPlayingDeadScene = false;
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
        levelControl.animScript = this;
    }

    public void AgentReset() { }

    public void CreatureAnimCall(int i, CreatureBase script)
    { 
        
    }

    public void TakeDamageAnim(int isPhysical) { }
    public void SoundMake(string src) {
        _nullUnit.PlaySound(src, AudioRolloffMode.Linear);
    }

    //요기구현필요
    public void AttackCalled(int i)
    {
        foreach (WorkerModel wm in currentAttackTarget) {
            AgentAnim targetAnim = null;
            if (wm is AgentModel)
            {
                targetAnim = AgentLayer.currentLayer.GetAgent(wm.instanceId).animTarget;
            }
            else {
                targetAnim = OfficerLayer.currentLayer.GetOfficer(wm.instanceId).animTarget;
            }
            targetAnim.AttackedEffectInBodyPos("Effect/Slash");
        }
        currentAttackTarget.Clear();
        
    }
}
