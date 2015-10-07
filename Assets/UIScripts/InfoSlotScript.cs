using UnityEngine;
using System.Collections;
using UnityEngine.UI;

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
}

public class InfoSlotScript : MonoBehaviour {
    public GameObject[] description;
    public GameObject selected;//the agent which now selected
    public GameObject CharacterSlot;
    public Image[] InfoImageList;
    public Image[] WorkImageList;
    public GameObject parent;
    public GameObject text;

    public SpriteList list;
    private ValueInfo average = new ValueInfo(5, 100, 5, 5);
    private ValueInfo level;// levels for now selected
        
	// Use this for initialization
	void Start () {
        foreach (GameObject temp in description) {
            temp.SetActive(false);            
        }
        //selected = null;

        foreach (Image temp in InfoImageList)
        {
            temp.gameObject.SetActive(true);
        }

        foreach (Image temp in WorkImageList)
        {
            temp.gameObject.SetActive(true);
        }
        SelectedAgent();
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

    public void SelectedAgent() {

        AgentSlotScript script = selected.GetComponent<AgentSlotScript>();
        //CharacterSlot.GetComponent<InfoCharacterScript>().setSlot(script);
        ValueInfo level = calcLevel(script.model);
        SetSprite(level, script.model);
        if (parent.transform.childCount > 0) {
            int i = parent.transform.childCount;
            for (int j = 0; j < i; j++) {
                Destroy(parent.transform.GetChild(j).gameObject);
            }
        }

        foreach (TraitTypeInfo t in script.model.traitList) {
            GameObject temp = Instantiate(text);
            temp.GetComponentInChildren<Text>().text = t.name;
            temp.transform.SetParent(parent.transform);
            temp.transform.localScale = new Vector3(1f, 1f, 1f);
        }

        foreach (Image temp in InfoImageList)
        {
            temp.gameObject.SetActive(true);
        }
        foreach (Image temp in WorkImageList)
        {
            temp.gameObject.SetActive(true);
        }
    }

    public void SetSprite(ValueInfo level, AgentModel model) {
        string loc = "UIResource/Icons/";
        for(int i = 0; i< InfoImageList.Length; i++){
            string fullpath = loc + i + level.stats[i];
            
            InfoImageList[i].GetComponent<Image>().sprite = ResourceCache.instance.GetSprite(fullpath);
            model.StatusSprites[i] = InfoImageList[i].GetComponent<Image>().sprite;
        }
        for (int i = 0; i < WorkImageList.Length; i++) { 
            string fullpath = loc + "Work_" + i;
            WorkImageList[i].GetComponent<Image>().sprite = ResourceCache.instance.GetSprite(fullpath);
            model.WorklistSprites[i] = WorkImageList[i].GetComponent<Image>().sprite;
        }
    }

    private ValueInfo calcLevel(AgentModel model) {
        int a, b, c, d;
        ValueInfo level;

        a = calc(model.hp, average.hp);
        b = calc(model.mental, average.mental);
        c = calc(model.work, average.workSpeed);
        d = calc(model.movement, average.movementSpeed);

        level = new ValueInfo(a, b, c, d);
        return level;
    }

    private int calc(int value, int standard)
    {
        if (value < standard)
        {
            return 0;
        }
        else if (value >= standard && value < 2 * standard)
        {
            return 1;
        }
        else return 2;
    }
}
