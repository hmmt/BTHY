using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RestrictionContent {
    public static string Woman = "Female";
    public static string Man = "Male";
    /*
    public static string Lifestyle_D = "life_d";
    public static string Lifestyle_I = "life_i";
    public static string Lifestyle_S = "life_s";
    public static string Lifestyle_C = "life_c";
    public static string Maxhealth = "maxhealth";
    public static string Maxmental = "maxmental";
    public static string Minhealth = "minhealth";
    public static string Minmental = "minmental";
    */

    public static int cnt = 10;

    public static List<string> GetList() {
        List<string> output = new List<string>();
        output.Add(Woman);
        output.Add(Man);
        /*
        output.Add(Lifestyle_D);
        output.Add(Lifestyle_I);
        output.Add(Lifestyle_S);
        output.Add(Lifestyle_C);
        output.Add(Maxhealth);
        output.Add(Maxmental);
        output.Add(Minhealth);
        output.Add(Minmental);
         */
        return output;
    }
}

public class RestrictionTable {
    public class TableElement{

        public class Restriction {
            public string desc;
            public bool isRestricted;

            public void OnChange() {
                isRestricted = !isRestricted;
            }
        }

        public long creatureID;
        public List<Restriction> list = new List<Restriction>();

        public void Init(CreatureModel model) {
            this.creatureID = model.instanceId;
            List<string> textList = RestrictionContent.GetList();

            foreach (string str in textList) {
                Restriction item = new Restriction();
                item.desc = str;
                item.isRestricted = false;
                this.list.Add(item);
            }
        }

        public List<string> GetRestrictionString() {
            List<string> output = new List<string>();

            foreach (Restriction r in this.list) {

                if (r.isRestricted == true) {
                    output.Add(r.desc);
                }
            }
            return output;
        }

        public Restriction[] GetRestriction() {
            return list.ToArray();
        }
    }

    private static RestrictionTable _instance;
    public static RestrictionTable instance {
        get {
            if (_instance == null) {
                _instance = new RestrictionTable();
            }
            return _instance;
        }
    }

    public List<TableElement> list;

    public RestrictionTable() {
        _instance = this;
        list = new List<TableElement>();
    }

    public void AddCreature(CreatureModel model) {
        TableElement t = new TableElement();
        t.creatureID = model.metaInfo.id;
        //t.list = new TableElement.Restriction();
        t.Init(model);
        this.list.Add(t);
    }

    public TableElement GetTableByCreature(CreatureModel model) {
        TableElement table = null;
        foreach (TableElement t in this.list) {
            if (t.creatureID.Equals(model.instanceId)) {
                table = t;
                break;
            }
        }
        if (table == null)
        {
            AddCreature(model);
            return GetTableByCreature(model);
        }
        return table;
    }

}

public class WorkRestrictionScript : MonoBehaviour {
    [System.Serializable]
    public class RestrictionItem {
        public Toggle button;
        public Text desc;

        [HideInInspector]
        public RestrictionTable.TableElement.Restriction target;

        public void Init(RestrictionTable.TableElement.Restriction item) {
            target = item;

            this.button.isOn = !item.isRestricted;
        }

        public void OnClick() {

            target.OnChange();
        }
    }

    public List<RestrictionItem> list;

    private CreatureModel _target = null;

    public void Init(CreatureModel target) {
        _target = target;
        
        Matching();
    }

    public void Matching() {
        RestrictionTable.TableElement table = RestrictionTable.instance.GetTableByCreature(_target);
        
        foreach (RestrictionTable.TableElement.Restriction item in table.list) {
            RestrictionItem target = GetItem(item.desc);
            if (target == null) {
                continue;
            }
            target.Init(item);
        }
    }

    public RestrictionItem GetItem(string desc) {
        RestrictionItem output = null;
        foreach (RestrictionItem item in this.list) {
            if (item.desc.text == desc) {
                output = item;
                break;
            }

        }
        return output;
    }

    public void OnClick(Toggle target) {
        RestrictionItem current = null;

        foreach (RestrictionItem item in this.list) {
            if (item.button.Equals(target)) {
                current = item;
                break;
            }
        }
        if (current == null) {
            Debug.Log("Error");
            return;
        }
        current.OnClick();
        if (SelectWorkAgentWindow.currentWindow != null) {
            SelectWorkAgentWindow.currentWindow.OnRestrictionChanged();
        }
        
    }

    public void OnClick(int i) { 
        if(i < 0 || i >= this.list.Count) return;
        RestrictionItem current = this.list[i];
        current.OnClick();
        if (SelectWorkAgentWindow.currentWindow != null)
        {
            SelectWorkAgentWindow.currentWindow.OnRestrictionChanged();
        }
    }
}
