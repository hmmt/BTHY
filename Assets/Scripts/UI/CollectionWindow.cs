using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CollectionWindow : MonoBehaviour {

    private CreatureModel creature;
    public GameObject backgroundDefault;
    public GameObject backgroundDiary;

    public Text descText;
    public Text observeText;

    //public UI.Text observeSubText;

	public Text code;
	public Text name;
    public Text nickname;
	public Text dangerLevel;
	public Text attackType;
	public Text intLevel;
    public Text observePercent;
    public Text DangerRank;//same with dangerLevel
	public Image profImage;
    public RectTransform descList;
    public GameObject description;
    public TextListScript listScirpt;
    
    [HideInInspector]
    public static CollectionWindow currentWindow = null;

    public void UpdateBg(string type)
    {
        if (type == "diary")
        {
            backgroundDefault.SetActive(false);
            backgroundDiary.SetActive(true);
        }
        else
        {
            backgroundDefault.SetActive(true);
            backgroundDiary.SetActive(false);
        }
    }

    public void onClickObserveButton()
    {
        Debug.Log("Work Count : "+creature.workCount+"Observe Condition : "+creature.observeCondition + "Observe Progress : "+creature.observeProgress+1);

        if (creature.NoticeDoObserve())
        {
            SelectObserveAgentWindow.CreateWindow(creature);
        }
        else
        {
            Debug.Log("관찰 조건이 충족되지 않았음");
        }
    }

	public static void Create(CreatureModel creature)
    {
        GameObject wndObject;
        CollectionWindow wnd;
        
        if (currentWindow != null)
        {
            wndObject = currentWindow.gameObject;
            //currentWindow.CloseWindow();
        }
        else
        {
            wndObject = Prefab.LoadPrefab("CollectionWindow");
        }
        wnd = wndObject.GetComponent<CollectionWindow>();

        wnd.listScirpt = wnd.descList.GetComponent<TextListScript>();
        wnd.listScirpt.DeleteAll();

        wnd.creature = creature;

        wnd.descText.text = creature.metaInfo.desc;
        wnd.observeText.text = creature.GetObserveText();

		wnd.name.text = creature.metaInfo.name;
		wnd.code.text = creature.metaInfo.codeId;
		wnd.attackType.text = creature.metaInfo.attackType;
		wnd.intLevel.text = creature.metaInfo.intelligence.ToString();
		wnd.dangerLevel.text = creature.metaInfo.level.ToString();
        wnd.observePercent.text = (float)creature.observeProgress / creature.metaInfo.observeLevel * 100+"%";
        wnd.nickname.text = wnd.name.text;
		wnd.profImage.sprite = Resources.Load<Sprite>("Sprites/" + creature.metaInfo.imgsrc);
        wnd.DangerRank.text =wnd.attackType.text + " " + wnd.dangerLevel.text;
        wnd.UpdateBg("default");

        string descTextfull = creature.metaInfo.desc;
        char[] determine = {'*'};
        string[] descary = descTextfull.Split(determine);
        
        foreach (string str in descary) {
            if (str.Equals(" ") || str.Equals("")) continue;
            wnd.listScirpt.MakeText(str);
        }
        wnd.listScirpt.SortList();
        currentWindow = wnd;
    }


    public CreatureModel GetCreature()
    {
        return creature;
    }

    public void CloseWindow()
    {
        //currentWindow = null;
        GameObject.FindGameObjectWithTag("AnimCollectionController")
            .GetComponent<Animator>().SetBool("isTrue", true);
        //Destroy(gameObject);
    }
}
