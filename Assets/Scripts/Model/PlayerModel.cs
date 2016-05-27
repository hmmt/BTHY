using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public enum EmergencyLevel { 
    NORMAL,
    LEVEL1,
    LEVEL2,
    LEVEL3,
    CHAOS//임시 값
}

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

    public EmergencyLevel currentEmergencyLevel = EmergencyLevel.NORMAL;

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
        Sefira s = SefiraManager.instance.GetSefira(area);
        if (areaList.Contains(area) && !openedAreaList.Contains(area))
        {
            openedAreaList.Add(area);
            UpdateArea(area);
            //SefiraManager.instance.getSefira(area).activated = true;
            //SefiraManager.instance.getSefira(area).initCreatureArray();
            s.activated = true;
            s.initCreatureArray();
            s.initOfficerGroup();
            
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
            CreatureManager.instance.AddCreature(100004, "left-upper-way2", -14f, -3.5f, added); //테10002 -> 늙은(100021) -> 마법소녀(100004)
            
            CreatureManager.instance.AddCreature(100008, "left-upper-way3", -24f, -3.5f, added); //큰새(100008) -> 구두(100003)
            CreatureManager.instance.AddCreature(100005, "right-upper-way2", 14f, -3.5f, added); //성냥팔이(100001) -> 아무 것도 없는 (100005) -> 노래하는 기계(100006)
            CreatureManager.instance.AddCreature(100022, "right-upper-way3", 24f, -3.5f, added); // 단한가지악(100009) -> 벽을 보는 여인(100022)
			CreatureManager.instance.AddCreature(100001, "left-down-way2", -8, -10f, added);
			CreatureManager.instance.AddCreature(100022, "right-down-way2", 8, -10f, added);
            
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
			CreatureManager.instance.AddCreature(100004, "T-left-way2", -10f, -50, added);
			CreatureManager.instance.AddCreature(100008, "T-left-way3", -20f, -50, added);

			CreatureManager.instance.AddCreature(100006, "T-right-way2", 10f, -50, added);
			CreatureManager.instance.AddCreature(100002, "T-right-way3", 20f, -50, added);
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

    public void AddCurrentEmergencyLevel() {
        int level = (int)this.currentEmergencyLevel;
        if (level > (int)EmergencyLevel.CHAOS)
        {
            return;
        }
        this.currentEmergencyLevel = GetEmergencyLevelByInt(level + 1);
    }

    public void SubCurrentEmergencyLevel() {
        int level = (int)this.currentEmergencyLevel;
        if (level < (int)EmergencyLevel.NORMAL){
            return;
        }
        this.currentEmergencyLevel = GetEmergencyLevelByInt(level - 1);
    }

    public EmergencyLevel GetCurrentEmergencyLevel() {
        return this.currentEmergencyLevel;
    }

    public void SetCurrentEmergencyLevel(EmergencyLevel level) {
        this.currentEmergencyLevel = level;
    }

    public void SetCurrentEmergencyLevel(int level)
    {
        if (level < (int)EmergencyLevel.NORMAL || level > (int)EmergencyLevel.CHAOS)
        {
            return;
        }
        this.currentEmergencyLevel = GetEmergencyLevelByInt(level);
    }

    public EmergencyLevel GetEmergencyLevelByInt(int level) {
        switch (level) { 
            case 0: return EmergencyLevel.NORMAL;
            case 1: return EmergencyLevel.LEVEL1;
            case 2: return EmergencyLevel.LEVEL2;
            case 3: return EmergencyLevel.LEVEL3;
            case 4: return EmergencyLevel.CHAOS;
            default: return EmergencyLevel.NORMAL;
        }
    }
}
