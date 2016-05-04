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
        if (model == null)
        {
            this.model = null;
            Empty();
            return;
        }

		this.model = model;

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

		UnitModel target = AutoCommandManager.instance.GetSuppressActionTarget (model);

		if (target != null)
		{
			index = 0;
		}
        SetSelected(index);
    }

    public void Empty() {
        Debug.Log("This slot is empty");
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
}
