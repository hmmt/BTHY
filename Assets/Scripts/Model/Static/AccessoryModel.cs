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

	SHIELD,

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
			item[(int)pos].ChangeSprite(info.imgsrc);
            return;
        }

        GameObject newAcc = Prefab.LoadPrefab("AccessoryItem");
        item[(int)pos] = newAcc.GetComponent<AccessoryModel>();
        item[(int)pos].Init(info, parent, pos);
    }

	public void SetAccessory(string imgPos, string imgsrc, float scale)
	{
		if (!isInitialized) {
			Debug.Log("init needed to accessoryUnit");
			return;
		}

		AccessoryPos pos = GetPos(imgPos);
		Transform parent = GetParent(pos, animatorScript);

		if (item[(int)pos] != null) {
			item[(int)pos].ChangeSprite(imgsrc);
			return;
		}

		GameObject newAcc = Prefab.LoadPrefab("AccessoryItem");
		item[(int)pos] = newAcc.GetComponent<AccessoryModel>();
		item[(int)pos].Init(imgsrc, parent, pos, scale);
	}

	public void RemoveAccessory(string imgPos)
	{
		AccessoryPos pos = GetPos(imgPos);

		if (item[(int)pos] != null) {
			item [(int)pos] = null;
			return;
		}
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
			case "SHIELD": return AccessoryPos.SHIELD;
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
		case AccessoryPos.SHIELD:
			return anim.body.transform.parent.transform;
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
    bool isInititialized = false;
    Vector3 positionVector;

    public void Init(TraitTypeInfo trait, Transform parentPos, AccessoryPos pos) {
        if (isInititialized) {
			ChangeSprite(trait.imgsrc);
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

	public void Init(string imgsrc, Transform parentPos, AccessoryPos pos, float scale)
	{
		if (isInititialized) {
			ChangeSprite(imgsrc);
			return;
		}
		isInititialized = true;
		SetPositionVector(pos);
		this.parent = parentPos;
		this.image = ResourceCache.instance.GetSprite("Sprites/Accessory/"+imgsrc);
		this.gameObject.transform.SetParent(parent);
		this.transform.localRotation = Quaternion.identity;
		this.transform.localScale = Vector3.one * scale;
		this.transform.localPosition = positionVector;
		this.renderer.sprite = image;
	}

	public void ChangeSprite(string imgsrc) {
        this.image = ResourceCache.instance.GetSprite("Sprites/Accessory/" + imgsrc);
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
			case AccessoryPos.SHIELD:
				this.positionVector = new Vector3(0, -0.3f, -50f);
				return;
        }
    }
}

