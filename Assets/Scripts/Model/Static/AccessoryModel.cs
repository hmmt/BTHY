using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public enum AccessoryPos { 
    HEAD,
    HAIR,
    EYE,
    MOUTH,
    RIGHTHAIR,
    LEFTHAIR,
    BODY,
    LEFTLEG,
    RIGHTLEG,

    NONE//dummy please not use
}

public class AccessoryUnit {
    public bool isInitialized = false;
    public AccessoryModel[] item = new AccessoryModel[(int)AccessoryPos.NONE];
    public AgentAnim animatorScript;

    public void SetAccessoryByTrait(TraitTypeInfo info) {
        if (!isInitialized) {
            Debug.Log("init needed to accessoryUnit");
            return;
        }

        AccessoryPos pos = GetPos(info.imgPos);
        Transform parent = GetParent(pos, animatorScript);

        if (item[(int)pos] != null) {
            item[(int)pos].ChangeSprite(info);
            return;
        }

        GameObject newAcc = Prefab.LoadPrefab("AccessoryItem");
        item[(int)pos] = newAcc.GetComponent<AccessoryModel>();
        item[(int)pos].Init(info, parent, pos);
    }

    public void Init(AgentAnim anim) {
        if (isInitialized) return;
        this.animatorScript = anim;
        isInitialized = true;
        for (int i = 0; i < item.Length; i++) {
            item[i] = null;
        }
    }

    public static AccessoryPos GetPos(string str)
    {
        switch (str) {
            case "HEAD": return AccessoryPos.HEAD;
            case "HAIR": return AccessoryPos.HAIR;
            case "EYE": return AccessoryPos.EYE;
            case "MOUTH": return AccessoryPos.MOUTH;
            case "RIGHTHAIR": return AccessoryPos.RIGHTHAIR;
            case "LEFTHAIR": return AccessoryPos.LEFTHAIR;
            case "BODY": return AccessoryPos.BODY;
            case "LEFTLEG": return AccessoryPos.LEFTLEG;
            case "RIGHTLEG": return AccessoryPos.RIGHTLEG;
            default: return AccessoryPos.NONE;
        }
    }

    public static Transform GetParent(AccessoryPos pos, AgentAnim anim) {
        switch (pos) {
                /*
            case AccessoryPos.HEAD:
            case AccessoryPos.HAIR:
            case AccessoryPos.EYE:
            case AccessoryPos.MOUTH:
            case AccessoryPos.RIGHTHAIR:
            case AccessoryPos.LEFTHAIR:
                return anim.face.transform.parent.transform;
            case AccessoryPos.BODY:
                return anim.body.transform;
            case AccessoryPos.LEFTLEG:
                return anim.B_up_leg.transform;
            case AccessoryPos.RIGHTLEG:
                return anim.F_up_leg.transform;
            default: return null;*/
            case AccessoryPos.HEAD:
            case AccessoryPos.HAIR:
            case AccessoryPos.EYE:
            case AccessoryPos.MOUTH:
            case AccessoryPos.RIGHTHAIR:
            case AccessoryPos.LEFTHAIR:
            case AccessoryPos.BODY:
            case AccessoryPos.LEFTLEG:
            case AccessoryPos.RIGHTLEG:
            default:
                return anim.face.transform.parent.transform;
        }
    }
}

public class AccessoryModel :MonoBehaviour{
    public Transform parent;
    private Sprite image;
    public SpriteRenderer renderer;
    public TraitTypeInfo trait;
    bool isInititialized = false;
    Vector3 positionVector;

    public void Init(TraitTypeInfo trait, Transform parentPos, AccessoryPos pos) {
        if (isInititialized) {
            ChangeSprite(trait);
            return;
        }
        isInititialized = true;
        SetPositionVector(pos);
        this.parent = parentPos;
        this.image = ResourceCache.instance.GetSprite("Sprites/Accessory/"+trait.imgsrc);
        this.gameObject.transform.SetParent(parent);
        this.transform.localRotation = Quaternion.identity;
        this.transform.localScale = Vector3.one;
        this.transform.localPosition = positionVector;
        this.renderer.sprite = image;
    }

    public void ChangeSprite(TraitTypeInfo trait) {
        this.image = ResourceCache.instance.GetSprite("Sprites/Accessory/" + trait.imgsrc);
        this.renderer.sprite = image;
    }

    public void SetPositionVector(AccessoryPos pos) {
        switch (pos) {
            case AccessoryPos.HEAD:
            case AccessoryPos.HAIR:
            case AccessoryPos.RIGHTHAIR:
            case AccessoryPos.LEFTHAIR:
                this.positionVector = new Vector3(-0.25f, -1.4f, -5f);
                return;

            case AccessoryPos.EYE:
            case AccessoryPos.MOUTH:
                this.positionVector = new Vector3(-0.41f, -1.35f, -5f);
                return;

            case AccessoryPos.BODY:
            case AccessoryPos.LEFTLEG:
            case AccessoryPos.RIGHTLEG:
                this.positionVector = new Vector3(0f,0f, -5f);
                return;
        }
    }
}

