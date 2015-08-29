using UnityEngine;
using System.Collections.Generic;

public class AgentLayer : MonoBehaviour, IObserver {

    public static AgentLayer currentLayer { private set; get; }

    private List<AgentUnit> agentList;

    private int zCount;

    void Awake()
    {
        currentLayer = this;
        agentList = new List<AgentUnit>();
        zCount = 0;
    }

    void OnEnable()
    {
        Notice.instance.Observe(NoticeName.AddAgent, this);
        Notice.instance.Observe(NoticeName.RemoveAgent, this);
    }

    void OnDisable()
    {
        Notice.instance.Remove(NoticeName.AddAgent, this);
        Notice.instance.Remove(NoticeName.RemoveAgent, this);
    }

    public void Init()
    {
        ClearAgent();
        foreach (AgentModel model in AgentManager.instance.GetAgentList())
        {
            AddAgent(model);
        }
    }


    public void AddAgent(AgentModel model)
    {
        GameObject newUnit = Prefab.LoadPrefab("unit");
        newUnit.transform.SetParent(transform, false);
        AgentUnit unit = newUnit.GetComponent<AgentUnit>();

        unit.GetComponent<SpriteRenderer>().sprite = null;
        unit.model = model;

        unit.SetMaxHP(model.maxHp);

        agentList.Add(unit);

        // set Z value
        unit.zValue = -zCount;

        // 다른 유닛의 Z값 범위를 침범하지 않도록 z스케일을 낮춘다.
        Vector3 unitScale = unit.transform.localScale;
        unitScale.z = 0.001f;
        unit.transform.localScale = unitScale;

        zCount = (zCount + 1) % 1000;
    }

    public void RemoveAgent(AgentModel model)
    {
        AgentUnit unit = GetAgent(model.instanceId);

        agentList.Remove(unit);
        Destroy(unit.gameObject);
    }

    public void ClearAgent()
    {
        foreach (AgentUnit agentUnit in agentList)
        {
            Destroy(agentUnit.gameObject);
        }
        agentList.Clear();
    }

    public AgentUnit GetAgent(long id)
    {
        foreach (AgentUnit agent in agentList)
        {
            if (agent.model.instanceId == id)
            {
                return agent;
            }
        }
        return null;
    }

    public void OnNotice(string notice, params object[] param)
    {
        if (notice == NoticeName.AddAgent)
        {
            foreach (object obj in param)
            {
                AddAgent((AgentModel)obj);
            }
        }
        else if (notice == NoticeName.RemoveAgent)
        {
            foreach (object obj in param)
            {
                RemoveAgent((AgentModel)obj);
            }
        }
    }
}
