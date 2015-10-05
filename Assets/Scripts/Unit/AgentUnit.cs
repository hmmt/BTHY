using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AgentUnit : MonoBehaviour {

    public AgentModel model;

    public GameObject agentWindow;
    public GameObject agentAttackedAnimator;
    public GameObject agentPlatform;

    public agentSkillDoing showSkillIcon;
    public AgentSpeech showSpeech;

    public Animator agentAnimator;
    public GameObject renderNode;

    public float oldPos;
    public float oldPosY;
    public bool agentMove=false;
    public bool agentDead = false;

    public TextMesh agentName;

    private string oldSefira;

    //각 직원 부위 스프라이트 결정 변수
    public GameObject faceSprite;
    public GameObject deadSprite;
    public GameObject hairSprite;

    public UnityEngine.UI.Text speachText;

    // layer에서 z값 순서 정하기 위한 값.
    public float zValue;

    //직원 대사
    string speach = "";

    void LateUpdate()
    {
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
    }

    void  Start()
    {
        agentAnimator.SetInteger("Sepira", 1);
        agentAnimator.SetBool("Change", false);
        oldPos = transform.localPosition.x;
        oldPosY = transform.localPosition.y;
        oldSefira = "1";
        agentName.text = model.name;

        faceSprite.GetComponent<SpriteRenderer>().sprite = ResourceCache.instance.GetSprite("Sprites/Agent/Face/Face_" + model.faceSpriteName + "_00");
        hairSprite.GetComponent<SpriteRenderer>().sprite = ResourceCache.instance.GetSprite("Sprites/Agent/Hair/Hair_M_" + model.hairSpriteName + "_00");

        ChangeAgentUniform();
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
                else
                {
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

	private void UpdateViewPosition()
	{
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
        Debug.Log("직원 복장 변경");

        agentAnimator.SetBool("Change", true);

        Debug.Log(agentAnimator.GetBool("Change"));

        if (model.currentSefira == "1")
        {
            agentAnimator.SetInteger("Sepira", 1);
        }

        else if (model.currentSefira == "2")
        {
            agentAnimator.SetInteger("Sepira", 2);
        }

        else if (model.currentSefira == "3")
        {
            agentAnimator.SetInteger("Sepira", 3);
        }

        else if (model.currentSefira == "4")
        {
            agentAnimator.SetInteger("Sepira", 4);
        }

        TimerCallback.Create(1, delegate()
        {
            if (agentAnimator.GetBool("Change"))
                agentAnimator.SetBool("Change", false);
        });
        oldSefira = model.currentSefira;
    }


	void FixedUpdate()
	{
        if (oldSefira != model.currentSefira)
        {
            ChangeAgentUniform();
        }

        if (oldPos != transform.localPosition.x)
        {
            agentAnimator.SetBool("AgentMove", true);
            faceSprite.GetComponent<Animator>().SetBool("Move", true);
            hairSprite.GetComponent<Animator>().SetBool("Move", true);
        }
        else
        {
            agentAnimator.SetBool("AgentMove", false);
            faceSprite.GetComponent<Animator>().SetBool("Move", false);
            hairSprite.GetComponent<Animator>().SetBool("Move", false);
        }

        if (oldPosY != transform.localPosition.y)
        {
            agentPlatform.SetActive(true);
        }

        else
        {
            agentPlatform.SetActive(false);
        }

        oldPosY = transform.localPosition.y;
        oldPos = transform.localPosition.x;

        int randLyricsTick = Random.Range(0, 3000);
        if (model.GetState() == AgentCmdState.IDLE && randLyricsTick == 0 && model.mental > 0 && !speachText.IsActive())
        {
            int randLyricsStory = Random.Range(0, 10);
            if (randLyricsStory < 8)
            {
                speach = AgentLyrics.instance.getLyricsByDay(PlayerModel.instance.GetDay());
            }
            else
            {
                speach = AgentLyrics.instance.getStoryLyrics();
            }
            Notice.instance.Send("AddPlayerLog", name + " : " + speach);
            Notice.instance.Send("AddSystemLog", name + " : " + speach);
            showSpeech.showSpeech(speach);
        }

        if (model.mental <= 0 && !speachText.IsActive())
        {
            speach = AgentLyrics.instance.getPanicLyrics();
            Notice.instance.Send("AddPlayerLog", name + " : " + speach);
            Notice.instance.Send("AddSystemLog", name + " : " + speach);
            showSpeech.showSpeech(speach);
            Debug.Log("패닉대사 " + speach);
        }
	}

	void Update()
	{
		UpdateViewPosition();
		UpdateDirection();
		SetCurrentHP (model.hp);
		UpdateMentalView ();

	}

	public void SetMaxHP(int maxHP)
	{
		GetComponentInChildren<AgentHPBar> ().SetMaxHP (maxHP);
	}

	public void SetCurrentHP(int hp)
	{
		GetComponentInChildren<AgentHPBar> ().SetCurrentHP (hp);
	}

	public void UpdateMentalView()
	{
		GetComponentInChildren<MentalViewer> ().SetMentalRate ((float)model.mental / (float)model.maxMental);
	}

	public void OpenStatusWindow()
	{
        AgentModel oldUnit = (AgentStatusWindow.currentWindow != null) ? AgentStatusWindow.currentWindow.target : null;
		AgentStatusWindow.CreateWindow (model);
        if (CollectionWindow.currentWindow != null)
        CollectionWindow.currentWindow.CloseWindow();

        speach = AgentLyrics.instance.getOnClickLyrics();
        Notice.instance.Send("AddPlayerLog", name + " : " + speach);
        Notice.instance.Send("AddSystemLog", name + " : " + speach);
        showSpeech.showSpeech(speach);
        Debug.Log("관리자한테 " + speach);

        // TODO : 최적화 필요
        agentWindow = GameObject.FindGameObjectWithTag("AnimAgentController");

        if (agentWindow.GetComponent<Animator>().GetBool("isTrue"))
        {
            Debug.Log(agentWindow.GetComponent<Animator>().GetBool("isTrue"));
            agentWindow.GetComponent<Animator>().SetBool("isTrue", false);
        }
        else if(oldUnit == model)
        {
            Debug.Log(agentWindow.GetComponent<Animator>().GetBool("isTrue"));
            agentWindow.GetComponent<Animator>().SetBool("isTrue", true);
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


}
