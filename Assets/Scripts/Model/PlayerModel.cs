﻿using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class PlayerModel {

    private HashSet<string> areaList;
    public HashSet<string> openedAreaList;

    public Vector3 playerSpot;
      
    private int day;

    // 지금까지 등장한 환상체 인덱스 기록하는 배열
    public List<int> inGameCreatureList= new List<int>();


    private static PlayerModel _instance;
    public static PlayerModel instance
    {
        get
        {
            if (_instance == null)
                _instance = new PlayerModel();
            return _instance;
        }
    }

    private PlayerModel()
    {
        areaList = new HashSet<string>();
        areaList.Add("1");
        areaList.Add("2");
        areaList.Add("3");
        areaList.Add("4");
        Init();
    }

    public void Init()
    {
        openedAreaList = new HashSet<string>();

        day = 0;
    }

    public void OpenArea(string area)
    {
        if (areaList.Contains(area) && !openedAreaList.Contains(area))
        {
            openedAreaList.Add(area);
            UpdateArea(area);
        }
    }

    private void UpdateAreaActive()
    {
        MapGraph.instance.InitActivates();
        foreach (string area in openedAreaList)
        {
            MapGraph.instance.ActivateArea(area);
        }
    }

    public bool IsOpenedArea(string area)
    {
        return openedAreaList.Contains(area);
    }

    public void SetDay(int day)
    {
        this.day = day;
        Notice.instance.Send(NoticeName.UpdateDay);
    }

    public int GetDay()
    {
        return day;
    }

    public string[] GetOpenedAreaList()
    {
        return openedAreaList.ToArray();
    }

    private void UpdateArea(string added)
    {
        MapGraph.instance.ActivateArea(added);
        foreach (CreatureModel unit in CreatureManager.instance.GetCreatureList())
        {
            if (unit.GetArea() == added)
            {
                //unit.gameObject.SetActive(true);
                //unit.room.gameObject.SetActive(true);
            }
        }

        if (added == "1")
        {
            CreatureManager.instance.AddCreature(30003, "S-left-way2", -9.6f, -1, added);
            CreatureManager.instance.AddCreature(RandomCreature(), "S-left-way3", -17.2f, -1, added);
            CreatureManager.instance.AddCreature(RandomCreature(), "S-right-way2", 9.6f, -1, added);
            CreatureManager.instance.AddCreature(RandomCreature(), "S-right-way3", 17.2f, -1, added);
            CreatureManager.instance.AddCreature(RandomCreature(), "S-bottom-way2-left2", -14, -8.5f, added);
            CreatureManager.instance.AddCreature(RandomCreature(), "S-bottom-way2-right2", 14, -8.5f, added);
        }
        else if (added == "2")
        {
            // Na??
            CreatureManager.instance.AddCreature(RandomCreature(), "N-top-up-way3", -41, -1.5f, added); // 마법소녀
            CreatureManager.instance.AddCreature(RandomCreature(), "N-top-down-way3", -41, -13.5f, added); // 보고 싶은 사신
            CreatureManager.instance.AddCreature(RandomCreature(), "N-bottom-way2-left2", -41, -31.5f, added); // 없는 책
            CreatureManager.instance.AddCreature(RandomCreature(), "N-bottom-way2-right2", -13, -31.5f, added); // 삐에로
        }
        else if (added == "3")
        {
            CreatureManager.instance.AddCreature(RandomCreature(), "H-top-up-way3", 41, -1.5f, added); // 남자 초상화
            CreatureManager.instance.AddCreature(RandomCreature(), "H-top-down-way3", 41, -13.5f, added); // 벽 여인
            CreatureManager.instance.AddCreature(RandomCreature(), "H-bottom-way2-left2", 41, -31.5f, added); // 잭이 없는 콩나무
            CreatureManager.instance.AddCreature(RandomCreature(), "H-bottom-way2-right2", 13, -31.5f, added); // (아무 것도 없는)
        }
        else if (added == "4")
        {
            CreatureManager.instance.AddCreature(RandomCreature(), "T-left-way2", -9.6f, -26, added); // 테레지아
            CreatureManager.instance.AddCreature(RandomCreature(), "T-left-way3", -17.2f, -26, added); // 아무말 없는 수녀

            CreatureManager.instance.AddCreature(RandomCreature(), "T-right-way2", 9.6f, -26, added); // 마법소녀
            CreatureManager.instance.AddCreature(RandomCreature(), "T-right-way3", 17.2f, -26, added); // 마법소녀
        }

        //Notice.instance.Send(NoticeName.AreaOpenUpdate, added);
    }

    public Dictionary<string, object> GetSaveData()
    {
        Dictionary<string, object> output = new Dictionary<string,object>();

        output.Add("areaList", openedAreaList.ToList());
        output.Add("day", day);

        return output;
    }

    public void LoadData(Dictionary<string, object> dic)
    {
        List<string> openedAreaListImp = new List<string>();
        GameUtil.TryGetValue(dic, "areaList", ref openedAreaListImp);
        foreach (string area in openedAreaListImp)
        {
            openedAreaList.Add(area);
        }
        GameUtil.TryGetValue(dic, "day", ref day);

        UpdateAreaActive();
    }

    // 환상체 랜덤 인덱스 함수
    public long RandomCreature()
    {
        int randomIndex = Random.Range(0, CreatureTypeList.instance.GetList().Length);

        if (inGameCreatureList.Count == 0)
        {
             inGameCreatureList.Add(randomIndex);

             return CreatureTypeList.instance.GetList()[randomIndex].id;
        }

        else
        {
            for (int i = 0; i < inGameCreatureList.Count; i++)
            {
                if (randomIndex == inGameCreatureList[i])
                {
                    randomIndex = Random.Range(0, CreatureTypeList.instance.GetList().Length);
                    i = 0;
                }
            }

            inGameCreatureList.Add(randomIndex);

            return CreatureTypeList.instance.GetList()[randomIndex].id;
        }
    }
}
