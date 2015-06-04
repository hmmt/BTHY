using UnityEngine;
using System.Collections;

public class ElevatorNarratior : MonoBehaviour {

    public ElevatorMover elevator;

    public bool checkTime=false;
    public static float time = 0.0f;

    public TextMesh[] narration;

    float textTime = 0;

    public AudioSource citySound;

	// Use this for initialization
	void Start () {

        time = 0.0f;
        citySound.time = 12f;
	
	}

    // Update is called once per frame
	void FixedUpdate () {

        if (elevator.isStop)
        {
            time += Time.deltaTime;
            if (!GlobalFunction.finishWork)
            {
                if (GlobalFunction.currentDay == 0)
                {
                    narration[0].text = "한동안 나는 끔찍한 매너리즘에 빠져 있었다.";
                    if (time >= 1.5)
                        narration[1].text = "간간히 암전도 생기곤 했다.";
                    if (time >= 3)
                        narration[2].text = "잘만 돌아가고 있는 테이프가 어느 순간 음악을 멈춘 것처럼,";
                    if (time >= 3.5)
                        narration[3].text = "그냥 정신이 까맣게 되곤 하는 것이다.";
                }

                else if (GlobalFunction.currentDay == 1)
                {
                    narration[0].text = "그 즈음 누군가 내게 한 회사를 소개시켜줬다. ";
                    if (time >= 1.5)
                        narration[1].text = "에너지를 생산하는 회사라는 데 잘 알려진 정도는 아니었고";
                    if (time >= 3)
                        narration[2].text = "딱히 특별한 기술도 필요한 직업도 아니여서 해보겠다고 대답했다.";
                    if (time >= 3.5)
                        narration[3].text = "";
                }

                else if (GlobalFunction.currentDay == 2)
                    narration[0].text = "그 상태가 몇 분, 몇 시간이 지속되는지는 스스로도 몰랐다. ";

                else if (GlobalFunction.currentDay == 3)
                    narration[0].text = "하고 싶은 것도 없었고 해야만 한다고 생각하는 것도 없었다.";

                else if (GlobalFunction.currentDay == 4)
                    narration[0].text = "그 즈음 누군가 내게 한 회사를 소개시켜줬다.";

                else if (GlobalFunction.currentDay == 5)
                    narration[0].text = "오늘은 첫 출근~";
            }

            else
            {
                if (GlobalFunction.currentDay == 0)
                {
                    narration[0].text = "퇴근";
                    if (time >= 1.5)
                        narration[1].text = "";
                    if (time >= 3)
                        narration[2].text = "";
                    if (time >= 3.5)
                        narration[3].text = "";
                }
            }


            if (time >= 5)
            {
                elevator.isStop = false;
                for (int i = 0; i < narration.Length; i++ )
                    narration[i].text = "";
                citySound.Stop();
            }
        }
	
	}

    public void stopElevator()
    {
           
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Player" )
        {
            elevator.isStop = true;
            citySound.Play();
        }
    }

    void OnTriggerExit2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Player")
        {
            
        }
    }
}
