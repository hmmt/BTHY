using UnityEngine;
using System.Collections;

public class TimerCallback : MonoBehaviour {

    private float elapsedTime;
    private float goalTime;

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

    public static void Create(float time, Callback callback)
    {
        GameObject obj = new GameObject();

        TimerCallback script = obj.AddComponent<TimerCallback>();
        script.goalTime = time;
        script.elapsedTime = 0;
        script.callback = callback;
    }
}
