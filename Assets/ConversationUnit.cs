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
    ConversationModel.Description desc;
    ConversationModel.Select select;
    
    public string SystemSymbol;

    public bool selected = false;
    public AudioSource pop;
    public AudioSource bg;

    void Awake()
    {
        _instance = this;
        
    }

    void Init() {
        model = ConversationManager.instance.GetDayScript();
    }

    void Start() {
        if (SystemSymbol == null) {
            SystemSymbol = "<<System>> ";
        }
        Init();
        bg.Play();
        Change();
        OnClick();
    }

    void End() {
        //한 개만 만들도록?
        if (desc.sys.Count > 0) {
            foreach (SystemMessage s in desc.sys) {
                if (s.GetType().Equals(SysMessageType.Keyword)) {
                    script.SysMake(SystemSymbol + ((KeywordMessage)s).GetMessage());
                    continue;
                }
                script.SysMake(SystemSymbol + s.GetMessage());
            }
        }
    }

    public void InturreptEnd() {
        script.SysMake(SystemSymbol + SystemMessageManager.instance.GetSysMessage(0).GetMessage());
    }

    void Change() {
        this.desc = model.GetDescByID(state);
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
        foreach (ConversationModel.Select.SelectNode s in select.list)
        {
            selectList.Add(s.desc);
        }
    }

    public void OnClick() {
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
        if (speaker.Equals(0))
        {
            script.LeftMake(str);
        }
        else {
            script.RightMake(str);
        }
        pop.PlayOneShot(pop.clip);
    }

    public void OnSelect(int i) {
        ConversationModel.Select.SelectNode selectedNode = null;
        string str = selectList[i - 1];
        string sub = str.Substring(3);
        pop.PlayOneShot(pop.clip);
        script.RightMake(sub);
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

}
