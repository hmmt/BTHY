using UnityEngine;
using System.Collections;

public class TimerCallback : MonoBehaviour {

    private float elapsedTime;
    private float goalTime = 0f;

    private Callback callback;

    void FixedUpdate()
    {
        elapsedTime += Time.deltaTime;

        if(goalTime <= elapsedTime)
        {
            callback();
            Destroy(gameObject);
        }
    }

    public static TimerCallback Create(float time, Callback callback)
    {
        GameObject obj = new GameObject();

        TimerCallback script = obj.AddComponent<TimerCallback>();
        script.goalTime = time;
        script.elapsedTime = 0;
        script.callback = callback;

        return script;
    }

    public static TimerCallback Create(float time, GameObject parent, Callback callback)
    {
        GameObject obj = new GameObject();

        TimerCallback script = obj.AddComponent<TimerCallback>();
        script.goalTime = time;
        script.elapsedTime = 0;
        script.callback = callback;

        obj.transform.SetParent(parent.transform);

        return script;
    }

    public void ExpandTime(float value) {
        goalTime += value;
    }
}
