using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WorkSlot : MonoBehaviour
{
    public int index;
    public RectTransform initialRect;
    public WorkInventory inventory;//일단 쓸 지 안쓸지 모르겠음

    public SkillTypeInfo currentSkill;
    public RectTransform NormalState;
    public Text Hint;
    public Button Add;
    public Button Sub;
    public Toggle LockButton;
    public Button Cnt;
    public Text CountText;
    private bool extended = false;
    private int agentcnt;

    public void Awake() {
        agentcnt = 0;
        SetButtonActive();
        SetCountText();
    }

    public void SetInventoryScript(WorkInventory script) {
        this.inventory = script;
    }

    public GameObject GetIcon() {
        return NormalState.GetChild(0).gameObject;
    }

    public GameObject GetText() {
        return NormalState.GetChild(1).gameObject;
    }

    public void ClearCurrentSkill() {
        GameObject icon = GetIcon();
        //임시스프라이트
        icon.GetComponent<Image>().sprite = ResourceCache.instance.GetSprite("warning");
        GameObject text = GetText();
        text.GetComponent<Text>().text = "할당되지 않음";
    }

    public void SetCurrentSkill(SkillTypeInfo skill) {
        this.currentSkill = skill;

        GameObject icon = GetIcon();
        icon.GetComponent<Image>().sprite = ResourceCache.instance.GetSprite(skill.imgsrc);
        GameObject text = GetText();
        text.GetComponent<Text>().text = skill.name;
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
    }

    public void SubAgent()
    {
        if (agentcnt == 0)
        {
            return;
        }

        agentcnt--;
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
        SetCountText();
        SetButtonActive();
    }

}
