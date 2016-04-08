using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class SkillTreeLeaf : MonoBehaviour {
    public SkillTreeItem parentScript;
    int index;

    public void Awake(){
        this.index = transform.GetSiblingIndex();
    }

    public void OnClick(BaseEventData eventData) {
        parentScript.OnClick(this.index);
    }
}
