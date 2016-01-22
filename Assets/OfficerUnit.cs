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
        officerAnimator.SetInteger("Sepira", 1);
        officerAnimator.SetBool("Change", false);

        if(animTarget != null)
            animTarget.SetClothes(Resources.LoadAll<Sprite>("Sprites/Agent/Test/AgentN1"));
        
        oldPos = transform.localPosition.x;
        oldPosY = transform.localPosition.y;

        officerName.text = model.name;

        faceSprite.GetComponent<SpriteRenderer>().sprite = ResourceCache.instance.GetSprite("Sprites/Agent/Face/Face_" + model.faceSpriteName + "_00");
        hairSprite.GetComponent<SpriteRenderer>().sprite = ResourceCache.instance.GetSprite("Sprites/Agent/Hair/Hair_M_" + model.hairSpriteName + "_00");
        
        //ChangeAgentUnifrom();과 동일?
        model.SetUnit(this);
    }

    private void UpdateDirection() {
        MapEdge currentEdge = model.GetCurrentEdge();
        int edgeDirection = model.GetEdgeDirection();

        if (currentEdge != null) {
            MapNode node1 = currentEdge.node1;
            MapNode node2 = currentEdge.node2;

            Vector2 pos1 = node1.GetPosition();
            Vector2 pos2 = node2.GetPosition();

            Transform puppetAnim = puppetNode.transform;
            Vector3 puppetScale = puppetAnim.localScale;

            if (edgeDirection == 1)
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
        if (model.GetState() == OfficerCmdState.IDLE && randLyricTick == 0 && model.mental > 0 ) {
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

    


}
