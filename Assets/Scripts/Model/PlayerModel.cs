using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class PlayerModel {

    private HashSet<string> areaList;
    public HashSet<string> openedAreaList;
    public List<long> openedAgentList;

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
        openedAgentList = new List<long>();

        // default
        openedAgentList.Add(1);
        openedAgentList.Add(2);
        openedAgentList.Add(3);
        openedAgentList.Add(4);

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
    public long[] GetAvailableAgentList()
    {
        return openedAgentList.ToArray();
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
            CreatureManager.instance.AddCreature(RandomCreature(), "1002001", -8, -1, added);
            CreatureManager.instance.AddCreature(RandomCreature(), "1003002", -16, -1, added);
            CreatureManager.instance.AddCreature(RandomCreature(), "1004101", 8, -1, added);
            CreatureManager.instance.AddCreature(RandomCreature(), "1004102", 17, -1, added);
            CreatureManager.instance.AddCreature(RandomCreature(), "1003111-left-1", -10, -9, added);
            CreatureManager.instance.AddCreature(RandomCreature(), "1003111-right-1", 10, -9, added);
        }
        else if (added == "2")
        {
            // Na??
            CreatureManager.instance.AddCreature(RandomCreature(), "N-way1-point2", -25, -4, added); // 마법소녀
            CreatureManager.instance.AddCreature(RandomCreature(), "N-way1-point3", -25, -14, added); // 보고 싶은 사신
            CreatureManager.instance.AddCreature(RandomCreature(), "N-way2-point1", -25, -26, added); // 없는 책
            CreatureManager.instance.AddCreature(RandomCreature(), "N-way2-point2", -25, -36, added); // 삐에로
        }
        else if (added == "3")
        {
            CreatureManager.instance.AddCreature(RandomCreature(), "H-way1-point2", 25, -4, added); // 남자 초상화
            CreatureManager.instance.AddCreature(RandomCreature(), "H-way1-point3", 25, -14, added); // 벽 여인
            CreatureManager.instance.AddCreature(RandomCreature(), "H-way2-point1", 25, -26, added); // 잭이 없는 콩나무
            CreatureManager.instance.AddCreature(RandomCreature(), "H-way2-point2", 25, -36, added); // (아무 것도 없는)
        }
        else if (added == "4")
        {
            CreatureManager.instance.AddCreature(RandomCreature(), "tessod-left-point", -10, -26, added); // 테레지아
            CreatureManager.instance.AddCreature(RandomCreature(), "tessod-right-point", 10, -26, added); // 아무말 없는 수녀


            CreatureManager.instance.AddCreature(RandomCreature(), "tessod-down-point", -6, -35, added); // 마법소녀
            CreatureManager.instance.AddCreature(RandomCreature(), "tessod-down-point", 6, -35, added); // 마법소녀
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
