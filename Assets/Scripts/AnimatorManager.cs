﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class AnimatorName {
    public static string AgentCtrl = "AgentCtrl";
    public static string OfficerCtrl = "AgentCtrl";
    public static string RedShoes = "RedShoes";

    public static long id_AgentCtrl = 50001;
    public static long id_OfficerCtrl = 50002;
    public static long id_RedShoes = 50003;
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
        public string name;
        public long id;
        public RuntimeAnimatorController controller;
        
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
        this.dynamicLib.Add(component);
    }

    public void SaveAnimator(long id, Animator animator, string name)
    {
        AnimatorComponet component = new AnimatorComponet();
        component.id = id;
        component.controller = animator.runtimeAnimatorController;
        component.name = name;
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
}