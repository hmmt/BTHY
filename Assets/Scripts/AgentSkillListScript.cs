using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AgentSkillListScript : MonoBehaviour {
    public GameObject slot;
    public RectTransform parent;
    private AgentModel target;
    public float spacing;
    private List<RectTransform> childList = new List<RectTransform>();

    public void Start() {
        foreach (RectTransform child in parent) {
            Destroy(child.gameObject);
        }
    }

    public void Init(AgentModel target) {
        this.target = target;

        foreach (RectTransform child in childList)
        {
            if (child.GetComponent<AgentSkillSlot>() != null) {
                Destroy(child.gameObject);
            }
        }
        childList.Clear();

        parent.sizeDelta = new Vector2(parent.sizeDelta.x, 0f);

        foreach (SkillCategory unit in target.GetSkillCategories()) {
            GameObject item = Instantiate(slot);
            AgentSkillSlot script = item.GetComponent<AgentSkillSlot>();
            script.Init(unit);
            item.transform.SetParent(parent);
            childList.Add(item.GetComponent<RectTransform>());
            item.transform.localScale = Vector3.one;
            item.transform.localPosition = Vector3.zero;
        }

        SortList();
    }

    public void SortList() {
        float posy = 0.0f;
        foreach (RectTransform child in childList) {
            if (child.GetComponent<AgentSkillSlot>() == null || child.GetComponent<AgentSkillSlot>().unit == null) continue;
            child.anchoredPosition = new Vector2(0.0f, posy);
            posy -= (child.rect.height + spacing);
        }
        parent.sizeDelta = new Vector2(parent.sizeDelta.x, -posy);
    }
}
