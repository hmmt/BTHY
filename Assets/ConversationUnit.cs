using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConversationUnit : MonoBehaviour {
    private static ConversationUnit _instance;
    public static ConversationUnit instance
    {
        get { return _instance; }
    }

    private ConversationModel model;
    public ConversationScript script;

    Queue<string> queue = new Queue<string>();
    List<string> selectList = new List<string>();
    long state = 0;
    long selectTarget = 0;
    int current = 0;
    short speaker = 0;
    public bool endDisplayed = false;
    ConversationModel.Description desc;
    ConversationModel.Select select;
    EndingModel end;
    static int endDate = -1;
    static bool isEnded = false;

    public string SystemSymbol;

    public bool selected = false;
    public AudioSource pop;
    public AudioSource bg;
    public Animator alert;

    void Awake()
    {
        _instance = this;   
        
    }

    void Init() {
        model = ConversationManager.instance.GetDayScript();
        if (isEnded && model.date > ConversationManager.instance.GetEndDate()) {
            this.endDisplayed = true;
        }
        alert.gameObject.SetActive(false);
        
    }

    void Start() {
        if (SystemSymbol == null) {
            SystemSymbol = "<<System>> ";
        }
        Init();
        bg.Play();
        if (endDisplayed) {
            InturreptEnd();
            return;
        }
        Change();
        OnClick();
    }

    void End() {
        //한 개만 만들도록?
        if (desc.sys.Count > 0) {
            foreach (SystemMessage s in desc.sys) {
                if (s.GetType().Equals(SysMessageType.Keyword)) {
                    script.SysMake(SystemSymbol + ((KeywordMessage)s).GetMessage() + " " + SystemMessageManager.instance.GetSysMessage(2).GetMessage());
                    continue;
                }
                script.SysMake(SystemSymbol + s.GetMessage());
            }
        }
        
    }

    public void InturreptEnd() {
        script.SysMake(SystemSymbol + SystemMessageManager.instance.GetSysMessage(1).GetMessage());
    }

    void Change() {
        this.desc = model.GetDescByID(state);
        if (desc.isEnd) {
            ConversationManager.instance.SetEnd(desc.endId);
            isEnded = true;
        }
        this.selectTarget = desc.selectId;

        this.speaker = desc.speaker;
        if (desc.id == 0 || selectTarget != 0) {
            this.select = model.GetSelectById(selectTarget);
        }
        
        SetString();
    }

    void SetString() {
        queue.Clear();
        selectList.Clear();
        foreach (string s in desc.desc)
        {
            queue.Enqueue(s);
        }
        if (select != null)
        {
            foreach (ConversationModel.Select.SelectNode s in select.list)
            {
                selectList.Add(s.desc);
            }
        }
    }

    public void OnClick() {
        if (endDisplayed) return;
        if (queue.Count == 0) {
            if (!selected)
            {
                selected = true;
                pop.PlayOneShot(pop.clip);
                if (desc.isEndScript())
                {
                    End();
                    return;
                }
                script.SelectMake(selectList.ToArray());
                return;
            }
            else return;
        }
        string str = queue.Dequeue();
        short sp = speaker;

        int subindex = str.IndexOf("#");
        string sub1, sub2;
        if (subindex != -1)
        {
            sub1 = str.Substring(0, subindex);
            sub2 = str.Substring(subindex+1);
            int subindex2 = sub2.IndexOf("$");

            if (subindex2 != -1) {
                string sub3;
                sub3 = sub2.Substring(subindex2);
                
                //make Alert
                short effect;
                Debug.Log(sub3);
                string effectId = sub3.Substring(1);
                effect = short.Parse(effectId);
                Effect(effect);
            }
            string speakerId = sub2.Substring(0,1);
            sp = short.Parse(speakerId);

            str = sub1;
        }
        script.MakeTextByID(sp , str);
        pop.PlayOneShot(pop.clip);
    }

    public void OnSelect(int i) {
        ConversationModel.Select.SelectNode selectedNode = null;
        string str = selectList[i - 1];
        string sub = str.Substring(3);
        pop.PlayOneShot(pop.clip);
        script.MakeTextByID(SpeakerID.PLAYER, sub);
        script.ClearSelect();
        selectList.Clear();
        selected = false;

        selectedNode = select.GetById(i - 1);
        if (selectedNode == null) {
            Debug.Log("뭔가 선택이 잘못된듯?");
        }
        state = selectedNode.descId;
        
        Change();
    }

    public void Effect(short id) {
        this.alert.gameObject.SetActive(true);
        alert.SetInteger("Cnt", 3);
    }
}
