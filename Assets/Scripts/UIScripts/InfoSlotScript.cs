using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

[System.Serializable]
public class ImageList
{
    public Sprite[] list;
}

[System.Serializable]
public class SpriteList {
    public ImageList health;
    public ImageList Mental;
    public ImageList workSpeed;
    public ImageList movementSpeed;
    public ImageList workList;
}

//Lists of worker's stats info e.g. Hp Max & average
public class ValueInfo {
    public int[] stats = new int[4];
    public int hp;
    public int mental;
    public int workSpeed;
    public int movementSpeed;

    public ValueInfo(int hp, int mental, int workspeed, int movement) {
        this.hp = hp;
        this.mental = mental;
        this.workSpeed = workspeed;
        this.movementSpeed = movement;
        this.stats[0] = hp;
        this.stats[1] = mental;
        this.stats[2] = workspeed;
        this.stats[3] = movement;
    }

    public static ValueInfo getAverage() {
        return new ValueInfo(5, 100, 5, 5);
    }
}

public class InfoSlotScript : MonoBehaviour {
    public GameObject[] description;
    public GameObject selected;//the agent which now selected
    public GameObject CharacterSlot;
    public Image[] InfoImageList;
    public Image[] WorkImageList;
    public Text[] WorkDescription;
    public GameObject parent;
    public GameObject text;
    private string[] desc;
    private string[] workDesc;

    public SpriteList list;
    private ValueInfo average = new ValueInfo(5, 100, 5, 5);
    private ValueInfo level;// levels for now selected
        
	// Use this for initialization
	void Start () {
        foreach (GameObject temp in description) {
            temp.SetActive(false);            
        }
        //selected = null;
        selected = description[0];
        description[0].SetActive(true);
        desc = new string[InfoImageList.Length];
        workDesc = new string[WorkImageList.Length];
        foreach (Image temp in InfoImageList)
        {
            temp.gameObject.SetActive(true);
            EventTrigger tri = temp.gameObject.AddComponent<EventTrigger>();
            EventTrigger.Entry enter = new EventTrigger.Entry();
            EventTrigger.Entry exit = new EventTrigger.Entry();
            enter.eventID = EventTriggerType.PointerEnter;
            
            exit.eventID = EventTriggerType.PointerExit;

            OverlayObject overlayItem = temp.gameObject.AddComponent<OverlayObject>();
            enter.callback.AddListener((eventdata) => { overlayItem.Overlay(); });
            exit.callback.AddListener((eventdata) => { overlayItem.Hide(); });
            tri.triggers.Add(enter);
            tri.triggers.Add(exit);
        }
        /*
        foreach (Image temp in WorkImageList)
        {

            temp.gameObject.SetActive(true);

            EventTrigger tri = temp.gameObject.AddComponent<EventTrigger>();
            EventTrigger.Entry enter = new EventTrigger.Entry();
            EventTrigger.Entry exit = new EventTrigger.Entry();
            enter.eventID = EventTriggerType.PointerEnter;
            exit.eventID = EventTriggerType.PointerExit;

            OverlayObject overlayItem = temp.gameObject.AddComponent<OverlayObject>();
            enter.callback.AddListener((eventdata) => { overlayItem.Overlay(); });

            exit.callback.AddListener((eventdata) => { overlayItem.Hide(); });
            tri.triggers.Add(enter);
            tri.triggers.Add(exit);
        }

        foreach (Text temp in WorkDescription) {
            temp.gameObject.SetActive(true);
        }
        */
	}
    
    public void OnPointerEnter(GameObject target) {
        foreach (GameObject temp in description) {
            if (temp.Equals(target))
            {
                temp.SetActive(true);
            }
            else {
                temp.SetActive(false);
            }
        }
        
    }

    public void SelectedAgent(AgentModel model)
    {
        //CharacterSlot.GetComponent<InfoCharacterScript>().setSlot(script);
        SetSprite(model);
        if (parent.transform.childCount > 0) {
            int i = parent.transform.childCount;
            for (int j = 0; j < i; j++) {
                Destroy(parent.transform.GetChild(j).gameObject);
            }
        }
        /*
        foreach (TraitTypeInfo t in model.traitList) {
            GameObject temp = Instantiate(text);
            temp.GetComponentInChildren<Text>().text = t.name;
            temp.transform.SetParent(parent.transform);
            temp.transform.localScale = new Vector3(1f, 1f, 1f);
        }
        */
        
        
        for (int i = 0; i < InfoImageList.Length; i++) {
            InfoImageList[i].gameObject.SetActive(true);
        }
        /*
        foreach (Image temp in WorkImageList)
        {
            temp.gameObject.SetActive(true);
        }
        foreach (Text temp in WorkDescription) {
            temp.gameObject.SetActive(true);
        }*/
        TextListScript listScript = transform.GetComponent<TextListScript>();

        listScript.DeleteAll();
        

        foreach (TraitTypeInfo t in model.traitList) {
            //
            listScript.MakeTraits(t);
        }
        listScript.SortBgListWithTraits();

        //SkillList작성
        AgentSkillListScript skillListScript = transform.GetComponent<AgentSkillListScript>();
        skillListScript.Init(model);
    }

    public void SetSprite(AgentModel model) {

        desc[0] = model.maxHp + "";
        desc[2] = model.workSpeed + "";
        desc[1] = model.maxMental + "";
        desc[3] = model.movement + "";
		/*
        workDesc[0] = model.directSkill.description;
        workDesc[1] = model.indirectSkill.description;
        workDesc[2] = model.blockSkill.description;
        */

        for(int i = 0; i< InfoImageList.Length; i++){
            InfoImageList[i].GetComponent<Image>().sprite = model.StatusSprites[i];
            OverlayObject script = InfoImageList[i].GetComponent<OverlayObject>();
            script.text = desc[i];
        }
        /*
        for (int i = 0; i < WorkImageList.Length; i++) { 
            WorkImageList[i].GetComponent<Image>().sprite = model.WorklistSprites[i];
            OverlayObject script = WorkImageList[i].GetComponent<OverlayObject>();
            script.text = workDesc[i];
            
        }*/
		/*
        WorkDescription[0].text = model.directSkill.name;
        WorkDescription[1].text = model.indirectSkill.name;
        WorkDescription[2].text = model.blockSkill.name;
        */
    }

}
