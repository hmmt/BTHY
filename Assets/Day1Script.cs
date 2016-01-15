using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class Day1Script : MonoBehaviour {
    private static Day1Script _instance;
    public static Day1Script instance {
        get { return _instance; }
    }
    public ConversationScript script;
    public class DAY1
    {
        public string[] desc0 ={
                        "안녕하세요 ‘관리자’ 님, 저는 당신의 원활한 관리작업을 위해 도움을 드릴 AI 앤젤라 입니다.",
                        "저는 오늘 회사에 줄무늬 넥타이를 매고 온 직원들이 몇 명인지 3초 만에 알려드릴 수 있어요.",
                        "그만큼 유능하고 빠른 계산을 할 수 있다는 뜻입니다.",
                        "비록 관리자님이 출근을 할 때마다 커피를 타오거나 자리를 정리할 수는 없겠지만 뭐, 당신이 환상체에게 물어뜯기는 걸 구경만 하는 것보단 낫지 않겠어요?",
                        "이쯤에서 제 소개는 마치도록 하죠.",
                        "조금 거만해보일 수도 있겠지만 놀랍게도, 모두 사실이에요.",
                        "질문 있나요?"
                    };

        public string[] select1 = { 
                            "1.	회사에 대해 조금 더 설명해 줄래요?",
                            "2.	당신에 대해 조금 더 설명해 줄래요?"
                       };

        public string[] desc1 = { 
                        "좋은 질문입니다.",
                        "세상에는 많은 종류의 에너지가 존재하죠.",
                        "하지만 우리가 에너지를 물 마시듯이 쓰는 것처럼 에너지도 흘러 넘치도록 존재할까요?",
                        "친환경 에너지를 만들기 위해 인류는 아주 오래 전부터 고민을 거듭해왔습니다.",
                        "태양열이나 풍력, 수력이라던지 다양한 종류가 있겠지만 쏟아붓는 돈에 비해 실용성은 쥐꼬리만큼만 있을 뿐이죠.",
                        "우리 에너지는, 엄밀히 말하면 바이오에너지 겠지만, 우리는 시시하게 풀이나 동물의 배설물 따위로 에너지를 추출하지는 않아요.",
                        "‘환상체’ 라는 유기체들로부터 ‘특별한 방법’ 을 통해 에너지를 얻는 것이죠."
                     };

        public string system1 = "<System> ‘특별한 방법’ 키워드 획득";

        public string[] desc2 = { 
                        "음, 이건 그다지 중요한 질문은 아니었던 것 같아요.",
                        "하지만 대답은 해드리죠.",
                        "저는 수 천가지의 알고리즘으로 만들어진 AI입니다.",
                        "그 중에는 관리자님을 보조하기 위한 기능도 백 여개 정도 내장되어 있어요.",
                        "저는 당신의 말동무도 될 수도 있고 든든한 ‘파트너’ 도 될 수 있습니다.",
                        "녹음된 가짜 목소리로 ‘내일도 내일의 태양이 떠오를 거에요’ 의 말을 지껄이는 기계들과 저는 분명히 달라요.",
                        "저는 감정을 표현할 수 있습니다."
                     };

        public string[] select2 = {
                            "1.	감정을 표현한다니요?",
                            "2.	파트너라니요?"
                       };

        public string[] desc3 = { 
                            "말 그대로에요.",
                            "저에겐 감정 표현에 대한 알고리즘이 존재합니다.",
                            "위급한 일이 생기면 ‘긴박한 어조’를 사용해서 경고를 표현할 것이고 에너지 생산율이 높다면 ‘기쁜 어조’를 활용해 칭찬을 할 것입니다.",
                            "사실, 감정을 흉내낸다는 것과 실제로 감정을 표현하는 것은 별 차이가 없어요.",
                            "연극이나 영화를 보고 울어본 경험이 있다면 제 말을 이해할 수 있겠지요."
                       };

        public string[] select3 = {
                            "1.	감정 흉내와 실제 표현은 다른 게 아닌가요?",
                            "2.	정말 대단한 알고리즘이군요."
                      };

        public string[] desc4 = { 
                            "오 당신이 남들보다 더 감정적인 사람이라는 것을 미리 눈치챘어야 했는데.",
                            "그거 아세요?",
                            "로보토미는 사실 수백 장쯤 되는 매뉴얼 ‘사전’ 을 당신에게 던져놓기만 해도 되는 것이었어요.",
                            "하지만 대신에 저라는 AI를 붙였죠.",
                            "AI라고 부르기에는 AI라는 단어가 너무나 아까운 저라는 존재요.",
                            "다시 한 번 물어볼게요.",
                            "그럼 당신은 안 좋은 일이 닥쳤을 때 알람시계처럼 ‘위급사항입니다’ 를 반복적으로 말하며 사이렌 소리를 흉내내는 걸 원하나요",
                            "아니면 침착하게 당황해 하는 당신을 일단 위로한 후 해결책을 차근차근 알려주길 원하는 것을 원하나요.",
                            "아, 대답 듣는 건 생략할게요.",
                            "너무 뻔하잖아요."
                         };
        public string system2 = "<System> 키워드 ‘AI’ 획득";

        public string[] desc5 = {
                            "바로 이 부분에서 로보토미가 당신을 관리자로 채용한 이유 중 한 가지를 알 수 있겠군요.",
                            "아마 저는 최선을 다해 당신을 도와드릴 수 있을 것 같네요"
                        };

        public string[] desc6 = {  
                             "당신은 저에게 단순히 비서를 바랄 수도 있겠지만 그것보단 조금 더 고차원적이고 소중한 개념이겠죠.",
                             "이 공간은 때때로 관리자님에게 있어서 공허함과 외로움을 느끼게 할거에요.",
                             "당신은 모든 걸 혼자 결정하고 그 결과를 감당해야 할 것이며 그 사실을 견디기 힘들 때도 있을거에요.",
                             "만약 그때 제가 있다면 당신은 한결 나아지리라는 것을 약속하죠."
                         };
    }
    DAY1 day = new DAY1();
    public Queue<string> queue = new Queue<string>();
    List<string> select = new List<string>();
    string systemmessage;
    int state = 0;
    int current = 0;
    public bool selected = false;
    string endMessage = "대화 종료";
    public AudioSource src;
    public AudioSource bg;
    

    void Awake() {
        _instance = this;   
    }

	// Use this for initialization
	void Start () {
        bg.Play();
        foreach (string s in day.desc0) {
            queue.Enqueue(s);
            Debug.Log("queue count : " + queue.Count);
        }
        foreach (string s in queue) {
            Debug.Log(s);
        }
        foreach (string s in day.select1) {
            select.Add(s);
        }
        systemmessage = "";
        OnClick();
        script.ToUpper();
	}

   /* void Change() {
        
    }*/

    void selectChange() { 
        
    }

    void End() {
        if (systemmessage != "") {
            script.SysMake(systemmessage);
        }
        script.SysMake(endMessage);
    }

    string[] Change() {
        switch (state) { 
            case 1:
                return day.desc1;
            case 2:
                return day.desc2;
            case 3:
                return day.desc3;
            case 4:
                return day.desc4;
            case 5:
                return day.desc5;
            case 6:
                return day.desc6;
        }
       return null;
    }

    public void OnClick() {
        if (queue.Count ==0)
        {
            if (!selected)
            {
                selected = true;
                src.PlayOneShot(src.clip);
                switch (state)
                {
                    case 1:
                    case 4:
                    case 5:
                    case 6:
                        End();
                        return;
                    default :
                        break;
                }

                script.SelectMake(select.ToArray());
                return;
            }
            else {
                return;
            }
            //script.ClearSelect();
        }
        string str = queue.Dequeue();
        script.LeftMake(str);
        src.PlayOneShot(src.clip);
    }

    public void OnSelect(int i) {
        //Debug.Log("selected : "+i);
        string str = select[i-1];
        string sub = str.Substring(3);
        src.PlayOneShot(src.clip);
        script.RightMake(sub);
        script.ClearSelect();
        select.Clear();
        selected = false;
        switch (state) { 
            case 0://default
                switch (i) { 
                    case 1:
                        state = 1;
                        //nonselect, system message, end
                        systemmessage = day.system1;
                        break;
                    case 2:
                        state = 2;
                        //select 2 make
                        foreach (string s in day.select2) {
                            select.Add(s);
                        }
                        break;
                }
                break;
            case 2:
                switch (i) { 
                    case 1:
                        state = 3;
                        //select 3 make
                        foreach (string s in day.select3) {
                            select.Add(s);
                        }
                        break;
                    case 2:
                        state = 6;
                        //nonselect, end
                        break;
                }
                break;
            case 3:
                switch (i) { 
                    case 1:
                        state = 4;
                        //nonselect, system message, end
                        systemmessage = day.system2;
                        break;
                    case 2:
                        state = 5;
                        //nonselect, end
                        break;
                }
                break;
        }
        string[] temp = Change();
        foreach (string s in temp) {
            queue.Enqueue(s);
        }
    }

}