using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[System.Serializable]
public class AgentUnitUI
{
    public Slider hp;
    public RectTransform workIcon;
    public bool Activated = false;

    public RecoilEffectUI healthRecoil;
    public RecoilEffectUI mentalRecoil;

    public void initUI() {
        hp.gameObject.SetActive(false);
        workIcon.gameObject.SetActive(false);
    }

    public void activateUI(AgentModel model) {
        hp.gameObject.SetActive(true);
        workIcon.gameObject.SetActive(true);
        this.Activated = true;
        hp.maxValue = model.maxHp;
        hp.value = model.hp;
    }

    //0번 : 검은색(붕괴 시 아이콘), 1번 : 멀쩡한 상태 아이콘
    //1번의 alpha값을 수정하는 것을 통해 효과
    public void setUIValue(AgentModel model) {
        if (!Activated) return;
        Color c = workIcon.GetChild(1).GetComponent<Image>().color;
        c.a = (float)model.mental / model.maxMental;

        workIcon.GetChild(1).GetComponent<Image>().color = c;
		hp.value = model.hp / (float)model.maxHp;
    }


}

[System.Serializable]
public class RecoilEffectUI {
    public enum RecoilArrow { 
        LEFTUP, UP, RIGHTUP, RIGHT, RIGHTDOWN, DOWN, LEFTDOWN, LEFT
    }

    public RectTransform rect;
    public float maxTime = 0.1f;
    public int scale = 2;
    public int recoilCount = 5;

    public static List<RecoilArrow> MakeRecoilArrow(int level)
    {
        List<RecoilArrow> list = new List<RecoilArrow>();
        int former = -1;
        for (int i = 0; i < level; i++)
        {
            int randVal;
            while (former == (randVal = Random.Range(0, 8))) ;
            former = randVal;
            RecoilArrow arrow = GetArrow(randVal);

            list.Add(arrow);

        }
        return list;
        //call recoil coroutine
    }

    public static Vector2 GetVector(RecoilArrow arrow, Vector2 initial)
    {
        float x;
        float y;
        int index = (int)arrow % 4;
        switch (index)
        {
            case 0:
                x = -1; y = 1;
                break;
            case 1:
                x = 0; y = 1;
                break;
            case 2:
                x = 1; y = 1;
                break;
            case 3:
                x = 1; y = 0;
                break;
            default:
                Debug.LogError("??");
                x = 0; y = 0;
                break;
        }
        int sign = 1;
        if ((int)arrow > 3)
        {
            sign = -1;
        }

        Vector2 output = new Vector2(initial.x + (x * sign),
                                     initial.y + (y * sign));
        return output;
    }

    public static RecoilArrow GetArrow(int index)
    {
        switch (index)
        {
            case 0: return RecoilArrow.LEFTUP;
            case 1: return RecoilArrow.UP;
            case 2: return RecoilArrow.RIGHTUP;
            case 3: return RecoilArrow.RIGHT;
            case 4: return RecoilArrow.RIGHTDOWN;
            case 5: return RecoilArrow.DOWN;
            case 6: return RecoilArrow.LEFTDOWN;
            case 7: return RecoilArrow.LEFT;
            default: return RecoilArrow.LEFTUP;
        }
    }

}

public class AgentUnit : MonoBehaviour, IOverlapOnclick {

    public AgentModel model;

    public GameObject agentWindow;
    public GameObject agentAttackedAnimator;
    public GameObject agentPlatform;

    public agentSkillDoing showSkillIcon;
    public AgentSpeech showSpeech;

    /*
    public Animator agentAnimator;
    public GameObject renderNode;
    */

    public GameObject puppetNode;
    public Animator puppetAnim;

    public AgentAnim animTarget;

    public float oldPos;
    public float oldPosY;
    public bool agentMove=false;
    public bool agentDead = false;

    public Text agentName;

    private string oldSefira;

    //각 직원 부위 스프라이트 결정 변수
    public GameObject faceSprite;
    public GameObject deadSprite;
    public GameObject hairSprite;

    public UnityEngine.UI.Text speechText;

    public AgentUnitUI ui;

    // layer에서 z값 순서 정하기 위한 값.
	private float zValueDefault;
    public float zValue;

    private bool uiOpened = false;

    public AccessoryUnit accessoryUnit;

    //직원 대사
    string speech = "";

    void LateUpdate()
    {
        /*
        foreach (var renderer in faceSprite.GetComponents<SpriteRenderer>())
        {
            if (model.mental > 0)
            {
                if (renderer.sprite.name == "Face_A_00")
                    renderer.sprite = ResourceCache.instance.GetSprite("Sprites/Agent/Face/Face_" + model.faceSpriteName + "_00");
                else if (renderer.sprite.name == "Face_A_01")
                    renderer.sprite = ResourceCache.instance.GetSprite("Sprites/Agent/Face/Face_" + model.faceSpriteName + "_01");
                else if (renderer.sprite.name == "Face_A_02")
                    renderer.sprite = ResourceCache.instance.GetSprite("Sprites/Agent/Face/Face_" + model.faceSpriteName + "_02");
            }

            else
            {
                if (renderer.sprite.name == "Face_A_00")
                    renderer.sprite = ResourceCache.instance.GetSprite("Sprites/Agent/Panic/panic_" + model.panicSpriteName + "_00");
                else if (renderer.sprite.name == "Face_A_01")
                    renderer.sprite = ResourceCache.instance.GetSprite("Sprites/Agent/Panic/panic_" + model.panicSpriteName + "_01");
                else if (renderer.sprite.name == "Face_A_02")
                    renderer.sprite = ResourceCache.instance.GetSprite("Sprites/Agent/Panic/panic_" + model.panicSpriteName + "_02");

            }
        }

        foreach (var renderer in hairSprite.GetComponents<SpriteRenderer>())
        {
            if (renderer.sprite.name == "Hair_M_A_00")
                renderer.sprite = ResourceCache.instance.GetSprite("Sprites/Agent/Hair/Hair_M_" + model.hairSpriteName + "_00");
            else if (renderer.sprite.name == "Hair_M_A_01")
                renderer.sprite = ResourceCache.instance.GetSprite("Sprites/Agent/Hair/Hair_M_" + model.hairSpriteName + "_01");
            else if (renderer.sprite.name == "Hair_M_A_02")
            {
                renderer.sprite = ResourceCache.instance.GetSprite("Sprites/Agent/Hair/Hair_M_" + model.hairSpriteName + "_02");
                renderer.transform.localScale.Set(-1,1,1);
            }
        }
         */
    }

	void Awake()
	{
		
	}

    void  Start()
    {
		AnimatorManager.instance.SaveAnimator (model.instanceId, puppetAnim);

        //agentAnimator.SetInteger("Sepira", 1);
        //agentAnimator.SetBool("Change", false);

        puppetAnim.SetInteger("Sefira", 1);
        puppetAnim.SetBool("Change", false);

        oldPos = transform.localPosition.x;
        oldPosY = transform.localPosition.y;
        oldSefira = "1";
        agentName.text = model.name;

        agentPlatform.SetActive(false);

        //faceSprite.GetComponent<SpriteRenderer>().sprite = ResourceCache.instance.GetSprite("Sprites/Agent/Face/Face_" + model.faceSpriteName + "_00");
        //hairSprite.GetComponent<SpriteRenderer>().sprite = ResourceCache.instance.GetSprite("Sprites/Agent/Hair/Hair_M_" + model.hairSpriteName + "_00");
        
        ui.initUI();
        /*
        if (PlayerModel.instance.IsOpenedArea("yessod") && CreatureManager.instance.yessodState == SefiraState.NORMAL)
        {
            uiOpened = true;
        }*/
        uiOpened = true;

        if (uiOpened) {
            ui.activateUI(model);
        }
        ChangeAgentUniform();

        Canvas canvas = this.showSpeech.GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;
        

        accessoryUnit = new AccessoryUnit();
        accessoryUnit.Init(this.animTarget);
    }

	public void SetDefaultZValue(float value)
	{
		zValueDefault = value;
		zValue = value;
	}
	public void ResetZValue()
	{
		zValue = zValueDefault;
	}

    /*
    public void DeadAgent()
    {
        deadSprite.gameObject.SetActive(true);
        faceSprite.gameObject.SetActive(false);
        hairSprite.gameObject.SetActive(false);
        agentAnimator.gameObject.SetActive(false);

        deadSprite.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Agent/Dead");
        deadSprite.transform.Rotate(0f, -90f, 0f);
    }*/

	private void UpdateDirection()
	{
		MovableObjectNode movable = model.GetMovableNode ();
		UnitDirection movableDirection = movable.GetDirection ();
		Transform puppet = puppetNode.transform.parent;

        Vector3 puppetScale = puppet.localScale;

		if (movableDirection == UnitDirection.RIGHT)
		{
            if (puppetScale.x > 0)
            {
                puppetScale.x = -puppetScale.x;
            }
		}
		else
		{
            if (puppetScale.x < 0)
            {
                puppetScale.x = -puppetScale.x;
            }
		}
		puppetNode.transform.parent.localScale = puppetScale;

		return;
        /*

        MapEdge currentEdge = model.GetCurrentEdge();
        int edgeDirection = model.GetEdgeDirection();

        if (currentEdge != null)
        {
            MapNode node1 = currentEdge.node1;
            MapNode node2 = currentEdge.node2;
            Vector2 pos1 = node1.GetPosition();
            Vector2 pos2 = node2.GetPosition();

            if (edgeDirection == 1)
            {
                //Transform anim = renderNode.transform;

                //Transform puppet = puppetNode.transform;

                //Vector3 puppetScale = puppet.localScale;

                if (pos2.x - pos1.x > 0 && puppetScale.x < 0)
                {
                    puppetScale.x = -puppetScale.x;
                }
                else if (pos2.x - pos1.x < 0 && puppetScale.x > 0)
                {
                    puppetScale.x = -puppetScale.x;
                }
                puppet.transform.localScale = puppetScale;
            }
            else
            {
                if (pos2.x - pos1.x > 0 && puppetScale.x > 0)
                {
                    puppetScale.x = -puppetScale.x;
                }
                else if (pos2.x - pos1.x < 0 && puppetScale.x < 0)
                {
                    puppetScale.x = -puppetScale.x;
                }
                puppet.transform.localScale = puppetScale;
            }
        }*/
	}

	private bool visible = true;

	private void UpdateViewPosition()
	{
		transform.localScale = new Vector3 (model.GetMovableNode ().currentScale, model.GetMovableNode ().currentScale, transform.localScale.z);
        MapEdge currentEdge = model.GetCurrentEdge();

		if(currentEdge != null && currentEdge.type == "door")
		{
			if(visible)
			{
				visible = false;
                Vector3 newPosition = model.GetCurrentViewPosition();
                newPosition.z = 100000f;
                transform.localPosition = newPosition;
			}
		}
		else
		{
			if(!visible)
			{
				visible = true;
			}
            Vector3 newPosition = model.GetCurrentViewPosition();
            newPosition.z = zValue;
			transform.localPosition = newPosition;
		}
	}

	////

	private float waitTimer = 0;

	// if map is destroyed....?

    public void ChangeAgentUniform()
    {
        
        if (animTarget != null)
        {
            if (model.currentSefira == "1")
            {
                //animTarget.SetClothes(Resources.LoadAll<Sprite>("Sprites/Agent/Test/AgentM1"));
            }
            else
            {
                //animTarget.SetClothes(Resources.LoadAll<Sprite>("Sprites/Agent/Test/AgentN1"));
            }
        }
        //agentAnimator.SetBool("Change", true);
		/*
        puppetAnim.SetBool("Change", true);

        if (model.currentSefira == "1")
        {
            agentAnimator.SetInteger("Sepira", 1);
            puppetAnim.SetInteger("Sefira", 1);
        }

        else if (model.currentSefira == "2")
        {
            agentAnimator.SetInteger("Sepira", 2);
            puppetAnim.SetInteger("Sefira", 2);
        }

        else if (model.currentSefira == "3")
        {
            agentAnimator.SetInteger("Sepira", 3);
            puppetAnim.SetInteger("Sefira", 3);
        }

        else if (model.currentSefira == "4")
        {
            agentAnimator.SetInteger("Sepira", 4);
            puppetAnim.SetInteger("Sefira", 4);
        }

        TimerCallback.Create(1, delegate()
        {
            if (puppetAnim.GetBool("Change"))
                puppetAnim.SetBool("Change", false);
        });
        */
        oldSefira = model.currentSefira;
    }


	void FixedUpdate()
	{
        if (oldSefira != model.currentSefira)
        {
            ChangeAgentUniform();
        }

		if(AnimatorUtil.HasParameter(puppetAnim, "Move"))
		{
	        if (oldPos != transform.localPosition.x)
	        {
	            //agentAnimator.SetBool("AgentMove", true);
	           // faceSprite.GetComponent<Animator>().SetBool("Move", true);
	           // hairSprite.GetComponent<Animator>().SetBool("Move", true);

	            animTarget.SetSpeed(model.movement / 4.0f);
	            puppetAnim.SetBool("Move", true);
	        }
	        else
	        {
	          //  agentAnimator.SetBool("AgentMove", false);
	         //   faceSprite.GetComponent<Animator>().SetBool("Move", false);
	          //  hairSprite.GetComponent<Animator>().SetBool("Move", false);

	            animTarget.SetSpeed(1);
	            puppetAnim.SetBool("Move", false);
	        }
		}
        /*
        if (oldPosY != transform.localPosition.y)
        {
            agentPlatform.SetActive(true);
        }

        else
        {
            agentPlatform.SetActive(false);
        }
        */
        oldPosY = transform.localPosition.y;
        oldPos = transform.localPosition.x;

        int randLyricsTick = Random.Range(0, 3000);
        //&& !speechText.IsActive()제거
        if (model.GetState() == AgentAIState.IDLE && randLyricsTick == 0 && model.mental > 0 )
        {
            int randLyricsStory = Random.Range(0, 10);
            if (randLyricsStory < 8)
            {
                speech = AgentLyrics.instance.getLyricsByDay(PlayerModel.instance.GetDay());
            }
            else
            {
                speech = AgentLyrics.instance.getStoryLyrics();
            }
            Notice.instance.Send("AddPlayerLog", model.name + " : " + speech);
            Notice.instance.Send("AddSystemLog", model.name + " : " + speech);
            showSpeech.showSpeech(speech);
        }

		if (model.mental <= 0 && speechText != null && !speechText.IsActive())
        {
            speech = AgentLyrics.instance.getPanicLyrics();
            Notice.instance.Send("AddPlayerLog", model.name + " : " + speech);
            Notice.instance.Send("AddSystemLog", model.name + " : " + speech);
            showSpeech.showSpeech(speech);
            Debug.Log("패닉대사 " + speech);
        }
        ui.setUIValue(model);
	}

	void Update()
	{
		UpdateViewPosition();
		UpdateDirection();
		///SetCurrentHP (model.hp);
		//UpdateMentalView ();
        /*
        if (PlayerModel.instance.IsOpenedArea("4") && CreatureManager.instance.yessodState == SefiraState.NORMAL)
        {
            uiOpened = true;
        }
        else uiOpened = false;
        */
        if (uiOpened)
        {
            ui.activateUI(model);
        }
        else {
            ui.initUI();
        }

        if (Input.GetKey(KeyCode.T)){
            Debug.Log("check"+PlayerModel.instance.IsOpenedArea("4") + " " +CreatureManager.instance.yessodState);
        }
        if (Input.GetKeyDown(KeyCode.G)) {
            model.mental -= 10;
           // Debug.Log("최대치" + model.maxMental + "현재" + model.mental + "알파" + mentalImage.color.a);
        }
        if (Input.GetKeyDown(KeyCode.H)) {
            model.hp -= 1;
        }
	}
    /*
	public void SetMaxHP(int maxHP)
	{
		GetComponentInChildren<AgentHPBar> ().SetMaxHP (maxHP);
	}

	public void SetCurrentHP(int hp)
	{
        if (PlayerModel.instance.IsOpenedArea("Yessod") && CreatureManager.instance.yessodState == SefiraState.NORMAL)
        {
            GetComponentInChildren<AgentHPBar>().gameObject.SetActive(true);
            GetComponentInChildren<AgentHPBar>().SetCurrentHP(hp);
        }

        else if (!GetComponentInChildren<AgentHPBar>().gameObject.activeInHierarchy)
        {
                GetComponentInChildren<AgentHPBar>().gameObject.SetActive(false);
        }
	}
    */
	public void UpdateMentalView()
	{
        if (PlayerModel.instance.IsOpenedArea("Yessod") && CreatureManager.instance.yessodState == SefiraState.NORMAL)
        {
            GetComponentInChildren<MentalViewer>().gameObject.SetActive(true);
            GetComponentInChildren<MentalViewer>().SetMentalRate((float)model.mental / (float)model.maxMental);
        }
        else if (!GetComponentInChildren<MentalViewer>().gameObject.activeInHierarchy)
        {
                GetComponentInChildren<MentalViewer>().gameObject.SetActive(false);
           
        }
        

	}

	public void SetAgentAnimatorModel()
	{
	}

	public void SetParameterOnce(string pname, int value)
	{
		if (animTarget != null)
		{
			animTarget.SetParameterOnce (pname, value);
		}
	}
	public void SetParameterOnce(string pname, bool value)
	{
		if (animTarget != null)
		{
			animTarget.SetParameterOnce (pname, value);
		}
	}
	public void SetParameterForSecond(string pname, bool value, float time)
	{
		if (animTarget != null)
		{
			animTarget.SetParameterForSecond (pname, value, time);
		}
	}
	public void SetParameterForSecond(string pname, int value, float time)
	{
		if (animTarget != null)
		{
			animTarget.SetParameterForSecond (pname, value, time);
		}
	}

	public void OnClick()
	{
        OpenStatusWindow();
	}

	public void OpenStatusWindow()
	{
        model.OnClick();
        
        AgentModel oldUnit = (AgentStatusWindow.currentWindow != null) ? AgentStatusWindow.currentWindow.target : null;
		AgentStatusWindow.CreateWindow (model);
        /*
        if (CollectionWindow.currentWindow != null)
        CollectionWindow.currentWindow.CloseWindow();
        */
        speech = AgentLyrics.instance.getOnClickLyrics();
        Notice.instance.Send("AddPlayerLog", model.name + " : " + speech);
        Notice.instance.Send("AddSystemLog", model.name + " : " + speech);
        showSpeech.showSpeech(speech);
        //Debug.Log("관리자에게 " + speech);
        

        // TODO : 최적화 필요
        /*
        agentWindow = GameObject.FindGameObjectWithTag("AnimAgentController");

        if (agentWindow.GetComponent<Animator>().GetBool("isTrue"))
        {
            //Debug.Log(agentWindow.GetComponent<Animator>().GetBool("isTrue"));
            agentWindow.GetComponent<Animator>().SetBool("isTrue", false);
        }
        else if(oldUnit == model)
        {
            //Debug.Log(agentWindow.GetComponent<Animator>().GetBool("isTrue"));
            agentWindow.GetComponent<Animator>().SetBool("isTrue", true);
        }
         */
	}

    public void Speech(string speechKey)
    {
        string speech;
        if (model.speechTable.TryGetValue(speechKey, out speech))
        {
            Notice.instance.Send("AddPlayerLog", name + " : " + speech);
            TextAppearNormalEffect.Create(GetComponent<DestroyHandler>(), new Vector2(0, 0.5f), 4f, name + " : " + speech, Color.blue);
        }
    }

    public void MakeAccessory(List<TraitTypeInfo> input) { 
        foreach(TraitTypeInfo trait in input){
            this.accessoryUnit.SetAccessoryByTrait(trait);
        }
    }

    public void UIRecoilInput(int level, int target) {
        RectTransform targetRect = null;
        RecoilEffectUI recoil = null;
        switch (target)
        {
            case 0:
                recoil = ui.healthRecoil;
                targetRect = ui.healthRecoil.rect;
                break;
            case 1:
                recoil = ui.mentalRecoil;
                targetRect = ui.mentalRecoil.rect;
                break;
            default:
                Debug.LogError("Error in recoil");
                return;
        }

        List<RecoilEffectUI.RecoilArrow> arrowList = 
            RecoilEffectUI.MakeRecoilArrow(level * recoil.recoilCount);
       
        Vector2 initalPos = targetRect.localPosition;
        Queue<Vector2> queue = new Queue<Vector2>();
        foreach (RecoilEffectUI.RecoilArrow arrow in arrowList) {
            queue.Enqueue(RecoilEffectUI.GetVector(arrow, initalPos));
        }
        queue.Enqueue(initalPos);

        StartCoroutine(UIRecoil(queue, recoil));
    }

    IEnumerator UIRecoil(Queue<Vector2> queue, RecoilEffectUI recoil) {
        int val = queue.Count;
        float step = 0.1f / val;

        print(recoil.scale);
        while (queue.Count > 1)
        {
            yield return new WaitForSeconds(step);
            recoil.rect.localPosition = 1f * queue.Dequeue();
        }
        recoil.rect.localPosition = queue.Dequeue();
    }
}
