using UnityEngine;
using System.Collections;

/// <summary>
/// This Interface need ActivatableObjectPos and EventTriggerTarget
/// 
/// </summary>
public interface IActivatableObject
{

    /// <summary>
    /// When Window is opened
    /// 
    /// </summary>
    void Activate();

    /// <summary>
    /// When Window is closed
    /// </summary>
    void Deactivate();

    /// <summary>
    /// When Pointer is Entered
    /// </summary>
    void OnEnter();

    /// <summary>
    /// When Pointer is Exited
    /// </summary>
    void OnExit();

    /// <summary>
    /// This Method should contain adding eventTrigger function
    /// </summary>
    void UIActivateInit();

    /// <summary>
    /// When Close Message Arrived
    /// </summary>
    void Close();
}

/// <summary>
/// LeftLower will not be used
/// </summary>
public enum ActivatableObjectPos { 
    RIGHTUPPER,
    LEFTUPPER,
    RIGHTLOWER,
    LEFTLOWER,
    ISOLATE
}

public class UIActivateManager : MonoBehaviour {
    public IActivatableObject[] activated = new IActivatableObject[(int)ActivatableObjectPos.ISOLATE + 1];

    private static UIActivateManager _instance = null;
    public static UIActivateManager instance {
        get { return _instance; }
    }
    public Camera UICamera;
    /// <summary>
    /// mouse entered object
    /// </summary>
    IActivatableObject currentTarget = null;

    public void Awake() {
        _instance = this;
        
    }

    public void Activate(IActivatableObject target, ActivatableObjectPos pos)
    {
        this.activated[(int)pos] = target;
    }

    public void Deactivate(ActivatableObjectPos pos)
    {
        this.activated[(int)pos] = null;
    }

    public void OnEnter(IActivatableObject target) {
        this.currentTarget = target;
    }

    public void OnExit() {
        this.currentTarget = null;
    }

    public void OnClickBackGround() {
        if (currentTarget != null) {
            Debug.Log("Current Target is Exist");
            return;
        }

        foreach (IActivatableObject obj in this.activated)
        {
            if (obj == null) continue;
            obj.Close();
        }
    }

    public Camera GetCam() {
        return this.UICamera;
    }
    
}
