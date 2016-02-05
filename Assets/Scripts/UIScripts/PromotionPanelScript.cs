using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class PromotionPanelScript : MonoBehaviour {
    public Image Hair;
    public Image Body;
    public Image Face;
    public Text name;
    public Button direct;
    public Button indirect;
    public Button block;
    public Button confirm;
    private ListSlotScript script;
    private AgentModel model;

    private Color selected;
    private Color normal;
    private GameObject targetObject;
    private int selectedSkill = -1;


	// Use this for initialization
	void Start () {
        selected = Color.white;
        normal = Color.gray;
        this.gameObject.SetActive(false);
	}

    public void UpdatePanel(ListSlotScript script)
    {
        this.script = script;
        this.model = script.getModel();
        Hair.sprite = model.tempHairSprite;
        //Body.sprite = ResourceCache.instance.GetSprite(this.model.bodyImgSrc);
        Face.sprite = model.tempFaceSprite;
        name.text = this.model.name;
        direct.image.sprite = this.model.WorklistSprites[0];
        indirect.image.sprite = this.model.WorklistSprites[1];
        block.image.sprite = this.model.WorklistSprites[2];

        direct.image.color = indirect.image.color = block.image.color = normal;
    }

    public void SetTarget(GameObject obj) {
        this.targetObject = obj;
    }

    public void Selected(int i) {
        if (this.selectedSkill == i) {
            selectedSkill = -1;
            direct.image.color = indirect.image.color = block.image.color = normal;
            return;
        }
        this.selectedSkill = i;
        switch (i) { 
            case 1:
                direct.image.color = selected;
                indirect.image.color = normal;
                block.image.color = normal;
                break;
            case 2:
                direct.image.color = normal;
                indirect.image.color = selected;
                block.image.color = normal;
                break;
            case 3:
                direct.image.color = normal;
                indirect.image.color = normal;
                block.image.color = selected;
                break;
        }
    }

    public void Deactivate() {
        if (targetObject != null) {
            targetObject.SetActive(true);
            targetObject = null;
        }
        gameObject.SetActive(false);
    }

    public void OnConfirm() {
        if (selectedSkill == -1) return;
        this.model.promoteSkill(this.selectedSkill);
        //StageUI.instance.PromotionAgent(this.model, model.level, script.ui.promotion);
        StageUI.instance.PromoteAgent(this.model, script.ui.promotion);
        targetObject.SetActive(true);
        Deactivate();
    }
}
