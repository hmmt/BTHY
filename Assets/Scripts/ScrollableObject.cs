using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScrollableObject :MonoBehaviour, IScrollTarget{

    private EventTrigger eventTrigger;

    public void Awake() {
        AddTrigger();
    }

    public void Regist()
    {
        CameraMover.instance.Registration(this);
    }

    public void DeRegist()
    {
        CameraMover.instance.DeRegistration();
    }

    public void AddTrigger()
    {
        eventTrigger = this.GetComponent<EventTrigger>();
        if (eventTrigger == null) {
            this.eventTrigger = this.gameObject.AddComponent<EventTrigger>();
        }

        EventTrigger.Entry enter = new EventTrigger.Entry();
        EventTrigger.Entry exit = new EventTrigger.Entry();

        enter.eventID = EventTriggerType.PointerEnter;
        exit.eventID = EventTriggerType.PointerExit;

        enter.callback.AddListener((eventData) => { Regist(); });
        exit.callback.AddListener((eventData) => { DeRegist(); });

        this.eventTrigger.triggers.Add(enter);
        this.eventTrigger.triggers.Add(exit);
    }
}