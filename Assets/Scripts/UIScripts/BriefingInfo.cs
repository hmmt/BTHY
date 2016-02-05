using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BriefingInfo : MonoBehaviour {

   public  UnityEngine.UI.Text Briefing;
   public  Text SefiraBrief;
   public Text AreaName;
   public Text DefaultBrief;
   public Image AreaImage;

    public string alwaysBriefing;
    public string tmpBriefing;
    public string area;
    public string defaultbrief;
    public int cost;
    private bool cancel = false;

	// Use this for initialization
	void Start () {
        cost = 0;
        SetDefaultBriefing();
        DefaultBrief.text = defaultbrief;
        setActive(false);
       // alwaysBriefing = "환영합니다.";
        //alertBriefing = "비상계획팀 - 시설의 비상상황을 관리하는 부서";

	}
	
	// Update is called once per frame
	void FixedUpdate () {

        SetBriefing();
        //Briefing.text = tmpBriefing;
        AreaName.text = area;
        SefiraBrief.text = tmpBriefing;
        if (!cancel && Input.GetMouseButtonDown(1)) {
            //Debug.Log("클릭");
            cancel = !cancel;//우클릭 시 초기화 되어야할텐데;
            
        }
        
	}

    public void CancelSelected(BaseEventData eventData) {
        if (!cancel) return;
        PointerEventData data = eventData as PointerEventData;
        if (data.button != PointerEventData.InputButton.Right) return;
        setActive(false);
        StageUI.instance.SetCurrentSefria("0");
    }

    private void setActive(bool f) {
        SefiraBrief.gameObject.SetActive(f);
        AreaImage.gameObject.SetActive(f);
        AreaName.gameObject.SetActive(f);
        
        DefaultBrief.gameObject.SetActive(!f);
    }
    
    public void SetBriefing()
    {
        if (StageUI.instance.currentSefriaUi == "2")
        {
            cancel = true;
            setActive(true);
            AreaImage.sprite = ResourceCache.instance.GetSprite("Sprites/Sefira/Netach_01");
            area = "비상계획팀";
            tmpBriefing = " 돌발적으로 일어나는 비상 상황들을 예측하거나 대비하여 계획을 세우는 팀입니다. 만약에 일어날 수 있는 환상체들의 폭주, 발작, 탈출 및 직원들의 패닉 상황을 위한 작업을 준비합니다. \n적색경보가 발생했을 경우 모든 지휘권은 이 부서에 넘어갑니다.";
            cost = 10;
        }
        else if (StageUI.instance.currentSefriaUi == "3")
        {
            cancel = true;
            setActive(true);
            AreaImage.sprite = ResourceCache.instance.GetSprite("Sprites/Sefira/Hod_01");
            area = "자재관리팀";
            tmpBriefing = " 회사에 대한 자재들을 관리하고 정기적인 점검을 통해 유지 및 보수하는 부서입니다.\n 환상체들의 탈출이나 파괴행위로 인해 부서진 건물들을 관리하고 청소합니다.\n 사무실 보단 현장 중심으로 돌아가는 부서로 여기 직원들은 언제나 각종 자재들을 가지고 있습니다. ";
            cost = 10;
        }
        else if (StageUI.instance.currentSefriaUi == "4")
        {
            cancel = true;
            setActive(true);
            AreaImage.sprite = ResourceCache.instance.GetSprite("Sprites/Sefira/Yessod_01");
            area = "솔루션계획팀";
            tmpBriefing = " 환상체들의 위험도와 특징들을 분류하고 이에 대한 해결법을 강구 및 해결해내는 부서 입니다.\n 복지관리팀에서부터 나오는 관찰기록이나 상담기록들을 토대로 <해결법>을 연구해 나갑니다. \n좀 더 명확하고 안전한 해결법을 위한 실험들을 진행하기도 합니다. \n\n\n 열기위한 에너지 비용:10"; 
        }
        else if (StageUI.instance.currentSefriaUi == "1")
        {
            cancel = true;
            setActive(true);
            AreaImage.sprite = ResourceCache.instance.GetSprite("Sprites/Sefira/Malkut_01");
            area = "Conducting & Surveillance Team";
            tmpBriefing = " This is a department that sets out plans and commands regarding of abnormalitic's management and surveillance. Here, you can give out direct orders by watching over the CCTV that has the facility under surveillance.";
        }
    }

    public void SetDefaultBriefing()
    {
        defaultbrief = "에너지주식회사에 입사하게 된 것을 진심으로 환영합니다.\n당신은 관리자로서 이 회사에 중요한역할을 하게 될 것입니다.\n저는 앞으로 당신을 도와줄 A.I 엔젤라 입니다. \n따라서 상호적인 대화는 불가능함을 미리 알려드립니다. \n하지만 든든한 파트너와 비서의 역할로서 최선을 다해 조력하겠습니다.\n작업을 시작하기에 앞서, 왼쪽 상단의 관리자를 위한 지침서를 꼭 읽어봐주세요. 이를 숙지하지 않음에서 오는 불이익은 모두 관리자 본인의 책임에 있습니다.";
    }

    public void SetNarrationByDay()
    {
        if (StageUI.instance.getCurrnetType() == StageUI.UIType.START_STAGE)
        {
            if (PlayerModel.instance.GetDay() == 0)
                tmpBriefing = "에너지주식회사에 입사하시게 된 걸 진심으로 환영합니다.\n저는 앞으로 당신에게 관리자로서 역할을 다하기 위해 지속적인 도움을 줄 안젤라 입니다. \n본 방송은 미리 녹음된 것임을 말씀드리는 바이며, 따라서 별도의 질문이나 상호작용은 불가능함을 알려드립니다.  ";
            
            else if (PlayerModel.instance.GetDay() == 1)
                tmpBriefing = "관리자님 좋은 아침입니다! \n관리자로서의 두 번째 날이 되었습니다.\n어제와 마찬가지로만 에너지를 생산해주시길 바랍니다. ";
            
            else if (PlayerModel.instance.GetDay() == 2)
                tmpBriefing = "관리자님 오늘도 좋은 아침입니다! \n관리자님께서 일정 에너지를 다 모으게 된다면 다른 부서가 열리게 됩니다.\n 각 부서의 성격에 따라 추가 되는 기능이 생깁니다. ";

            else if (PlayerModel.instance.GetDay() == 3)
                tmpBriefing = "관리자님 오늘도 좋은 하루가 되길 바랍니다. \n0000 이상의 에너지를 생산하게 되면 관리자님은 새로운 부서인 자재관리팀 혹은 비상계획팀을 활성화 시킬 수 있습니다. \n부서가 다양할수록 관리 시스템은 더욱 최적화 될 것입니다. ";

            else if (PlayerModel.instance.GetDay() == 4)
                tmpBriefing = "관리자님, 좋은 아침입니다! \n오늘도 늘 그렇듯이 힘찬 하루를 시작하길 바랍니다." ;

            else if (PlayerModel.instance.GetDay() == 5)
                tmpBriefing = "안녕하세요 관리자님!\n 오늘도 변함없이 활기찬 하루가 되길 바랍니다. \n관리자님의 꾸준한 노력으로 인해 우리 회사는 목표한 에너지 수준에 손쉽게 도달해가고 있습니다.\n앞으로도 지금과 같이 환상체들에 대한 작업에 힘써주시길 바랍니다.";

            else if (PlayerModel.instance.GetDay() == 6)
                tmpBriefing = "안녕하세요 관리자님! \n오늘로써 관리자로서 회사에 입사한지 일주일이 되었습니다.\n오늘도 힘찬 하루가 되길 바랍니다. ";
            
            else if (PlayerModel.instance.GetDay() == 7)
                tmpBriefing = "안녕하세요 관리자님! \n오늘로써 관리자로서 회사에 입사한지 일주일이 되었습니다.\n오늘도 힘찬 하루가 되길 바랍니다. ";

            else if (PlayerModel.instance.GetDay() == 8)
                tmpBriefing = "안녕하세요 관리자님!\n 좋은 하루가 되길 바랍니다.";

            else if (PlayerModel.instance.GetDay() == 9)
                tmpBriefing = "반갑습니다 관리자님!\n 오늘도 우리 회사의 자랑스러운 관리자로서 에너지의 생산증대에 더욱 더 박차를 가해주시길 바랍니다!";
            
            else if (PlayerModel.instance.GetDay() == 10)
                tmpBriefing = "관리자님 오늘도 활기찬 하루가 되길 바랍니다. ";

            else if (PlayerModel.instance.GetDay() == 11)
                tmpBriefing = "관리자님 오늘은 어떤 하루를 보내실 건가요?\n관리자님이 입사한 이후로 에너지의 총 생산은 200%가 증가하였습니다.\n오늘도 행복하고 즐거운 작업이 되길 바랍니다.";

            else if (PlayerModel.instance.GetDay() == 12)
                tmpBriefing = "관리자님, 안녕하세요! \n우리 에너지주식회사는 언제나 당신의 무궁한 발전을 기원합니다.\n관리자님은 이미 우리와 가족과 다름 없는 관계로 아주 오랜 기간 동안 우리와 함께 하게 될 것입니다.\n오늘도 에너지 생산을 위한 힘찬 하루를 보내게 되길 바랍니다. \n";
            else if (PlayerModel.instance.GetDay() == 13)
                tmpBriefing = "관리자님 오늘은 날씨가 매우 화창합니다.\n가끔은 여유롭게 휴식을 즐기며 커피라도 한 잔 하는 건 어떨까요?\n우리 에너지주식회사는 당신을 응원합니다.";
            else if (PlayerModel.instance.GetDay() == 14)
                tmpBriefing = "안녕하세요 관리자님\n 화창한 날씨가 계속 되고 있습니다. \n우리 에너지주식회사는 관리자님의 끊임없는 수고로 더욱 번성하고 발전하고 있습니다.";
            else if (PlayerModel.instance.GetDay() == 15)
                tmpBriefing = "관리자님 좋은 아침입니다! \n오늘도 근사한 하루를 보내시길 바랍니다.  ";
            else
                tmpBriefing = "관리자님 오늘도 활기찬 하루가 되길 바랍니다. ";

        }

        else if (StageUI.instance.getCurrnetType() == StageUI.UIType.END_STAGE)
        {
            tmpBriefing = "수고하셨습니다.";
        }
    }
    
}
