using UnityEngine;
using System.Collections;

public class MatchGirl  : CreatureBase {
    public class MatchGirlEffect {
        public GameObject small = null;
        public GameObject big = null;
        Vector3 smallSize;
        Vector3 bigSize;
        bool init = false;

        public ParticleSystem currentParticle;

        public void SetEffect(int level) {
            float scaleFactor = 1f;
            if (level < 3)
            {
                if(!small.activeSelf)
                    small.gameObject.SetActive(true);
                if(big.activeSelf)
                    big.gameObject.SetActive(false);

                if (level != 0) {
                    //scaleFactor = scaleFactor + ( 0.1f * level );
                    scaleFactor += 0.5f * level;
                }

                small.transform.localScale = smallSize * scaleFactor;
            }
            else {
                if(!big.activeSelf)
                    big.gameObject.SetActive(true);
                if(small.activeSelf)
                    small.gameObject.SetActive(false);

                //scaleFactor = scaleFactor + (0.1f * (level - 2));
                scaleFactor += 0.5f * (level-2);

                big.transform.localScale = bigSize * scaleFactor;
            }
        }

        public void Init(Transform parent)
        {
            if (this.init) return;
            this.init = true;

            foreach (Transform t in parent)
            {
                if (t.name == "BigFire")
                {
                    big = t.gameObject;
                }
                else if (t.name == "SmallFire")
                {
                    small = t.gameObject;
                    currentParticle = small.GetComponent<ParticleSystem>();
                }

                if (big != null && small != null)
                {

                    break;
                }
            }

            smallSize = new Vector3(small.transform.localScale.x,
                                    small.transform.localScale.y,
                                    small.transform.localScale.z);
            bigSize = new Vector3(big.transform.localScale.x,
                                  big.transform.localScale.y,
                                  big.transform.localScale.z);

            SetEffect(0);
        }

        public bool isInitiated() {
            return this.init;
        }

        public void EffectDisabled() {
            this.small.SetActive(false);
            this.big.SetActive(false);
        }

        public void EffectEnabled() {
            this.SetEffect(0);
        }
    }
    public class MatchGirlTimer {
        public bool activated = false;
        public float elapsed = 0f;
        public static float maxTime = 6f;

        public void StartTimer() {
            if (this.activated) return;
            this.activated = true;
            this.elapsed = 0f;
        }

        public void StopTimer() {
            if (!this.activated) return;
            this.activated = false;
            this.elapsed = 0f;
        }

        public void AddElapsed(float value) {
            this.elapsed += value;
        }

        public float GetElpased() {
            return this.elapsed;
        }

    }

    float maxTime = 6f;
    float currentTime = 0f;
    
    int explosionStack = 0;
    bool exploded = false;
    bool panicStartMove = false;
    MatchGirlEffect effectSystem = new MatchGirlEffect();
    MatchGirlTimer timer = new MatchGirlTimer();
    UseSkill currentSkill = null;
    bool isPlayingAttackScene = false;
    bool escapeCall = false;
    bool kill = false;
    bool attackedAnimatorReset = false;
    AgentModel killingTarget = null;
    Animator targetAnimator = null;

    string isolateEffect = "Effect/Creature/MatchGirl/MatchGirl_Isolate";
    string sefiraEffect = "Effect/Creature/MatchGirl/MatchGirl_Sefira";


	public override void OnInit()
	{
		//model.SetCurrentNode (MapGraph.instance.GetNodeById("malkuth-1-4"));
        //effectSystem initialize

        model.escapeType = CreatureEscapeType.FACETOSEFIRA;
	}

	public override void OnFixedUpdate (CreatureModel creature)
	{
        if (!effectSystem.isInitiated()) {
            CreatureUnit currentUnit = CreatureLayer.currentLayer.GetCreature(model.instanceId);
            GameObject effectParent = currentUnit.creatureAnimator.gameObject;
            effectSystem.Init(effectParent.transform);
        }

        if (model.GetFeelingPercent() < 100f )
        {
            if (this.explosionStack < 4)
                timer.StartTimer();
        }
        else {
            timer.StopTimer();
        }

        if (timer.activated && !isPlayingAttackScene) {
            timer.AddElapsed(Time.deltaTime);
            if (timer.GetElpased() > MatchGirlTimer.maxTime) {
                timer.StopTimer();
                AddExplosionLevel();
            }
        }

        if (isPlayingAttackScene)
        {
            if (model.GetAnimScript().animator.GetInteger("AttackEnd") == 1) {
                model.GetAnimScript().animator.SetInteger("AttackEnd", 0);
                if (kill)
                {
                    killingTarget.TakePhysicalDamageByCreature(5);
                    //dead
                    killingTarget = null;
                }
                else {
                    attackedAnimatorReset = true;
                }

                isPlayingAttackScene = false;
            }
            
        }

        if (attackedAnimatorReset) {
            if (targetAnimator.GetInteger("AttackEnd") == 1) {
                AnimatorManager.instance.ResetAnimatorTransform(killingTarget.instanceId);
                killingTarget.ResetAnimator();
                targetAnimator = null;
                killingTarget = null;
                attackedAnimatorReset = false;
            }

        }

        if (escapeCall && !isPlayingAttackScene)
        {
            Debug.Log("이제탈출해야징");
            model.StopEscapeWork();
            escapeCall = false;
        }
	}

	public override void OnReturn ()
	{
		model.energyPoint = 130;
	}

    public override void OnSkillFailWorkTick(UseSkill skill)
	{
		if(Random.value <= 0.4)
		{
            ActivateSkill(skill);
		}
	}

	public void ActivateSkill(UseSkill skill)
	{
        Debug.Log("MatchGirl ActivateSkill");
        // show effect

		/*
        skill.PauseWorking();
        SoundEffectPlayer.PlayOnce("creature/match_girl/matchgirl_ability", skill.targetCreatureView.transform.position);

        OutsideTextEffect effect = OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/matchgirl/01_matchGirl_out_typo_01", CreatureOutsideTextLayout.CENTER_BOTTOM, 0, 6);
        effect.transform.localScale = new Vector3(1.1f, 1.1f, 1);

        // skill이 이미 release 될 상황 고려 필요
        effect.GetComponent<DestroyHandler>().AddReceiver(delegate() { skill.ResumeWorking(); });

        OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/matchgirl/01_matchGirl_out_typo_02", CreatureOutsideTextLayout.CENTER_BOTTOM, 1, 5)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/matchgirl/01_matchGirl_out_typo_03", CreatureOutsideTextLayout.CENTER_BOTTOM, 2, 4)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/matchgirl/01_matchGirl_out_typo_04", CreatureOutsideTextLayout.CENTER_BOTTOM, 3, 3)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
        

        skill.targetCreature.ShowNarrationText("special_ability1", skill.agent.name);
        TimerCallback.Create(3.0f, delegate() { skill.targetCreature.ShowNarrationText("special_ability2", skill.agent.name); });
        TimerCallback.Create(6.0f,
            delegate() {
                skill.targetCreature.ShowNarrationText("special_ability3", skill.agent.name);
                float[] randomTable = { 0.1f, 0.2f, 0.3f, 0.4f, 0.5f, 0.6f };

                float damage = randomTable[Random.Range(0, randomTable.Length)];

                skill.agent.TakePhysicalDamage(skill.agent.hp - (int)(skill.agent.hp * (1 - damage)));
                skill.agent.TakeMentalDamage(skill.agent.mental - (int)(skill.agent.mental * (1 - damage)));

                SoundEffectPlayer.PlayOnce("creature/match_girl/matchgirl_ability_damage", skill.targetCreatureView.transform.position);

                string[] typos = {"typo/matchgirl/01_matchGirl_specialAttack_00",
                    "typo/matchgirl/01_matchGirl_specialAttack_01",
                    "typo/matchgirl/01_matchGirl_specialAttack_02"};


                TypoEffect.Create(skill.targetCreatureView, typos[Random.Range(0, typos.Length)], 0, 2);

                skill.CheckLive();
            });
       */
	}

    public override void OnSkillNormalAttack(UseSkill skill)
    {
        /*
        OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/matchgirl/01_matchGirl_commonAttack_00", CreatureOutsideTextLayout.CENTER_BOTTOM, 1, 7)
            .transform.localScale = new Vector3(1.1f, 1.1f, 1);
         * */

        CreatureUnit unit = CreatureLayer.currentLayer.GetCreature(model.instanceId);
        unit.animTarget.SendMessage("Attack");

        string[] typos = {"typo/matchgirl/01_matchGirl_commonAttack_00",
                    "typo/matchgirl/01_matchGirl_commonAttack_01",
                    "typo/matchgirl/01_matchGirl_commonAttack_02"};

        skill.agent.TakePhysicalDamageByCreature(1);
        skill.CheckLive();
        if (skill.agent.isDead())
        {
            AgentUnit agentUnit = AgentLayer.currentLayer.GetAgent(skill.agent.instanceId);
            //agentUnit.animTarget.PlayMatchGirlDead();
        }


        TypoEffect.Create(skill.targetCreatureView, typos[Random.Range(0, typos.Length)], 0, 2);
    }

	public override void OnEnterRoom(UseSkill skill)
	{
        CreatureUnit unit = CreatureLayer.currentLayer.GetCreature(model.instanceId);
        if (this.explosionStack == 4) {
            Explode(skill);
            return;
        }

        //unit.animTarget.SendMessage("Attack");
		/*
		skill.PauseWorking ();
		//SoundEffectPlayer.PlayOnce("match_strike_1.wav", skill.targetCreature.transform.position);


		OutsideTextEffect effect = OutsideTextEffect.Create (skill.targetCreature.instanceId, "typo/matchgirl/01_matchGirl_enter_typo_01", CreatureOutsideTextLayout.CENTER_BOTTOM, 0, 8);
		effect.transform.localScale = new Vector3(1.1f,1.1f,1);

		// skill이 이미 release 될 상황 고려 필요
		effect.GetComponent<DestroyHandler> ().AddReceiver (delegate() {skill.ResumeWorking();});


		OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/matchgirl/01_matchGirl_enter_typo_02", CreatureOutsideTextLayout.CENTER_BOTTOM, 1, 7)
			.transform.localScale = new Vector3(1.1f,1.1f,1);
		OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/matchgirl/01_matchGirl_enter_typo_03", CreatureOutsideTextLayout.CENTER_BOTTOM, 2, 6)
			.transform.localScale = new Vector3(1.1f,1.1f,1);
		OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/matchgirl/01_matchGirl_enter_typo_04", CreatureOutsideTextLayout.CENTER_BOTTOM, 3, 5)
			.transform.localScale = new Vector3(1.1f,1.1f,1);
		OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/matchgirl/01_matchGirl_enter_typo_05", CreatureOutsideTextLayout.CENTER_BOTTOM, 4, 4)
			.transform.localScale = new Vector3(1.1f,1.1f,1);
		OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/matchgirl/01_matchGirl_enter_typo_06", CreatureOutsideTextLayout.CENTER_BOTTOM, 5, 3)
			.transform.localScale = new Vector3(1.1f,1.1f,1);
		OutsideTextEffect.Create(skill.targetCreature.instanceId, "typo/matchgirl/01_matchGirl_enter_typo_07", CreatureOutsideTextLayout.CENTER_BOTTOM, 6, 2)
			.transform.localScale = new Vector3(1.1f,1.1f,1);
		*/

        /*
        skill.CheckLive();
        if (skill.agent.isDead())
        {
            AgentUnit agentUnit = AgentLayer.currentLayer.GetAgent(skill.agent.instanceId);
            //agentUnit.animTarget.PlayMatchGirlDead();
        }*/
	}

    public void AddExplosionLevel() {
        if (explosionStack > 4) {
            return;
        }
        explosionStack++;
        effectSystem.SetEffect(explosionStack);
    }

    public void Explode(UseSkill skill) {
        if (exploded)
        {
            //탈출해야됨
            escapeCall = true;
        }
        else {
            exploded = true;
        }

        //애니메이션 및 이펙트 재생
        GameObject boom = Prefab.LoadPrefab(this.isolateEffect);
        boom.transform.position = model.GetWorkspaceNode().GetPosition();
        ParticleDestroy pd = boom.GetComponent<ParticleDestroy>();
        pd.DelayedDestroy(10);
        model.SendAnimMessage("Attack");

        this.currentSkill = skill;
        skill.PauseWorking(10f);

        Animator agentAnim = AgentLayer.currentLayer.GetAgent(skill.agent.instanceId).puppetAnim;
        AnimatorManager.instance.ResetAnimatorTransform(skill.agent.instanceId);
        AnimatorManager.instance.ChangeAnimatorByName(skill.agent.instanceId, AnimatorName.MatchedGirl_AgentCTRL,
                                                    agentAnim, true, false);
        targetAnimator = agentAnim;
        isPlayingAttackScene = true;
        killingTarget = skill.agent;
        if (skill.agent.hp < 5)
        {
            kill = true;
            agentAnim.SetBool("Kill", true);
        }
        else {
            kill = false;

            skill.agent.TakePhysicalDamageByCreature(5f);
            agentAnim.SetBool("Attack", true);
        }

        /*
        //직원 작업의 종료
        if (skill.agent.isDead())
        {
            //skill.agent.Die();
        }
        else {
            //skill.agentView.puppetAnim.SetInteger("PhysicalAttacked", 1);
        }*/


        /*
        if (escapeCall) {
            model.StopEscapeWork();
            return;
        }
        */
        if (!escapeCall) {
            InitExplosionLevel();
        }
    }

    public void InitExplosionLevel() {

        explosionStack = 0;
        effectSystem.SetEffect(explosionStack);
    }

    public override string GetDebugText()
    {
        return this.explosionStack.ToString();
    }

    public override bool hasUniqueEscape()
    {
        return true;
    }

    public override void UniqueEscape()
    {
        if (model.GetCreatureCurrentCmd() == null)
        {
            if (panicStartMove) {

                model.SendAnimMessage("SefiraExplosion");
                SefiraExplosion();
                ResetAfterExplosion();
            }

            panicStartMove = true;
            MapNode[] graph = MapGraph.instance.GetSefiraNodes(model.sefira);
            model.MoveToNode(graph[graph.Length / 2]);

        }
        else {
            
        }
    }

    void SefiraExplosion() {
        System.Collections.Generic.List<WorkerModel> list = new System.Collections.Generic.List<WorkerModel>();

        foreach (AgentModel am in model.sefira.agentList) {
			if (am.GetMovableNode().GetPassage() == MapGraph.instance.GetSefiraPassage(model.sefira.indexString))
            {
                list.Add(am);
            }
        }

        foreach (OfficerModel om in model.sefira.officerList) {
			if (om.GetMovableNode().GetPassage() == MapGraph.instance.GetSefiraPassage(model.sefira.indexString))
            {
                list.Add(om);
            }
        }

        foreach (WorkerModel wm in list) {
            wm.TakePhysicalDamageByCreature(5);
            Debug.Log(wm.name + " Damaged");
        }

    }

    void ResetAfterExplosion() {
        panicStartMove = false;

        GameObject sefiraBoom = Prefab.LoadPrefab(this.sefiraEffect);
        sefiraBoom.transform.position = model.GetMovableNode().GetCurrentViewPosition();
        ParticleDestroy pd = sefiraBoom.GetComponent<ParticleDestroy>();
        pd.DelayedDestroy(5.5f);

        CameraMover.instance.Recoil(3);
        model.state = CreatureState.SUPPRESSED;
        InitExplosionLevel();
        
        effectSystem.EffectDisabled();
    }

    public override void OnTimerEnd()
    {
        if (this.currentSkill != null) {
            this.currentSkill.agent.StopAction();
            this.currentSkill = null;
        }
    }

    public override bool isAttackInWorkProcess()
    {
        return false;
    }
}
