using UnityEngine;
using System.Collections;

public class NullthingLevelController : MonoBehaviour {
    [System.Serializable]
    public class WorkerSprite {
        public SpriteRenderer Face;
        public SpriteRenderer Hair;
        public SpriteRenderer Body;
        public SpriteRenderer RightUpArm;
        public SpriteRenderer RightDownArm;
        public SpriteRenderer LeftUpArm;
        public SpriteRenderer LeftDownArm;
        public SpriteRenderer RightUpLeg;
        public SpriteRenderer RightDownLeg;
        public SpriteRenderer LeftUpLeg;
        public SpriteRenderer LeftDownLeg;

        public SpriteRenderer Symbol;//if needed;

        public void Init() {
            this.Face.sprite = WorkerSpriteManager.instance.GetRandomFaceSprite();
            this.Hair.sprite = WorkerSpriteManager.instance.GetRandomHairSprite();
        }

        public void SetSprite(WorkerModel model) {
            AgentAnim anim = null;
            if (model is AgentModel) {
                AgentUnit unit = AgentLayer.currentLayer.GetAgent(model.instanceId);
                anim = unit.animTarget;
                this.Symbol.gameObject.SetActive(true);
            }
            else if (model is OfficerModel){
                OfficerUnit unit = OfficerLayer.currentLayer.GetOfficer(model.instanceId);
                anim = unit.animTarget; 
                this.Symbol.gameObject.SetActive(false);
            }

            this.Face.sprite = anim.face.sprite;
            this.Hair.sprite = anim.hair.sprite;
            this.Body.sprite = anim.body.sprite;
            this.LeftDownArm.sprite = anim.B_low_arm.sprite;
            this.LeftUpArm.sprite = anim.B_up_arm.sprite;
            this.RightDownArm.sprite = anim.F_low_arm.sprite;
            this.RightUpArm.sprite = anim.F_up_arm.sprite;
            this.LeftDownLeg.sprite = anim.B_low_leg.sprite;
            this.LeftUpLeg.sprite = anim.B_up_leg.sprite;
            this.RightDownLeg.sprite = anim.F_low_leg.sprite;
            this.RightUpLeg.sprite = anim.F_up_leg.sprite;
        }
    }

    public WorkerSprite spriteSet;

    public GameObject Good;
    public GameObject Normal;
    public GameObject Bad;
    public GameObject Egg;

    public AgentUnit agentUnit = null;
    public OfficerUnit officerUnit = null;
    NullthingAnim animController;
    public Animator[] animators;
    object unit{
        get {
            if (agentUnit != null) {
                return agentUnit as AgentUnit;
            }
            else if (officerUnit != null) {
                return officerUnit as OfficerUnit;
            }
            return null;
        }
        set {
            if (value is AgentModel) {
                unit = value as AgentModel;
            }
            else if (value is OfficerModel) {
                unit = value as OfficerModel;
            }
        }
    }
    Transform reference;
    MovableObjectNode movableObject;

    public void Start()
    {
        spriteSet.Init();
        Egg.gameObject.SetActive(false);
        animController = this.gameObject.GetComponent<NullthingAnim>();
        Good.gameObject.SetActive(true);
        Normal.gameObject.SetActive(false);
        Bad.gameObject.SetActive(false);
        reference = Good.gameObject.transform;

    }

    public void Init(CreatureModel nullthing) {
        this.movableObject = nullthing.GetMovableNode();
        //Bad.GetComponent<AnimatorEventScript>().SetTarget(nullthing.script as IAnimatorEventCalled);
        //Normal.GetComponent<AnimatorEventScript>().SetTarget(nullthing.script as IAnimatorEventCalled);
        Egg.GetComponent<AnimatorEventScript>().SetTarget(nullthing.script as IAnimatorEventCalled);
    }

    public void Change(WorkerModel model) {
        spriteSet.SetSprite(model);
        Transform targetTransform = null;
        if (model is AgentModel) {
            agentUnit = AgentLayer.currentLayer.GetAgent(model.instanceId);
            if (officerUnit != null) {
                officerUnit.gameObject.SetActive(false);
            }
            unit = agentUnit;
            targetTransform = (unit as AgentUnit).gameObject.transform;
            //(unit as AgentUnit).blockMoving = true;
            //model.movableNode = this.movableObject;
            
        }
        else if (model is OfficerModel) {
            officerUnit = OfficerLayer.currentLayer.GetOfficer(model.instanceId);
            if (agentUnit != null)
            {
                agentUnit.gameObject.SetActive(false);
            }
            unit = officerUnit;
            targetTransform = (unit as OfficerUnit).gameObject.transform;
        }

        //targetTransform.SetParent(this.transform, true);
        /*
        targetTransform.localPosition = reference.localPosition;
        targetTransform.localScale = reference.localScale;
        targetTransform.localRotation = reference.localRotation;
         */
        /*
        targetTransform.localPosition = Vector3.zero;
        targetTransform.localScale = new Vector3(5, 5, 1);
        targetTransform.localRotation = Quaternion.identity;
         */
    }

    public void Disable(NullCreature.NullState state) {
        switch (state) { 
            case NullCreature.NullState.BROKEN:
                animController.animator = animators[1];
                this.Good.gameObject.SetActive(false);
                this.Normal.gameObject.SetActive(true);
                this.Bad.gameObject.SetActive(false);
                break;
            case NullCreature.NullState.CREATURE:
                animController.animator = animators[2];
                this.Good.gameObject.SetActive(false);
                this.Normal.gameObject.SetActive(false);
                this.Bad.gameObject.SetActive(true);
                break;
            case NullCreature.NullState.WORKER:
                animController.animator = animators[0];
                this.Good.gameObject.SetActive(true);
                this.Normal.gameObject.SetActive(false);
                this.Bad.gameObject.SetActive(false);
                break;
        }
    }

    public void Appear(NullCreature.NullState current) {
        switch ( current) { 
            case NullCreature.NullState.WORKER:
                this.Good.gameObject.SetActive(true);
                break;
            case NullCreature.NullState.BROKEN:
                this.Normal.gameObject.SetActive(true);
                break;
            case NullCreature.NullState.CREATURE:
                this.Bad.gameObject.SetActive(true);
                break;
        }
    }

    public void ChangeTransform() {
        Animator EggAnim = this.Egg.GetComponent<Animator>();
        this.Egg.gameObject.SetActive(true);
        EggAnim.SetBool("Transform", true);

    }

    public void EndTransform() {
        Animator EggAnim = this.Egg.GetComponent<Animator>();
        EggAnim.SetBool("Transform", false);
        this.Egg.gameObject.SetActive(false);
    }
    
}
