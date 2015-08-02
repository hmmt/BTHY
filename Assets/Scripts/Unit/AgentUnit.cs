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

    public float oldPos;
    public float oldPosY;
    public bool agentMove=false;

    public TextMesh agentName;

    private string oldSefira;

    void  Start()
    {
        agentAnimator.SetInteger("Sepira", 1);
        agentAnimator.SetBool("Change", false);
        oldPos = transform.localPosition.x;
        oldPosY = transform.localPosition.y;
        oldSefira = model.currentSefira;
        agentName.text = model.name;
        //currentNode = MapGraph.instance.GetNodeById("1001002");
    }

	private void UpdateDirection()
	{
        MapEdge currentEdge = model.GetCurrentEdge();
        int edgeDirection = model.GetEdgeDirection();

		if(currentEdge != null)
		{
			MapNode node1 = currentEdge.node1;
			MapNode node2 = currentEdge.node2;
			Vector2 pos1 = node1.GetPosition();
			Vector2 pos2 = node2.GetPosition();

			if(edgeDirection == 1)
			{
				Transform anim = transform.Find("Anim");
				Vector3 scale = anim.localScale;
				if(pos2.x - pos1.x > 0 && scale.x < 0)
				{
					scale.x = -scale.x;
				}
				else if(pos2.x - pos1.x < 0 && scale.x > 0)
				{
					scale.x = -scale.x;
				}
				anim.transform.localScale = scale;
			}
			else
			{
				Transform anim = transform.Find("Anim");
				Vector3 scale = anim.localScale;
				if(pos2.x - pos1.x > 0 && scale.x > 0)
				{
					scale.x = -scale.x;
				}
				else if(pos2.x - pos1.x < 0 && scale.x < 0)
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
			transform.localPosition = model.GetCurrentViewPosition();
		}
	}

	////

	private float waitTimer = 0;

	// if map is destroyed....?


	void FixedUpdate()
	{
        if (oldSefira != model.currentSefira)
        {

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

        if (oldPos != transform.localPosition.x)
        {
            agentAnimator.SetBool("AgentMove", true);
        }
        else
        {
            agentAnimator.SetBool("AgentMove", false);
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
