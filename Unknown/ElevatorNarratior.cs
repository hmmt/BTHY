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
                    narration[0].text = "아는 사람의 소개로 ‘에너지주식회사’ 라는,";
                    if (time >= 1.5)
                        narration[1].text = "꽤 커다란 기업의 ‘관리자’ 업무를 맡게 되었다.";
                    if (time >= 3)
                        narration[2].text = "지금 상황에선 새로운 시작이 될 기회였다. ";
                    if (time >= 3.5)
                        narration[3].text = "모든 게 낯설겠지만 차근차근 익숙해져 가면 될 일이다. ";
                    if (time >= 5)
                        narration[4].text = "그렇게 생각했다.      ";
                }

                else if (GlobalFunction.currentDay == 1)
                {
                    narration[0].text = "생각보다 ‘관리자’가 할 일은 그렇게 복잡하지도, 거창한 것도 아니었다. ";
                    if (time >= 1.5)
                        narration[1].text = "환상체라는 것을 관리하면 그것에서부터 에너지가 생산된다. ";
                    if (time >= 3)
                        narration[2].text = "이 간단한 공식만 기억하면 될 일이었다. ";
                    if (time >= 3.5)
                        narration[3].text = "모든 게 잘 될 것이다 라는 자신감이 들었다. ";
                }

                else if (GlobalFunction.currentDay == 2)
                {
                    narration[0].text = "이 회사에는 ‘직원’들이 많았다.";
                    if (time >= 1.5)
                        narration[1].text = "관리자의 역할은 환상체 뿐만 아니라 직원들 까지 원활히 관리하는 것에 있다고 앤젤라가 말 한 적이 있었다.";
                    if (time >= 3)
                        narration[2].text = "하지만 그들을 감시하는 것 이외에는 딱히 할 수 있는 일이 없었다. ";
                    if (time >= 3.5)
                        narration[3].text = "심지어 출근할 때마다 말을 거는 앤젤라 와도 대화는 할 수 없었다.  ";
                    if (time >= 5)
                        narration[4].text = "관리자는 꽤 고독한 직업인 것 같았다. ";

                }
                else if (GlobalFunction.currentDay == 3)
                {
                    narration[0].text = "일정 에너지 이상을 모으게 되면";
                    if (time >= 1.5)
                        narration[1].text = " 더 많은 부서들을 관리할 수 있다고 했다.  ";
                    if (time >= 3)
                        narration[2].text = "그렇게 되면 새로운 직원들과  ";
                    if (time >= 3.5)
                        narration[3].text = " 관리자로서 할 수 있는 기능이 늘어날 것이다.";
                }
                else if (GlobalFunction.currentDay == 4)
                {
                    narration[0].text = "생각보다 ‘관리자’가 할 일은 그렇게 복잡하지도, 거창한 것도 아니었다. ";
                    if (time >= 1.5)
                        narration[1].text = "환상체라는 것을 관리하면 그것에서부터 에너지가 생산된다. ";
                    if (time >= 3)
                        narration[2].text = "이 간단한 공식만 기억하면 될 일이었다. ";
                    if (time >= 3.5)
                        narration[3].text = "모든 게 잘 될 것이다 라는 자신감이 들었다. ";
                }
                else if (GlobalFunction.currentDay == 5)
                {
                    narration[0].text = "관리작업을 시작하러 엘리베이터에 타는 것이 이토록 익숙해지게 될 줄은 몰랐었다. ";
                    if (time >= 1.5)
                        narration[1].text = "그리고 문득 궁금증이 들었다. ";
                    if (time >= 3)
                        narration[2].text = "이 직원들과 나는, 별반 뚜렷한 차이점은 없어 보였다. ";
                    if (time >= 3.5)
                        narration[3].text = "나는 딱히 대단한 능력이 있었던 것도 아니고 바깥에서 흔히 볼 수 있었던, 그저 그런 놈 중 한 명이었다.";
                    if (time >= 5)
                        narration[4].text = "아무 연고도 없이 나를 관리자 라는 중요한 위치에 앉힌 건 어떤 연유에서였을까. ";
                }
                else if (GlobalFunction.currentDay == 6)
                {
                    narration[0].text = "엘리베이터는 덜컹거림 없이 부드럽게 움직이며 내려간다. ";
                    if (time >= 1.5)
                        narration[1].text = "거울이라도 보며 옷매무새를 매만질까 하다가도 언제나 관둔다. ";
                    if (time >= 3)
                        narration[2].text = "사람 한 명 탈만한 딱 그 정도의 엘리베이터에는";
                    if (time >= 3.5)
                        narration[3].text = "그 흔한 거울조차 없었다.  ";
                 }
                else if (GlobalFunction.currentDay == 7)
                {
                    narration[0].text = "생각보다 순조롭게 7일이 흘러갔다. ";
                    if (time >= 1.5)
                        narration[1].text = "거북스러울 만큼 친절했던 엔젤라의 목소리는 ";
                    if (time >= 3)
                        narration[2].text = "이제 하루의 일과를 시작하기 위해 늘 듣는 알람소리 마냥 편했다.";
                    if (time >= 3.5)
                        narration[3].text = "몇일 전 들었던 께름측한 위화감 조차 착각일 거라 생각될 정도로..";
                 }
                else if (GlobalFunction.currentDay ==8)
                {
                    narration[0].text = "관리 근무 중에 몇몇 직원들과 대화를 해 본적이 있었다.";
                    if (time >= 1.5)
                        narration[1].text = "그리고 문득 궁금증이 들었다. ";
                    if (time >= 3)
                        narration[2].text = "그들 중 몇몇은 경계하는 듯 하면서도";
                    if (time >= 3.5)
                        narration[3].text = "관리자 라는 내 직위를 인식했는지 대놓고 꺼려하지는 않았다.";
                    if (time >= 5)
                        narration[4].text = "그래도 말을 거니 조금은 분위기가 활기차진 것 같았다. ";
                }
                else if (GlobalFunction.currentDay == 9)
                {
                    narration[0].text = "푹 쉬었다고 생각했는데도 피곤함이 가시지 않았다. ";
                    if (time >= 1.5)
                        narration[1].text = "";
                    if (time >= 3)
                        narration[2].text = " ";
                    if (time >= 3.5)
                        narration[3].text = "언젠가부터 첫 날 입사했을 때의 긴장감은 무디어져 있었다. ";
                 }
                else if (GlobalFunction.currentDay == 10)
                {
                    narration[0].text = "꿈자리가 뒤숭숭해 잠을 조금 설친 듯 피곤했다.";
                    if (time >= 1.5)
                        narration[1].text = "괜히 머리를 대 여섯번 매만졌을 즈음에야";
                    if (time >= 3)
                        narration[2].text = "그 심심했던 엘리베이터에 ";
                    if (time >= 3.5)
                        narration[3].text = "울이 붙어 있음을 깨달았다.  ";
                }
                else if (GlobalFunction.currentDay == 11)
                {
                    narration[0].text = "이 회사에 취직하기 전에";
                    if (time >= 1.5)
                        narration[1].text = "나는 한창 뜻 모를 매너리즘에 빠져 있었다.";
                    if (time >= 3)
                        narration[2].text = "그리고 관리자로서의 입사가  ";
                    if (time >= 3.5)
                        narration[3].text = "나한테 어떤 계기가 되어줄 거라고 생각하고 있었다. ";
                    if (time >= 5)
                        narration[4].text = "나도 모르게 그렇게 믿었다 보다.  ";
                }

                else if (GlobalFunction.currentDay == 12)
                {
                    narration[0].text = "회사에 입사한 지 십여일 정도 밖에 지나지 않았다.";
                    if (time >= 1.5)
                        narration[1].text = "길지 않았던 그 시간 동안 어느새 정도 들었다. ";
                    if (time >= 3)
                        narration[2].text = "지금처럼만 일을 해서 에너지를 생산해 내기만 한다면. ";
                    if (time >= 3.5)
                        narration[3].text = "그런 날들이 반복된다면.";
                }

                else if (GlobalFunction.currentDay == 13)
                {
                    narration[0].text = "한 때, 종종 스스로를 버리고 싶다고 생각했던 때가 있었다. ";
                    if (time >= 1.5)
                        narration[1].text = "무의식 중에 남겨진 ";
                    if (time >= 3)
                        narration[2].text = "자기혐오와 낮아진 자존감을 품고  ";
                    if (time >= 3.5)
                        narration[3].text = "살아오던 때가 있었다. ";
                }

                else if (GlobalFunction.currentDay == 14)
                {
                    narration[0].text = "그러고 보니 오늘 날씨는 어땠던가.";
                    if (time >= 1.5)
                        narration[1].text = "제대로 햇빛을 마주 본지가 아주 오래 전 일만 같았다.";
                    if (time >= 3)
                        narration[2].text = "햇빛 없는 지하실 아래 ";
                    if (time >= 3.5)
                        narration[3].text = "오늘도 회사는 돌아간다. ";
                }

                else if (GlobalFunction.currentDay == 15)
                {
                    narration[0].text = "엘리베이터가 조금씩 덜컹거린 다는 걸 알았다. ";
                    if (time >= 1.5)
                        narration[1].text = "분명 몇 일 전까지만 해도 안정적으로 작동했었다. ";
                    if (time >= 3)
                        narration[2].text = "직원들에게 알리려다가 ";
                    if (time >= 3.5)
                        narration[3].text = "곧 그만두었다. ";
                }

                else if (GlobalFunction.currentDay == 16)
                {
                    narration[0].text = "엘리베이터의 흔들림이 어제보다 심해졌다. ";
                    if (time >= 1.5)
                        narration[1].text = "";
                    if (time >= 3)
                        narration[2].text = "";
                    if (time >= 3.5)
                        narration[3].text = "아무 것도 붙잡지 않고 있다가 그대로 균형을 잃을 뻔 했다. ";
                }

                else if (GlobalFunction.currentDay == 17)
                {
                    narration[0].text = "엘리베이터의 흔들림은 여전했다. ";
                    if (time >= 1.5)
                        narration[1].text = "흔한 비상벨 버튼 조차 없는 엘리베이터엔";
                    if (time >= 3)
                        narration[2].text = "거울만이 위태 위태하게 붙어 있었다. ";
                    if (time >= 3.5)
                        narration[3].text = "";
                    if (time >= 5)
                        narration[4].text = "멀미가 날 것 같았다. ";
                }
                else if (GlobalFunction.currentDay == 18)
                {
                    narration[0].text = "불안하게 움직이던 엘리베이터가 결국 멈췄다.";
                    if (time >= 1.5)
                        narration[1].text = "불도 들어오지 않고 별 다른 충격 없이 계속해서 멈춰 있었다.";
                    if (time >= 3)
                        narration[2].text = "그 익숙한 앤젤라의 인사말로 들리지 않았다. ";
                    if (time >= 3.5)
                        narration[3].text = "그리고 한참 후에야";
                    if (time >= 5)
                        narration[4].text = " 상황을 알아보려고 온 직원들에 의해 겨우 정상작동이 되었다.  ";
                }


            }

            else
            {
                if (GlobalFunction.currentDay == 0)
                {
                    narration[0].text = "환상체 라는 괴 생물체를 보았다.";
                    if (time >= 1.5)
                        narration[1].text = "어떤 건 징그럽게 생겼고";
                    if (time >= 3)
                        narration[2].text = "어떤 건 그런대로 견딜 수 있었다. ";
                    if (time >= 3.5)
                        narration[3].text = "첫 날이라 피곤함을 느낄 새도 없이 정신 없이 흘러갔다.";
                    if (time >= 4.5)
                        narration[4].text = "에너지를 생성하는 데 어떻게든 성공한 것 같았다. ";
                }

                else  if (GlobalFunction.currentDay == 1)
                {
                    narration[0].text = "둘째 날도 마무리 됐다";
                    if (time >= 1.5)
                        narration[1].text = "여전히 헤매긴 했지만";
                    if (time >= 3)
                        narration[2].text = " 차차 익숙해져 가고 있었다. ";
                    if (time >= 3.5)
                        narration[3].text = "환상체 라는 낯선 것과 조우한 후의";
                    if (time >= 4.5)
                        narration[4].text = "그 당혹스러움도 나아져 가고 있었다. ";
                }

                else if (GlobalFunction.currentDay == 2)
                {
                    narration[0].text = "처음으로 직원들과 대화를 해보았다. ";
                    if (time >= 1.5)
                        narration[1].text = "";
                    if (time >= 3)
                        narration[2].text = "";
                    if (time >= 3.5)
                        narration[3].text = "그들과 좀 더 친해져도 나쁘지 않을 것 같았다. ";
                }

                else if (GlobalFunction.currentDay == 3)
                {
                    narration[0].text = "관리자가 된지도 사흘이 지났다.";
                    if (time >= 1.5)
                        narration[1].text = "관리를 할 수 있을지 염려되었던 것과는 달리";
                    if (time >= 3)
                        narration[2].text = "엔젤라는 ‘곧 잘 한다’ 라고 해주었다. ";
                    if (time >= 3.5)
                        narration[3].text = "누군가에게 제대로 된 칭찬을 듣는 건 오랜만이어서";
                    if (time >= 4.5)
                        narration[4].text = " 잠시동안 묘한 기분이었다.";
                }

                else if (GlobalFunction.currentDay == 4)
                {
                    narration[0].text = "오늘까지 회사에 근무한지 5일 째 되는 날이었다.";
                    if (time >= 1.5)
                        narration[1].text = "근무를 하면서 몇 가지의 위화감이 들었다. ";
                    if (time >= 3)
                        narration[2].text = "어떤 것에 대한 위화감인지는 아직 잘 모르겠다. ";
                    if (time >= 3.5)
                        narration[3].text = "착각일 수도 있었겠지만 ";
                    if (time >= 4.5)
                        narration[4].text = "혹시 몰라 일기장에 기록을 한다. ";
                }

                else if (GlobalFunction.currentDay == 5)
                {
                    narration[0].text = "환상체들의 수는 점점 더 늘어날 것이다. ";
                    if (time >= 1.5)
                        narration[1].text = "그건 곧 에너지의 생산량도 늘어나는 의미이기도 했다.";
                    if (time >= 3)
                        narration[2].text = "";
                    if (time >= 3.5)
                        narration[3].text = "점점 한 사람의 몫을 온전히 다해가는 것 같았다. ";
                }

                else if (GlobalFunction.currentDay == 6)
                {
                    narration[0].text = "오늘도 무사히 작업이 끝났다. ";
                    if (time >= 1.5)
                        narration[1].text = "";
                    if (time >= 3)
                        narration[2].text = "";
                    if (time >= 3.5)
                        narration[3].text = "일주일은 생각보다 빠르게 지나갔던 것 같다. ";
                }

                else if (GlobalFunction.currentDay == 7)
                {
                    narration[0].text = "다른 날과 다르게 조금 피곤했다. ";
                    if (time >= 1.5)
                        narration[1].text = "";
                    if (time >= 3)
                        narration[2].text = "";
                    if (time >= 3.5)
                        narration[3].text = "집에 빨리 가고 싶어 발걸음을 재촉했다.";
                }

                else if (GlobalFunction.currentDay ==8)
                {
                    narration[0].text = "퇴근을 해도 좋다는 앤젤라의 목소리가";
                    if (time >= 1.5)
                        narration[1].text = "유독 반갑게 들렸다.";
                    if (time >= 3)
                        narration[2].text = "초심과는 멀어진 것 같아,";
                    if (time >= 3.5)
                        narration[3].text = "그녀는 단지 방송 목소리임에도 불구하고";
                    if (time >= 5)
                        narration[4].text = "살짝 죄책감이 생겼다. ";
                }

                else if (GlobalFunction.currentDay == 9)
                {
                    narration[0].text = "다시 거울을 보는데";
                    if (time >= 1.5)
                        narration[1].text = "스스로가 너무 지쳐 보여서 조금 놀랐다.";
                    if (time >= 3)
                        narration[2].text = "다크서클 같은 것이 ";
                    if (time >= 3.5)
                        narration[3].text = "턱까지 내려올 기세였던 것이다.";
                }

                else if (GlobalFunction.currentDay == 10)
                {
                    narration[0].text = "";
                    if (time >= 1.5)
                        narration[1].text = "";
                    if (time >= 3)
                        narration[2].text = "";
                    if (time >= 3.5)
                        narration[3].text = "하루가 마무리 됐다. ";
                }

                else if (GlobalFunction.currentDay == 11)
                {
                    narration[0].text = "퇴근시간에 맞춰 직원들이 움직인다.";
                    if (time >= 1.5)
                        narration[1].text = "몇몇 들은 뒤를 돌며 인사를 건네기도 한다. ";
                    if (time >= 3)
                        narration[2].text = "웃으면서 ‘내일 보자’고 인사에 답했다. ";
                }

                else if (GlobalFunction.currentDay ==12)
                {
                    narration[0].text = "";
                    if (time >= 1.5)
                        narration[1].text = "오늘치의 일을 다 마쳤다.  ";
                }

                else if (GlobalFunction.currentDay == 13)
                {
                    narration[0].text = "";
                    if (time >= 1.5)
                        narration[1].text = "늘 그렇듯이 하루가 무사히 마무리 되었다. ";
                }

                else if (GlobalFunction.currentDay == 14)
                {
                    narration[0].text = "";
                    if (time >= 1.5)
                        narration[1].text = "오늘도 작업이 무사히 끝났다. ";
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
