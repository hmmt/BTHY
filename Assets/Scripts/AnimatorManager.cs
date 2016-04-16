using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class AnimatorName {
    public static string AgentCtrl = "AgentCtrl";
    public static string OfficerCtrl = "AgentCtrl";
	public static string RedShoes_attract = "RedShoes_attracted";
	public static string RedShoes_infected = "RedShoes_infected";
    public static string RedShoes_victim = "RedShoes_victim";
    public static string Machine_attract = "Machine_attracted";
    public static string Machine_victim = "Machine_victim";
	public static string Teddy_agent = "Teddy_agent";
    

    public static long id_AgentCtrl = 50001;
    public static long id_OfficerCtrl = 50002;
    public static long id_RedShoes_attract = 60001;
    public static long id_RedShoes_infected = 60002;
    public static long id_RedShoes_victim = 60003;
    public static long id_Machine_attract = 70001;
    public static long id_Machine_victim = 70002;
	public static long id_Teddy_agent = 80001;
}

public class AnimatorManager : MonoBehaviour{
    private static AnimatorManager _instance;
    public static AnimatorManager instance {
        get {
            if (_instance == null) {
                _instance = new AnimatorManager();
            }
            return _instance;
        }
    }

    [System.Serializable]
    public class AnimatorComponet {
        public class TransformElement {
            public class Element {
                public Vector3 position;
                public Quaternion rotation;
                public Vector3 scale;
				public bool active;
				public float alpha;

                public Transform target;

                public Element(Transform transform) {
					this.position = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
					this.rotation = new Quaternion(transform.localRotation.x, transform.localRotation.y, transform.localRotation.z, transform.localRotation.w);
                    this.scale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
					this.active = transform.gameObject.activeSelf;
					SpriteRenderer r = transform.GetComponent<SpriteRenderer>();
					if(r != null)
					{
						this.alpha = r.color.a;
					}

                    this.target = transform;
                }

                public void Reset() {
					target.localPosition = this.position;
                    target.localRotation = this.rotation;
                    target.localScale = this.scale;

					SpriteRenderer r = target.GetComponent<SpriteRenderer>();
					if(r != null)
					{
						r.color = new Color (r.color.r, r.color.g, r.color.b, alpha);
					}
                }
            }

            public List<Element> list = new List<Element>();

            public TransformElement(Animator target) {
                SaveTransform(target.transform);
            }

            public void SaveTransform(Transform transform) {
                foreach (Transform t in transform) {
                    if (t.childCount > 0) {
                        SaveTransform(t);
                    }
                    Element element = new Element(t);
                    this.list.Add(element);
                }
            }

            public void GetTransform() {
                foreach (Element e in this.list) {
                    e.Reset();
                }
            }
        }
        public TransformElement element;

        public string name;
        public long id;
        public RuntimeAnimatorController controller;

        public void InitTransform(Animator animator) {
            this.element = new TransformElement(animator);
        }

        public void ResetTransform() {
            element.GetTransform();
        }
    }

    public List<AnimatorComponet> staticLib;//참고용
    public List<AnimatorComponet> dynamicLib;//저장용
    
    public AnimatorManager() {
        _instance = this;
        staticLib = new List<AnimatorComponet>();
        dynamicLib = new List<AnimatorComponet>();
    }

    public void SaveAnimator(long id, Animator animator) {
        AnimatorComponet component = new AnimatorComponet();
        component.id = id;
        component.controller = animator.runtimeAnimatorController;
        component.name = "";
        component.InitTransform(animator);
        this.dynamicLib.Add(component);
    }

    public void SaveAnimator(long id, Animator animator, string name)
    {
        AnimatorComponet component = new AnimatorComponet();
        component.id = id;
        component.controller = animator.runtimeAnimatorController;
        component.name = name;
        component.InitTransform(animator);
        this.dynamicLib.Add(component);
    }

    public void ChangeAnimatorWithSave(long id, Animator animator, string target) {
        AnimatorComponet changer = this.GetStaticAnimator(target);
        SaveAnimator(id, animator);
        animator.runtimeAnimatorController = changer.controller;
    }

    public void ChangeAnimatorWithSave(long id, Animator animator, string target ,string name)
    {
        AnimatorComponet changer = this.GetStaticAnimator(target);
        SaveAnimator(id, animator, name);
        animator.runtimeAnimatorController = changer.controller;
    }

    /// <summary>
    /// change animator
    /// </summary>
    /// <param name="targetID">target animator which will change</param>
    /// <param name="animator">changed animator</param>
    /// <param name="isStatic">get animator controller from static references or saved animators</param>
    /// <param name="isSaved">represent this animator will saved for after</param>
    public void ChangeAnimatorByID(long id,long targetID, Animator animator, bool isStatic, bool isSaved) {
        AnimatorComponet target = null;
        if (isStatic)
        {
             target = GetStaticAnimator(targetID);
           
        }
        else {
             target = GetSavedAnimator(targetID,false);
        }
        if (target == null)
        {
            Debug.Log("Error");
            return;
        }
        if (isSaved)
        {
            SaveAnimator(id, animator);
        }
        animator.runtimeAnimatorController = target.controller;
    }

    public void ChangeAnimatorByName(long id, string targetStr, Animator animator, bool isStatic, bool isSaved) {
        AnimatorComponet target = null;
        if (isStatic)
        {
            target = GetStaticAnimator(targetStr);
        }
        else
        {
            target = GetSavedAnimator(targetStr,false);
        }
        if (target == null)
        {
            Debug.Log("Error");
            return;
        }
        if (isSaved)
        {
            SaveAnimator(id, animator);
        }
        animator.runtimeAnimatorController = target.controller;
    }

    public AnimatorComponet GetSavedAnimator(long id, bool isDeleted) {
        AnimatorComponet output = null;

        foreach (AnimatorComponet c in this.dynamicLib) {
            if (c.id == id) {
                output = c;
                output.ResetTransform();
                if (isDeleted) {
                    dynamicLib.Remove(c);
                }
                break;
            }
        }
        return output;
    }

    public AnimatorComponet GetSavedAnimator(string name, bool isDeleted)
    {
        AnimatorComponet output = null;

        foreach (AnimatorComponet c in this.dynamicLib)
        {
            if (c.name == name) 
            {
                output = c;
                output.ResetTransform();
                if (isDeleted)
                {
                    dynamicLib.Remove(c);
                }
                break;
            }
        }
        return output;
    }

    public AnimatorComponet GetStaticAnimator(long id) {
        AnimatorComponet output = null;

        foreach (AnimatorComponet c in this.staticLib)
        {
            if (c.id == id)
            {
                output = c;
                break;
            }
        }
        return output;
    }

    public AnimatorComponet GetStaticAnimator(string name)
    {
        AnimatorComponet output = null;

        foreach (AnimatorComponet c in this.staticLib)
        {
            if (c.name == name)
            {
                output = c;
                
                break;
            }
        }
        return output;
    }

	public void ResetAnimatorTransform(long id)
	{
		AnimatorComponet output = null;

		foreach (AnimatorComponet c in this.dynamicLib) {
			if (c.id == id) {
				output = c;
				output.ResetTransform();
				break;
			}
		}
	}
}