using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkillTypeInfo {

	public long id;
	public string name;
	public string type;

    public string description;

	public int amount;

    public string imgsrc;

    public string bonusType;

    public float amountBonusD;
    public float feelingBonusD;
    public int mentalReduceD;
    public int mentalTickD;

    public float amountBonusI;
    public float feelingBonusI;
    public int mentalReduceI;
    public int mentalTickI;

    public float amountBonusS;
    public float feelingBonusS;
    public int mentalReduceS;
    public int mentalTickS;

    public float amountBonusC;
    public float feelingBonusC;
    public int mentalReduceC;
    public int mentalTickC;

    public long[] nextSkillIdList;

    public string category;
}

public class SkillUnit {
    public string name;
    public int level;

}

public class SkillCategory {
    public string name;
    public int tier;//Tier
    public List<SkillUnit> list;//totalSkills
    public List<SkillUnit> currentList;
    public int currentLevel;
    public int MaxLevel;

    public long id;

    public SkillCategory(string name, int tier) {
        this.name = name;
        this.tier = tier;
        list = new List<SkillUnit>();
        currentLevel = 1;
        MaxLevel = 3;
    }

    public void AddSkill(SkillUnit item) {
        this.list.Add(item);
    }

    private int CompareByLevel(SkillUnit a, SkillUnit b) {
        if (a == null || b == null) {
            return 0;
        }

        return a.level.CompareTo(b.level);
    }

    public void SortList() {
        list.Sort(CompareByLevel);
    }

    public SkillUnit[] GetAry() {
        List<SkillUnit> output = new List<SkillUnit>();

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

    public SkillUnit FindByName(string name) {
        SkillUnit output = null;
        foreach (SkillUnit unit in list) {
            if (unit.name.Equals(name)) {
                output = unit;
                break;
            }
        }
        return output;
    }

    public SkillUnit[] GetByLevel(int level) {
        if (level < 1 || level > MaxLevel) {
            return null;
        }

        List<SkillUnit> output = new List<SkillUnit>();

        foreach (SkillUnit unit in list) {
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
        newItem.list = new List<SkillUnit>(this.list.ToArray());
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
    }

    private int CompareByTier(SkillCategory a, SkillCategory b) {
        if (a == null || b == null) {
            return 0;
        }

        return a.tier.CompareTo(b.tier);
    }

}