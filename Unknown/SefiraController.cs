using UnityEngine;
using System.Collections;

public class SefiraController : MonoBehaviour
{
    //SefiraObject들을 배치할 것
    //일단 10초에 한 번 확인하게 만듦
    float elapsed = 0;
    float wait = 10f;

    public void Start() { 
        
    }

    public void Update() {
        elapsed += Time.deltaTime;
        if (elapsed > wait) {
            elapsed = 0;
            CheckSefira();
        }
    }

    //Sefira 직원 배치 등의 상황을 통해 현재 개방 여부를 확인
    public void CheckSefira() {
        foreach (Sefira sefira in SefiraManager.instance.sefiraList) {
            if (!sefira.activated) continue;
            if (sefira.officerList.Count < 5) {
                Debug.Log(sefira.name + " 비활성화");
            }
        }
    }
}
