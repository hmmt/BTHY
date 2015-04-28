using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AgentSelection : MonoBehaviour {

    public Transform agentScrollTarget;

    private int state1 = 0;

    private CreatureUnit targetCreature = null;
    private IsolateRoom targetRoom = null;

    List<GameObject> selectedAgentList = new List<GameObject>();

    public static AgentSelection currentWindow = null;

    public static AgentSelection CreateWindow(CreatureUnit unit)
    {
        if (currentWindow != null)
        {
            currentWindow.CloseWindow();
        }

        GameObject newObj = Instantiate(Resources.Load<GameObject>("Prefabs/SelectWorkAgentWindow")) as GameObject;

        AgentSelection inst = newObj.GetComponent<AgentSelection>();
        //inst.ShowSelectAgent (unit.gameObject);
        inst.targetCreature = unit;
        inst.ShowAgentList();

        currentWindow = inst;

        return inst;
    }


    public void OnClickClose()
    {
        CloseWindow();
    }

    public void SelectAgentSkill(AgentUnit agent, SkillTypeInfo skillInfo)
    {
        //UseSkill.InitUseSkillAction(skillInfo, selectedAgentList[0].GetComponent<AgentUnit>(), targetCreature);
        UseSkill.InitUseSkillAction(skillInfo, agent, targetCreature);
        CloseWindow();
    }

    public void ShowAgentList()
    {
        AgentUnit[] agents = AgentFacade.instance.GetAgentList();

        float posy = 0;
        foreach (AgentUnit unit in agents)
        {
            GameObject slot = Prefab.LoadPrefab("AgentSlotPanel");

            slot.transform.SetParent(agentScrollTarget, false);

            RectTransform tr = slot.GetComponent<RectTransform>();
            tr.localPosition = new Vector3(0, posy, 0);
            AgentSlotPanel slotPanel = slot.GetComponent<AgentSlotPanel>();

            AgentUnit copied = unit;
            slotPanel.skillButton1.onClick.AddListener(
                delegate() {
                    ObserveCreature.Create(copied, targetCreature);
                });

            Texture2D tex = Resources.Load<Texture2D>("Sprites/" + unit.imgsrc);
            slotPanel.agentIcon.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));

            posy -= 100f;
        }
    }

    public void CloseWindow()
    {
        //gameObject.SetActive (false);
        currentWindow = null;
        Destroy(gameObject);
    }
}
