using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkillTypeInfo {
    public class SkillTrait {
        public string name;//not used
        public float min;
        public float max;

        public SkillTrait() {
            name = "";
            min = 0;
            max = 0;
        }

        public void SetValue(float min, float max) {
            this.min = min;
            this.max = max;
        }
    }
    public string name;
    public int level;
	public long id;
    public string description;
    public float amount;//작업량?
    public float feeling;//기분수치 반영값
    public float energy;//에너지생산량
    public float coolTime;//쿨타임

    public int animID;//애니메이션 id

    public CreatureType type;

    public SkillTrait intelligence;//정신
    public SkillTrait size;//크기
    public SkillTrait attack;//공격
    public SkillTrait gorgeous;//고상한 정도 : 스킬 적용 대상 지정용
    public SkillTrait damage;//피해량

    public string imgsrc;

    public SkillTypeInfo() {
        this.name = "";
        this.level = 0;
        this.id = 0;
        this.description = "";
        this.type = CreatureType.NONE;
        this.intelligence = new SkillTrait();
        this.size = new SkillTrait();
        this.attack = new SkillTrait();
        this.gorgeous = new SkillTrait();
        this.damage = new SkillTrait();
        this.imgsrc = "";
    }
}

/*
public class SkillUnit {
    public class SkillTrait {
        public string name;//not used
        public float min;
        public float max;

        public SkillTrait() {
            name = "";
            min = 0;
            max = 0;
        }

        public void SetValue(float min, float max) {
            this.min = min;
            this.max = max;
        }
    }
    public string name;
    public int level;

    public float amount;//작업량?
    public float feeling;//기분수치 반영값
    public float energy;//에너지생산량
    public float coolTime;//쿨타임

    public CreatureType type;

    public SkillTrait intelligence;//정신
    public SkillTrait size;//크기
    public SkillTrait attack;//공격
    public SkillTrait gorgeous;//고상한 정도 : 스킬 적용 대상 지정용
    public SkillTrait damage;//피해량

    public string imgsrc;

    public SkillUnit() {
        this.name = "";
        this.level = 0;

        this.type = CreatureType.NONE;
        this.intelligence = new SkillTrait();
        this.size = new SkillTrait();
        this.attack = new SkillTrait();
        this.gorgeous = new SkillTrait();
        this.damage = new SkillTrait();
        imgsrc = "";
    }
}*/

/// <summary>
/// Collections of Skills for Agent Working.
/// Each Agents has not each skills but categories.
/// </summary>
public class SkillCategory {
    public string name;
    public int tier;//Tier
    public List<SkillTypeInfo> list;//totalSkills
    //public List<SkillTypeInfo> currentList;
    public int currentLevel;
    public int MaxLevel;

    public long id;

    public SkillCategory(string name, int tier) {
        this.name = name;
        this.tier = tier;
        list = new List<SkillTypeInfo>();
        currentLevel = 1;
        MaxLevel = 3;
    }

    public void AddSkill(SkillTypeInfo item)
    {
        this.list.Add(item);
    }

    private int CompareByLevel(SkillTypeInfo a, SkillTypeInfo b)
    {
        if (a == null || b == null) {
            return 0;
        }

        return a.level.CompareTo(b.level);
    }

    public void SortList() {
        list.Sort(CompareByLevel);
    }

    public SkillTypeInfo[] GetAry()
    {
        List<SkillTypeInfo> output = new List<SkillTypeInfo>();

        for (int i = 0; i < list.Count; i++) {
            if (list[i].level <= currentLevel)
            {
                output.Add(list[i]);
                continue;
            }
            else
                break;
        }
        return output.ToArray();
    }

    public SkillTypeInfo FindByName(string name)
    {
        SkillTypeInfo output = null;
        foreach (SkillTypeInfo unit in list)
        {
            if (unit.name.Equals(name)) {
                output = unit;
                break;
            }
        }
        return output;
    }

    public SkillTypeInfo[] GetByLevel(int level)
    {
        if (level < 1 || level > MaxLevel) {
            return null;
        }

        List<SkillTypeInfo> output = new List<SkillTypeInfo>();

        foreach (SkillTypeInfo unit in list)
        {
            if (unit.level < level) {
                continue;
            }
            else if(unit.level == level){
                output.Add(unit);
            }
            else
                break;
        }

        return output.ToArray();  
    }

    public bool SkillLevelUp() {
        if (this.currentLevel >= MaxLevel) {
            Debug.Log("Error");
            return false;
            //error
        }

        currentLevel++;
        return true;    
    }

    public SkillCategory GetCopy() {
        SkillCategory newItem = new SkillCategory(this.name, this.tier);
        newItem.list = new List<SkillTypeInfo>(this.list.ToArray());
        newItem.currentLevel = this.currentLevel;
        newItem.MaxLevel = this.MaxLevel;
        return newItem;
    }
}

public class SkillManager {
    private static SkillManager _instance = null;
    public static SkillManager instance
    {
        get { 
            if (_instance == null){
                _instance = new SkillManager();    
            }
            return _instance;
        }
    }

    public bool isLoaded = false;
    public List<SkillCategory> list;

    public SkillManager() {
        this.list = new List<SkillCategory>();
        
    }

    public void AddCategory(SkillCategory item)
    {
        this.list.Add(item);
    }

    public SkillCategory GetCategoryByName(string name) {
        SkillCategory output = null;

        foreach (SkillCategory cat in list) {
            if (cat.name.Equals(name)) {
                output = cat;
                break;
            }
        }
        return output;
    }

    public SkillCategory GetCategoryBySkill(SkillTypeInfo skill) {
        SkillCategory output = null;

        foreach (SkillCategory cat in list) {
            if (cat.FindByName(skill.name) != null) {
                output = cat;
                break;
            }
        }

        return output;
    }

    public SkillCategory GetRandomCategory(int tier, List<SkillCategory> except)
    {
        List<SkillCategory> tempList = new List<SkillCategory>();

        for (int i = 0; i < list.Count; i++) {
            if (list[i].tier < tier)
            {
                continue;
            }
            else if (list[i].tier == tier)
            {
                foreach (SkillCategory item in except) { 
                    if(item.name.Equals(list[i].name)){
                        continue;
                    }
                }
                tempList.Add(list[i]);
            }
            else break;
        }
        if (tempList.Count == 0) return null;
        
        int randVal = Random.Range(0, tempList.Count);
        return tempList[randVal].GetCopy();
    }

    public void SortList()
    {
        list.Sort(CompareByTier);

        foreach (SkillCategory item in list) {
            item.SortList();
        }
        this.isLoaded = true;
        //PromotionSkillTree.instance.Init();
        foreach (Sefira sefira in SefiraManager.instance.sefiraList) {
            sefira.InitAgentSkillCategory(this.list);
        }
    }

    private int CompareByTier(SkillCategory a, SkillCategory b) {
        if (a == null || b == null) {
            return 0;
        }

        return a.tier.CompareTo(b.tier);
    }

}

public class SkillCategoryName {
    public static string Nutrition = "Nutrition";
    public static string Treatment = "Treatment";
    public static string Clean = "Clean";
    public static string Communion = "Communion";
    public static string Control = "Control";

    public static string GetCategoryName(SkillCategory target) {
        switch (target.name) {
            case "Nutrition":
                return Nutrition;
            case "Treatment":
                return Treatment;
            case "Clean":
                return Clean;
            case "Communion":
                return Communion;
            case "Control":
                return Control;
            default:
                return "";
        }
    }

    public static string GetCategoryName(SkillTypeInfo target) {
        return GetCategoryName(SkillManager.instance.GetCategoryBySkill(target));
    }
}