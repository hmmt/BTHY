using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum LyricType { 
    DAY1,
    DAY,
    DAYSMALL,
    CHAT,
    MENTALBAD,
    LEVELUP,
    ESCAPE,
    SAD
}

public class AgentLyrics {
    public class AgentLyric {
        public int id;
        public string desc;

        public AgentLyric( int id, string desc) {
            this.id = id;
            this.desc = desc;
        }

    }
    public class LyricList {
        public List<AgentLyric> list;

        public LyricType type;

        //default : Error

        public LyricList(LyricType type, AgentLyric[] ary) {
            this.type = type;
            this.list = new List<AgentLyric>(ary);
        }

        public LyricList(LyricType type, List<AgentLyric> list)
        {
            this.type = type;
            this.list = new List<AgentLyric>(list.ToArray());
        }

        public List<AgentLyric> GetAllLyrics() {
            return this.list;
        }

        public AgentLyric GetRandomLyric() {
            int randIndex = UnityEngine.Random.Range(0, list.Count);
            return list[randIndex];
        }

        public AgentLyric GetUniqueLyricById(int id) {
            AgentLyric output = null;
            foreach (AgentLyric item in list) {
                if (item.id.Equals(id)) {
                    output = item;
                    break;
                }
            }
            return output;
        }

        public AgentLyric GetUniqueLyricByIndex(int index)
        {
            AgentLyric output = null;
            if (index < 0 || index >= list.Count) {
                Debug.Log("Error");
                return null;
            }
            output = list[index];
            return output;
        }
    }

    public class CreatureReaction {
        public int level;
        public string desc;
    }

    public class CreatureReactionList {
        public long creatureId;
        public List<CreatureReaction> lib = new List<CreatureReaction>();

        public string GetDesc(int level) {
            CreatureReaction output = null;
            foreach (CreatureReaction cr in lib) {
                if (cr.level == level) {
                    output = cr;
                    break;
                }
            }
            if (output == null) return null;
            return output.desc;
        }
    }

    private static AgentLyrics _instance = null;

    public static AgentLyrics instance
    {
        get
        {
            if (_instance == null) {
                _instance = new AgentLyrics();

            }
            return _instance;
        }
    }

    public List<LyricList> list;
    public Dictionary<long, CreatureReactionList> creatureRecation;
   
    private bool isLoaded = false;

    public void Init(List<LyricList> inputList, Dictionary<long, CreatureReactionList> dic) {
        list = new List<LyricList>(inputList.ToArray());
        creatureRecation = dic;
        this.isLoaded = true;
    }

    public bool IsLoaded() {
        return isLoaded;
    }

    public string getLyricsByDay(int day) {
        string output = null;

        if (day == 1)
        {
            output = this.GetLyricByType(LyricType.DAY1).GetRandomLyric().desc;
        }
        else {
            int randVal = UnityEngine.Random.Range(0, 10);
            if (randVal <= 3)
                output = this.GetLyricByType(LyricType.DAYSMALL).GetRandomLyric().desc;
            else
                output = this.GetLyricByType(LyricType.DAY).GetRandomLyric().desc;
        }
        return output;
    }

    public CreatureReactionList GetCreatureReaction(long id) {
        CreatureReactionList output = null;
        if (creatureRecation.TryGetValue(id, out output)) {
            return output;
        }
        return null;
    }
    /*
    public string getLyricsByDay(int day)
    {
        int randFlag = 0;
        string lyrics = "";

        switch (day)
        {
            case 0:
                randFlag = Random.Range(1,10);

                if(randFlag == 1)
                {
                    lyrics = "오늘 새로운 관리자가 온다던데.";
                }

                else if(randFlag == 2)
                {
                    lyrics = "새로운 관리자가 왔다 하더라도 똑같이 머저리 겠지! ";
                }

                else if (randFlag == 3)
                {
                    lyrics = "아 일 하기 싫다. 나도 관리자나 돼서 너네들 감시나 해봤으면..";
                }

                else if (randFlag == 4)
                {
                    lyrics = "야 너네 그 소식 들었냐 관리자가 새로 왔대.";
                }

                else if (randFlag ==5)
                {
                    lyrics = "새로운 관리자가 오던 말던 내 월급이나 올라갔으면.";
                }

                else if (randFlag == 6)
                {
                    lyrics = "관리자는 연봉이 얼마일까.. 우리보단 높겠지?";
                }

                else if (randFlag == 7)
                {
                    lyrics = "누가 관리자 불러와서 신고식 같은거 했으면 좋겠다.";
                }

                else if (randFlag == 8)
                {
                    lyrics = "관리자가 되면 자리에 앉아서 버튼만 누르면 되는 거 아냐? ";
                }

                else if (randFlag == 9)
                {
                    lyrics = "나도 학창시절에 공부만 더 열심히 했으면 관리자나 해볼걸.";
                }

                else if (randFlag == 10)
                {
                    lyrics = "새로운 관리자님 얼굴 한번 보고 싶다.";
                }
            break;

            case 1 :
            randFlag = Random.Range(1, 30);

            if (randFlag == 1)
            {
                lyrics = "어이, 오늘 작업 끝나고 한 잔 콜?";
            }

            else if (randFlag == 2)
            {
                lyrics = "오늘은 빌어먹을 관리자 놈이 어떤 작업을 시킬까";
            }

            else if (randFlag == 3)
            {
                lyrics = "하마터면 오늘 인생 마감할 뻔했어. ";
            }

            else if (randFlag == 4)
            {
                lyrics = "관찰일지 쓰는 거 귀찮아 죽겠다.";
            }

            else if (randFlag == 5)
            {
                lyrics = "사내연애 하는 게 그렇게 좋지 않다면서?";
            }

            else if (randFlag == 6)
            {
                lyrics = "위험하고, 수상하다는 점만 빼면 우리 회사도 그렇게 나쁘진 않지. \n월급은 높은 편이니까.";
            }

            else if (randFlag == 7)
            {
                lyrics = "아까 점심시간 때 먹은 치킨이 이빨에 아직 껴 있는 것 같아!";
            }

            else if (randFlag == 8)
            {
                lyrics = "직원 휴게실이 너무 좁아. 쉬라는 건지 말라는 건지.";
            }

            else if (randFlag == 9)
            {
                lyrics = "그 직원 소식 들었어? 비상자재팀의.. 그 다리가 잘렸다던..";
            }
            else if (randFlag == 10)
            {
                lyrics = "이런 날씨에는 데이트나 해야 되는데 ";
            }

            else if (randFlag == 11)
            {
                lyrics = "생각 같아선 언제라도 회사 때려치고 싶다. ";
            }
            else if (randFlag == 12)
            {
                lyrics = "관리자만 1인 엘리베이터를 탄다는 소문이 사실일까? ";
            }

            else if (randFlag == 13)
            {
                lyrics = "그래도 새로 온 관리자 말이야, 아예 멍청이는 아닌 가봐. ";
            }
            else if (randFlag == 14)
            {
                lyrics = "생산량이 늘었다는 데 우리 월급이나 올려줬으면 좋겠다.";
            }
            else if (randFlag == 15)
            {
                lyrics = "그래도 열심히 하면 승진의 기회가 있지 않을까.   ";
            }
            else if (randFlag == 16)
            {
                lyrics = "그래도 회사 급식은 꽤 맛있는 편이지.";
            }
            else if (randFlag == 17)
            {
                lyrics = "요즘 스트레스 수치가 미친듯이 올라가는 것 같아. ";
            }
            else if (randFlag == 18)
            {
                lyrics = "우리 엄마는 아직도 내가 정확히 어떤 회사에서 일하는 지 모르셔.";
            }
            else if (randFlag == 19)
            {
                lyrics = "지휘감시팀이 그렇게 꿀을 빠는 부서라며? ";
            }
            else if (randFlag == 20)
            {
                lyrics = "갑자기 햄버거가 먹고 싶다.";
            }
            else if (randFlag == 21)
            {
                lyrics = "10분 지각한 거 가지고 상사가 엄청 혼내지 뭐야.\n 자기는 엊그제 20분 지각했으면서.";
            }
            else if (randFlag == 22)
            {
                lyrics = "우리 부서에 새로 들어온 신입사원, 군기가 빠졌어. \n커피 같은 건 알아서 타와야지.";
            }
            else if (randFlag == 23)
            {
                lyrics = "자재관리 부서만큼은 별로 들어가고 싶지 않은데. \n제일 고생한다고 들었거든..";
            }
            else if (randFlag == 24)
            {
                lyrics = "옛날에 상사가 항상 하던 말이 있었지. \n매뉴얼 대로만 따르면, 목숨에는 아무런 지장이 없을 거라고.";
            }
            else if (randFlag == 25)
            {
                lyrics = "우리 회사가 휴게실 만큼은 아주 근사해. ";
            }
            else if (randFlag == 26)
            {
                lyrics = "근데 이 환상체들 정말 안전한 거 맞아? \n아무리 봐도 그렇겐 보이지 않는단 말이지.";
            }
            else if (randFlag == 27)
            {
                lyrics = "꼭 흉악하게 생긴 환상체 만이 위험한 건 아니야. ";
            }
            else if (randFlag == 28)
            {
                lyrics = "바깥에서는 환상체 존재에 대해 거의 모르고 있어.\n 자칫 새어나갔다간 부서 최고 책임자들이 다 목이 날라가게 될 거야.";
            }

            else if (randFlag ==29)
            {
                lyrics = "관찰일기를 쓰는 것 만큼 귀찮은 일은 또 없을거야.";
            }
            else if (randFlag == 30)
            {
                lyrics = "베테랑 직원이라도 \n관리 직전엔 긴장을 감출 수는 없을 거야.";
            }
            break;

            case 2:


            break;

        }

       return lyrics;

    }
     */

    public string getStoryLyrics(){
        return getLyricsByDay(2);
    }

    /*
    public string getStoryLyrics()
    {
        string lyrics = "";
        int randFlag = Random.Range(1,16);

        if (randFlag == 1)
        {
            lyrics = "일기를 쓰는 건 생각보다 매우 의미 있는 행위야. \n너무 머리가 좋아서 세세한 모든 것들 것 다 기억할 수 있는게 아니라면 말이야.";
        }

        else if (randFlag == 2)
        {
            lyrics = "나라면 여유가 있을 때 일기 쓰는 버릇을 들이겠어.";
        }

        else if (randFlag == 3)
        {
            lyrics = "이 회사에 너무 커다란 신뢰를 갖지는 않는 게 좋을거야. ";
        }

        else if (randFlag == 4)
        {
            lyrics = "앤젤라 말이야, 맨날 우리한테 퇴근 시간이 됐다고 알리는 녹음 방송, 그거 일일히 다 녹음을 했다는 소리인가?";
        }

        else if (randFlag == 5)
        {
            lyrics = "환상체는 아주 오래 전부터 있었던 존재야. \n하지만 그것을 가둬놓기 시작한 건 그렇게 오래되진 않았을 거야.";
        }

        else if (randFlag == 6)
        {
            lyrics = "어릴 적 꿈은 이런 게 아니었지. \n난 좀 더 위대한 사람이 될 줄 알았어. \n알잖아, 우리 모두는 처음에는 엄청난 무언가를 상상하곤 했지.”";
        }

        else if (randFlag == 7)
        {
            lyrics = "힘들어도 뭐 별 수 있나. 참고 사는 거지.";
        }

        else if (randFlag == 8)
        {
            lyrics = "어쩔 수 없어. 뭔가 바꾸기엔 너무 늦은 시기야.";
        }
        else if (randFlag == 9)
        {
            lyrics = "혹시 집에 일기장이 있다면 일기 쓰는 습관을 게을리 하지 말라고. 하루 하루를 기록하는 건 생각보다 중요한 역할이잖아.";
        }
        else if (randFlag == 10)
        {
            lyrics = "어릴 때로 돌아가고 싶다는 생각을 가끔 하게 돼. 공상으로 그칠 뿐이지만.";
        }
        else if (randFlag == 11)
        {
            lyrics = "남들보다 대단한 사람이 될 수 있다고 생각했던 때가 있었지. 치기 어렸지만 그때만큼 열정 있었던 때도 또 없었어.";
        }
        else if (randFlag == 12)
        {
            lyrics = "만족스러운 인생을 살았다고 자부할 수 있는 사람이 몇 이나 될까?";
        }
        else if (randFlag == 13)
        {
            lyrics = "햄스터 챗바퀴 굴리는 것 같은 지겨운 반복 작업! 분명 처음 입사했을 때만 해도 열정에 넘쳤었는데";
        }
        else if (randFlag == 14)
        {
            lyrics = "누가 환상체들을 끔찍하다고 생각하겠어. 오랫동안 관찰을 해보면 알게 될 거야. 그들은.";
        }
        else if (randFlag == 15)
        {
            lyrics = "한 때 나도 관리자가 돼보고 싶다고 생각했었어. 물론 지금은 전혀 아냐. 오히려 동정심이 들 정도지.";
        }
        else if (randFlag == 16)
        {
            lyrics = "우리 같은 일개 직원들한테 계속해서 말을 걸어봐야 얻을 수 있는 건 얼마 없을 거야. 만약 뭔갈 좀 더 알고 싶다면 계급이 높은 직원들에게 대화를 거는 편이 좋을 걸. ";
        }

        return lyrics;
    }
     */
    public string getPanicLyrics() {
        string output = null;
        output = this.GetLyricByType(LyricType.MENTALBAD).GetRandomLyric().desc;
        return output;
    }
    /*
    public string getPanicLyrics()
    {
        string speach = "";
        int randFlag = Random.Range(1, 15);

        if (randFlag == 1)
        {
            speach = "명령을 내려주세요! 명령을 내려주세요! 명령을 내려주세요! 명령을 내려주세요! 명령을 내려주세요! ";
        }

        else if(randFlag == 2)
        {
            speach = "그냥 다 없어져라. 이 회사도 우리도 당신도..";
        }

        else if (randFlag == 3)
        {
            speach = "관리자님! 저를 그곳으로 보내지 말아주세요! 저는 죽고 싶지 않아요! 토막나고 싶지 않아요!";
        }

        else if (randFlag == 4)
        {
            speach = "내 동료의 뇌수가 바닥에 굴러다니고 있었어.. 당신도 봤어? 어서 치워야 할텐데… 피가 관리소를 다 뒤엎고 있잖아..";
        }

        else if (randFlag == 5)
        {
            speach = "우리는 다 죽을거야. 이 비참한 곳에서 저 괴물들한테 온 몸이 물어 뜯긴 채로 죽을 거라고. ";
        }

        else if (randFlag == 6)
        {
            speach = "혹시 ‘자살’해라 라는 명령은 없을까? 지금 제일 필요한 명령이 바로 그거 같거든.";
        }

        else if (randFlag == 7)
        {
            speach = "명령을 해주세요! 창고에 있는 도끼를 가지고 저 환상체들을 으깨면 될까요?";
        }

        else if (randFlag == 8)
        {
            speach = "죽기 싫어. 내 동료들 처럼 그렇게 잔인하게 죽고 싶지 않아..";
        }

        else if (randFlag == 9)
        {
            speach = "하하하하하하하하하하하하하하하하하하하하하하하하하하하하하하하하하";
        }

        else if (randFlag == 10)
        {
            speach = "날 보내지 마!!! 날 죽게 하지 마!!!! ";
        }

        else if (randFlag == 11)
        {
            speach = "아까부터 누가 계속 말을 거는 것 같아. 계속 나보고 죽으라고 속삭이는데 좀 잡아서 혼내 줘.";
        }

        else if (randFlag == 12)
        {
            speach = "머릿속에 나 말고 다른 사람이 있는 것 같아. 계속 끔찍한 말을 퍼붓는데 잠을 못 자겠어..";
        }

        else if (randFlag == 13)
        {
            speach = "죽어간동료에겐미안하다는것건강과운동풍족하고회사의생산에기여그것부터하는것이수명걱정없이살수있는것이무엇인지알아서";
        }

        else if (randFlag == 14)
        {
            speach = "너무 춥지 않아요? 난방 좀 틀어주세요.. 추워서 계속 땀이 나네.. ";
        }

        else if (randFlag == 15)
        {
            speach = "어제는 죽은 동료를 봤어요. 같이 밥을 먹고 커피를 마셨죠. 잘 지내는 것 같아 안심이었어요. ";
        }

      return speach;
    }*/

    public string getOnClickLyrics() {
        string output = null;
        output = this.GetLyricByType(LyricType.CHAT).GetRandomLyric().desc;
        return output;
    }

    /*
    public string getOnClickLyrics()
    {
        string speach = "";
        int randFlag = Random.Range(1, 30);

        if (randFlag == 1)
        {
            speach = "좋은 날씨 입니다 관리자님!";
        }

        else if (randFlag == 2)
        {
            speach = "관리자님! 식사는 하셨나요? ";
        }

        else if (randFlag == 3)
        {
            speach = "관리자님 혹시 커피 드시겠습니까?\n 매우 지쳐보여서 말이죠.";
        }

        else if (randFlag == 4)
        {
            speach = "안녕하세요 관리자님! 늘 그렇듯이 즐거운 작업 중입니다. 하하하.";
        }

        else if (randFlag == 5)
        {
            speach = "관리자님, 오해하실까봐 말씀드리지만 \n방금 말 한 ‘거지 같은 놈’ 은 절대로 관리자님을 지칭하는 게 아닙니다. ";
        }

        else if (randFlag == 6)
        {
            speach = "관리자님 저희 휴게실에 향이 끝내주는 커피가 한 박스 들어왔는데,\n 한가할 때면 언제든지 놀러오십시오! ";
        }

        else if (randFlag == 7)
        {
            speach = "관리자님! 좋은 하루 입니다.";
        }

        else if (randFlag == 8)
        {
            speach = "관리자님 오늘도 잘 부탁드립니다!";
        }

        else if (randFlag == 9)
        {
            speach = "관리자님, \n있다 퇴근 후에 저희끼리 회식이 있을 것 같은데 괜찮으시면 같이 가시죠!";
        }

        else if (randFlag == 10)
        {
            speach = "안녕하세요 관리자님!\n 그런데 직원들을 잡아먹는 환상체가 있다는 게 정말인가요? ";
        }

        else if (randFlag == 11)
        {
            speach = "관리자님 커피 한 잔 하시겠습니까? ";
        }

        else if (randFlag == 12)
        {
            speach = "막 신입사원들에게 관리자님 이야기를 하고 있던 참이었습니다.\n 얼마나 위대하고 똑똑하고 대단한 분이신가에 대해서요!";
        }

        else if (randFlag == 13)
        {
            speach = "관리자님 혹시나 해서 하는 말이지만 \n방금까지 제가 중얼거렸던 욕은 절대 관리자님과는 상관이 업는 이야기 입니다.";
        }

        else if (randFlag == 14)
        {
            speach = "관리자님, 위험한 작업 시킬 때 저는 제외해주시면 안될까요?  ";
        }

        else if (randFlag == 15)
        {
            speach = "아 새로 오신 관리자님 이시군요!\n 얘기 많이 들었습니다!  ";
        }

        else if (randFlag == 16)
        {
            speach = "제 주변에서 다 관리자님에 대한 칭찬이 자자하다니까요. ";
        }

        else if (randFlag == 17)
        {
            speach = "관리자님, 그 ‘아무 것도 없는’ 에 대한 작업만큼은 좀 자제해주세요.";
        }

        else if (randFlag == 18)
        {
            speach = "관리자님 사무실에는 음료수랑 커피가 \n종류별로 구비돼있다는 소문이 정말인가요? ";
        }

        else if (randFlag == 19)
        {
            speach = "관리자님 안녕하세요!\n 언제 한 번 관리자님 사무실에 저도 초대해주세요!";
        }

        else if (randFlag == 20)
        {
            speach = "관리자님! \n혹시 직원들의 복지제도를 전면 개선하실 생각은 없으신가요? ";
        }

        else if (randFlag == 21)
        {
            speach = "관리자님 직원 휴게실에 맥주 자판기를 배치하면 어떨까요? \n분명 사기가 매우 오를겁니다.";
        }

        else if (randFlag == 22)
        {
            speach = "관리자님 혹시 직원 식당을 뷔페 풀 코스로 바꿀 의향은 없으신가요? ";
        }

        else if (randFlag == 23)
        {
            speach = "관리자님 냉커피 한 잔 드시고 하십시오!\n 제가 특별히 맛 좋게 타드리겠습니다!";
        }

        else if (randFlag == 24)
        {
            speach = "관리자님 제가 어제 무단결근을 한 건 연어를 먹고 급 탈이 나서 그만..";
        }

        else if (randFlag == 25)
        {
            speach = "관리자님 무단결근 3회 이상 하면 경고 없이 퇴사를 명령할 수 있는\n 제 안건에 대해서 진지하게 생각해 주세요. 요즘 버릇없는 신입사원들이 많아서 말입니다.";
        }

        else if (randFlag == 26)
        {
            speach = "표현은 잘 하지 않지만 우린 모두 관리자님을 존경하고 있습니다!";
        }

        else if (randFlag ==27)
        {
            speach = "관리자님 있다 끝나고 맥주 한 잔 하시는 거 어떠세요?";
        }

        else if (randFlag == 28)
        {
            speach = "관리자님 마법소녀가 소문처럼 그렇게 위험한 환상체인가요? \n저는 신입사원이라 아직 보지 못했어요..";
        }

        else if (randFlag == 29)
        {
            speach = "관리자님, 몇 일전에 환상체에게 물려서 손가락을 조금 다쳤는데 \n이것도 보험 처리가 될까요?";
        }

        else if (randFlag == 30)
        {
            speach = "관리자님, 감기 조심하십시오!";
        }

        return speach;

    }*/

    public string getEscapePanicLyrics() {
        string output = null;
        output = this.GetLyricByType(LyricType.ESCAPE).GetRandomLyric().desc;
        return output;
    }

    public LyricList GetLyricByType(LyricType type) {
        LyricList output = null;

        foreach (LyricList target in this.list) {
            if (target.type == type) {
                output = target;
                break;
            }
        }
        return output;
    }

    /*
    public string getEscapePanicLyrics()
    {
        string speach = "";
        int randFlag = Random.Range(1, 30);

        if (randFlag == 1)
        {
            speach = "모두 피해! 그냥 도망쳐!";
        }

        else if (randFlag == 2)
        {
            speach = "죽고 싶지 않아! 날 두고 도망치지 마! ";
        }

        else if (randFlag == 3)
        {
            speach = "살려줘!!!! ";
        }

        else if (randFlag == 4)
        {
            speach = "관리자님! 제발 살려주세요! ";
        }

        else if (randFlag == 5)
        {
            speach = "꺼져! 난 여기서 죽지 않을거야!";
        }

        else if (randFlag == 6)
        {
            speach = "관리자님! 보고 계시죠? 제발 살려주세요! 빨리 이것들을 쫓아내주세요!";
        }

        else if (randFlag == 7)
        {
            speach = "누가 비상계획팀 들을 좀 불러봐! 이러다 다 죽을거야! ";
        }

        else if (randFlag == 8)
        {
            speach = "살려주세요!!!!" ;
        }

        else if (randFlag == 9)
        {
            speach = "다른 직원들을 빨리 불러와!! ";
        }

        else if (randFlag == 10)
        {
            speach = "살려줘!! 살려달라고!!";
        }

        else if (randFlag == 11)
        {
            speach = "왜 아무도 안 오는 거야!!! 우릴 여기서 다 죽게 할 셈이야? ";
        }

        else if (randFlag == 12)
        {
            speach = "저것은 우릴 다 죽이려고 할거야! 우린 다 끝났어! ";
        }

         else if (randFlag == 13)
        {
            speach = "다 끝났어 이제.. 우린 다 죽었어..";
        }

       else if (randFlag == 14)
        {
            speach = "난 죽지 않을 거야!! 누가 어떻게 좀 해봐!";
        }

        else if (randFlag == 15)
        {
            speach = "내가 죽으면 부모님한테 죄송하다고 전해 드려.. ";
        }

        return speach;

    }
    */
}
