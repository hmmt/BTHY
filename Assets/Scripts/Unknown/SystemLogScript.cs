using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class SystemLogScript : MonoBehaviour, IObserver {
    public LoggingScript script;
    
    void OnEnable()
    {
        Notice.instance.Observe(NoticeName.AddSystemLog, this);
    }

    void OnDisable()
    {
        Notice.instance.Remove(NoticeName.AddSystemLog, this);
    }

    public void OnNotice(string notice, params object[] param)
    {
        if (NoticeName.AddSystemLog == notice) {
            script.MakeText("" + (string)param[0]);
        }
        script.Sort();
    }
}
