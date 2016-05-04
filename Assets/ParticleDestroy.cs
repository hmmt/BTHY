using UnityEngine;
using System.Collections;

public class ParticleDestroy : MonoBehaviour {
    float elapsed = 0f;
    float total = 0f;
    bool clockStarted = false;

    public void DestroyNow() {
        Destroy(this.gameObject);
    }

    public void DelayedDestroy(float time) {
        SetClock(time);
    }

    public void FixedUpdate() {
        if (clockStarted) {
            elapsed += Time.deltaTime;
            if (elapsed > total) {
                this.DestroyNow();
            }
        }
    }

    public void SetClock(float time) {
        this.clockStarted = true;
        this.total = time;
        this.elapsed = 0f;
    }


    
}
