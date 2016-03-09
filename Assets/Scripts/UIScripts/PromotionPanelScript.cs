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
    private SkillCategory targetSkill;
    private float Height;


	// Use this for initialization
	void Start () {
        selected = Color.white;
        normal = Color.gray;
        this.gameObject.SetActive(false);
        Height = this.GetComponent<RectTransform>().rect.height;
	}

    public void UpdatePanel(ListSlotScript script)
    {
        this.script = script;
        this.model = script.getModel();
        this.model.PrePromotion();
        Hair.sprite = model.tempHairSprite;
        //Body.sprite = ResourceCache.instance.GetSprite(this.model.bodyImgSrc);
        Face.sprite = model.tempFaceSprite;
        name.text = this.model.name;
        direct.image.sprite = this.model.WorklistSprites[0];
        indirect.image.sprite = this.model.WorklistSprites[1];
        block.image.sprite = this.model.WorklistSprites[2];

        direct.image.color = indirect.image.color = block.image.color = normal;
        PromoteAgent();
        PromotionSkillTree.instance.SetModel(script.getModel());
    }

    public void SetTarget(GameObject obj) {
        this.targetObject = obj;
    }

    public float GetHeight() {
        return this.Height;
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
        PromotionSkillTree.instance.GetSelected(ref targetSkill, ref selectedSkill);
        if (selectedSkill == -1 || targetSkill == null) return;

        //this.model.promoteSkill(this.selectedSkill);
        this.model.Promotion(targetSkill);
        //StageUI.instance.PromotionAgent(this.model, model.level, script.ui.promotion);
        
        //StageUI.instance.PromoteApply(this.model);
        targetObject.SetActive(true);
        Deactivate();
        AgentListScript.instance.SetPromotionEnable(true);
    }

    private void PromoteAgent() {
        AgentModel target = this.model;
        int levelIndex = target.level - 1;
        
        if (levelIndex < 0 || levelIndex >= 4) 
            return;
        
        if (target == null)
            return;
        int cost = AgentLayer.currentLayer.AgentPromotionCost[levelIndex];
        //energy value
        if (EnergyModel.instance.GetLeftEnergy() < cost)//에너지 값
        {
            //Not enough energy
            Debug.Log("Not enough Energy");
            return;
        }
        else { 
            //reduce energy
            EnergyModel.instance.SetLeftEnergy(EnergyModel.instance.GetLeftEnergy() - cost);
        }

        TraitTypeInfo[] addList = TraitTypeList.instance.GetTrait(target);

        //model level up
        target.level = target.level+1;


        foreach (TraitTypeInfo trait in addList) {
            target.applyTrait(trait);
        }

        target.calcLevel();

        script.ui.promotion.gameObject.SetActive(false);
        StageUI.instance.PromoteAgent(this.model, script.ui.promotion);
        //StageUI에서 list 정리 시킬 것
        //StageUI.instance.PromoteApply(target);

       // if (target.level < 5) script.ui.promotion.gameObject.SetActive(true);
    }
}
