using UnityEngine;
using System.Collections;

public class RotateAngle : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void FixedUpdate()
    {
        Vector3 flashPoint = transform.position;
        Vector3 mousePoint = Input.mousePosition; // 각 지점 저장

        mousePoint.z = flashPoint.z - Camera.main.transform.position.z; //z축의 좌표를 맞춘다.
        Vector3 newPoint = Camera.main.ScreenToWorldPoint(mousePoint); //스크린의 좌표를 게임 내 월드 좌표로 전환한다.

        float x = newPoint.x - flashPoint.x;
        float y = newPoint.y - flashPoint.y;

        float seta= Mathf.Atan2(y, x) * Mathf.Rad2Deg; // 아크탄젠트로 seta를 구하고, rad2deg로 라디안 값을 디그리 값으로 바꾼다.

     if (!GameObject.Find("Player").GetComponent<PlayerController>().facingRight)
        {
            seta *= -1;
        }

     Debug.Log(seta);
        //if (seta > -90 && seta < 90)
            transform.rotation = Quaternion.Euler(0f,0f,seta);

    }
}
