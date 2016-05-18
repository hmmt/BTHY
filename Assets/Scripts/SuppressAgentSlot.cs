using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SuppressAgentSlot : MonoBehaviour {
    
	public AgentModel model;

    public Image face;
    public Image hair;
    public Image suppressIcon;

    public GameObject portrait;

    public Image Target;
    public Sprite Normal;
    public Sprite Selected;
    public Sprite Suppressing;

    public Text name;
    public Text grade;
    public Text currentHealth;
    public Text currentMental;
    public Text movement;

    //현재 행동의 표현은 아이콘 or 텍스트?
    public Text currentAction;
    public Image[] suppressAction;

    public Slider hp;
    public Image mental;
    //public Image icon;

    private Color Select, NonSelect;
    private int index = -1;
    public bool isSelected = false;


    public void Awake() {
        Select = Color.white;
        NonSelect = Color.gray;
    }

    public void Init(AgentModel model) {

        Target.sprite = Normal;
        if (model == null)
        {
            this.model = null;
            Empty();
            return;
        }
        NonEmpty();

		this.model = model;
        this.hp.maxValue = model.maxHp;
        this.hp.minValue = 0;
        Debug.Log(model.name);
        index = -1;
        face.sprite = model.tempFaceSprite;
        hair.sprite = model.tempHairSprite;

        name.text = model.name;
        grade.text = model.level.ToString();
        currentHealth.text = model.hp.ToString();
        currentMental.text = model.mental.ToString();
        movement.text = model.movement.ToString();
        currentAction.text = "";
        if (model.GetState() == AgentAIState.SUPPRESS_CREATURE || model.GetState() == AgentAIState.SUPPRESS_WORKER)
        {
            Target.sprite = Suppressing;
        }

		UnitModel target = AutoCommandManager.instance.GetSuppressActionTarget (model);

		if (target != null)
		{
			index = 0;
		}
        SetSelected(index);
    }

    public void Empty() {
        //Debug.Log("This slot is empty");
        this.portrait.gameObject.SetActive(false);
        this.suppressIcon.gameObject.SetActive(false);
        this.name.gameObject.SetActive(false);
        this.grade.gameObject.SetActive(false);
        
    }

    public void NonEmpty() {
        this.portrait.gameObject.SetActive(true);
        this.suppressIcon.gameObject.SetActive(true);
        this.name.gameObject.SetActive(true);
        this.grade.gameObject.SetActive(true);
        
    }

    public void SetSelected(int i) {
        if (this.model == null) return;
        if (i < -1 || i >= 3) {
            return;
        }

        for (int cnt = 0; cnt < 3; cnt++) {
            if (cnt == i) {
                suppressAction[cnt].color = Select;
                continue;
            }
            suppressAction[cnt].color = NonSelect;
        }
    }

    public void OnClick() {
        if (this.model == null) return;
        if (model.GetState() == AgentAIState.SUPPRESS_CREATURE || model.GetState() == AgentAIState.SUPPRESS_WORKER)
        {
            return;
        }
        
        isSelected = !isSelected;

        if (isSelected)
        {
            //SpriteChanage;
            Target.sprite = this.Selected;
            SuppressWindow.currentWindow.OnSetSuppression(model);
        }
        else {
            Target.sprite = this.Normal;
            SuppressWindow.currentWindow.OnDeleteSuppression(model);
        }
        //SuppressWindow.currentWindow.OnSetSuppression(model);
        
    }

    public void OnClick(int i) {
        if (this.model == null) {
            return;
        }
        if (this.index == i) {
            SetSelected(-1);
            this.index = -1;
			AutoCommandManager.instance.SetSuppressAction (model, null);
            return;
        }
		SuppressWindow.currentWindow.OnSetSuppression (model);
        this.index = i;
        SetSelected(i);
    }

    public float GetHeight() {
        return this.GetComponent<RectTransform>().rect.height;
    }

    public void SetPanic() {
        //for temporary;
        this.model = null;
    }

    public bool CheckModel(AgentModel model) {
        return this.model == model;
    }

    public int GetCurrentSelected() {
        return this.index;
    }

    public void Update() {
        CheckState();
    }

    public void CheckState() {
        if (this.model == null) return;
        this.hp.value = model.hp;

        float mentalValue =(float)this.model.mental / this.model.maxMental;
        Color mentalColor = this.mental.color;
        mentalColor.a = 1 - mentalValue;
        this.mental.color = mentalColor;
        
    }
}
