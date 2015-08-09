using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AgentList : MonoBehaviour {

    public Transform agentScrollTarget;
    public Transform anchor;
    public Animator slideAnim;

    private int state1 = 0;

    List<AgentModel> selectedAgentList = new List<AgentModel>();

    public static AgentList currentWindow = null;

    /*
    public static AgentList CreateWindow()
    {
        if (currentWindow != null)
        {
            currentWindow.CloseWindow();
        }

        GameObject newObj = Prefab.LoadPrefab("AgentList");

        AgentList inst = newObj.GetComponent<AgentList>();

        inst.ShowAgentList();
        inst.slideAnim.SetBool("Slide", false);

        currentWindow = inst;
        return inst;

    }
    */

    // Use this for initialization
    public void ShowAgentList()
    {
        AgentModel[] agents = AgentManager.instance.GetAgentList();

        foreach (Transform child in agentScrollTarget.transform)
        {
            Destroy(child.gameObject);
        }

        float posy = 0;
        foreach (AgentModel unit in agents)
        {

            GameObject slot = Prefab.LoadPrefab("AgentSlotPanelList");

            slot.transform.SetParent(agentScrollTarget, false);

            RectTransform tr = slot.GetComponent<RectTransform>();
            tr.localPosition = new Vector3(0, posy, 0);
            AgentSlotPanelList slotPanel = slot.GetComponent<AgentSlotPanelList>();

            slotPanel.panelImage.sprite = Resources.Load<Sprite>("Sprites/UI/SystemUI/AgentListPanel");
            slotPanel.agentName.text = unit.name;
            slotPanel.agentHP.text = HealthCheck(unit);
            slotPanel.agentMental.text = MentalCheck(unit);
            slotPanel.agentLevel.text = "직원 등급" + unit.level;
            slotPanel.agentSefria.text = SefiraCheck(unit);

            AgentModel copied = unit;
            slotPanel.agentInfoButton.onClick.AddListener(() => AgentStatusOpen(copied));


            Texture2D tex3 = Resources.Load<Texture2D>(unit.bodyImgSrc);
            slotPanel.agentBody.sprite = Sprite.Create(tex3, new Rect(0, 0, tex3.width, tex3.height), new Vector3(0.5f, 0.5f, 0.5f));
            Texture2D tex1 = Resources.Load<Texture2D>(unit.faceImgSrc);
            slotPanel.agentFace.sprite = Sprite.Create(tex1, new Rect(0, 0, tex1.width, tex1.height), new Vector3(0.5f, 0.5f, -1f));
            Texture2D tex2 = Resources.Load<Texture2D>(unit.hairImgSrc);
            slotPanel.agentHair.sprite = Sprite.Create(tex2, new Rect(0, 0, tex2.width, tex2.height), new Vector3(0.5f, 0.5f, -1f));

            posy -= 100f;
        }

        // scroll rect size
        Vector2 scrollRectSize = agentScrollTarget.GetComponent<RectTransform>().sizeDelta;
        scrollRectSize.y = -posy; //+ 100f;
        agentScrollTarget.GetComponent<RectTransform>().sizeDelta = scrollRectSize;
    }

    public void AgentStatusOpen(AgentModel unit)
    {
        AgentUnit dummy = AgentLayer.currentLayer.GetAgent(unit.instanceId);
        dummy.OpenStatusWindow();
    }


    public void AgentListAnimButton()
    {
        if (slideAnim.GetBool("Slide"))
        {
            slideAnim.SetBool("Slide", false);
        }

        else
        {
            slideAnim.SetBool("Slide", true);
            ShowAgentList();
        }

    }

    public string MentalCheck(AgentModel unit)
    {
        if (unit.mental >= unit.maxMental * 2 / 3f)
        {
            return "멘탈 : 건강";
        }

        else if (unit.mental <= unit.maxMental * 2 / 3f && unit.mental >= unit.maxMental * 1 / 3f)
        {
            return "멘탈 : 보통";
        }

        else if (unit.mental >= unit.maxMental * 1 / 3f)
        {
            return "멘탈 : 심각";
        }

        else
        {
            return "멘탈 : ???";
        }

    }

    public string HealthCheck(AgentModel unit)
    {

        if (unit.hp >= unit.hp * 2 / 3f)
        {
            return "신체 : 건강";
        }

        else if (unit.hp <= unit.maxHp * 2 / 3f && unit.hp >= unit.maxHp * 1 / 3f)
        {
            return "신체 : 보통";
        }

        else if (unit.hp >= unit.maxHp * 1 / 3f)
        {
            return "신체 : 심각";
        }

        else
        {
            return "신체 : ???";
        }
    }

    public string SefiraCheck(AgentModel unit)
    {
        if (unit.currentSefira == "1")
        {
            return "배치 - 지휘감시팀";
        }

        else if (unit.currentSefira == "2")
        {
             return "배치 - 비상계획팀";
        }

        else if (unit.currentSefira == "3")
        {
             return "배치 - 자재관리팀";
        }

        else if (unit.currentSefira == "4")
        {
            return "배치 - 솔루션개발팀";
        }

        else
        {
            return "배치 오류";
        }



    }
}
