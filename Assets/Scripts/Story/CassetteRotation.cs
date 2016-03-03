using UnityEngine;
using System.Collections;

public class CassetteRotation : MonoBehaviour {

    public float speed;
    public bool clockWise;

    private RectTransform rect;
    private Vector3 axis;
    private float realSpeed;
    public bool rotate = false;

    public void Start() {
        rect = this.GetComponent<RectTransform>();
        axis = new Vector3(0f, 0f, 1f);
        if (clockWise)
        {
            realSpeed = speed * 0.05f;
        }
        else {
            realSpeed = speed * -0.05f;
        }

        StartCoroutine(Acceleration());
    }

    public IEnumerator Acceleration() {
        int cnt = 0;
        while (cnt < 100) {
            rect.Rotate(axis, realSpeed * 0.01f* cnt);
            cnt++;
            yield return new WaitForSeconds(1 / 50f);
        }
        rotate = true;
    }

    public void Update() {
        if (rotate)
        {
            rect.Rotate(axis, realSpeed);
        }
    }

}
