using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ManageWorkIconState { 
    COLOR,
    DEFAULT,
    BLACK
}

public class IconId {
    public static int Meal1 = 1001;
    public static int Meal2 = 1002;
    public static int Communion1 = 1011;
    public static int Communion2 = 1012;
    public static int Play1 = 1021;
    public static int Play2 = 1022;
    public static int Violent1 = 1031;
    public static int Violent2 = 1032;
    public static int Clean1 = 1041;
    public static int Clean2 = 1042;
    public static int Special1 = 1000;

    public static int Panic = 2000;
    public static int Block = 1001;
    public static int Gun = 1002;
    public static int Stick = 1003;
    public static int Murder = 2001;
    public static int Suicide = 2002;
    public static int Wander = 2003;
    public static int Release = 2004;
    public static int Principled = 3001;
    public static int Pacifist = 3002;
    public static int Optimist = 3003;
    public static int Rationalist = 3004;
}

public class IconManager : MonoBehaviour {
    [System.Serializable]
    public class Icon {
        public string name;
        public int id;
        public Sprite icon;
    }

    [System.Serializable]
    public class WorkIcon
    {
        public string name;
        public List<Sprite> sprites;
        [HideInInspector]
        public List<Icon> icons;
        public int id;
        public int defaultIndex;

        public void Init() {
            foreach (Sprite s in sprites) {
                Icon icon = new Icon();
                icon.icon = s;
                icon.id = this.id;
                icon.name = this.name + this.id.ToString();
                icons.Add(icon);
            }
        }

        public Icon GetIcon(ManageWorkIconState state) {
            int index = (int)state;
            if (index > sprites.Count) index = defaultIndex;
            return this.icons[index];
        }

        public Icon GetDefault() {
            return this.icons[defaultIndex];
        }
    }
    
    private static IconManager _instance = null;
    public static IconManager instance {
        get {
            return _instance;
        }
    }

    public List<Icon> list;
    public List<WorkIcon> workIcon;
    
    public void Awake() {
        _instance = this; 
        foreach (WorkIcon w in workIcon)
        {
            w.Init();
        }
    }

    public void Start() {
        
    }

    public Icon GetIcon(string name)
    {
        Icon output = null;
        foreach (Icon i in list) {
            if (i.name == name) {
                output = i;
                break;
            }
        }
        return output;  
    }

    public Icon GetIcon(int id)
    {
        Icon output = null;
        foreach (Icon i in list)
        {
            if (i.id == id)
            {
                output = i;
                break;
            }
        }
        return output;
    }

    public WorkIcon GetWorkIcon(int id) {
        WorkIcon output = null;
        foreach (WorkIcon w in this.workIcon) {
            if (w.id == id) {
                output = w;
                break;
            }
        }
        return output;
    }
}
