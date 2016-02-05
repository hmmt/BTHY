using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public struct SlotData
{
    public string name;
    public string level;
    public string life;
    public Sprite sefira;
    public Sprite hair;
    public Sprite face;
    public Sprite body;
}

[System.Serializable]
public class SlotAttributes {

    public Image hair;
    public Image face;
    public Image body;
    public Text name;
    public Text level;
    public Text life;
    public Button promotion;
    public Button allocate;
    public Image currentSefira;

    private SlotData data;

    public void SetSefira(AgentModel model) {
        data.sefira = model.getCurrentSefiraSprite();

        currentSefira.sprite = data.sefira;
    }

    public void ActivateAllocateButton(AgentModel model) {
        allocate.gameObject.SetActive(true);

        if (StageUI.instance.currentSefriaUi == "0")
        {
            allocate.gameObject.SetActive(false);
            return;
        }

        if (!PlayerModel.instance.IsOpenedArea(StageUI.instance.currentSefriaUi)) {
            allocate.gameObject.SetActive(false);
            return;
        }
        if ((model.currentSefira == StageUI.instance.currentSefriaUi)) {
            allocate.gameObject.SetActive(false);
        }
    }

    public void SetData(AgentModel model) {
        data.name = model.name;
        data.level = model.level + "";
        data.life = model.LifeStyle();
        SetSefira(model);
        data.hair = model.tempHairSprite;
        data.face = model.tempFaceSprite;
        //data.body = ResourceCache.instance.GetSprite(model.bodyImgSrc);
    }

    public void SetAttributes() {
        hair.sprite = data.hair;
        face.sprite = data.face;
        //body.sprite = data.body;
        name.text = data.name;
        level.text = data.level;
        life.text = data.life;
        currentSefira.sprite = data.sefira;
    }
}
/*
public class SlotData
{
    public string name;
    public string level;
    public string life;
    public Sprite sefira;
    public Sprite hair;
    public Sprite face;
    public Sprite body;

    public void Setdata(AgentModel model) {
        name = model.name;
        level = model.level + "";
        life = model.LifeStyle();
        sefira = model.getCurrentSefiraSprite();
        hair = ResourceCache.instance.GetSprite(model.hairImgSrc);
        face = ResourceCache.instance.GetSprite(model.faceImgSrc);
        body = ResourceCache.instance.GetSprite(model.bodyImgSrc);
    }

    public void SetSefira(AgentModel model) {
        sefira = model.getCurrentSefiraSprite();
    }
}
*/

public class ListSlotScript : MonoBehaviour {
    private AgentModel model;
    public SlotAttributes ui;
    public int index = 0;
    public bool state = false;//false == small, true == extended;
    
    public void Init(AgentModel model) {
        SetModel(model);
        
        ui.SetData(model);
        ui.SetAttributes();
        ui.ActivateAllocateButton(model);
    }

    public void SetModel(AgentModel model)
    {
        this.model = model;
    }

    public AgentModel getModel() {
        return this.model;
    }

    public void SetChange() {
        ui.SetData(model);
        ui.SetAttributes();
        ui.ActivateAllocateButton(model);
    }

    public void SetSefiraChange() {
        ui.SetSefira(model);
    }

    public float GetHeight() {
        float temp;
        temp = gameObject.GetComponent<RectTransform>().rect.height;
        return temp;
    }

    public void OnAllocateClick() {
        StageUI.instance.SetAgentSefriaButton(model);
        SetChange();
    }

    public void OnAllocateCancel() { 
        
    }
    
    public void OnExtension()
    {
        if (this.state) {
            AgentListScript.instance.SetExtendedDisabled();
            this.state = false;
            return;
        }
        this.state = true;
        AgentListScript.instance.SetExtended(this);
    }

    public void ShowPromotionButton()
    {
        AgentListScript.instance.ActivatePromotionPanel(this);
    }

}
