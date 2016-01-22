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
    public List<Description> descList = new List<Description>();
    public List<Select> selectList = new List<Select>();

    public void InitDescList(Description[] ary) {
        descList = new List<Description>(ary);
        //로드 검사
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
    private bool _loaded = false;

    public ConversationManager() {
        _loaded = false;
    }

    public bool isLoaded() {
        return _loaded;
    }

    public void Init(ConversationModel[] input) {
        _loaded = true;
        Debug.Log("Loaded");
        list = new List<ConversationModel>(input);
    }


    public ConversationModel GetDayScript() {
        ConversationModel output = null;
        if (_loaded == false) {
            Debug.Log("no loaded");
            return null;
        }
        int daynum = PlayerModel.instance.GetDay();
        daynum--;
        Debug.Log("오늘은 " + (daynum+1) + "일");
        if (daynum < 0) {
            daynum = 0;
        }
        if (daynum > list.Count){
            Debug.Log("no script for date " + daynum);
            daynum = 0;
        }
        output = list[daynum];

        return output;
        
    }
}
