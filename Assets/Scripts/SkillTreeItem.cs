using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillTreeItem : MonoBehaviour {

    private SkillCategory currentCategory;
    public string target;
    public string dispayTarget;
    public int activated = -1;



    private Color white;
    private Color gray;
    private Image targetImage;

    public void Awake() {
        white = new Color(1f, 1f, 1f, 1f);
        gray = new Color(1f, 1f, 1f, 0.5f);
    }

    public void SetCategory(SkillCategory category) {
        currentCategory = category;
        this.dispayTarget = currentCategory.name;
    }

    public SkillCategory GetCategory() {
        return currentCategory;
    }

    public void DummyCheck() {
        Debug.Log(currentCategory.name);
    }

    public void Init(int targetIndex)
    {
        //bug.Log(currentCategory.name);
        this.activated = targetIndex;
        
        foreach (Transform t in this.transform.GetChild(0).transform) {
            int index = t.GetSiblingIndex();
            Image tempimg = t.GetComponent<Image>();

            if (index < targetIndex) {
                tempimg.color = white;
            }
            else if (index == targetIndex)
            {
                tempimg.color = gray;
                t.GetComponent<EventTrigger>().enabled = true;
                targetImage = tempimg;
            }
            else {
                tempimg.color = gray;
            }
        }
    }

    public void OnClick(int index) {
        PromotionSkillTree.instance.SetSelectedTarget(this, 
            activated);
        targetImage.color = white;
    }

    public void OnDisabled() {
        targetImage.color = gray;
    }

    public void OnConfirm() {
        targetImage = null;
        this.activated = -1;
        foreach (Transform t in this.transform.GetChild(0).transform)
        {
            EventTrigger trigger = t.GetComponent<EventTrigger>();
            Image img = t.GetComponent<Image>();

            trigger.enabled = false;
            //img.color = gray;
            img.color = gray;
        }
    }

}
