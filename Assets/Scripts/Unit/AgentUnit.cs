using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public enum RecoilArrow
{
    LEFTUP, UP, RIGHTUP, RIGHT, RIGHTDOWN, DOWN, LEFTDOWN, LEFT
}

[System.Serializable]
public class AgentUnitUI
{
    public Slider hp;
    public RectTransform workIcon;
    public bool Activated = false;
    public Image mental;
    public Image touchArea;
    public Text Name;

    public RecoilEffectUI healthRecoil;
    public RecoilEffectUI mentalRecoil;

    public void initUI() {
        hp.gameObject.SetActive(false);
        workIcon.gameObject.SetActive(false);
        touchArea.gameObject.SetActive(false);
    }

    public void activateUI(AgentModel model) {
        hp.gameObject.SetActive(true);
        workIcon.gameObject.SetActive(true);
        touchArea.gameObject.SetActive(true);
        this.Activated = true;
        hp.maxValue = model.maxHp;
        hp.value = model.hp;
        Name.text = model.name;
    }

    //0번 : 검은색(붕괴 시 아이콘), 1번 : 멀쩡한 상태 아이콘
    //1번의 alpha값을 수정하는 것을 통해 효과
    public void setUIValue(AgentModel model) {
        if (!Activated) return;
        /*
        Color c = workIcon.GetChild(1).GetComponent<Image>().color;
        c.a = (float)model.mental / model.maxMental;
        */
        float currentAlpha = (float)model.mental / model.maxMental;
        mental.color = new Color(currentAlpha, currentAlpha, currentAlpha, 1);

        //workIcon.GetChild(1).GetComponent<Image>().color = c;
		hp.value = model.hp / (float)model.maxHp;
    }

}

[System.Serializable]
public class RecoilEffectUI :RecoilEffect{
    
    public RectTransform rect;

    public RecoilEffectUI() {
        base.targetTransform = rect;
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

    //public Text agentName;

    private string oldSefira;

    //각 직원 부위 스프라이트 결정 변수

    public UnityEngine.UI.Text speechText;

    public AgentUnitUI ui;

    // layer에서 z값 순서 정하기 위한 값.
	private float zValueDefault;
    public float zValue;

    private bool uiOpened = false;

    public AccessoryUnit accessoryUnit;

    public bool blockScaling = false;

    public bool dead = false;

    Dictionary<string, SoundEffectPlayer> sounds = new Dictionary<string, SoundEffectPlayer>();

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
        //agentName.text = model.name;

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

        animTarget.Init(this.model);
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
        if (blockMoving) return;
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
	        //if (oldPos != transform.localPosition.x)
			if(model.GetMovableNode().IsMoving())
	        {
	            //agentAnimator.SetBool("AgentMove", true);
	           // faceSprite.GetComponent<Animator>().SetBool("Move", true);
	           // hairSprite.GetComponent<Animator>().SetBool("Move", true);
				//Debug.Log("MOVE!!!!!!!!!!!!");
	            animTarget.SetSpeed(model.movement / 4.0f);
	            puppetAnim.SetBool("Move", true);
	        }
	        else
	        {
	          //  agentAnimator.SetBool("AgentMove", false);
	         //   faceSprite.GetComponent<Animator>().SetBool("Move", false);
	          //  hairSprite.GetComponent<Animator>().SetBool("Move", false);

				//Debug.Log("STOP???????????????????????");
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

        if (Input.GetKeyDown(KeyCode.Alpha8)) {
            this.model.TakeMentalDamage(1000);
        }
	}

	void Update()
	{
        if (dead) return;
        if (model.isDead()) {
            ui.initUI();
            dead = true;
            return;
        }
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

        if (model.nullParasite != null) {
            Debug.Log("Null Parasited");
            SuppressWindow.CreateNullCreatureSuppressWindow(model.nullParasite.GetModel(), this.model);
        }
        else if (SuppressWindow.currentWindow.nullEscapedList.Count > 0 && this.model.IsSuppable() == false)
        {
            Debug.Log("Agent Null");
            
            SuppressWindow.CreateNullCreatureSuppressWindow(SuppressWindow.currentWindow.nullEscapedList[0].GetModel(), this.model);
        }
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

	public void MakeAccessory(string imgpos, string imgsrc)
	{
		accessoryUnit.SetAccessory (imgpos, imgsrc, 1f);
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

        List<RecoilArrow> arrowList = 
            RecoilEffectUI.MakeRecoilArrow(level * recoil.recoilCount);
       
        Vector3 initalPos = targetRect.localPosition;
        Queue<Vector3> queue = new Queue<Vector3>();
        foreach (RecoilArrow arrow in arrowList) {
            queue.Enqueue(RecoilEffectUI.GetVector(arrow, initalPos, recoil.scale));
        }
        queue.Enqueue(initalPos);
        if (this.gameObject.activeSelf) {

            StartCoroutine(UIRecoil(queue, recoil));
        }
    }

    IEnumerator UIRecoil(Queue<Vector3> queue, RecoilEffectUI recoil) {
        int val = queue.Count;
        float step = recoil.maxTime / val;

        while (queue.Count > 1)
        {
            yield return new WaitForSeconds(step);
            recoil.rect.localPosition = queue.Dequeue();
        }
        recoil.rect.localPosition = queue.Dequeue();
    }



    public bool isMovingByMannually = false;
    bool isMovingStarted = false;
    public bool blockMoving = false;
    IEnumerator MannualMoving(Vector3 pos , bool blockMoving) {
        Transform target = this.gameObject.transform;
        Vector3 initial = new Vector3(target.position.x, target.position.y, target.position.z);
        Vector3 reference = new Vector3(pos.x - target.position.x,
            pos.y - target.position.y,
            0f);
        int cnt = 3;
        this.blockMoving = blockMoving;
        
        while (cnt > 0) {
            yield return new WaitForSeconds(0.1f);
            target.position = new Vector3(initial.x + (reference.x / 3f) * (4 - cnt), initial.y, initial.z);
            cnt--;
        }
        
        isMovingByMannually = true;
    }

    public bool MannualMovingCall(Vector3 pos) {
        if (!isMovingStarted)
        {
            isMovingStarted = true;
            isMovingByMannually = false;
            StartCoroutine(MannualMoving(pos, true));
            return false;
        }

        if (isMovingByMannually) {
            isMovingByMannually = false;
            isMovingStarted = false;
            return true;
        }
        return false;
    }

    IEnumerator MannualMoving(Vector3 pos, bool blockMoving, float unitWaitTime)
    {
        Transform target = this.gameObject.transform;
        Vector3 initial = new Vector3(target.position.x, target.position.y, target.position.z);
        Vector3 reference = new Vector3(pos.x - target.position.x,
            pos.y - target.position.y,
            0f);
        int cnt = 3;
        this.blockMoving = blockMoving;

        while (cnt > 0)
        {
            yield return new WaitForSeconds(unitWaitTime);
            target.position = new Vector3(initial.x + (reference.x / 3f) * (4 - cnt), initial.y + (reference.y / 3f) * (4 - cnt), initial.z);
            cnt--;
        }

        isMovingByMannually = true;
    }

    public bool MannualMovingCall(Vector3 pos, float unitWaitTime)
    {
        if (!isMovingStarted)
        {
            isMovingStarted = true;
            isMovingByMannually = false;
            StartCoroutine(MannualMoving(pos, true, unitWaitTime));
            return false;
        }

        if (isMovingByMannually)
        {
            isMovingByMannually = false;
            isMovingStarted = false;
            return true;
        }
        return false;
    }

    public void PlaySound(string src, string key, bool isLoop) {
        SoundEffectPlayer output = null;

        if (isLoop)
        {
            output = SoundEffectPlayer.Play(src, this.gameObject.transform);
        }
        else {
            output = SoundEffectPlayer.PlayOnce(src, this.gameObject.transform.position);
        }

        if (key != null) {
            this.sounds.Add(key, output);
        }
    }

    public void StopSound(string key) {
        SoundEffectPlayer sep = null;

        if(this.sounds.TryGetValue(key, out sep)){
            sep.Stop();
        }
    }
}
