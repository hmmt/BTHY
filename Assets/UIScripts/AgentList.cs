using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public interface ICollectionWindow {
    void Open();
    void Close();
}

public class AgentList : MonoBehaviour, ICollectionWindow {

    public bool isOpened = false;

    public Transform agentScrollTarget;
    public Transform anchor;
    public Animator slideAnim;
    public GameObject extend;

    private int state1 = 0;

    List<AgentModel> selectedAgentList = new List<AgentModel>();

    public static AgentList currentWindow = null;
    public int extended = -1;
    private int oldExt;
    public CollectionListScript otherWindow;
    private Sprite[] sefiraImage = new Sprite[10];
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

    public void Start() {
        sefiraImage[0] = ResourceCache.instance.GetSprite("Sprites/UI/StageUI/Malkuth_Icon");
        sefiraImage[1] = ResourceCache.instance.GetSprite("Sprites/UI/StageUI/Netzzach_Icon");
        sefiraImage[2] = ResourceCache.instance.GetSprite("Sprites/UI/StageUI/Hod_Icon");
        sefiraImage[3] = ResourceCache.instance.GetSprite("Sprites/UI/StageUI/Yessod_Icon");
        extend.gameObject.SetActive(false);
    }

    public void ShowAgentListD() {
        AgentModel[] agents = AgentManager.instance.GetAgentList();

        foreach (Transform child in agentScrollTarget.transform) {
            if (child.tag == "AgentButtonPanel") continue;
            Destroy(child.gameObject);
        }

        GameObject[] list = new GameObject[agents.Length];
        int i = 0;

        foreach (AgentModel unit in agents) {
            GameObject slot = Prefab.LoadPrefab("Slot/AgentListPanelObject");
            Sprite sefiraSprite;
            slot.SetActive(true);
            slot.transform.SetParent(agentScrollTarget, false);
            list[i] = slot;

            AgentListPanelScript slotPanel = slot.GetComponent<AgentListPanelScript>();

            if (i % 2 == 0)
            {
                slotPanel.panelImage.sprite = ResourceCache.instance.GetSprite("UIResource/Collection/Semi");
            }
            else {
                slotPanel.panelImage.sprite = ResourceCache.instance.GetSprite("UIResource/Collection/Dark");
            }
            slotPanel.Name.text = unit.name;
            slotPanel.Level.text = unit.level + "등급";

            slotPanel.Mental.text = unit.mental + "/" + unit.maxMental;
            slotPanel.setIndex(i);
            /*
            if (i == extended)
            {
                AgentModel oldUnit = (AgentStatusWindow.currentWindow != null) ? AgentStatusWindow.currentWindow.target : null;
                AgentStatusWindow.CreateWindow(unit);
                if (CollectionWindow.currentWindow != null)
                    CollectionWindow.currentWindow.CloseWindow();
            }
            else {
            }*/
            AgentModel copied = unit;
           // slotPanel.agentInfoButton.onClick.AddListener(() => AgentStatusOpen(copied));
            slotPanel.body.sprite = ResourceCache.instance.GetSprite(unit.bodyImgSrc);
            slotPanel.face.sprite = ResourceCache.instance.GetSprite(unit.faceImgSrc);
            slotPanel.hair.sprite = ResourceCache.instance.GetSprite(unit.hairImgSrc);
            slotPanel.skill[0].sprite = copied.WorklistSprites[0];
            slotPanel.skill[1].sprite = copied.WorklistSprites[1];
            slotPanel.skill[2].sprite = copied.WorklistSprites[2];

            switch (copied.currentSefira) { 
                case "1":
                    sefiraSprite = sefiraImage[0];
                    break;
                case "2":
                    sefiraSprite = sefiraImage[1];
                    break;
                case "3":
                    sefiraSprite = sefiraImage[2];
                    break;
                case "4":
                    sefiraSprite = sefiraImage[3];
                    break;
                default:
                    sefiraSprite = sefiraImage[0];
                    break;                    
            }
            slotPanel.Sefira.sprite = sefiraSprite;
            slotPanel.model = copied;
            if (i == extended) {
                extend.GetComponent<SefiraButtonScript>().SetScript(slotPanel);
            }
            i++;
            
        }
        if (extended == oldExt) {
            extended = -1;
        }

        if (extended == -1)
        {
            extend.gameObject.SetActive(false);
        }
        else {
            extend.gameObject.SetActive(true);
            extend.GetComponent<SefiraButtonScript>().InitButton();
           
        }

        float posy = 0.0f;
        foreach (GameObject child in list) {
            float size;
            RectTransform rt = child.GetComponent<RectTransform>();

            size = rt.rect.height;

            rt.localPosition = new Vector3(0.0f, posy, 0);
            posy -= size;

            if (rt.GetComponent<AgentListPanelScript>().index == extended) {
                RectTransform extendRect = extend.GetComponent<RectTransform>();
                float tempsize = extendRect.rect.height;
                extendRect.localPosition = new Vector3(0.0f, posy, 0.0f);
                
                posy -= tempsize;
            }
        }

        Vector2 scrollRectSize = agentScrollTarget.GetComponent<RectTransform>().sizeDelta;
        scrollRectSize.y = -posy;
        agentScrollTarget.GetComponent<RectTransform>().sizeDelta = scrollRectSize;

        oldExt = extended;
    }
    /*
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

            GameObject slot = Prefab.LoadPrefab("Slot/AgentListPanelObject");

            slot.transform.SetParent(agentScrollTarget, false);

            RectTransform tr = slot.GetComponent<RectTransform>();
            tr.localPosition = new Vector3(0, posy, 0);
            AgentSlotPanelList slotPanel = slot.GetComponent<AgentSlotPanelList>();

            slotPanel.panelImage.sprite = ResourceCache.instance.GetSprite("Sprites/UI/SystemUI/AgentListPanel");
            slotPanel.agentName.text = unit.name;
            slotPanel.agentHP.text = HealthCheck(unit);
            slotPanel.agentMental.text = MentalCheck(unit);
            slotPanel.agentLevel.text = "직원 등급" + unit.level;
            slotPanel.agentSefria.text = SefiraCheck(unit);

            AgentModel copied = unit;
            slotPanel.agentInfoButton.onClick.AddListener(() => AgentStatusOpen(copied));


            slotPanel.agentBody.sprite = ResourceCache.instance.GetSprite(unit.bodyImgSrc);
            slotPanel.agentFace.sprite = ResourceCache.instance.GetSprite(unit.faceImgSrc);
            slotPanel.agentHair.sprite = ResourceCache.instance.GetSprite(unit.hairImgSrc);

            posy -= 100f;
        }

        // scroll rect size
        Vector2 scrollRectSize = agentScrollTarget.GetComponent<RectTransform>().sizeDelta;
        scrollRectSize.y = -posy; //+ 100f;
        agentScrollTarget.GetComponent<RectTransform>().sizeDelta = scrollRectSize;
    }
    */
    public void AgentStatusOpen(AgentModel unit)
    {
        AgentUnit dummy = AgentLayer.currentLayer.GetAgent(unit.instanceId);
        dummy.OpenStatusWindow();
    }
    

    public void AgentListAnimButton()
    {
        if (slideAnim.GetBool("Slide"))
        {
            isOpened = false;
            slideAnim.SetBool("Slide", false);
        }
        else
        {
            isOpened = true;
            slideAnim.SetBool("Slide", true);
            if (otherWindow.isOpened) {
                otherWindow.isOpened = false;
                otherWindow.slideAnim.SetBool("Slide", false);
            }
            ShowAgentListD();
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
    
    public void Open()
    {
        isOpened = true;
        ShowAgentListD();
    }

    public void Close()
    {
        isOpened = false;
    }
}
