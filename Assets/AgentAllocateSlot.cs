using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AgentAllocateSlot : MonoBehaviour {
    [System.Serializable]
    public class AgentAllocateSlotUI {
        public Image Face;
        public Image Hair;

        public Text Name;
        public Text LifeStyle;
        public Text Grade;

        public Image Health;
        public Image Mental;
        public Image Movement;
        public Image WorkSpeed;

        public Image WorkIcon1;
        public Text work1;
        public Image WorkIcon2;
        public Text work2;
        public Image WorkIcon3;
        public Text work3;

        public Image SuppressIcon;
        public Text suppressLevel;

        public Button Promotion;
        public Button Allocate;

        

        public void Init(AgentModel model) {
            SetDefaultData(model);
            SetWorkIcon(model);
            SetStat(model);
            SetWork(model);
            SetSuppressIcon(model);
            PromotionSetActivate(true);

        }

        public void SetWorkIcon(AgentModel model) {
            Sprite[] icons = AgentModel.GetAgentSkillSprite(model);
            WorkIcon1.sprite = icons[0];
            WorkIcon2.sprite = icons[1];
            WorkIcon3.sprite = icons[2];
        }

        public void SetDefaultData(AgentModel model)
        {
            this.Name.text = model.name;
            this.LifeStyle.text = model.LifeStyle();

            this.Face.sprite = model.tempFaceSprite;
            this.Hair.sprite = model.tempHairSprite;

        }

        public void SetStat(AgentModel model) {
            this.Grade.text = AgentModel.GetLevelGradeText(model);
            this.Health.sprite = model.StatusSprites[0];
            this.Mental.sprite = model.StatusSprites[1];
            this.WorkSpeed.sprite = model.StatusSprites[2];
            this.Movement.sprite = model.StatusSprites[3];
        }

        public void SetWork(AgentModel model) { 
            //temporary null
        }

        public void SetSuppressIcon(AgentModel model) {
            this.SuppressIcon.sprite = AgentModel.GetSuppressIcon(model);
        }

        public void PromotionSetActivate(bool b) {
            this.Promotion.gameObject.SetActive(b);
        }
    }

    [System.Serializable]
    public class SpriteSets {
        public Sprite promotion_normal;
        public Sprite promotion_disabled;
        public Sprite promotion_overlay;
        public Sprite allocate_normal;
        public Sprite allocate_overlay;
        public Sprite allocate_malkut;
        public Sprite allocate_yesod;

        public AgentAllocateSlotUI ui;

        public void Init(AgentAllocateSlotUI ui) {
            this.ui = ui;
        }

        public void SetSpriteInitial(bool promotionEnabled, Sefira currentSefira) {
            SetPromotion(promotionEnabled);
            SetAllocate(currentSefira);
        }

        public void SetPromotion(bool isEnabled) {
            if (isEnabled)
            {
                ui.Promotion.interactable = true;
                ui.Promotion.image.sprite = promotion_normal;
                SpriteState state = ui.Promotion.spriteState;
                state.highlightedSprite = promotion_overlay;
                state.pressedSprite = promotion_disabled;
                state.disabledSprite = promotion_normal;
                ui.Promotion.spriteState = state;
            }
            else {
                ui.Promotion.interactable = false;
                ui.Promotion.image.sprite = promotion_disabled;
            }
        }

        public void SetAllocate(Sefira currentSefira) {
            if (currentSefira == null) {
                ui.Allocate.image.sprite = allocate_normal;
                SpriteState sets = ui.Allocate.spriteState;
                sets.highlightedSprite = allocate_overlay;
                sets.disabledSprite = allocate_normal;
                sets.pressedSprite = allocate_overlay;
                ui.Allocate.spriteState = sets;

                return;
            }
            if (currentSefira.name == SefiraName.Malkut) {
                ui.Allocate.image.sprite = allocate_malkut;
                SpriteState sets = ui.Allocate.spriteState;
                sets.highlightedSprite = allocate_malkut;
                sets.disabledSprite = allocate_malkut;
                sets.pressedSprite = allocate_malkut;
                ui.Allocate.spriteState = sets;
            }
            else if(currentSefira.name == SefiraName.Yesod){
                ui.Allocate.image.sprite = allocate_yesod;
                SpriteState sets = ui.Allocate.spriteState;
                sets.highlightedSprite = allocate_yesod;
                sets.disabledSprite = allocate_yesod;
                sets.pressedSprite = allocate_yesod;
                ui.Allocate.spriteState = sets;
            }
        }


    }

    protected AgentModel model = null;
    RectTransform rect;
    public RectTransform infoScrollTarget;

    public AgentAllocateSlotUI UI;
    Sefira currentAgentSefira;
    public SpriteSets sets;
    public long id {
        get {
            if (model == null) {
                return -1;
            }
            return this.model.instanceId;
        }    
    }

    public TraitListScript traits;

    public int allocatedIndex = -1;

    public void SetModel(AgentModel model) {
        this.model = model;
        sets.SetSpriteInitial(model.isPromotable, null);
        ShowTrait();
    }

    public void Awake() {
        rect = this.GetComponent<RectTransform>();
        sets.Init(this.UI);
    }

    public float GetHeight() {
        return rect.rect.height;
    }

    public Vector2 GetAnchoredPosition() {
        return rect.anchoredPosition;
    }

    public void SetInitialTransform() {
        this.rect.localPosition = Vector3.zero;
        this.rect.localScale = Vector3.one;
    }

    public void SetPos(float pos) {
        this.rect.localPosition = Vector3.zero;
        this.rect.anchoredPosition = new Vector2(0, pos);
    }

    public void Init() {
        this.UI.Init(this.model);
        this.sets.SetSpriteInitial(model.isPromotable, SefiraManager.instance.getSefira(model.currentSefira));
    }

    public void OnCancelAgent() {
        sets.SetAllocate(null);
        this.allocatedIndex = -1;
    }

    public void SetAllocatedIndex(int index) {
        this.allocatedIndex = index;
        //print("Set index: " + index);
        sets.SetAllocate(SefiraManager.instance.getSefira(model.currentSefira));
    }

    public void OnAllocateClick() {
        if (this.allocatedIndex != -1)
        {
            //Debug.Log("Cancel agent allocated");
            SefiraAgentSlot.instance.CancelSefiraAgent(this.model, allocatedIndex);
            this.allocatedIndex = -1;
            sets.SetAllocate(null);
            return;
        }
        //Debug.Log("Allocate agent");
        StageUI.instance.SetAgentSefriaButton(this.model);
        currentAgentSefira = SefiraManager.instance.getSefira(model.currentSefira);
        //Debug.Log(currentAgentSefira);
        sets.SetAllocate(currentAgentSefira);
        //ui change
    }

    public void OnConfirmPromotion() { 
        //model promote;
        Debug.Log("Promote agent " + this.model.name);

        //Trait update
        ShowTrait();
    }

    public bool CheckAgent(AgentModel check) {
        if (check == this.model) {
            return true;    
        }
        return false;
    }

    bool isUp = true;
    bool isInitiated = false;
    public void Update() {

        if (isUp)
        {
            if (infoScrollTarget.anchoredPosition.y < 76f)
            {
                isUp = false;
                infoScrollTarget.anchoredPosition = new Vector2(infoScrollTarget.anchoredPosition.x, -77.05f);
            }
        }
        else {
            if (infoScrollTarget.anchoredPosition.y > -76f) {
                isUp = true;
                infoScrollTarget.anchoredPosition = new Vector2(infoScrollTarget.anchoredPosition.x, 77.05f);
            }
        }
    }

    public void ShowTrait() {
        traits.DeleteAll();
        foreach (TraitTypeInfo trait in model.traitList) {
            traits.MakeTraitSimple(trait);
        }
    }
    //etc -> UI elements
	
}
