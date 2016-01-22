using UnityEngine;
using System.Collections.Generic;

public class MapSefiraArea
{
    public string sefiraName;
    //private List<PassageObjectModel> passageObjects;
    private List<MapNode> nodeList;

    public MapSefiraArea()
    {
        nodeList = new List<MapNode>();
    }
    public MapSefiraArea(string name)
    {
        nodeList = new List<MapNode>();
        this.sefiraName = name;
    }

    public void AddNode(MapNode node)
    {
        nodeList.Add(node);
    }
    public MapNode[] GetNodeList()
    {
        return nodeList.ToArray();
    }

    public void InitActivates()
    {
        foreach (MapNode node in nodeList)
        {
            node.activate = false;
        }
        Notice.instance.Send(NoticeName.AreaUpdate, sefiraName, false);
    }
    public void ActivateArea()
    {
        foreach (MapNode node in nodeList)
        {
            node.activate = true;
        }
        Notice.instance.Send(NoticeName.AreaOpenUpdate, sefiraName);
    }

    public void DeactivateArea()
    {
        foreach (MapNode node in nodeList)
        {
            node.activate = false;
        }
        Notice.instance.Send(NoticeName.AreaUpdate, sefiraName, false);
    }

    public void SetHorror()
    {
    }

    public int GetHorror()
    {
        return 0;
    }
}
