using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[System.Serializable]
public class AgentAttributes {
    public AgentModel model;

    public Image hair;
    public Image face;
    public Image body;
    public Text HP;
    public Text Name;
    public Text Level;
    public Button Promotion;
    public Button Add;
    public Image current;
}

[System.Serializable]
public class AgentGetAttributes {
    public AgentModel model;

    public string name;
    public string hp;
    public string level;
    public Sprite sefira;
    public Sprite hair;
    public Sprite face;
    public Sprite body;
    /*
    public AgentGetAttributes() {
        name = null;
        hp = null;
        level = null;
        sefira = null;
        hair = null;
        face = null;
        body = null;
    }*/
}

public class AgentSlotScript : MonoBehaviour {

    public bool state = false;
    public GameObject small;
    public GameObject extend;
    public GameObject promote;

    int skill_promote;
    public int index;
    private bool nowstate;
    public bool promotionState;
    public bool smallstate;
    public bool bigstate;
    public AgentSlotInList scirpt;
    public AgentModel model;

    public AgentAttributes attr1;
    public AgentAttributes attr2;
    public AgentGetAttributes display;

    public void Start() {
        small.SetActive(true);        
        extend.SetActive(false);
        promote.SetActive(false);
        
    }

    public void Update() {
        small.SetActive(smallstate);
        extend.SetActive(bigstate);
        promote.SetActive(promotionState);
    }

    public void Change(int i) {
        /*
        state = !state;
        if (state)
        {
            StageUI.instance.SetExtendedList(index);
        }
        else {
            StageUI.instance.SetExtendedList(-1);
        }
        */
        //display = null;
        Debug.Log(i);
        switch (i) { 
            case 0:
                StageUI.instance.SetExtendedList(-1, 0);
                break;
            case 1:
                if (bigstate) {
                    StageUI.instance.SetExtendedList(-1, 0);
                    break;
                }
                StageUI.instance.SetExtendedList(index, 1);
                break;
            case 2:
                StageUI.instance.SetExtendedList(index, 2);
                break;

        }

        /*
        if (!promotionState)
        {
            
        }
        else
        {
            
        }*/
        Debug.Log(smallstate+""+bigstate+""+promotionState);

        
        Debug.Log(small.activeInHierarchy + " " + extend.activeInHierarchy + " " + promote.activeInHierarchy);
        StageUI.instance.ShowAgentList();
       
    }

    public void DisplayItems()
    {
        attr1.hair.sprite = attr2.hair.sprite = display.hair;
        attr1.face.sprite = attr2.face.sprite = display.face;
        attr1.body.sprite = attr2.body.sprite = display.body;

        attr1.HP.text = display.hp;
        attr1.Name.text = attr2.Name.text = display.name;
        attr1.Level.text = display.level;
        attr1.current.sprite = display.sefira;
    }

    public float GetSize() {
        float temp;

        if (bigstate)
        {
            temp = 0.45f;
        }
        else {
            temp = 0.15f;
        }
        return temp;
    }

    public void OnPromoteClick(int skill) {
        StageUI.instance.SetExtendedList(index, 0);
        skill_promote = skill;        
    }

    public void OnConfirm() {
        model.promoteSkill(skill_promote);
        StageUI.instance.PromotionAgent(model, model.level, scirpt.promotion);
    }

    public void ShowPromotionButton(AgentModel agent)
    {
        //Debug.Log(agent);
        scirpt = small.GetComponent<AgentSlotInList>();
        if (agent.expSuccess < 2 && agent.expSuccess >= 0 && agent.level == 1)
        {
            
            scirpt.promotion.gameObject.SetActive(true);
            //button.gameObject.SetActive(true);
            //button.GetComponentInChildren<UnityEngine.UI.Text>().text = "승급 비용 2";
            //button.onClick.AddListener(() => PromotionAgent(agent, 1, button));
        }

        else if (agent.expSuccess < 3 && agent.expSuccess >= 2 && agent.level == 2)
        {

            scirpt.promotion.gameObject.SetActive(true);
            //button.GetComponentInChildren<UnityEngine.UI.Text>().text = "승급 비용 5";
            //button.onClick.AddListener(() => PromotionAgent(agent, 2, button));
        }

        else
        {
            scirpt.promotion.gameObject.SetActive(false);
        }
    }
}
