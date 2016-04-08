using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SuppressAgentSlot : MonoBehaviour {

	private AgentModel model;

    public Image face;
    public Image hair;

    public Text name;
    public Text grade;
    public Text currentHealth;
    public Text currentMental;
    public Text movement;

    //현재 행동의 표현은 아이콘 or 텍스트?
    public Text currentAction;
    public Image[] suppressAction;

    private Color Select, NonSelect;
    private int index = -1;

    public void Awake() {
        Select = Color.white;
        NonSelect = Color.gray;
    }

    public void Init(AgentModel model) {
		this.model = model;


        index = -1;
        face.sprite = model.tempFaceSprite;
        hair.sprite = model.tempHairSprite;

        name.text = model.name;
        grade.text = model.level.ToString();
        currentHealth.text = model.hp.ToString();
        currentMental.text = model.mental.ToString();
        movement.text = model.movement.ToString();
        currentAction.text = "";
        SetSelected(index);
    }

    public void SetSelected(int i) {
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

    public void OnClick(int i) {
        if (this.index == i) {
            SetSelected(-1);
            this.index = -1;
            return;
        }
		SuppressWindow.currentWindow.OnSetSuppression (model);
        this.index = i;
        SetSelected(i);
    }

    public SuppressAction.Weapon GetSuppresstype() {
        switch (index) { 
            case 0:
                return SuppressAction.Weapon.STICK;
            case 1:
                return SuppressAction.Weapon.GUN;
            default:
                return SuppressAction.Weapon.NONE;
        }
    }

    public float GetHeight() {
        return this.GetComponent<RectTransform>().rect.height;
    }
}
