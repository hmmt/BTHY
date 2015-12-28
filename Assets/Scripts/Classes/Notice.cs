using UnityEngine;
using System.Collections.Generic;

public delegate void NoticeReciever(params object[] param);

public class NoticeName
{
    //public static string StartScene = "StartScene";

    public static string FixedUpdate = "FixedUpdate";
    public static string MoveUpdate = "MoveUpdate"; // after FixedUpdate

    public static string EnergyTimer = "EnergyTimer";
    public static string AddNarrationLog = "AddNarrationLog";

    public static string AreaOpenUpdate = "AreaOpenUpdate";
    public static string AreaUpdate = "AreaUpdate";

    public static string AddPlayerLog = "AddPlayerLog";
    public static string AddSystemLog = "AddSystemLog";
    public static string CreatureFeelingUpdateTimer = "CreatureFeelingUpdateTimer";
    public static string UpdateEnergy = "UpdateEnergy";
    public static string UpdateDay = "UpdateDay";

    public static string AddAgent = "AddAgent";
    public static string AddOfficer = "AddOfficer";
    public static string RemoveAgent = "RemoveAgent";
    public static string RemoveOfficer = "RemoveOfficer";
    public static string InitAgent = "InitAgent";
    public static string ChangeAgentSefira = "ChangeAgentSefira";
    public static string ChangeAgentSefira_Late = "ChangeAgentSefira_Late";
    public static string ChangeAgentState = "ChangeAgentState";

    public static string AddCreature = "AddCreature";
    public static string UpdateCreatureRes = "UpdateCreatureRes"; // 환상체의 이미지 변경. 지금은 안 씀.
    public static string RemoveCreature = "RemoveCreature";

    public static string LoadMapGraphComplete = "LoadMapGraphComplete";

    public static string EscapeCreature = "EscapeCreature";

    // PassageObject
    public static string AddPassageObject = "AddPassageObject";

    public static string MakeName(string noticeName, params string[] param)
    {
        string output = noticeName;

        foreach (string p in param)
        {
            output += "_" + p;
        }

        return output;
    }
}

public class Notice
{
    private class CallbackObserver : IObserver
    {
        public int noticeId;
        public NoticeReciever callback;
        public CallbackObserver(int noticeId, NoticeReciever callback)
        {
            this.noticeId = noticeId;
            this.callback = callback;
        }
        public void OnNotice(string notice, params object[] param)
        {
            callback(param);
        }
    }

    private static Notice _instance;

    public static Notice instance
    {
        get
        {
            if (_instance == null)
                _instance = new Notice();
            return _instance;
        }
    }

    private Dictionary<string, List<IObserver>> noticeList;

    private int lastNoticeId;

    private Notice()
    {
        lastNoticeId = 0;
        noticeList = new Dictionary<string, List<IObserver>>();
    }

    public void Observe(string notice, IObserver observer)
    {
        List<IObserver> obList;

        if (noticeList.TryGetValue(notice, out obList))
        {
            obList.Add(observer);
        }
        else
        {
            obList = new List<IObserver>();
            obList.Add(observer);
            noticeList.Add(notice, obList);
        }
    }

    public int Observe(string notice, NoticeReciever observer)
    {
        int noticeId = ++lastNoticeId;
        Observe(notice, new CallbackObserver(noticeId, observer));
        return lastNoticeId;
    }


    public void Remove(string notice, IObserver observer)
    {
        List<IObserver> obList;

        if (noticeList.TryGetValue(notice, out obList))
        {
            obList.Remove(observer);
        }
    }

    public void Remove(string notice, int noticeId)
    {
        List<IObserver> obList;

        if (noticeList.TryGetValue(notice, out obList))
        {
            foreach (IObserver observer in obList)
            {
                CallbackObserver ob = (CallbackObserver)observer;
                if (ob != null && ob.noticeId == noticeId)
                {
                    obList.Remove(observer);
                    break;
                }
            }
        }
    }

    public void Send(string notice, params object[] param)
    {
        List<IObserver> obList;

        if (noticeList.TryGetValue(notice, out obList))
        {
            IObserver[] list = obList.ToArray();
            foreach (IObserver ob in list)
            {
                ob.OnNotice(notice, param);
            }
        }
    }
}