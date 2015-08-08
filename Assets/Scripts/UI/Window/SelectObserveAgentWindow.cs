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
        ObserveCreature.Create(agent,targetCreature);
        //UseSkill.InitUseSkillAction(skillInfo, agent, targetCreature);
        CloseWindow();
    }

    public void ShowAgentList()
    {
        AgentModel[] agents = AgentManager.instance.GetAgentList();

        float posy = 0;
        foreach (AgentModel unit in agents)
        {
            if (unit.GetState() == AgentCmdState.WORKING)
                continue;

            GameObject slot = Prefab.LoadPrefab("AgentSlotPanelObserve");

            slot.transform.SetParent(agentScrollTarget, false);

            RectTransform tr = slot.GetComponent<RectTransform>();
            tr.localPosition = new Vector3(0, posy, 0);

            AgentSlotPanelObserve slotPanel = slot.GetComponent<AgentSlotPanelObserve>();
            slotPanel.skillButton1.image.sprite = Resources.Load<Sprite>("Sprites/" + unit.directSkill.imgsrc);


            AgentModel copied = unit;
            slotPanel.skillButton1.onClick.AddListener(() => SelectAgentSkill(copied));


            Texture2D tex3 = Resources.Load<Texture2D>(unit.bodyImgSrc);
            slotPanel.agentBody.sprite = Sprite.Create(tex3, new Rect(0, 0, tex3.width, tex3.height), new Vector3(0.5f, 0.5f, 0.5f));
            Texture2D tex1 = Resources.Load<Texture2D>(unit.faceImgSrc);
            slotPanel.agentFace.sprite = Sprite.Create(tex1, new Rect(0, 0, tex1.width, tex1.height), new Vector3(0.5f, 0.5f, -1f));
            Texture2D tex2 = Resources.Load<Texture2D>(unit.hairImgSrc);
            slotPanel.agentHair.sprite = Sprite.Create(tex2, new Rect(0, 0, tex2.width, tex2.height), new Vector3(0.5f, 0.5f, -1f));

            Debug.Log(tr.localPosition);

            posy -= 50f;
        }
    }
    public void CloseWindow()
    {
        //gameObject.SetActive (false);
        currentWindow = null;
        Destroy(gameObject);
    }
}
