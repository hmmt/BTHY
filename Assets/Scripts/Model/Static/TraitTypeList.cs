using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TraitTypeList
{
    private static TraitTypeList _instance;
    public static TraitTypeList instance
    {
        get
        {
            if (_instance == null)
                _instance = new TraitTypeList();
            return _instance;
        }
    }

    private List<TraitTypeInfo> _list;
    private List<TraitTypeInfo> _NFList;
    private List<TraitTypeInfo> _EIList;
    private List<TraitTypeInfo>[] levelList;

    private bool _loaded = false;

    public bool loaded
    {
        get { return _loaded; }
    }

    private TraitTypeList()
    {
        _list = new List<TraitTypeInfo>();
    }

    public void Init(TraitTypeInfo[] list, TraitTypeInfo[] EI, TraitTypeInfo[] NF, List<TraitTypeInfo>[] level)
    {
        _list = new List<TraitTypeInfo>(list);
        _NFList = new List<TraitTypeInfo>(NF);
        _EIList = new List<TraitTypeInfo>(EI);
        levelList = new List<TraitTypeInfo>[5];
        for (int i = 0; i < 5; i++) {
            levelList[i] = new List<TraitTypeInfo>(level[i]);
        }
        _loaded = true;
    }

    public TraitTypeInfo[] GetList()
    {
        return _list.ToArray();
    }

    public TraitTypeInfo GetData(long id)
    {
        foreach (TraitTypeInfo info in _list)
        {
            if (info.id == id)
            {
                return info;
            }
        }
        return null;
    }


    public TraitTypeInfo GetRandomEiTrait(int level)
    {
        int i = 0;

        while (true)
        {
            i = Random.Range(0, _list.Count);

            if (_list[i].randomFlag == 1 && _list[i].level == level)
                if (_list[i].discType == 1 || _list[i].discType == 2)
                {
                    return _list[i];
                }
        }
    }

    public TraitTypeInfo GetRandomEITrait(List<TraitTypeInfo> agentList) {
        List<TraitTypeInfo> tempList = new List<TraitTypeInfo>(_EIList);

        foreach (TraitTypeInfo t in agentList) {

            if (tempList.Contains(t))
            {
                tempList.Remove(t);
            }
        }

        int randIndex = Random.Range(0, tempList.Count);
        return tempList[randIndex];
    }

    public TraitTypeInfo GetRandomNFTrait(List<TraitTypeInfo> agentList)
    {
        List<TraitTypeInfo> tempList = new List<TraitTypeInfo>(_NFList);

        foreach (TraitTypeInfo t in agentList)
        {
            if (tempList.Contains(t))
            {
                tempList.Remove(t);
            }
        }

        int randIndex = Random.Range(0, tempList.Count);
        return tempList[randIndex];
    }

    public TraitTypeInfo GetRandomNfTrait(int level)
    {
        int i = 0;

        while (true)
        {
            i = Random.Range(0, _list.Count);
            if (_list[i].randomFlag == 1 && _list[i].level == level)
                if (_list[i].discType == 3 || _list[i].discType == 4)
                {
                    return _list[i];
                }
        }
    }

    public TraitTypeInfo GetRandomInitTrait()
    {
        int i = 0;

        while (true)
        {
            i = Random.Range(0, _list.Count);
            if (_list[i].randomFlag == 1 && _list[i].discType == 0 && _list[i].level == 1)
            {
                return _list[i];
            }

        }
    }

    public TraitTypeInfo GetTraitWithLevel(int traitLevel)
    {
        int i = 0;
        while (true)
        {
            i = Random.Range(0, _list.Count);
            if (_list[i].randomFlag == 1 && _list[i].level == traitLevel)
            {
                return _list[i];
            }
        }
    }

    public TraitTypeInfo GetTraitWithLevel(AgentModel agent) {
        List<TraitTypeInfo> tempList = new List<TraitTypeInfo>(levelList[agent.level]);

        foreach (TraitTypeInfo t in agent.traitList) {
            if (tempList.Contains(t))
            {       
                tempList.Remove(t);

            }
        }

        int randIndex = Random.Range(0, tempList.Count);
        return tempList[randIndex];
    }

    public TraitTypeInfo GetTraitWithId(long id)
    {
        for (int i = 0; i < _list.Count; i++)
        {
            if (id == _list[i].id)
                return _list[i];
        }

        return null;
    }

    public TraitTypeInfo[] GetTrait(AgentModel model) {
        int modular = model.level % 2;
        List<TraitTypeInfo> output = new List<TraitTypeInfo>();

        if (modular == 1)
        {
            //홀수 ( 1->2 , 3->4)
            //output.Add(GetRandomNFTrait(model.traitList));
            int rand = Random.Range(0, 2);
            if (rand == 0) { 
                output.Add(GetRandomNFTrait(model.traitList));
            }
            else
            {
                output.Add(GetRandomEITrait(model.traitList));
            }
            output.Add(GetTraitWithLevel(model));
            output.Add(GetTraitWithLevel(model));
        }
        else { 
            //짝수 (0->1, 2->3, 4->5)
            output.Add(GetRandomEITrait(model.traitList));
            output.Add(GetRandomNFTrait(model.traitList));
            output.Add(GetTraitWithLevel(model));
        }

        return output.ToArray();
    }
}

