using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SelectObserveAgentWindow : MonoBehaviour
{

    public Transform agentScrollTarget;

    private int state1 = 0;

    private CreatureModel targetCreature = null;

    List<GameObject> selectedAgentList = new List<GameObject>();

    public static SelectObserveAgentWindow currentWindow = null;

    public static SelectObserveAgentWindow CreateWindow(CreatureModel unit)
    {
        if (currentWindow != null)
        {
            currentWindow.CloseWindow();
        }

        GameObject newObj = Prefab.LoadPrefab("SelectObserveAgentWindow");

        SelectObserveAgentWindow inst = newObj.GetComponent<SelectObserveAgentWindow>();

        inst.targetCreature = unit;
        inst.ShowAgentList();

        currentWindow = inst;

        return inst;
    }

    public void OnClickAgentOK()
    {

    }
    public void OnClickClose()
    {
        CloseWindow();
    }

    public void SelectAgentSkill(AgentModel agent)
    {
        //ObserveCreature.Create(agent,targetCreature);
		agent.ObserveCreature(targetCreature);
        //UseSkill.InitUseSkillAction(skillInfo, agent, targetCreature);
        CloseWindow();
    }

    public void ShowAgentList()
    {
        AgentModel[] agents = AgentManager.instance.GetAgentList();

        float posy = 0;
        int cnt = 0;
        foreach (AgentModel unit in agents)
        {
            if (unit.GetState() == AgentCmdState.WORKING)
                continue;

            GameObject slot = Prefab.LoadPrefab("AgentSlotPanelObserve");
           
            slot.transform.SetParent(agentScrollTarget, false);

            RectTransform tr = slot.GetComponent<RectTransform>();
            tr.localPosition = new Vector3(0, tr.rect.y /2 + posy, 0);

            AgentSlotPanelObserve slotPanel = slot.GetComponent<AgentSlotPanelObserve>();
            //slotPanel.skillButton1.image.sprite = Resources.Load<Sprite>("Sprites/" + unit.directSkill.imgsrc);

            if (cnt % 2 == 0)
            {
                slotPanel.bg.sprite = ResourceCache.instance.GetSprite("UIResource/Collection/Semi");
            }
            else {
                slotPanel.bg.sprite = ResourceCache.instance.GetSprite("UIResource/Collection/Dark");
            }

            AgentModel copied = unit;
            slotPanel.skillButton1.onClick.AddListener(() => SelectAgentSkill(copied));


            slotPanel.agentBody.sprite = ResourceCache.instance.GetSprite(unit.bodyImgSrc);
            slotPanel.agentFace.sprite = ResourceCache.instance.GetSprite(unit.faceImgSrc);
            slotPanel.agentHair.sprite = ResourceCache.instance.GetSprite(unit.hairImgSrc);

            //Debug.Log(tr.localPosition);

            posy -= tr.rect.height;
            cnt++;
        }

        RectTransform scrollTarget = agentScrollTarget.GetComponent<RectTransform>();
        Vector2 delta = scrollTarget.sizeDelta;
        delta = new Vector2(delta.x, -posy);
        scrollTarget.sizeDelta = delta;

    }
    public void CloseWindow()
    {
        //gameObject.SetActive (false);
        currentWindow = null;
        Destroy(gameObject);
    }
}
