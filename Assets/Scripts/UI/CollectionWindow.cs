using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class CollectionWindow : MonoBehaviour {

    private CreatureModel creature;
    public Text[] low;
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
    public RectTransform observeList;

    public TextListScript listScirpt;
    public TextListScript observeScript;
    public RectTransform observeButton;

    public static string nodata= "unknown";

    [HideInInspector]
    public static CollectionWindow currentWindow = null;

    public void onClickObserveButton()
    {
        Debug.Log("Work Count : "+creature.workCount+"Observe Condition : "+creature.observeCondition + "Observe Progress : "+creature.observeProgress+1);

        SelectObserveAgentWindow.CreateWindow(creature);

        return;
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
        wnd.observeScript = wnd.observeList.GetComponent<TextListScript>();
        wnd.listScirpt.DeleteAll();

        wnd.creature = creature;
        /*
        wnd.descText.text = creature.metaInfo.desc;
        wnd.observeText.text = creature.GetObserveText();
        */

        /*
         from here, should display data by observe level
         */

        wnd.DisplayData(wnd);

        /*
		wnd.profImage.sprite = Resources.Load<Sprite>("Sprites/" + creature.metaInfo.imgsrc);
        if (creature.metaInfo.tempPortrait != null) {
            wnd.profImage.sprite = creature.metaInfo.tempPortrait;
        }*/


        //wnd.DangerRank.text =wnd.attackType.text + " " + wnd.dangerLevel.text;
       // wnd.UpdateBg("default");
        //wnd.observeButton.gameObject.SetActive(false);
        
        /*
        string descTextfull = creature.metaInfo.desc[0];
        char[] determine = {'*'};
        string[] descary = descTextfull.Split(determine);
        
        foreach (string str in descary) {
            Debug.Log(str);
            if (str.Equals(" ") || str.Equals("")) continue;
            wnd.listScirpt.MakeTextWithBg(str);
        }
        wnd.listScirpt.SortBgList();
         * 
         */
        currentWindow = wnd;
        
        wnd.SetObserveText();
    }

    public void DisplayData(CollectionWindow wnd) {
        CreatureTypeInfo info = wnd.GetCreature().metaInfo;
        CreatureTypeInfo.ObserveTable table = info.observeTable;
        int clevel = info.CurrentObserveLevel;

        DisplayText(clevel, table.name, wnd.name, info.name);
        DisplayText(clevel, table.attackType, wnd.attackType, info.attackType.ToString());
        DisplayText(clevel, table.riskLevel, wnd.dangerLevel, info.level);
        //DisplayText(clevel, nickname.text, 
        string dangerRank = wnd.attackType.text + " " + wnd.dangerLevel.text;
        wnd.DangerRank.text = dangerRank;

        if (clevel >= table.portrait)
        {
            profImage.color = new Color(1, 1, 1, 1);
            //Debug.Log("Sprites/" + info.portraitSrc);
            profImage.sprite = Resources.Load<Sprite>("Sprites/" + info.portraitSrc);

        }
        else {
            profImage.color = new Color(1, 1, 1, 0);
        }

        char[] determine = { '*' };
        for (int i = 0; i < table.desc.Count; i++) {
            if (clevel < table.desc[i]) {
                continue;
            }

            string descTextFull = info.desc[i];
            string[] descary = descTextFull.Split(determine);
            foreach (string str in descary) {
                if (str.Equals(" ") || str.Equals("")) continue;
                wnd.listScirpt.MakeTextWithBg(str);
            }
            
        }
        wnd.listScirpt.SortBgList();
        
        //record(관찰기록 띄우기)
    }

    private void DisplayText(int current, int level, Text target, string data)
    {
        if (current >= level)
        {
            target.text = data;
        }
        else {
            target.text = nodata;   
        }
    }

    public void SetObserveText() {
        string output = currentWindow.creature.GetObserveText();
        currentWindow.observeScript.DeleteAll();
        currentWindow.observeScript.MakeTextWithBg(output);
        currentWindow.observeScript.SortBgList();
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

    public void setObserveButton(bool mode) {
        currentWindow.observeButton.gameObject.SetActive(mode);
    }

    public void FixedUpdate() {
        bool state;
        if (currentWindow.creature.state == CreatureState.WAIT)
        {
            state = true;
        }
        else {
            state = false;
        }


        currentWindow.observeButton.gameObject.SetActive((creature.NoticeDoObserve() && state));
    }

    public void OnClickPortrait() {
        Vector2 pos = currentWindow.creature.position;
        Camera.main.transform.position = new Vector3(pos.x, pos.y, -20f);
    }
}
