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
            SetStat(model);
            SetWork(model);
            SetSuppressIcon(model);
            PromotionSetActivate(true);
        }

        public void SetDefaultData(AgentModel model) {
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
            //also
        }

        public void PromotionSetActivate(bool b) {
            this.Promotion.gameObject.SetActive(b);
        }
    }

    protected AgentModel model = null;
    RectTransform rect;

    public AgentAllocateSlotUI UI;
    public long id {
        get {
            if (model == null) {
                return -1;
            }
            return this.model.instanceId;
        }    
    }

    public void SetModel(AgentModel model) {
        this.model = model;
    }

    public void Awake() {
        rect = this.GetComponent<RectTransform>();
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
    }

    public void OnAllcateClick() {
        StageUI.instance.SetAgentSefriaButton(this.model);
        //ui change
    }
    //etc -> UI elements
	
}
