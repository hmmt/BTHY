﻿using UnityEngine;
using System.Collections;

public class BriefingInfo : MonoBehaviour {

   public  UnityEngine.UI.Text Briefing;

    public string alwaysBriefing;
    public string tmpBriefing;

	// Use this for initialization
	void Start () {

        SetDefaultBriefing();

       // alwaysBriefing = "환영합니다.";
        //alertBriefing = "비상계획팀 - 시설의 비상상황을 관리하는 부서";

	}
	
	// Update is called once per frame
	void FixedUpdate () {
        SetBriefing();
        Briefing.text = tmpBriefing;
	
	}

    public void SetBriefing()
    {
        if (StageUI.instance.currentSefriaUi == "2")
        {
            tmpBriefing = "비상계획팀 \n\n 돌발적으로 일어나는 비상 상황들을 예측하거나 대비하여 계획을 세우는 팀입니다. 만약에 일어날 수 있는 환상체들의 폭주, 발작, 탈출 및 직원들의 패닉 상황을 위한 작업을 준비합니다. \n적색경보가 발생했을 경우 모든 지휘권은 이 부서에 넘어갑니다. \n\n\n 열기위한 에너지 비용:10";
        }
        else if (StageUI.instance.currentSefriaUi == "3")
        {
            tmpBriefing = "자재관리팀 \n\n회사에 대한 자재들을 관리하고 정기적인 점검을 통해 유지 및 보수하는 부서입니다.\n 환상체들의 탈출이나 파괴행위로 인해 부서진 건물들을 관리하고 청소합니다.\n 사무실 보단 현장 중심으로 돌아가는 부서로 여기 직원들은 언제나 각종 자재들을 가지고 있습니다. "; 
        }
        else if (StageUI.instance.currentSefriaUi == "4")
        {
            tmpBriefing = "솔루션계획팀 \n\n 환상체들의 위험도와 특징들을 분류하고 이에 대한 해결법을 강구 및 해결해내는 부서 입니다.\n 복지관리팀에서부터 나오는 관찰기록이나 상담기록들을 토대로 <해결법>을 연구해 나갑니다. \n좀 더 명확하고 안전한 해결법을 위한 실험들을 진행하기도 합니다. \n\n\n 열기위한 에너지 비용:10"; 
        }
        else if (StageUI.instance.currentSefriaUi == "1")
        {
            tmpBriefing = "지휘감시팀 \n\n 직원 및 환상체들을 감시하고 환상체 관리를 위한 지휘를 내리거나 기획을 하는 부서입니다. 이 부서에서는 격리소 곳곳을 비춰주는 CCTV 들을 통해 즉각적인 명령과 지휘를 통솔합니다. \n\n\n 열기위한 에너지 비용:10";
        }
    }

    public void SetDefaultBriefing()
    {
        tmpBriefing = "에너지주식회사에 입사하게 된 것을 진심으로 환영합니다.\n당신은 관리자로서 이 회사에 중요한역할을 하게 될 것입니다.\n저는 앞으로 당신을 도와줄 A.I 엔젤라 입니다. \n따라서 상호적인 대화는 불가능함을 미리 알려드립니다. \n하지만 든든한 파트너와 비서의 역할로서 최선을 다해 조력하겠습니다.\n작업을 시작하기에 앞서, 왼쪽 상단의 관리자를 위한 지침서를 꼭 읽어봐주세요. 이를 숙지하지 않음에서 오는 불이익은 모두 관리자 본인의 책임에 있습니다.";
    }

    public void SetNarrationByDay()
    {
        if (StageUI.instance.getCurrnetType() == StageUI.UIType.START_STAGE)
        {
            if (PlayerModel.instance.GetDay() == 0)
                tmpBriefing = "우어어어엉";
        }

        else if (StageUI.instance.getCurrnetType() == StageUI.UIType.END_STAGE)
        {
            tmpBriefing = "수고하셨습니다.";
        }
    }
    
}
