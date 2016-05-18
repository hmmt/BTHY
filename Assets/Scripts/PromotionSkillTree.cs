using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PromotionSkillTree : MonoBehaviour {
    private static PromotionSkillTree _instance = null;
    public static PromotionSkillTree instance {
        get {
            return _instance;
        }
    }

    public AgentModel target;

    public SkillTreeItem selected;
    public string selectedname;
    public int level;

    public List<SkillCategory> currentCategory;
    public SkillTreeItem[] childTree;
    public MenuScript menu;

    private bool isLoaded = false;

    public void Awake() {
        _instance = this;
        level = -1;
    }

    public void Start() {
        PromotionSkillTreeInit();
    }

    public void PromotionSkillTreeInit() {
        if (isLoaded == false) {
            isLoaded = true;
            Init();
        }
    }

    public void Init() {
        currentCategory = new List<SkillCategory>(SkillManager.instance.list.ToArray());
        for (int i = childTree.Length - 1; i >= 0; i--) {
            SkillCategory cat = SkillManager.instance.GetCategoryByName(childTree[i].target).GetCopy();
            childTree[i].SetCategory(cat);
            childTree[i].OnConfirm();
        }

    }

    public void Check()
    {
        Debug.Log(childTree[0].GetCategory().name);
        for (int i = 0; i < childTree.Length; i++) {
            Debug.Log(childTree[i].GetCategory().name);
        }
    }

    public void SetModel(AgentModel model)
    {
        this.target = model;
        foreach (Menu button in menu.menus) {
            button.button.enabled = true;
        }

        menu.OnClick(menu.menus[0].button);

        if (model.level < 3) {
            menu.menus[1].button.enabled = false;
            menu.menus[2].button.enabled = false;
        }

        for (int i = 0; i < childTree.Length; i++) {
            childTree[i].Init(0);
        }

        foreach (SkillTreeItem item in this.childTree) {
            item.Init(0);
        }

        Queue<SkillCategory> queue = new Queue<SkillCategory>(target.GetSkillCategories());

        while(queue.Count > 0 ){
            SkillCategory skill = queue.Dequeue();
            
            for (int i = 0; i < childTree.Length; i++) {
                if (childTree[i].GetCategory().name.Equals(skill.name)) {
                    
                    childTree[i].Init(skill.currentLevel);
                    break;
                }
            }
        }
    }

    public void SetSelectedTarget(SkillTreeItem target, int level)
    {
        if (selected != null) {
            selected.OnDisabled();
        }
        selected = target;
        this.level = level+1;
        selectedname = selected.dispayTarget;
    }

    public void GetSelected(ref SkillCategory target, ref int levelindex) {
        if (selected == null || level == -1) {
            levelindex = -1;
            target = null;
            return;
        }
        levelindex = this.level;
        target = this.selected.GetCategory();
        
        return;
    }

    public void OnConfirm() {
        this.selected = null;
        this.target = null;
        this.level = -1;

        foreach (SkillTreeItem item in this.childTree) {
            item.OnConfirm();
        }
    }

}
