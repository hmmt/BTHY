using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AccessoryModel :MonoBehaviour{
    public Transform parent;
    private Sprite image;
    public SpriteRenderer renderer;
    public TraitTypeInfo trait;

    public void Init(TraitTypeInfo trait, Transform parentPos) {
        this.parent = parentPos;
        this.image = ResourceCache.instance.GetSprite("Sprites/Accessory/"+trait.imgsrc);
        this.gameObject.transform.SetParent(parent);
        this.transform.localRotation = Quaternion.identity;
        this.transform.localScale = Vector3.one;
        this.transform.localPosition = new Vector3(0f, 0f, -5f);
        this.renderer.sprite = image;
    }
}

