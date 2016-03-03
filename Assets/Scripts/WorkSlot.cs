using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class WorkSlot : MonoBehaviour, IObserver
{
    public int index;
    public RectTransform initialRect;
    public WorkInventory inventory;//일단 쓸 지 안쓸지 모르겠음
    public GameObject SelectImage;
	public CreatureModel targetCreature;
    public SkillTypeInfo currentSkill;
    public RectTransform NormalState;
    public Text Hint;
    public Button Add;
    public Button Sub;
    public Toggle LockButton;
    public Button Cnt;
    public Text CountText;
    public Image Success;
    private bool extended = false;
    private int agentcnt;
    private List<AgentModel> agentList;
    private float possibility = 0.0f;
    public Sprite[] LockSprite;

    public GameObject AgentPortrait;
    public Image Face;
    public Image Hair;

    public string test = "";

	/*
    public void Awake() {
        agentcnt = 0;
        SetButtonActive();
        SetCountText();
    }*/

	public void Init(WorkSettingElement workSetting, int index)
	{
        agentList = new List<AgentModel>();
        //NormalState = gameObject.transform.GetChild(2).GetComponent<RectTransform>();
		targetCreature = workSetting.creature;
		this.index = index;
		this.agentcnt = workSetting.slots [index].agentCnt;
		if (workSetting.slots [index].skill != null) {
			SetCurrentSkill (workSetting.slots [index].skill);
		} else {
			ClearCurrentSkill ();
        }
        Notice.instance.Observe(NoticeName.ReportAgentSuccess, this);
        Notice.instance.Observe(NoticeName.WorkEndReport, this);

		SetButtonActive();
		SetCountText();
        CalcSuccessPossibility();
        int randVal = Random.Range(0, 100);
        this.test = "" + randVal;
        SetLockImage();
        SelectImage.gameObject.SetActive(false);
        AgentPortrait.gameObject.SetActive(false);
	}

    public void SetLockImage() {
        Toggle LockItem = LockButton.GetComponent<Toggle>();
        Image LockImage = LockItem.GetComponent<Image>();
        if (LockItem.isOn)
        {
            LockImage.sprite = LockSprite[0];
        }
        else { 
            LockImage.sprite = LockSprite[1];
        }
    }

    public void SetInventoryScript(WorkInventory script) {
        this.inventory = script;
    }

    public GameObject GetIcon() {
        return NormalState.GetChild(1).gameObject;
    }

    public GameObject GetLeftImage() {
        return NormalState.gameObject;
    }

    public GameObject GetText() {
        return NormalState.GetChild(2).gameObject;
    }

    public GameObject GetImage() {
        return NormalState.GetChild(0).gameObject;
    }

    public void ClearCurrentSkill() {
		this.currentSkill = null;
        //Debug.Log(NormalState);
        GameObject icon = GetIcon();
        //임시스프라이트
        icon.GetComponent<Image>().sprite = ResourceCache.instance.GetSprite("Sprites/UI/" + "warning");
        GameObject text = GetText();
        text.GetComponent<Text>().text = "Unassigned";
        text.GetComponent<Text>().fontSize = 13;

		UpdateWorkSetting ();
    }

    public void SetCurrentSkill(SkillTypeInfo skill) {
        this.currentSkill = skill;

        GameObject icon = GetIcon();
        icon.GetComponent<Image>().sprite = ResourceCache.instance.GetSprite("Sprites/" + skill.imgsrc);
        GameObject text = GetText();
        text.GetComponent<Text>().text = skill.name;

        text.GetComponent<Text>().fontSize = 14;

		UpdateWorkSetting ();
    }

    public bool IsLocked() {
        return LockButton.isOn;
    }

    public void SetCountText() {
        CountText.text = this.agentcnt + "";
    }

    public void AddAgent() {
        if (agentcnt == 5) {
            return;
        }
        agentcnt++;
        SetButtonActive();
        SetCountText();
		UpdateWorkSetting ();
    }

    public void SubAgent()
    {
        if (agentcnt == 0)
        {
            return;
        }

        agentcnt--;

		UpdateWorkSetting ();

        SetButtonActive();
        SetCountText();
    }

    public void SetButtonActive() {
        if (extended) return;
        if (agentcnt == 0)
        {
            Sub.gameObject.SetActive(false);
        }
        else if(Sub.gameObject.activeSelf == false){
            Sub.gameObject.SetActive(true);
        }

        if (agentcnt == 5)
        {
            Add.gameObject.SetActive(false);
        }
        else if (Add.gameObject.activeSelf == false) {
            Add.gameObject.SetActive(true);
        }
    }

    public int GetCount() {
        return this.agentcnt;
    }

    public void SetHintText(string str) {
        this.Hint.text = str;
    }

    public void ClearAgentCnt() {
        this.agentcnt = 0;
		UpdateWorkSetting ();

        SetCountText();
        SetButtonActive();
    }

	private void UpdateWorkSetting()
	{
		WorkSettingElement setting = TempAgentAI.instance.GetWorkSetting (targetCreature);
		setting.slots [index].agentCnt = agentcnt;
		setting.slots [index].skill = currentSkill;
		Notice.instance.Send (NoticeName.ChangeWorkSetting, setting.creature);
	}

    public void OnNotice(string notice, params object[] param)
    {
        if (notice == NoticeName.ReportAgentSuccess) {
            if ((param[1] as CreatureModel).Equals(this.targetCreature) 
                && (param[2] as SkillTypeInfo).Equals(this.currentSkill)) {
                this.agentList.Add((AgentModel)param[0]);

                SetAgentPortrait(agentList[0]);
            }
            //calc success percentage
            CalcSuccessPossibility();
        }
        else if(notice == NoticeName.WorkEndReport){
            ClearAgentPortrait();
        }
    }

    public void SetAgentPortrait(AgentModel model) {
        this.AgentPortrait.gameObject.SetActive(true);
        this.Face.sprite = model.tempFaceSprite;
        this.Hair.sprite = model.tempHairSprite;
    }

    public void ClearAgentPortrait() {
        this.AgentPortrait.gameObject.SetActive(false);
        
    }

    private void CalcSuccessPossibility() {
        Image img = GetImage().GetComponent<Image>();
        if (img == null) {
            img = GetImage().AddComponent<Image>();
        }
        if (agentList.Count == 0) {
            img.color = Color.black;
            return;
        }
        float sum = 0.0f;
        
        foreach (AgentModel am in this.agentList) {
            sum += am.successPercent;
        }
        this.agentList.Clear();

        if(sum > 100f) sum = 100f;
        /*
        if (0.0f < sum&& sum <= 33f) {
            img.color = Color.red;
        }
        else if(33f < sum && sum <= 66f){
            img.color = Color.red + Color.green;
        }
        else {
            img.color = Color.green;
        }*/
    }

    public void CloseWindow() {
        Notice.instance.Remove(NoticeName.ReportAgentSuccess, this);
    }
}
