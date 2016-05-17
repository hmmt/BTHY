using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum SpaceObjectType { 
    LONGHALLWAY,
    HALLWAY,
    ELEVATORSHORT,
    ELEVATORLONG,
    HUBSHORT,
    HUBLONG,
    ISOLATE,
    NONE//sefira core room
}

public class SefiraController : MonoBehaviour
{
    [System.Serializable]
    public class SefiraSprite {
        public int SefiraIndex;
        public Sprite LongHallwayFrame;
        public Sprite HallwayFrame;
        public Sprite ElevatorLongFrame;
        public Sprite ElevatorShortFrame;
        public Sprite HubShortFrame;
        public Sprite HubLongFrame;
        public Sprite IsolateRoomFrame;

        public Sprite Energy;

        public Sprite GetSprite(SpaceObjectType type) {
            switch (type)
            {
                case SpaceObjectType.LONGHALLWAY:
                    return this.LongHallwayFrame;
                case SpaceObjectType.HALLWAY:
                    return this.HallwayFrame;
                case SpaceObjectType.ELEVATORLONG:
                    return this.ElevatorLongFrame;
                case SpaceObjectType.ELEVATORSHORT:
                    return this.ElevatorShortFrame;
                case SpaceObjectType.HUBSHORT:
                    return this.HubShortFrame;
                case SpaceObjectType.HUBLONG:
                    return this.HubLongFrame;
                case SpaceObjectType.ISOLATE:
                    return this.IsolateRoomFrame;
                default :
                    return null;
            }
        }
        //may be add agent sprite
    }

    //접근점을 만들어서 스프라이트를 읽을 수 있게끔 만들겠음
    //SefiraObject들을 배치할 것
    //일단 10초에 한 번 확인하게 만듦
    float elapsed = 0;
    float wait = 10f;

    private static SefiraController _instance = null;
    public static SefiraController instance {
        get {
            return _instance;
        }
    }

    public List<SefiraSprite> sefiraSprite;

    public void Awake() {
        _instance = this;
    }

    public void Start() { 
        
    }

    public void Update() {
        elapsed += Time.deltaTime;
        if (elapsed > wait) {
            elapsed = 0;
            CheckSefira();
        }
    }

    public SefiraSprite GetSefiraSprite(Sefira target) {
        SefiraSprite output = null;
        foreach (SefiraSprite ss in this.sefiraSprite) {
            if (ss.SefiraIndex == target.index) {
                output = ss;
                break;
            }
        }
        return output;
    }
    public SefiraSprite GetSefiraSprite(string indexString)
    {
        SefiraSprite output = null;
        foreach (SefiraSprite ss in this.sefiraSprite)
        {
            if (ss.SefiraIndex == (int)float.Parse(indexString))
            {
                output = ss;
                break;
            }
        }
        return output;
    }
    public SefiraSprite GetSefiraSprite(int index)
    {
        SefiraSprite output = null;
        foreach (SefiraSprite ss in this.sefiraSprite)
        {
            if (ss.SefiraIndex == index)
            {
                output = ss;
                break;
            }
        }
        return output;
    }

    //Sefira 직원 배치 등의 상황을 통해 현재 개방 여부를 확인
    public void CheckSefira() {
        foreach (Sefira sefira in SefiraManager.instance.sefiraList) {
            if (!sefira.activated) continue;
            if (sefira.officerList.Count < 5) {
                Debug.Log(sefira.name + " 비활성화");
            }
        }
    }


}
