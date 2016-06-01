using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class CollectionWindow : MonoBehaviour, IActivatableObject {

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
    public Color zayin;
    public Color teth;
    public Color he;
    public Color waw;
    public Color aleph;
	public Image profImage;
    public RectTransform descList;
    public RectTransform observeList;

    public TextListScript listScirpt;
    public TextListScript observeScript;
    public RectTransform observeButton;

    public Animator windowAnim;

    public static string nodata= "unknown";

    [HideInInspector]
    public ActivatableObjectPos windowPos = ActivatableObjectPos.RIGHTUPPER;

    public RectTransform eventTriggerTarget;

    bool activatableObjectInitiated = false;

    [HideInInspector]
    public static CollectionWindow currentWindow = null;

    public void onClickObserveButton()
    {
        //Debug.Log("Work Count : "+creature.workCount+"Observe Condition : "+creature.observeCondition + "Observe Progress : "+creature.observeProgress+1);

        //SelectObserveAgentWindow.CreateWindow(creature);

        return;
		/*
        if (creature.NoticeDoObserve())
        {
            SelectObserveAgentWindow.CreateWindow(creature);
        }
        else
        {
            Debug.Log("관찰 조건이 충족되지 않았음");
        }
        */
    }

	public static void Create(CreatureModel creature)
    {

        //currentWindow.windowAnim.SetBool("isTrue", false);
        
        if (currentWindow.gameObject.activeSelf)
        {
            if (currentWindow.creature == creature)
            {
                return;
            }
        }
        else {
            currentWindow.gameObject.SetActive(true);
            currentWindow.Activate();
        }

        CollectionWindow wnd = currentWindow;

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
        if (wnd.activatableObjectInitiated == false) {
            wnd.UIActivateInit();
        }

        

        Canvas canvas = wnd.transform.GetChild(0).GetComponent<Canvas>();
        canvas.worldCamera = UIActivateManager.instance.GetCam();

        currentWindow = wnd;
        
        wnd.SetObserveText();
    }

    public void Start() {
        currentWindow = this;
        currentWindow.gameObject.SetActive(false);
    }

    

    public void DisplayData(CollectionWindow wnd) {
        CreatureTypeInfo info = wnd.GetCreature().metaInfo;
        CreatureTypeInfo.CreatureDataTable dataTable = info.dataTable;
        CreatureTypeInfo.ObserveTable table = info.observeTable;
        int currentObservationLevel = info.CurrentObserveLevel;

        string name = (string)dataTable.GetList("name").GetData(currentObservationLevel);
        DisplayText(name, wnd.name);
        string riskLevel = (string)dataTable.GetList("horrorLevel").GetData(currentObservationLevel);
        DisplayText(riskLevel, wnd.dangerLevel);

        switch(riskLevel)
        {
            case ("ZAYIN"): wnd.dangerLevel.color = zayin; Debug.Log("자인"); break;
            case ("TETH"): wnd.dangerLevel.color = teth; Debug.Log("테트"); break;
            case ("HE"): wnd.dangerLevel.color = he; Debug.Log("헤헤"); break;
            case ("WAW"): wnd.dangerLevel.color = waw; Debug.Log("와우"); break;
            case (" ALEPH"): wnd.dangerLevel.color = aleph; Debug.Log("알레프"); break;
            default: break;
        }

        string dangerRank = info.attackType.ToString() + " " + wnd.dangerLevel.text;
        wnd.DangerRank.text = dangerRank;

        switch (dangerRank)
        {
            case ("ZAYIN"): wnd.DangerRank.color = zayin; Debug.Log("자인2"); break;
            case ("TETH"): wnd.DangerRank.color = teth; Debug.Log("테트2"); break;
            case ("HE"): wnd.DangerRank.color = he; Debug.Log("헤헤2"); break;
            case ("WAW"): wnd.DangerRank.color = waw; Debug.Log("와우2"); break;
            case (" ALEPH"): wnd.DangerRank.color = aleph; Debug.Log("알레프2"); break;
            default: break;
        }
        profImage.sprite = ResourceCache.instance.GetSprite("Sprites/Unit/creature/dummy");
        /*
        if (currentObservationLevel >= dataTable.GetList("portrait").GetLevel(currentObservationLevel))
        {
            //print(creature.metaInfo.portraitSrc);
            //profImage.sprite = Resources.Load<Sprite>("Sprites/" + creature.metaInfo.portraitSrc);
        }
        else {
            profImage.sprite = ResourceCache.instance.GetSprite("Sprites/Unit/creature/dummy");
        }
        */
        /*
        CreatureTypeInfo info = wnd.GetCreature().metaInfo;
        int clevel = info.CurrentObserveLevel;
        
        DisplayText(clevel, table.name, wnd.name, info.name);
        DisplayText(clevel, table.attackType, wnd.attackType, info.attackType.ToString());
        DisplayText(clevel, table.riskLevel, wnd.dangerLevel, info.level);
        //DisplayText(clevel, nickname.text, 
        

        if (clevel >= table.portrait)
        {
            //Debug.Log("Sprites/" + info.portraitSrc);
            profImage.sprite = Resources.Load<Sprite>("Sprites/" + info.portraitSrc);

        }
        else {
            profImage.sprite = ResourceCache.instance.GetSprite("Sprites/Unit/creature/dummy");
        }
        */
        
        char[] determine = { '*' };
        for (int i = 0; i < table.desc.Count; i++) {
            if (currentObservationLevel < table.desc[i]) {
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

    private void DisplayText(string data, Text target) {
        if (data == null)
        {
            target.text = nodata;
        }
        else {
            target.text = data;
        }
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

        Deactivate();
        /*
        GameObject.FindGameObjectWithTag("AnimCollectionController")
            .GetComponent<Animator>().SetBool("isTrue", true);
         */
        currentWindow.gameObject.SetActive(false);
        //currentWindow.windowAnim.SetBool("isTrue", true);
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


        //currentWindow.observeButton.gameObject.SetActive((creature.NoticeDoObserve() && state));
    }

    public void OnClickPortrait() {
        Vector2 pos = currentWindow.creature.position;
        Camera.main.transform.position = new Vector3(pos.x, pos.y, -20f);
    }

    public void Activate()
    {
        UIActivateManager.instance.Activate(currentWindow, currentWindow.windowPos);
    }

    public void Deactivate()
    {
        UIActivateManager.instance.Deactivate(currentWindow.windowPos);
    }

    public void OnEnter()
    {
        UIActivateManager.instance.OnEnter(currentWindow);
    }

    public void OnExit()
    {
        UIActivateManager.instance.OnExit();
    }

    public void UIActivateInit()
    {
        activatableObjectInitiated = true;
        EventTrigger eventTrigger = this.eventTriggerTarget.GetComponent<EventTrigger>();
        if (eventTrigger == null)
        {
            eventTrigger = this.eventTriggerTarget.gameObject.AddComponent<EventTrigger>();
        }

        EventTrigger.Entry enter = new EventTrigger.Entry();
        EventTrigger.Entry exit = new EventTrigger.Entry();

        enter.eventID = EventTriggerType.PointerEnter;
        exit.eventID = EventTriggerType.PointerExit;

        enter.callback.AddListener((eventData) => { OnEnter(); });
        exit.callback.AddListener((eventData) => { OnExit(); });

        eventTrigger.triggers.Add(enter);
        eventTrigger.triggers.Add(exit);
    }

    public void Close()
    {
        this.CloseWindow();
    }
}
