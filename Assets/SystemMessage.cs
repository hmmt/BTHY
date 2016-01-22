using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum SysMessageType { 
    Message,
    Keyword
}

public class SystemMessage {
    public long id;
    public SysMessageType type;
    public string name;

    public SystemMessage() {
        this.type = SysMessageType.Message;
    }

    public virtual string GetMessage() {
        return this.name;
    }

    public SysMessageType GetType() {
        return this.type;
    }
}

public class KeywordMessage : SystemMessage{
    public string desc;

    public KeywordMessage() {
        this.type = SysMessageType.Keyword;
    }

    
}

public class SystemMessageManager {
    private static SystemMessageManager _instance;
    public static SystemMessageManager instance {
        get { 
            if(_instance ==null)
                _instance = new SystemMessageManager();
            return _instance;
        }
    }
    private bool _loaded = false;

    public List<SystemMessage> messageList;
    public List<KeywordMessage> keywordList;

    public SystemMessageManager() {
        _loaded = false;
    }

    public bool isLoaded(){
        return _loaded;
    }

    public KeywordMessage GetKeyword(long id) {
        KeywordMessage output = null;
        foreach (KeywordMessage km in keywordList) {
            if (km.id == id) {
                output = km;
                break;
            }
        }

        return output;
    }

    public SystemMessage GetSysMessage(long id) {
        SystemMessage output = null;
        foreach (SystemMessage sm in messageList) {
            if (sm.id == id) {
                output = sm;
                break;
            }
        }
        return output;
    }

    public void Init(SystemMessage[] sys, KeywordMessage[] keyword) {
        this._loaded = true;
        messageList = new List<SystemMessage>(sys);
        keywordList = new List<KeywordMessage>(keyword);

    }
}