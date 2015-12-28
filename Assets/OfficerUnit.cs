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

    public float oldPos;
    public float oldPosY;
    public bool officerMove = false;
    public bool officerDead = false;
    public float zValue;
    public Text officerName;

    //private string oldSefira;

    public GameObject faceSprite;
    public GameObject hairSprite;

    private bool uiOpened = false;

    string speech = "";

    void LateUpdate() {
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
        }
    }

    void Start() {
        officerAnimator.SetInteger("Sepira", 1);
        officerAnimator.SetBool("Change", false);
        
        oldPos = transform.localPosition.x;
        oldPosY = transform.localPosition.y;

        officerName.text = model.name;

        faceSprite.GetComponent<SpriteRenderer>().sprite = ResourceCache.instance.GetSprite("Sprites/Agent/Face/Face_" + model.faceSpriteName + "_00");
        hairSprite.GetComponent<SpriteRenderer>().sprite = ResourceCache.instance.GetSprite("Sprites/Agent/Hair/Hair_M_" + model.hairSpriteName + "_00");
        
        //ChangeAgentUnifrom();과 동일?

    }

    private void UpdateDirection() {
        MapEdge currentEdge = model.GetCurrentEdge();
        int edgeDirection = model.GetEdgeDirection();

        if (currentEdge != null) {
            MapNode node1 = currentEdge.node1;
            MapNode node2 = currentEdge.node2;

            Vector2 pos1 = node1.GetPosition();
            Vector2 pos2 = node2.GetPosition();

            if (edgeDirection == 1)
            {
                Transform anim = renderNode.transform;

                Vector3 scale = anim.localScale;

                if (pos2.x - pos1.x > 0 && scale.x < 0)
                {
                    scale.x = -scale.x;
                }
                else if (pos2.x - pos1.x < 0 && scale.x > 0)
                {
                    scale.x = -scale.x;
                }
                anim.transform.localScale = scale;
            }
            else {
                Transform anim = renderNode.transform;
                Vector3 scale = anim.localScale;

                if (pos2.x - pos1.x > 0 && scale.x > 0)
                {
                    scale.x = -scale.x;
                }
                else if (pos2.x - pos1.x < 0 && scale.x < 0)
                {
                    scale.x = -scale.x;
                }
                anim.transform.localScale = scale;
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
        officerAnimator.SetInteger("Sepira", int.Parse(model.currentSefira));
        TimerCallback.Create(1, delegate()
        {
            if (officerAnimator.GetBool("Change"))
                officerAnimator.SetBool("Change", false);
        });

    }

    void FixedUpdate() {

        if (oldPos != transform.localPosition.x)
        {
            officerAnimator.SetBool("AgentMove", true);
            faceSprite.GetComponent<Animator>().SetBool("Move", true);
            hairSprite.GetComponent<Animator>().SetBool("Move", true);
            //officerAnimator.SetBool();

        }
        else {
            officerAnimator.SetBool("AgentMove", false);
            faceSprite.GetComponent<Animator>().SetBool("Move", false);
            hairSprite.GetComponent<Animator>().SetBool("Move", false);
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

    }

    void Update() {
        UpdateViewPosition();
        UpdateDirection();

        
    }

    


}
