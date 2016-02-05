using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[System.Serializable]
public class AgentAttributes {

    public Image hair;
    public Image face;
    public Image body;
    public Text HP;
    public Text Name;
    public Text Level;
    public Button Promotion;
    public Button Add;
    public Image current;//Current Sefira
}

[System.Serializable]
public class AgentGetAttributes {

    public string name;
    public string hp;
    public string level;
    public Sprite sefira;
    public Sprite hair;
    public Sprite face;
    public Sprite body;

}

public class AgentSlotScript : MonoBehaviour {

    public bool state = false;
    public GameObject small;
    public GameObject promote;

    int skill_promote;
    public int index;
    private bool nowstate;
    public bool promotionState;
    public bool smallstate;

    public AgentSlotInList scirpt;
    public AgentModel model;

    public AgentAttributes attr1;
    public AgentGetAttributes display;
    public Image PanelImage;

    public void Start() {
        small.SetActive(true);
        promote.SetActive(false);

        scirpt = small.GetComponent<AgentSlotInList>();
    }

    /*
    public void Update() {
        small.SetActive(smallstate);
        promote.SetActive(promotionState);
    }
    */

    public void setValues() {
        attr1.Name.text = model.name;
        
        attr1.Level.text = "" + model.level;
        attr1.HP.text = model.LifeStyle();
        switch (model.currentSefira)
        {
            case "0":
                attr1.current.sprite = ResourceCache.instance.GetSprite("Sprites/UI/StageUI/None_Icon");
                break;
            case "1":
                attr1.current.sprite = ResourceCache.instance.GetSprite("Sprites/UI/StageUI/Malkuth_Icon");
                break;
            case "2":
                attr1.current.sprite = ResourceCache.instance.GetSprite("Sprites/UI/StageUI/Netzzach_Icon");
                break;
            case "3":
                attr1.current.sprite = ResourceCache.instance.GetSprite("Sprites/UI/StageUI/Hod_Icon");
                break;
            case "4":
                attr1.current.sprite = ResourceCache.instance.GetSprite("Sprites/UI/StageUI/Yessod_Icon");
                break;
            default:
                attr1.current.sprite = ResourceCache.instance.GetSprite("Sprites/UI/StageUI/None_Icon");
                break;
        }
        attr1.hair.sprite = ResourceCache.instance.GetSprite(model.hairImgSrc);
        attr1.face.sprite = ResourceCache.instance.GetSprite(model.faceImgSrc);
        attr1.body.sprite = ResourceCache.instance.GetSprite(model.bodyImgSrc);
        smallstate = true;

        //DisplayItems();
    }

    /*
    public void Change(int i) {        
        switch (i) { 
            case 0://초기화
                StageUI.instance.SetInformationState(false);
                StageUI.instance.SetExtendedList(-1);
                smallstate = true;
                promotionState = false;
                break;
            case 1://진급
                StageUI.instance.SetInformationState(false);
                StageUI.instance.SetExtendedList(-1);
                smallstate = true;
                promotionState = true;
                break;
            case 2://정보창
                StageUI.instance.SetInformationState(false);
                StageUI.instance.SetExtendedList(-1);
                smallstate = false;
                promotionState = false;
                break;
        }

        StageUI.instance.ShowAgentList();
    }
    */

    public void DisplayItems()
    {
        attr1.hair.sprite = display.hair;
        attr1.face.sprite = display.face;
        attr1.body.sprite = display.body;

        attr1.HP.text = display.hp;
        attr1.Name.text = display.name;
        attr1.Level.text = display.level;
        attr1.current.sprite = display.sefira;
    }

    public float GetSize() {
        float temp;
        temp = small.GetComponent<RectTransform>().rect.height;
        if (!smallstate)
        {
            temp *= 3;
        }

        return temp;
    }

    public void OnExtention() {
        StageUI.instance.SetExtendedList(true, index);
        smallstate = false;
        //StageUI.instance.ShowAgentList();
    }

    public void OnPromoteClick(int skill) {
        //StageUI.instance.SetExtendedList(false, 0);
        promote.SetActive(true);
        small.SetActive(false);
        skill_promote = skill;
    }

    public void OnConfirm() {
        model.promoteSkill(skill_promote);
        StageUI.instance.PromotionAgent(model, model.level, scirpt.promotion);
        promote.SetActive(false);
        small.SetActive(true);
    }

    public void OnCancel() {
        skill_promote = -1;
        promote.SetActive(false);
        small.SetActive(true);
    }
    
    public void ShowPromotionButton(AgentModel agent)
    {
        if (agent.expSuccess < 2 && agent.expSuccess >= 0 && agent.level == 1)
        {
            scirpt.promotion.gameObject.SetActive(true);
        }

        else if (agent.expSuccess < 3 && agent.expSuccess >= 2 && agent.level == 2)
        {
            scirpt.promotion.gameObject.SetActive(true);
        }

        else
        {
            scirpt.promotion.gameObject.SetActive(false);
        }
    }
}
