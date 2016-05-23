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

    public GameObject puppetNode;
    public Animator puppetAnim;
    public AgentAnim animTarget;

    public float oldPos;
    public float oldPosY;
    public bool officerMove = false;
    public bool officerDead = false;
    public float zValue;
    public float tempZval;
    public Text officerName;

    //private string oldSefira;
    private bool changeState = false;
    private string currentBool = "";

    private bool uiOpened = false;

    string speech = "";

    public const float backZVal = 0;

    public Dictionary<string, SoundEffectPlayer> sounds;

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

        //if(animTarget != null)
            //animTarget.SetClothes(Resources.LoadAll<Sprite>("Sprites/Agent/Test/AgentN1"));
        
        oldPos = transform.localPosition.x;
        oldPosY = transform.localPosition.y;

        officerName.text = model.name;

        //faceSprite.GetComponent<SpriteRenderer>().sprite = ResourceCache.instance.GetSprite("Sprites/Agent/Face/Face_" + model.faceSpriteName + "_00");
        //hairSprite.GetComponent<SpriteRenderer>().sprite = ResourceCache.instance.GetSprite("Sprites/Agent/Hair/Hair_M_" + model.hairSpriteName + "_00");
        
        //ChangeAgentUnifrom();과 동일?
        model.SetUnit(this);
        this.animTarget.Init(this.model);
    }

    private void UpdateDirection() {
        if (blockDir) return;
        if (this.model.startSpecialAction) {
            if (this.model.lookingDir != LOOKINGDIR.NOCARE) {
                Transform localpuppet = puppetNode.transform.parent;

                Vector3 localpuppetScale = localpuppet.localScale;
                
                if (this.model.lookingDir == LOOKINGDIR.LEFT)
                {
                    if (localpuppetScale.x < 0)
                        localpuppetScale.x = -localpuppetScale.x;
                }
                else {
                    if (localpuppetScale.x > 0)
                    {
                        localpuppetScale.x = -localpuppetScale.x;
                    }
                }

                localpuppet.transform.localScale = localpuppetScale;
                return;
            }
        }

		MovableObjectNode movable = model.GetMovableNode ();
		UnitDirection movableDirection = movable.GetDirection ();

		Transform puppet = puppetNode.transform.parent;

		Vector3 puppetScale = puppet.localScale;

		if (movableDirection == UnitDirection.RIGHT)
		{
			if (puppetScale.x > 0)
				puppetScale.x = -puppetScale.x;
		}
		else
		{
			if (puppetScale.x < 0)
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
        if (blockMoving) {
            return;
        }
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
	        if (model.GetMovableNode().IsMoving())
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
            //Debug.Log("발견");
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
    public bool blockMoving = false;
    public bool blockDir = false;
    IEnumerator MannualMoving(Vector3 pos, bool blockMoving, bool zVal, bool shouldMoveZaxis, bool zPos)
    {
        Transform target = this.gameObject.transform;
        Vector3 initial = new Vector3(target.position.x, target.position.y, target.position.z);

        tempZval = this.zValue + 0.05f;
        float z = tempZval;
        float toScale;
        if (zVal)
        {
            z = tempZval;
            toScale = -0.1f;
        }
        else
        {
            z = this.zValue;
            toScale = 0.1f;
        }

        Vector3 reference = new Vector3(pos.x - target.position.x,
            pos.y - target.position.y,
            z);
        int cntMax = 20;
        int cnt = cntMax;
        float unitScale = (toScale / (float)(cntMax));
        //isKilled = true;
        this.blockMoving = blockMoving;
        puppetAnim.SetBool("Move", true);
        Vector3 initialScale = puppetNode.transform.localScale;
        
        while (cnt > 0)
        {
            puppetAnim.SetBool("Move", true);
            yield return new WaitForSeconds(0.01f);

            if (!zVal)
            {
                target.position = new Vector3(initial.x + (reference.x / (float)cntMax) * ((cntMax - 1) - cnt),
                                              initial.y + (reference.y / (float)cntMax) * ((cntMax - 1) - cnt),
                                              initial.z + (reference.z / (float)cntMax) * ((cntMax - 1) - cnt));

            }
            else {
                target.position = new Vector3(initial.x + (reference.x / (float)cntMax) * ((cntMax - 1) - cnt),
                                              initial.y + (reference.y / (float)cntMax) * ((cntMax - 1) - cnt),
                                              initial.z );
            }
            /*target.localScale = new Vector3(initialScale.x + unitScale,
                                            initialScale.x + unitScale,
                                            initialScale.z);
             */
            if (cnt % 2 == 0 && shouldMoveZaxis)
            {
                /*
                puppetNode.transform.localScale = new Vector3(puppetNode.transform.localScale.x + unitScale,
                                                            puppetNode.transform.localScale.y + unitScale,
                                                            initialScale.z);*/
                float factor = 1;
                if (puppetNode.transform.localScale.x < 0) {
                    factor = -1;
                }
                puppetNode.transform.localScale = new Vector3(initialScale.x + factor * unitScale * (cntMax - cnt),
                                                              initialScale.y + unitScale * (cntMax - cnt),
                                                              initialScale.z);
                /*
                puppetNode.transform.localScale = new Vector3(initialScale.x * unitScale * (((float)(cntMax - cnt) / cntMax)),
                                                              initialScale.y * unitScale * (((float)(cntMax - cnt) / cntMax)),
                                                              initialScale.z);
                 */
            }
           
            cnt--;
        }
        puppetAnim.SetBool("Move", false);
        isMovingByMannually = true;
        
    }

    /*
    IEnumerator MannualMoving(Vector3 pos, bool moveZval, bool moveBackward, int unitCount, float totalTime, bool moveAnim) {
        Transform target = gameObject.transform;

        Vector3 initialPos = new Vector3(target.position.x, target.position.y, target.position.z);
        Vector3 initialScale = puppetNode.transform.localScale;

        Vector3 reference = new Vector3(pos.x - target.position.x, pos.y - target.position.y,
                                        pos.z);

        int cntMax = unitCount;
        int cnt = cntMax;

        float unitScale = afterScale * (float)1 / cntMax;
        float scaleFactor = 1;
        float currentScale = initialScale.y;

        tempZval = pos.z;
        this.blockMoving = true;

        if (afterScale == initialScale.y)
        {
            scaleFactor = 0;
        }
        else if (afterScale < initialScale.y){
            scaleFactor = -1;
        }

        unitScale *= scaleFactor;

        while (cnt > 0) {
            if (moveAnim)
            {
                this.puppetAnim.SetBool("Move", true);
            }
            
            yield return new WaitForSeconds(totalTime / (float)cntMax);
            float zValue = 0f;
            if (moveZval)
            {
                zValue = initialPos.z + (reference.z / (float)cntMax) * ((cntMax - 1) - cnt);
            }
            else {
                zValue = initialPos.z;
            }

            target.position = new Vector3(initialPos.x + (reference.x / (float)cntMax) * ((cntMax - 1) - cnt),
                                          initialPos.y + (reference.y / (float)cntMax) * ((cntMax - 1) - cnt),
                                          zValue);

            if (moveZval) {
                float factor = 1;
                if (puppetNode.transform.localScale.x < 0) {
                    factor = -1;
                }

                target.localScale = new Vector3(initialScale.x + factor * unitScale * (cntMax - cnt),
                                                initialScale.y + unitScale * (cntMax - cnt),
                                                initialScale.z);
                
                
            }

            cnt--;
        }

        if (moveAnim)
        {
            this.puppetAnim.SetBool("Move", false);
        }
        isMovingByMannually = true;
        
    }
    
    */

    IEnumerator MannualMoving(Vector3 pos, bool block, bool moveZ, bool moveAnim, bool scaling, bool small, float unitWaitTime) {
        Transform target = gameObject.transform;

        Vector3 initialPos = new Vector3(target.position.x, target.position.y, target.position.z);
        Vector3 reference = new Vector3(pos.x - target.position.x, pos.y - target.position.y, pos.z - target.position.z);
        Vector3 initialScale = puppetNode.transform.localScale;
        int cntMax = 20;
        int cnt = cntMax;
        this.blockMoving = block;

        float toScale = 1f;
        if (scaling) {
            if (small)
            {
                toScale = -0.1f;
            }
            else {
                toScale = 0.1f;
            }
        }

        float unitScale = (toScale / (float)cntMax);

        while (cnt > 0) {
            if (moveAnim) {
                this.puppetAnim.SetBool("Move", true);
            }

            yield return new WaitForSeconds(unitWaitTime);
            float zValue = this.zValue;

            if (moveZ) {
                zValue = initialPos.z + (reference.z / (float)cntMax) * ((cntMax - 1) - cnt);
            }

            target.position = new Vector3(initialPos.x + (reference.x / (float)cntMax) * ((cntMax - 1) - cnt),
                                          initialPos.y + (reference.y / (float)cntMax) * ((cntMax - 1) - cnt),
                                          zValue);

            if (scaling) {
                if (cnt % 2 == 0) {
                    float factor = 1;
                    if (puppetNode.transform.localScale.x < 0) {
                        factor = -1;
                    }

                    puppetNode.transform.localScale = new Vector3(initialScale.x + factor * unitScale * (cntMax - cnt),
                                                              initialScale.y + unitScale * (cntMax - cnt),
                                                              initialScale.z);
                }
            }
            cnt--;
        }
        if (moveAnim) {
            this.puppetAnim.SetBool("Move", false);
        }

        isMovingByMannually = true;
    }

    public bool MannualMovingCall(Vector3 pos, bool blockMov, bool moveZ, bool moveAnim, bool scailing, bool small, float unitWaitTime)
    {
        if (!isMovingStarted)
        {
            isMovingStarted = true;
            isMovingByMannually = false;
            StartCoroutine(MannualMoving(pos, blockMov, moveZ, moveAnim, scailing, small, unitWaitTime));
            return false;
        }

        if (isMovingByMannually)
        {

            isMovingStarted = false;
            return true;
        }
        return false;
    }


    public bool MannualMovingCall(Vector3 pos, bool mode, bool moveZaxis, bool zVal)
    {
        if (!isMovingStarted)
        {
            isMovingStarted = true;
            isMovingByMannually = false;
            StartCoroutine(MannualMoving(pos, true , mode, moveZaxis, zVal));
            return false;
        }

        if (isMovingByMannually)
        {
            
            isMovingStarted = false;
            return true;
        }
        return false;
    }
    /*
    public bool MannualMovingCall(Vector3 pos, bool moveZ, int count, float totalTime, bool moveAnim, float afterScale) {
        if (!isMovingStarted) {
            isMovingStarted = true;
            isMovingByMannually = false;
            StartCoroutine(MannualMoving(pos, moveZ, count, totalTime, moveAnim, afterScale));
            return false;
        }

        if (isMovingByMannually){
            isMovingStarted = false;
            return true;
        }
        return false;
    }
    */
    public bool CheckMannualMovingEnd() {
        
        if (isMovingByMannually) {
            isMovingStarted = false;
            return true;
        }
        return false;
    }

    public void ReleaseUpdatePosition() {
        this.blockMoving = false;
    }

    public void PlaySound(string src, string key, bool isLoop)
    {
        SoundEffectPlayer output = null;

        if (isLoop)
        {
            output = SoundEffectPlayer.Play(src, this.gameObject.transform);
        }
        else
        {
            output = SoundEffectPlayer.PlayOnce(src, this.gameObject.transform.position);
        }

        if (key != null)
        {
            this.sounds.Add(key, output);
        }
    }

    public void StopSound(string key)
    {
        SoundEffectPlayer sep = null;

        if (this.sounds.TryGetValue(key, out sep))
        {
            sep.Stop();
        }
    }
}
