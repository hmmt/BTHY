using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class OfficerUnit : MonoBehaviour {
    public OfficerModel model;
    //public GameObject officerWindow;
    public GameObject officerAttackedAnimator;
    public GameObject officerPlatform;
    
    public AgentSpeech showSpeech;
    public Animator officerAnimator;
    public GameObject renderNode;

    public GameObject puppetNode;
    public Animator puppetAnim;
    public AgentAnim animTarget;

    public float oldPos;
    public float oldPosY;
    public bool officerMove = false;
    public bool officerDead = false;
    public float zValue;
    public Text officerName;

    //private string oldSefira;
    private bool changeState = false;
    private string currentBool = "";
    public GameObject faceSprite;
    public GameObject hairSprite;

    private bool uiOpened = false;

    string speech = "";

    void LateUpdate() {
        /*
        foreach (var renderer in faceSprite.GetComponents<SpriteRenderer>()) {
            if (renderer.sprite.name == "Face_A_00")
                renderer.sprite = ResourceCache.instance.GetSprite("Sprites/Agent/Face/Face_" + model.faceSpriteName + "_00");
            else if (renderer.sprite.name == "Face_A_01")
                renderer.sprite = ResourceCache.instance.GetSprite("Sprites/Agent/Face/Face_" + model.faceSpriteName + "_01");
            else if (renderer.sprite.name == "Face_A_02")
                renderer.sprite = ResourceCache.instance.GetSprite("Sprites/Agent/Face/Face_" + model.faceSpriteName + "_02");
        }

        foreach (var renderer in hairSprite.GetComponents<SpriteRenderer>()) {
            if (renderer.sprite.name == "Hair_M_A_00")
                renderer.sprite = ResourceCache.instance.GetSprite("Sprites/Agent/Hair/Hair_M_" + model.hairSpriteName + "_00");
            else if (renderer.sprite.name == "Hair_M_A_01")
                renderer.sprite = ResourceCache.instance.GetSprite("Sprites/Agent/Hair/Hair_M_" + model.hairSpriteName + "_01");
            else if (renderer.sprite.name == "Hair_M_A_02")
            {
                renderer.sprite = ResourceCache.instance.GetSprite("Sprites/Agent/Hair/Hair_M_" + model.hairSpriteName + "_02");
                renderer.transform.localScale.Set(-1, 1, 1);
            }
        }*/
    }

    void Start() {
        AnimatorManager.instance.SaveAnimator(model.instanceId, puppetAnim);
        officerAnimator.SetInteger("Sepira", 1);
        officerAnimator.SetBool("Change", false);

        if(animTarget != null)
            //animTarget.SetClothes(Resources.LoadAll<Sprite>("Sprites/Agent/Test/AgentN1"));
        
        oldPos = transform.localPosition.x;
        oldPosY = transform.localPosition.y;

        officerName.text = model.name;

        //faceSprite.GetComponent<SpriteRenderer>().sprite = ResourceCache.instance.GetSprite("Sprites/Agent/Face/Face_" + model.faceSpriteName + "_00");
        //hairSprite.GetComponent<SpriteRenderer>().sprite = ResourceCache.instance.GetSprite("Sprites/Agent/Hair/Hair_M_" + model.hairSpriteName + "_00");
        
        //ChangeAgentUnifrom();과 동일?
        model.SetUnit(this);
    }

    private void UpdateDirection() {
		MovableObjectNode movable = model.GetMovableNode ();
		UnitDirection movableDirection = movable.GetDirection ();

		Transform puppet = puppetNode.transform;

		Vector3 puppetScale = puppet.localScale;

		if (movableDirection == UnitDirection.RIGHT)
		{
			if (puppetScale.x < 0)
				puppetScale.x = -puppetScale.x;
		}
		else
		{
			if (puppetScale.x > 0)
				puppetScale.x = -puppetScale.x;
		}
		puppet.transform.localScale = puppetScale;

		return;

        MapEdge currentEdge = model.GetCurrentEdge();
		EdgeDirection edgeDirection = model.GetEdgeDirection();

        if (currentEdge != null) {
            MapNode node1 = currentEdge.node1;
            MapNode node2 = currentEdge.node2;

            Vector2 pos1 = node1.GetPosition();
            Vector2 pos2 = node2.GetPosition();

			if (edgeDirection == EdgeDirection.FORWARD)
            {

                if (pos2.x - pos1.x > 0 && puppetScale.x < 0)
                {
                    puppetScale.x = -puppetScale.x;
                }
                else if (pos2.x - pos1.x < 0 && puppetScale.x > 0)
                {
                    puppetScale.x = -puppetScale.x;
                }
                puppetAnim.transform.localScale = puppetScale;
            }
            else {

                if (pos2.x - pos1.x > 0 && puppetScale.x > 0)
                {
                    puppetScale.x = -puppetScale.x;
                }
                else if (pos2.x - pos1.x < 0 && puppetScale.x < 0)
                {
                    puppetScale.x = -puppetScale.x;
                }
                puppetAnim.transform.localScale = puppetScale;
            }
        }
    }

    private bool visible = true;

    private void UpdateViewPosition() {
        if (isKilled) return;
		transform.localScale = new Vector3 (model.GetMovableNode ().currentScale, model.GetMovableNode ().currentScale, transform.localScale.z);
        MapEdge currentEdge = model.GetCurrentEdge();

        if (currentEdge != null && currentEdge.type == "door") {
            if (visible) {
                visible = false;
                Vector3 newPosition = model.GetCurrentViewPosition();
                newPosition.z = 100000f;
                transform.localPosition = newPosition;
            }
        }
        else
        {
            if (!visible)
            {
                visible = true;
            }
            Vector3 newPosition = model.GetCurrentViewPosition();
            newPosition.z = zValue;
            transform.localPosition = newPosition;
        }
    }

    private float waitTimer = 0;

    public void ChangeOfficerUniform() {
        officerAnimator.SetBool("Change", true);

        puppetAnim.SetBool("Change", true);

        puppetAnim.SetInteger("Sefira", int.Parse(model.currentSefira));
        TimerCallback.Create(1, delegate()
        {
            if (puppetAnim.GetBool("Change"))
                puppetAnim.SetBool("Change", false);
        });

    }

    void FixedUpdate() {
        /*

        if (!changeState)
        {

            switch (model.GetState())
            {
                case OfficerCmdState.MEMO_STAY:
                    changeState = true;
                    currentBool = "Memo";
                    puppetAnim.SetBool(currentBool, changeState);
                    break;
                case OfficerCmdState.DOCUMENT:
                    currentBool = "Document";
                    changeState = true;
                    puppetAnim.SetBool(currentBool, changeState);
                    break;
            }
        }
        else
        {
            if (model.GetState() == OfficerCmdState.IDLE || model.GetState() == OfficerCmdState.RETURN)
            {
                changeState = false;
                puppetAnim.SetBool(currentBool, changeState);
                currentBool = "";
            }
        }
        */
        
		if(AnimatorUtil.HasParameter(puppetAnim, "Move"))
		{
	        if (oldPos != transform.localPosition.x)
	        {
	            /*
	            puppetAnim.SetBool("Move", true);
	            faceSprite.GetComponent<Animator>().SetBool("Move", true);
	            hairSprite.GetComponent<Animator>().SetBool("Move", true);
	            //officerAnimator.SetBool();
	            */
	            puppetAnim.SetBool("Move", true);
	        }
	        else {
	            /*
	            officerAnimator.SetBool("AgentMove", false);
	            faceSprite.GetComponent<Animator>().SetBool("Move", false);
	            hairSprite.GetComponent<Animator>().SetBool("Move", false);
	             */
	            puppetAnim.SetBool("Move", false);
	        }
		}
        

        if (oldPosY != transform.localPosition.y)
        {
            officerPlatform.SetActive(true);
        }
        else {
            officerPlatform.SetActive(false);
        }

        oldPos = transform.localPosition.x;
        oldPosY = transform.localPosition.y;

        int randLyricTick = Random.Range(0, 3000);
        if (model.GetState() == OfficerAIState.IDLE && randLyricTick == 0 && model.mental > 0 ) {
            int randLyricsStroy = Random.Range(0, 10);
            if (randLyricsStroy < 8)
            {
              
                speech = AgentLyrics.instance.getLyricsByDay(PlayerModel.instance.GetDay());
            }
            else {
                speech = AgentLyrics.instance.getStoryLyrics();
            }

            //log send;
            showSpeech.showSpeech(speech);
        }

        //패닉 대사
        if (Input.GetKeyDown(KeyCode.O)) {
            model.TakeMentalDamage(20);
            Debug.Log("TakeMentalDamage " + model.mental + "/" + model.maxMental);
        }
        

        if (model.panicFlag) { 
            //make panic Action
            Debug.Log("발견");
            showSpeech.showSpeech("What the Fuck???");
            model.panicFlag = false;
        }
    }

    void Update() {
        UpdateViewPosition();
        UpdateDirection();
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
		model.OnClick ();
        if (model.nullParasite != null) {
            //SuppressWindow.CreateWindow(model.nullParasite.GetModel());
            SuppressWindow.CreateNullCreatureSuppressWindow(model.nullParasite.GetModel(), this.model);
        }
        else if (SuppressWindow.currentWindow.nullEscapedList.Count > 0 && this.model.IsSuppable() == false)
        {
            SuppressWindow.CreateNullCreatureSuppressWindow(SuppressWindow.currentWindow.nullEscapedList[0].GetModel(), this.model);
        }
	}



    public bool isMovingByMannually = false;
    bool isMovingStarted = false;
    bool isKilled = false;//temporary
    IEnumerator MannualMoving(Vector3 pos)
    {
        Transform target = this.gameObject.transform;
        Vector3 initial = new Vector3(target.position.x, target.position.y, target.position.z);
        Vector3 reference = new Vector3(pos.x - target.position.x,
            pos.y - target.position.y,
            0f);
        int cnt = 3;
        isKilled = true;
        while (cnt > 0)
        {
            yield return new WaitForSeconds(0.1f);
            target.position = new Vector3(initial.x + (reference.x / 3f) * (4 - cnt), initial.y, initial.z);
            cnt--;
        }
        isMovingByMannually = true;
        
    }

    public bool MannualMovingCall(Vector3 pos)
    {
        if (!isMovingStarted)
        {
            isMovingStarted = true;
            isMovingByMannually = false;
            StartCoroutine(MannualMoving(pos));
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
}
