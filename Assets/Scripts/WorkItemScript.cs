using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class WorkItemScript : MonoBehaviour {
    public Text tier;
    public Text name;
    public Image icon;
    public Image bg;

    public SkillTypeInfo info;

    public void SetInfo(SkillTypeInfo info) {
        this.info = info;
        tier.text = "1";
        name.text = info.name;
        icon.sprite = ResourceCache.instance.GetSprite("Sprites/UI/skill/" + info.imgsrc);
    }

    public void OnClick() {
    }

    public GameObject GetIconObject() {
        return icon.gameObject;
    }

    public void SetBg(Sprite c) {
        this.bg.sprite = c;
    }
    
}
