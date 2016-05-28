using UnityEngine;
using System.Collections;

public class CreatureAttackInfo
{
    public float physicsProb;
    public float mentalProb;
    public int physicsDmg;
    public int mentalDmg;

    public CreatureAttackInfo() { }
    public CreatureAttackInfo(float physicsProb, float mentalProb, int physicsDmg, int mentalDmg)
    {
        this.physicsProb = physicsProb;
        this.mentalProb = mentalProb;
        this.physicsDmg = physicsDmg;
        this.mentalDmg = mentalDmg;
    }
}

public class CreatureBase {
    public class CreatureTimer {
        public float elapsed = 0f;
        public float maxTime = 0f;
        public bool started = false;
        public bool oneShot = false;//true : 한 번만 돌게 됨
        public bool enabled = true;

        public CreatureTimer(float Time) {
            this.maxTime = Time;
        }

        public CreatureTimer() { 
            
        }

        public void TimerStart(bool oneShot) {
            this.started = true;
            this.oneShot = oneShot;
            this.elapsed = 0f;
        }

        public void TimerStart(float Time, bool oneShot) {
            this.started = true;
            this.oneShot = oneShot;
            this.maxTime = Time;
            this.elapsed = 0f;
        }

        public bool isStarted() {
            return started;
        }

        public bool TimerRun() {
            if (!started) {
                return false;
            }
            elapsed += Time.deltaTime;
            if (elapsed > maxTime)
            {
                elapsed = 0f;
                if (oneShot) {
                    this.started = false;
                }
                return true;
            }
            else return false;
        }

        public void TimerStop() {
            oneShot = false;
            started = false;
            elapsed = 0f;
        }

    }
    public class SensingModule
    {
        float leftX;
        float rightX;
        float downY;
        float upY;

        bool enabled = true;

        public void Set(float x1, float x2, float y1, float y2)
        {
            this.leftX = x1;
            this.rightX = x2;
            this.downY = y1;
            this.upY = y2;
        }

        public void SetEnabled(bool b)
        {
            this.enabled = b;
        }

        public bool GetEnabled()
        {
            return this.enabled;
        }

        public bool Check(Vector3 pos)
        {
            if (!enabled) return false;
            if (pos.x > leftX && pos.x < rightX)
            {
                if (pos.y > downY && pos.y < upY)
                {
                    return true;
                }
            }
            return false;
        }

        public void Print()
        {
            Debug.Log(leftX + " " + rightX + " " + downY + " " + upY);
        }
    }

    protected CreatureModel model;
    public CreatureSpecialSkill skill;
    public bool hasUniqueEscapeLogic;

    int currentSkillResult = -1;

    public void SetModel(CreatureModel model)
    {
        this.model = model;
    }

    public virtual void OnInit()
    {
    }

	public virtual void OnViewInit(CreatureUnit unit)
	{
	}

    public virtual void OnFixedUpdate(CreatureModel creature)
	{

	}

	public virtual void OnFixedUpdateInSkill(UseSkill skill)
	{
	}

	public virtual void OnSkillStart(UseSkill skill)
	{
	}

    public virtual void OnSkillNormalAttack(UseSkill skill)
    {
    }

    public virtual void OnSkillSucceedWorkTick(UseSkill skill)
    {
    }

    public virtual void OnSkillFailWorkTick(UseSkill skill)
    {
    }

    // UseSkill의 ProccessWorkTick 이 실행될 때
	public virtual void OnSkillTickUpdate(UseSkill skill)
	{

	}

    public virtual void OnSkillGoalComplete(UseSkill skill)
    {
    }
	
	public virtual void OnSkillFinishUpdate(UseSkill skill)
	{
	}

	public virtual void OnEnterRoom(UseSkill skill)
	{
		
	}

	public virtual void OnRelease(UseSkill skill)
	{
	}

	public virtual void OnReturn()
	{
	}
	/*
    public virtual CreatureAttackInfo GetAttackInfo(UseSkill skill)
    {
        return new CreatureAttackInfo(
            model.metaInfo.physicsProb,
            model.metaInfo.mentalProb,
            model.metaInfo.physicsDmg,
            model.metaInfo.mentalDmg
            );
    }*/

	public virtual bool IsEscapable()
	{
		return true;
	}

    public virtual bool hasUniqueEscape() {
        return false;
    }

    public virtual void UniqueEscape() { 
        
    }

    public virtual SkillTypeInfo GetSpecialSkill()
    {
        return null;
    }

	public virtual string GetDebugText()
	{
		return "";
	}

    public virtual void OnTimerEnd() { 
        
    }


    /// <summary>
    /// make effect by input
    /// 0 - good
    /// 1 - normal
    /// 2 - bad
    /// </summary>
    /// <param name="index">0 -> good, 1 -> normal, 2 -> bad</param>
    public virtual void MakeEffect(IsolateRoom room) {
        GameObject heart;
        GameObject energy;
        switch (currentSkillResult)
        {
            case 0:
                heart = Prefab.LoadPrefab("Effect/Isolate/GoodWork");
                energy = Prefab.LoadPrefab("Effect/Isolate/EnergyUp");
                break;
            case 2:
                heart = Prefab.LoadPrefab("Effect/Isolate/BadWork");
                energy = Prefab.LoadPrefab("Effect/Isolate/EnergyDown");
                break;
            default: return;
        }
        heart.transform.SetParent(room.transform);
        heart.transform.localScale = Vector3.one;
        heart.transform.localPosition = Vector3.zero;
        energy.transform.SetParent(room.transform);
        energy.transform.localScale = Vector3.one;
        energy.transform.localPosition = Vector3.zero;

        ParticleDestroy pd_h = heart.GetComponent<ParticleDestroy>();
        ParticleDestroy pd_e = energy.GetComponent<ParticleDestroy>();
        pd_e.DelayedDestroy(10f);
        pd_h.DelayedDestroy(5f);

    }

    public void MakeEffectAlter(IsolateRoom room, int result) {
        GameObject heart;
        GameObject energy;
        switch (result)
        {
            case 0:
                heart = Prefab.LoadPrefab("Effect/Isolate/GoodWork");
                energy = Prefab.LoadPrefab("Effect/Isolate/EnergyUp");
                break;
            case 2:
                heart = Prefab.LoadPrefab("Effect/Isolate/BadWork");
                energy = Prefab.LoadPrefab("Effect/Isolate/EnergyDown");
                break;
            default: return;
        }
        heart.transform.SetParent(room.transform);
        heart.transform.localScale = Vector3.one;
        heart.transform.localPosition = Vector3.zero;
        energy.transform.SetParent(room.transform);
        energy.transform.localScale = Vector3.one;
        energy.transform.localPosition = Vector3.zero;

        ParticleDestroy pd_h = heart.GetComponent<ParticleDestroy>();
        ParticleDestroy pd_e = energy.GetComponent<ParticleDestroy>();
        pd_e.DelayedDestroy(10f);
        pd_h.DelayedDestroy(5f);
    }

    public void SetCurrentSkillResult(int index) {
        if (currentSkillResult != -1) {
            return;
        }
        currentSkillResult = index;
    }

    public void ResetCurrentSkillResult() {
        currentSkillResult = -1;
    }

    /// <summary>
    /// calculate max 100 percent probability
    /// </summary>
    /// <param name="probability">calculated percentage</param>
    /// <returns>if correct return true else false</returns>
    public virtual bool Prob(int probability)
    {
        int randVal = Random.Range(0, 100);
        if (randVal < probability)
        {
            return true;
        }
        return false;
    }

    public virtual bool isAttackInWorkProcess() {
        return true;    
    }

    public virtual bool hasUniqueFinish() {
        return false;
    }

    public virtual void UniqueFinish(UseSkill skill) { 
        
    }

    public virtual bool AutoFeelingDown() {
        return true;
    }

    public virtual void AgentAnimCalled(int i, WorkerModel actor) { 
        
    }

    public virtual void MakingEffect(string effect, float effectLength, string sound, Transform parent, int recoil) {
        Transform p = parent;
        CreatureUnit unit = CreatureLayer.currentLayer.GetCreature(this.model.instanceId);
        if (parent == null) {
            parent = unit.gameObject.transform;
        }

        GameObject effectObject = Prefab.LoadPrefab(effect);

        effectObject.transform.SetParent(p);
        effectObject.transform.localScale = Vector3.one;
        effectObject.transform.localPosition = Vector3.zero;
        effectObject.transform.localRotation = Quaternion.identity;

        ParticleDestroy pd = effectObject.GetComponent<ParticleDestroy>();
        pd.DelayedDestroy(effectLength);

        unit.PlaySound(sound);

        if (recoil > 0) {
            CameraMover.instance.Recoil(recoil);
        }

    }

    public virtual void MakingEffect(string effect, float effectLength, string sound, Vector3 pos, int recoil)
    {
        CreatureUnit unit = CreatureLayer.currentLayer.GetCreature(this.model.instanceId);
       
        GameObject effectObject = Prefab.LoadPrefab(effect);

        effectObject.transform.position = pos;

        ParticleDestroy pd = effectObject.GetComponent<ParticleDestroy>();
        pd.DelayedDestroy(effectLength);

        unit.PlaySound(sound);

        if (recoil > 0)
        {
            CameraMover.instance.Recoil(recoil);
        }

    }

    public virtual void OnAgentWorkEndAnimationPlayed(UseSkill skill) { 
        
    }
}
