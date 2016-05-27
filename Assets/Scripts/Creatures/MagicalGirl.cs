using UnityEngine;

using System.Collections;
using System.Collections.Generic;

public class MagicalGirl : CreatureBase {
    // 기분상태에 따라 마취약 투여가 가능해야 함.
    public enum MagicalGirlState { 
        GOOD,
        NORMAL,
        BAD
    }
    const float AnestheticFeelingDown = 80f;
    
    public MagicalGirlState currentState = MagicalGirlState.GOOD;

    bool anestheticReady = false;//마취가능상태
    bool isAnesthetized = false;//현재마취된상태
    bool isEscaped = false;
    bool coolTimeRun = false;
    bool canMove = true;
    int former = -1;
    float afterAnestheticFeeling = 0f;

    CreatureTimer timer = new CreatureTimer(30f);
    CreatureTimer coolTime = new CreatureTimer(10f);
    CreatureTimer attackDelay = new CreatureTimer(3f);
    CreatureTimer goodDefaultAction = new CreatureTimer(10f);

    public MagicalGirlAnim animScript;
    public MagicalLaserScript laser;

    Sefira _currentSefira;
    CreatureUnit _unit;
    PassageObjectModel currentPassage;
    MapNode currentMovingTarget = null;
    WorkerModel currentAttackTarget = null;
    bool isAreaHasTarget = false;
    bool isAttacking = false;
    bool isTransforming = false;

    List<WorkerModel> rangedTarget = new List<WorkerModel>();

    public override void OnInit()
    {
        anestheticReady = true;//for initial
        model.AddFeeling(100f);
        coolTime.enabled = false;
        model.escapeType = CreatureEscapeType.WANDER;
    }

    public override void OnViewInit(CreatureUnit unit)
    {
        this.animScript = unit.animTarget as MagicalGirlAnim;
        this.animScript.SetScript(this);
        animScript.ChangeMagicalState(false);
        this.laser = animScript.laser;
        this.laser.Init(this.model);
        _unit = CreatureLayer.currentLayer.GetCreature(model.instanceId);
        _currentSefira = model.sefira;

        goodDefaultAction.TimerStart(true);
    }

    private void ChangeDark(CreatureModel creature)
    {
        animScript.ChangeMagicalState(true);
    }

    private void ChangeNormal(CreatureModel creature)
    {
        animScript.ChangeMagicalState(false);
    }

    // 역변
    public override void OnFixedUpdate(CreatureModel creature)
    {
        if (isEscaped) return;

        CheckFeeling();
        if (timer.TimerRun()) {
            AnestheticEnd();    
        }

        if (currentState == MagicalGirlState.GOOD) {
            if (goodDefaultAction.TimerRun()) {
                //if (Random.Range(0, 3) == 0) {
                if(true){
                    animScript.NormalAnim.SetBool("GoodDefault", true);    
                }
                goodDefaultAction.TimerStart(true);
            }
        }
    }

    public void SetUnitDirection(UnitDirection dir) {
        this.model.GetMovableNode().SetDirection(dir);
    }

    public override void UniqueEscape()
    {
        //Movement Make
        //WorkerModel attackTarget = null;
        currentPassage = model.GetMovableNode().GetPassage();
        if (currentPassage == null) return;
        /*
        if (currentAttackTarget != null)
        {
            if (currentAttackTarget.isDead())
            {
                Debug.Log("Dead");
                currentAttackTarget = null;
                ReleaseMovement();
                attackDelay.TimerStop();
            }
        }

        if (currentAttackTarget == null) {
            //Debug.Log("Check");
            CheckRange();

            if (attackTarget != null && this.attackDelay.isStarted() == false)
            {
                StartAttack(attackTarget);
            }
        }*/

        //isAreaHasTarget = CheckRange();
        if (!isAreaHasTarget) {
           // List<WorkerModel> list = new List<WorkerModel>();
            isAreaHasTarget = CheckRange();

            if (isAreaHasTarget)
            {
                StartAttack();
            }
            else {
                ReleaseMovement();
            }
        }
        else
        {
            if (attackDelay.TimerRun())
            {
                PreAttack();
            }
        }

        if (canMove)
        {
            if (model.GetMovableNode().GetCurrentNode() == currentMovingTarget || model.GetMovableNode().IsMoving() == false)
            {
                currentMovingTarget = null;
                MakeMovement();
            }
        }
    }

    public void FailAttack() {
        ReleaseMovement();
        attackDelay.TimerStop();
        isAreaHasTarget = false;
    }

    public void SuccessAttack() {
       // ReleaseMovement();
        //currentAttackTarget = null;
        
        isAreaHasTarget = false;
        ReleaseMovement();
    }

    public void StartAttack()
    {
        StopMovement();
        //Debug.Log("공격 개시");
        //this.currentAttackTarget = target;
        PreAttack();
        //this.attackDelay.TimerStart(true);

    }

    public void PreAttack() {
        animScript.SnakeAnim.SetBool("Attack", true);
    }

    public void Attack() {
        //if (currentAttackTarget == null) return;
        //Debug.Log("공격");
        laser.EnableLaser(currentPassage, this.rangedTarget);

    }

    public void EndAttack()
    {
        attackDelay.TimerStart(true);
        animScript.animator.SetBool("AttackTransition", true);
    }

    void MakeMovement() {
        
        if (canMove == false) {
            return;
        }

        if (currentMovingTarget != null) {
            model.GetMovableNode().MoveToNode(currentMovingTarget);
            return;
        }
        //Debug.Log("Move");
        MapNode movingTarget = null;

        MapNode[] nodes = null;
        int randVal;
        while (former == (randVal = UnityEngine.Random.Range(0, _currentSefira.departmentNum + 1))) ;
        former = randVal;
        if (randVal == _currentSefira.departmentNum)
        {
            nodes = MapGraph.instance.GetSefiraNodes(_currentSefira);
        }
        else {
            nodes = MapGraph.instance.GetAdditionalSefira(_currentSefira);
            
        }
        movingTarget = nodes[Random.Range(0, nodes.Length)];
        currentMovingTarget = movingTarget;
        model.MoveToNode(movingTarget);
    }

    void StopMovement() {
        Debug.Log("Stop");
        canMove = false;
        animScript.SendMessage("Stop");
        model.GetMovableNode().StopMoving();
        model.ClearCommand();
    }

    void ReleaseMovement() {
        canMove = true;
        MakeMovement();
    }

    

    bool CheckRange()
    {
        PassageObjectModel currentPassage = model.GetMovableNode().GetPassage();
        List<WorkerModel> rangedTarget = new List<WorkerModel>();//같은 구역에 있는 직원들
        if (currentPassage != null) {
            foreach (AgentModel am in this._currentSefira.agentList) {
                
                if (am.GetMovableNode().GetPassage() == currentPassage) {
                    if (am.isDead()) continue;
                    rangedTarget.Add(am);
                }
            }

            foreach (OfficerModel om in this._currentSefira.officerList) {
                if (om.GetMovableNode().GetPassage() == currentPassage) {
                    if (om.isDead()) continue;
                    rangedTarget.Add(om);
                }
            }

            if (rangedTarget.Count == 0) {
                return false;
            }

            this.rangedTarget.Clear();
            foreach (WorkerModel wm in rangedTarget) {
                this.rangedTarget.Add(wm);
            }


            return true;


            /*
            UnitDirection dir = RotateDir(firstTarget.GetMovableNode());
            float currentModelX = model.GetMovableNode().GetCurrentViewPosition().x;
            foreach (WorkerModel wm in rangedTarget)
            {
                if (wm == firstTarget) {
                    dirTarget.Add(wm);

                }
                if (currentModelX < wm.GetMovableNode().GetCurrentViewPosition().x)
                {
                    if (dir == UnitDirection.RIGHT)
                    {
                        dirTarget.Add(wm);
                    }
                }
                else {
                    if (dir == UnitDirection.LEFT) {
                        dirTarget.Add(wm);
                    }
                }
            }*/
            //가장 가까이 있는 직원을 타겟으로 
            /*
            WorkerModel current = null;
            float currentMinVal = 10000f;
            UnitDirection currentDir = UnitDirection.RIGHT;
            float stdX = model.GetMovableNode().GetCurrentViewPosition().x;

            foreach (WorkerModel wm in rangedTarget) {
                float currentX = wm.GetMovableNode().GetCurrentViewPosition().x;
                float val = 0f;
                UnitDirection unitDir = UnitDirection.RIGHT;
                if (currentX > stdX)
                {
                    val = currentX - stdX;
                    unitDir = UnitDirection.RIGHT;
                }
                else {
                    val = stdX - currentX;
                    unitDir = UnitDirection.LEFT;
                }

                if (val < currentMinVal) {
                    current = wm;
                    currentDir = unitDir;
                }
            }
            if (current == null) {
                Debug.Log("Error");
                return;
            }
            Debug.Log("Current MagicalGirl Target : " + current.name + " //dir : " + currentDir);

            target = current;
             */
            
        }
        return false;
        
    }

    public UnitDirection RotateDir(MovableObjectNode mov) {
        float x = mov.GetCurrentViewPosition().x;
        UnitDirection output = UnitDirection.RIGHT;
        if (model.GetMovableNode().GetCurrentViewPosition().x < x)
        {
            output = UnitDirection.RIGHT;
        }
        else {
            output = UnitDirection.LEFT;
        }
        model.GetMovableNode().SetDirection(output);
        return output;
    }

	public override void OnReturn ()
	{
        coolTime.enabled = false;
        coolTime.TimerStop();

        isEscaped = false;
	}

    private void SkillDarkAttack(UseSkill skill)
    {
        
    }

    public override void OnEnterRoom(UseSkill skill)
    {
        if (skill.skillTypeInfo.id == 40004)
        {
            Anesthetic();
        }
    }

    public override void OnRelease(UseSkill skill)
    {
        if (currentState == MagicalGirlState.GOOD)
        {
            //smth effect
            AgentMentalRecovery(skill.agent);
        }
        else { 
            //AgentMentalDamage();
        }

        if (skill.skillTypeInfo.id == 40004) {
            if (isAnesthetized) {
                Debug.Log("TimerStart");
                timer.TimerStart(true);
            }
        }
    }

    public void Escape() {
        ChangeDark(this.model);
        isEscaped = true;
        model.StopEscapeWork();
        isTransforming = false;
        MakeMovement();
        //model.state = CreatureState.ESCAPE;
    }

    public void PreEscape() {
        
        animScript.NormalAnim.SetBool("ChangeTrans", true);
    }

    public void CheckFeeling() { 
        //smth flags check
        if (model.GetFeelingPercent() < 20f && isEscaped == false)
        {
            if (!isTransforming) {
                isTransforming = true;
                PreEscape();
            }
        }

        if (model.GetFeelingPercent() < 33f && this.currentState != MagicalGirlState.BAD) { 
            //state transition
            this.currentState = MagicalGirlState.BAD;

            if (animScript.NormalAnim.GetInteger("DefaultTrans") != 2)
            {
                animScript.NormalAnim.SetInteger("DefaultTrans", 2);
            }

            if (anestheticReady) {
                anestheticReady = false;
            }

            
            //Escape();
        }

        if (model.GetFeelingPercent() >= 33f && model.GetFeelingPercent() < 66f
            && this.currentState != MagicalGirlState.NORMAL)
        {
            this.currentState = MagicalGirlState.NORMAL;
            if (animScript.NormalAnim.GetInteger("DefaultTrans") != 1) {
                animScript.NormalAnim.SetInteger("DefaultTrans", 1);
            }

            if (!isAnesthetized && !anestheticReady) {
                if (coolTime.enabled)
                {
                    if (coolTime.TimerRun())
                    {
                        Debug.Log("coolTime ended");
                        coolTime.enabled = false;
                        anestheticReady = true;
                    }
                }
                else {
                    anestheticReady = true;
                }
            }
        }

        if (model.GetFeelingPercent() >= 66f && this.currentState != MagicalGirlState.GOOD) {
            this.currentState = MagicalGirlState.GOOD;
            if (animScript.NormalAnim.GetInteger("DefaultTrans") != 0)
            {
                animScript.NormalAnim.SetInteger("DefaultTrans", 0);
            }

            if (anestheticReady)
            {
                anestheticReady = false;
            }
        }
    }

    public void Anesthetic() {
        if (isAnesthetized) return;
        if (!anestheticReady) return;

        if (!coolTime.enabled) coolTime.enabled = true;
        Debug.Log("AnestheticStart");
        this.anestheticReady = false;
        this.isAnesthetized = true;
        afterAnestheticFeeling = -1 * AnestheticFeelingDown;
        
    }

    public override bool hasUniqueEscape()
    {
        return true;
    }

    public override bool hasUniqueFinish()
    {
        return false;
    }

    public void AgentMentalRecovery(AgentModel worker) {
        animScript.NormalAnim.SetBool("Attack", true);
        worker.RecoverMental(5);
    }

    public void AnestheticEnd() {
        isAnesthetized = false;
        Debug.Log("AnestheticEnd");
        if (afterAnestheticFeeling < 0)
        {
            model.SubFeeling(-1 * afterAnestheticFeeling);
        }
        else {
            model.AddFeeling(afterAnestheticFeeling);
        }
        coolTime.TimerStart(true);
    }

    public override bool AutoFeelingDown()
    {
        if (isAnesthetized) return false;
        return true;
    }

    public override SkillTypeInfo GetSpecialSkill()
    {
        if (anestheticReady && currentState == MagicalGirlState.NORMAL)
        {
            return SkillTypeList.instance.GetData(40004);
        }
        else {
            return null;
        }
    }

    public override string GetDebugText()
    {
        if (coolTime.enabled == false)
        {
            return this.anestheticReady.ToString() + " " + this.currentState.ToString();
        }
        else {
            return this.coolTime.elapsed.ToString();
        }
    }
}
