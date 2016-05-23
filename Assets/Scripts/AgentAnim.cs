using UnityEngine;
using System.Collections.Generic;

public class AgentAnim : MonoBehaviour , IAnimatorEventCalled{
	private class ParameterInfo
	{
		public enum ParameterType
		{
			INT,
			FLOAT,
			BOOL
		}
		
		public string name;
		public int ivalue;
		public bool bvalue;
		public int state;
		public float remainTimer;

		public ParameterType type;

		public ParameterInfo(string name, int value, int state)
		{
			this.name = name;
			this.ivalue = value;
			this.state = state;
			this.type = ParameterType.INT;
		}

		public ParameterInfo(string name, bool value, int state)
		{
			this.name = name;
			this.bvalue = value;
			this.state = state;
			this.type = ParameterType.BOOL;
		}

		public ParameterInfo(string name, int value, int state, float remainTimer)
		{
			this.name = name;
			this.ivalue = value;
			this.state = state;
			this.remainTimer = remainTimer;
			this.type = ParameterType.INT;
		}

		public ParameterInfo(string name, bool value, int state, float remainTimer)
		{
			this.name = name;
			this.bvalue = value;
			this.state = state;
			this.remainTimer = remainTimer;
			this.type = ParameterType.BOOL;
		}
	}

	private Animator animator;

	public Sprite[] faceSprites;

	public SpriteRenderer hair;
	public SpriteRenderer face;

	public SpriteRenderer body;
	
	public SpriteRenderer B_low_leg;
	public SpriteRenderer B_up_leg;
	
	public SpriteRenderer F_low_leg;
	public SpriteRenderer F_up_leg;
	
	public SpriteRenderer B_hand;
	public SpriteRenderer B_low_arm;
	public SpriteRenderer B_up_arm;
	
	public SpriteRenderer F_hand;
	public SpriteRenderer F_low_arm;
	public SpriteRenderer F_up_arm;

    public SpriteRenderer Symbol;
	
	//private Dictionary<string, SpriteRenderer> rendererTable;
	private Dictionary<string, SpriteRenderer> clothesRendererTable;

	private Stack<ParameterInfo> updatedParameters;
	private List<ParameterInfo> updatedParametersMoment;

    private WorkerModel model;
    public WorkerModel Model {
        get { return model; }
    }

    private Sprite defaultFace;

    public void Init(WorkerModel am) {
        this.model = am;
        this.AnimatorEventInit();
        if (am is OfficerModel) {
            defaultFace = (am as OfficerModel).faceSprite;
        }
        else if (am is AgentModel) {
            defaultFace = (am as AgentModel).tempFaceSprite;
        }
    }
	
	void Awake()
	{
		updatedParameters = new Stack<ParameterInfo> ();
		updatedParametersMoment = new List<ParameterInfo> ();

		animator = GetComponent<Animator> ();
		if (animator == null) {
			Debug.LogError("Animator is not found!");
		}

		clothesRendererTable = new Dictionary<string, SpriteRenderer>();
		
		clothesRendererTable.Add("body", body);
		
		clothesRendererTable.Add("B_low_leg", B_low_leg);
		clothesRendererTable.Add("B_up_leg", B_up_leg);
		
		clothesRendererTable.Add("F_low_leg", F_low_leg);
		clothesRendererTable.Add("F_up_leg", F_up_leg);
		
		//clothesRendererTable.Add("B_hand", B_hand);
		clothesRendererTable.Add("B_low_arm", B_low_arm);
		clothesRendererTable.Add("B_up_arm", B_up_arm);
		
		//clothesRendererTable.Add("F_hand", F_hand);
		clothesRendererTable.Add("f_low_arm", F_low_arm);
		clothesRendererTable.Add("F_up_arm", F_up_arm);
	}
	
	
	public void SetHair(Sprite sprite)
	{
		this.hair.sprite = sprite;
	}
	public void SetFace(Sprite sprite)
	{
		this.face.sprite = sprite;
	}
	public void SetClothes(Sprite[] sprites)
	{
		foreach (Sprite sprite in sprites)
		{
			SpriteRenderer renderer;
			if (clothesRendererTable.TryGetValue(sprite.name, out renderer))
			{
				renderer.sprite = sprite;
			}
		}
	}

	public void SetSpeed(float speed)
	{
		animator.speed = speed;
	}

    public void PlayMatchGirlDead()
    {
        animator.speed = 1;
        animator.SetBool("Dead", true);
    }

	public void SetParameterOnce(string pname, int value)
	{
		updatedParameters.Push (new ParameterInfo (pname, animator.GetInteger (pname), animator.GetAnimatorTransitionInfo(0).nameHash));
		animator.SetInteger (pname, value);
	}

	public void SetParameterOnce(string pname, bool value)
	{
		updatedParameters.Push (new ParameterInfo (pname, animator.GetBool (pname), animator.GetAnimatorTransitionInfo(0).nameHash));
		animator.SetBool (pname, value);
	}

	public void SetParameterForSecond(string pname, int value, float time)
	{
		updatedParametersMoment.Add (new ParameterInfo (pname, animator.GetInteger (pname), animator.GetAnimatorTransitionInfo(0).nameHash, time));
		animator.SetInteger (pname, value);
	}
	public void SetParameterForSecond(string pname, bool value, float time)
	{
		updatedParametersMoment.Add (new ParameterInfo (pname, animator.GetBool (pname), animator.GetAnimatorTransitionInfo(0).nameHash, time));
		animator.SetBool (pname, value);
	}

	void LateUpdate()
	{
		while (updatedParameters.Count > 0) {
			ParameterInfo info = updatedParameters.Pop ();
			if(info.type == ParameterInfo.ParameterType.INT)
				animator.SetInteger(info.name, info.ivalue);
			else
				animator.SetBool(info.name, info.bvalue);
		}

		List<ParameterInfo> rmList = new List<ParameterInfo> ();
		foreach (ParameterInfo info in updatedParametersMoment)
		{
			info.remainTimer -= Time.deltaTime;

			if (info.remainTimer <= 0)
			{
				if(info.type == ParameterInfo.ParameterType.INT)
					animator.SetInteger(info.name, info.ivalue);
				else
					animator.SetBool(info.name, info.bvalue);
				
				rmList.Add (info);
			}
		}

		foreach (ParameterInfo info in rmList) {
			updatedParametersMoment.Remove (info);
		}
	}

    public void OnCalled()
    {
        if (this.model is AgentModel)
        {
            this.model.OnWorkEndFlag = true;
        }
        else if (this.model is OfficerModel)
        {
            AgentReset();
            //(this.model as OfficerModel).EndSpecialAction();
            (this.model as OfficerModel).SpecialActionReturn();
        }
    }

    public void OnCalled(int i)
    {

    }

    public void AnimatorEventInit()
    {
        AnimatorEventScript animEvent = this.GetComponent<AnimatorEventScript>();

        animEvent.SetTarget(this);
    }

    public void CreatureAnimCall(int i, CreatureBase script)
    { 
        //smth;
        script.AgentAnimCalled(i, this.model);
        /*
        if (this.model is AgentModel)
        {
            UseSkill skill = (this.model as AgentModel).currentSkill;
            if (skill != null)
            {
                if (skill.targetCreature.script != null)
                {
                    skill.targetCreature.script.AgentAnimCalled(i, this.model);
                }
            }
            else
            {
                script.AgentAnimCalled(i, this.model);
            }
        }
        else {
            script.AgentAnimCalled(i, this.model);
        }*/

    }

    public void AgentReset() {
        model.ResetAnimator();
		if(model is AgentModel)
			(model as AgentModel).WorkEndReaction();

    }

    public void SetSprite() {
        
        Sefira sefira = SefiraManager.instance.GetSefira(this.model.currentSefira);
        
        if (sefira == null) return;
        if (this.model is OfficerModel) {
            WorkerSpriteSet set = OfficerLayer.currentLayer.GetOfficerSpriteSet(sefira);
            SetWorkerSprite(set);
        }
        else if (this.model is AgentModel) {
            WorkerSpriteSet set = AgentLayer.currentLayer.GetAgentSpriteSet(sefira);
            SetWorkerSprite(set);
        }
    }

    public void SetWorkerSprite(WorkerSpriteSet spriteSet) {
        this.body.sprite = spriteSet.Body;
        this.B_low_arm.sprite = spriteSet.LeftDownHand;
        this.B_up_arm.sprite = spriteSet.LeftUpHand;
        this.F_low_arm.sprite = spriteSet.RightDownHand;
        this.F_up_arm.sprite = spriteSet.RightUpHand;
        this.B_low_leg.sprite = spriteSet.LeftDownLeg;
        this.B_up_leg.sprite = spriteSet.LeftUpLeg;
        this.F_low_leg.sprite = spriteSet.RightDownLeg;
        this.F_up_leg.sprite = spriteSet.RightUpLeg;

        if (this.model is AgentModel) {
            this.Symbol.sprite = spriteSet.Symbol;
        }
    }

    public void ChangeFaceSprite(Sprite s) {
        SetFace(s);
    }

    public void ChangeFaceToDefault() {

        SetFace(defaultFace);
    }

    public void PlaySoundOneShot(string src) {
        if (this.model is AgentModel)
        {
            AgentUnit unit = AgentLayer.currentLayer.GetAgent(model.instanceId);
            unit.PlaySound(src, null, false);
        }
        else
        {
            OfficerUnit unit = OfficerLayer.currentLayer.GetOfficer(model.instanceId);
            unit.PlaySound(src, null, false);
        }
    }

    public void PlaySound(string src, string key, bool isLoop)
    {
        if (this.model is AgentModel)
        {
            AgentUnit unit = AgentLayer.currentLayer.GetAgent(model.instanceId);
            unit.PlaySound(src, key, isLoop);
        }
        else
        {
            OfficerUnit unit = OfficerLayer.currentLayer.GetOfficer(model.instanceId);
            unit.PlaySound(src, key, isLoop);
        }
    }

    public void TakeDamageAnim(int isPhysical) {

        if (this.model is AgentModel)
        {
            AgentUnit unit = AgentLayer.currentLayer.GetAgent(this.model.instanceId);
            unit.animTarget.AttackedEffectByRandomPos("Effect/HitEffectGun");
        }
        else {
            OfficerUnit unit = OfficerLayer.currentLayer.GetOfficer(this.model.instanceId);

            if (isPhysical == 1)
            {
                unit.puppetAnim.SetInteger("PhysicalAttacked", UnityEngine.Random.Range(1, 4));
            }
            else {
                unit.puppetAnim.SetInteger("MentalAttacked", UnityEngine.Random.Range(1, 4));
            }

            unit.animTarget.AttackedEffectByRandomPos("Effect/HitEffectGun");
        }

    }

    public void AttackCalled(int i) { 
        //상대에게 맞는 모션 불러야함
        if (this.model is AgentModel)
        {
            AgentModel am = this.model as AgentModel;
            if (am.attackTargetWorker != null) {
                if (am.attackTargetWorker is AgentModel) {
                    AgentUnit targetUnit = AgentLayer.currentLayer.GetAgent(am.attackTargetWorker.instanceId);
                    targetUnit.animTarget.TakeDamageAnim(i);
                }
                else if (am.attackTargetWorker is OfficerModel)
                {
                    OfficerUnit targetUnit = OfficerLayer.currentLayer.GetOfficer(am.attackTargetWorker.instanceId);
                    targetUnit.animTarget.TakeDamageAnim(i);
                }
            }
        }
        else {
            OfficerModel om = this.model as OfficerModel;
            if (om.attackTargetWorker != null)
            {
                if (om.attackTargetWorker is AgentModel)
                {
                    AgentUnit targetUnit = AgentLayer.currentLayer.GetAgent(om.attackTargetWorker.instanceId);
                    targetUnit.animTarget.TakeDamageAnim(i);
                }
                else if (om.attackTargetWorker is OfficerModel)
                {
                    OfficerUnit targetUnit = OfficerLayer.currentLayer.GetOfficer(om.attackTargetWorker.instanceId);
                    targetUnit.animTarget.TakeDamageAnim(i);
                }
            }
        }
    }

    public void AttackedEffectByRandomPos(string src) {
        GameObject ge = Prefab.LoadPrefab(src);
        Vector3 pos = this.model.GetMovableNode().GetCurrentViewPosition();
        
        //ge.transform.localPosition = pos;
        ge.transform.SetParent(this.body.gameObject.transform);
        ge.transform.localPosition = Vector3.zero;
    }


    public void SoundMake(string src) { }
}
