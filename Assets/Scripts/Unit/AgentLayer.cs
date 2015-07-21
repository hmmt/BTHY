using UnityEngine;
using System.Collections.Generic;

public class AgentLayer : MonoBehaviour, IObserver {

    public static AgentLayer currentLayer { private set; get; }

    private List<AgentUnit> agentList;

    void Awake()
    {
        currentLayer = this;
        agentList = new List<AgentUnit>();
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
        newUnit.transform.SetParent(transform);
        AgentUnit unit = newUnit.GetComponent<AgentUnit>();

        unit.GetComponent<SpriteRenderer>().sprite = null;
        unit.model = model;

        unit.SetMaxHP(model.maxHp);

        agentList.Add(unit);
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
