using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class AngelaConversaion {
    public class CreatureReaction {
        public string desc;
        public int level;
    }

    public class CreatureReactionList {
        public List<CreatureReaction> list = new List<CreatureReaction>();
        public long creatureId;

        public string GetDesc(int level) {
            CreatureReaction output = null;
            foreach (CreatureReaction cr in list) {
                if (cr.level == level) {
                    output = cr;
                    break;
                }
            }
            if (output == null) return null;
            return output.desc;
        }
    }

    private static AngelaConversaion _instance;
    public static AngelaConversaion instance {
        get {
            if (_instance == null) {
                _instance = new AngelaConversaion();
            }
            return _instance;
        }
    }

    public Dictionary<long, CreatureReactionList> lib;

    bool isLoaded = false;

    public bool loaded {
        get {
            return isLoaded;
        }
    }

    private AngelaConversaion() {
        _instance = this;
    }

    public void Init(Dictionary<long, CreatureReactionList> creatureReaction) {
        isLoaded = true;
        this.lib = creatureReaction;
    }

    public CreatureReactionList GetReactionList(long id) {
        CreatureReactionList output = null;
        if (lib.TryGetValue(id, out output)) {
            return output;
        }
        return null;
    }

    public void MakeCreatureReaction(CreatureModel targetCreature, int level) {
        long targetId = targetCreature.metadataId;
        CreatureReactionList targetList = null;
        if ((targetList = GetReactionList(targetId)) != null) {
            string desc = targetList.GetDesc(level);
            if (desc != null) {
                desc = "Angela : " + desc;
                SendSystemLogMessage(desc);
                SendNarrationLogMessage(targetCreature, desc);
            }
        }
    }

    public void SendSystemLogMessage(string desc) {
        Notice.instance.Send(NoticeName.AddSystemLog, desc);
    }

    public void SendNarrationLogMessage(CreatureModel model, string desc) {
        Notice.instance.Send("AddNarrationLog", desc, model);
        model.narrationList.Add(desc);
    }
}