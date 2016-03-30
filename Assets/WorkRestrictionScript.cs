using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

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
        if (table == null) {
            Debug.Log("Finding error in restriction");

        }
        return table;
    }

}

public class WorkRestrictionScript : MonoBehaviour {
    [System.Serializable]
    public class RestrictionItem {
        public Toggle button;
        public Text desc;

        public void Init(RestrictionTable.TableElement.Restriction item) {
            this.button.isOn = item.isRestricted;
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
            if (output.desc.text == desc) {
                output = item;
                break;
            }

        }
        return output;
    }

    
}
