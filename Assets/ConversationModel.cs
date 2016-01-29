using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConversationModel
{
    public class Description {

        public long id;
        public long selectId;
        public short speaker;
        public static bool isloaded = false;
        public bool innerText = false;
        public string tempdesc;

        public bool isEnd = false;
        public string endId = "";

        public List<string> desc;
        public List<SystemMessage> sys = new List<SystemMessage>();

        public Description() {
            isloaded = false;
        }

        public void loadText()
        {
            //not implemented
            isloaded = true;
            desc = new List<string>(TextConverter.GetTextFromFormatProcessText(tempdesc));
            
        }

        public void loadInnerText()
        {

        }

        public string[] GetDescList() {
            return desc.ToArray();
        }

        public bool isEndScript() {
            if (selectId.Equals(0)){
                Debug.Log("끝");
                return true;
            }
            return false;
        }

    }
    public class Select {
        public class SelectNode {
            public long id;
            public long descId;
            public string desc;
            public int favor;
        }

        public long id;
        public List<SelectNode> list = new List<SelectNode>();

        public SelectNode GetById(long id) {
            SelectNode output = null;
            foreach (SelectNode node in list) {
                if (node.id.Equals(id)) {
                    output = node;
                }
            }
            return output;
        }
    }

    public int date;
    public List<Description> descList;
    public List<Select> selectList;
    
    public ConversationModel() { }

    public void InitDescList(Description[] ary) {
        descList = new List<Description>(ary);
        //로드 검사
    }

    public void InitSelectList(Select[] ary) {
        selectList = new List<Select>(ary);
    }

    public Description GetDescByID(long id) {
        Description output = null;
        foreach (Description d in descList) {
            if (d.id.Equals(id)) {
                output = d;
                break;
            }
        }
        return output;
    }

    public Select GetSelectById(long id) {
        Select output = null;
        foreach (Select s in selectList) {
            if (s.id.Equals(id)) {
                output = s;
                break;
            }
        }
        return output;
    }

}

public class EndingModel :ConversationModel{
    public string target;
    public int date;

    public EndingModel() { 
        
    }
}

public class ConversationManager {
    private static ConversationManager _instance;
    public static ConversationManager instance {
        get {
            if (_instance == null)
                _instance = new ConversationManager();
            return _instance;
        }
    }

    public List<ConversationModel> list;
    public List<EndingModel> ending;

    private bool endLoad = false;
    private string endId = "";
    private bool _loaded = false;
    private int cnt = 0;
    private int endDate = -1;

    public ConversationManager() {
        _loaded = false;
    }

    public bool isLoaded() {
        return _loaded;
    }

    public void Init(ConversationModel[] input, EndingModel[] endInput) {
        _loaded = true;
       // Debug.Log("Loaded");
        list = new List<ConversationModel>(input);
        ending = new List<EndingModel>(endInput);
    }

    public ConversationModel GetDayScript() {
        ConversationModel output = null;
            
        if (_loaded == false) {
            Debug.Log("no loaded");
            return null;
        }

        int daynum = PlayerModel.instance.GetDay();
        daynum--;
        if (daynum < 0) {
            daynum = 0;
        }
        if (daynum > list.Count){
            Debug.Log("no script for date " + daynum);
            daynum = 0;
        }

        if (endLoad) {
            endLoad = false;
            output = GetEnd(endId);
            cnt++;
        }
        else output = list[cnt++];
        
        return output;
        
    }

    public void SetEnd(string id) {
        this.endId = id;
        this.endLoad = true;
        this.endDate = GetEnd(id).date;
    }

    public int GetEndDate() {
        return this.endDate;
    }

    public EndingModel GetEnd(string str) {
        EndingModel output = null;

        foreach (EndingModel em in ending) {
            if (em.target.Equals(str)) {
                output = em;
                return output;
            }
        }
        return output;
    }
}
