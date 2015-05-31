using UnityEngine;
using System.Collections.Generic;

public class PlayerModel {

    private HashSet<string> areaList;
    public HashSet<string> openedAreaList { private set; get; }
    public List<long> openedAgentList;

    public Vector3 playerSpot;
      
    private int day;

    private static PlayerModel _instance;
    public static PlayerModel instnace
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
            CreatureManager.instance.AddCreature(10001, "1002001", -8, -1);
            CreatureManager.instance.AddCreature(10002, "1003002", -16, -1);
            CreatureManager.instance.AddCreature(10003, "1004101", 8, -1);
            CreatureManager.instance.AddCreature(10004, "1004102", 17, -1);
            CreatureManager.instance.AddCreature(10005, "1003111-left-1", -10, -9);
            CreatureManager.instance.AddCreature(10006, "1003111-right-1", 10, -9);
        }
        else if (added == "2")
        {
            // Na??
            CreatureManager.instance.AddCreature(20005, "N-way1-point2", -25, -4); // 마법소녀
            CreatureManager.instance.AddCreature(20002, "N-way1-point3", -25, -14); // 보고 싶은 사신
            CreatureManager.instance.AddCreature(20006, "N-way2-point1", -25, -26); // 없는 책
            CreatureManager.instance.AddCreature(20004, "N-way2-point2", -25, -36); // 삐에로
        }
        else if (added == "3")
        {
            CreatureManager.instance.AddCreature(20001, "H-way1-point2", 25, -4); // 남자 초상화
            CreatureManager.instance.AddCreature(20003, "H-way1-point3", 25, -14); // 벽 여인
            CreatureManager.instance.AddCreature(30002, "H-way2-point1", 25, -26); // 잭이 없는 콩나무
            CreatureManager.instance.AddCreature(20003, "H-way2-point2", 25, -36); // (아무 것도 없는)
        }
        else if (added == "4")
        {
            CreatureManager.instance.AddCreature(30004, "tessod-left-point", -10, -26); // 테레지아
            CreatureManager.instance.AddCreature(30001, "tessod-right-point", 10, -26); // 아무말 없는 수녀


            //CreatureManager.instance.AddCreature(20005, "tessod-down-point", -6, -35); // 마법소녀
            //CreatureManager.instance.AddCreature(20005, "tessod-down-point", 6, -35); // 마법소녀
        }

        Notice.instance.Send(NoticeName.AreaOpenUpdate, added);
    }
}
