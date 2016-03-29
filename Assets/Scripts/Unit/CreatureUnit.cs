using UnityEngine;
using System.Collections.Generic;

public class CreatureUnit : MonoBehaviour {
    

    public CreatureModel model;

    public IsolateRoom room;
    public SpriteRenderer spriteRenderer;
    public SpriteRenderer returnSpriteRenderer;


    // ?
    public CreatureAnimScript animTarget;

    // 아직 안 씀

    
    public Animator creatureAnimator;

    private Vector3 directionScaleFactor = new Vector3(1f, 1f, 1f);
    private Vector3 scaleFactor = new Vector3(1f, 1f, 1f);

    Vector2 oldScale;

    private Vector3 viewPosition;
    private bool visible = true;

    private bool mousePointEnter = false;

   private void UpdateViewPosition()
   {
       MapEdge currentEdge = model.GetCurrentEdge();

       if (currentEdge != null && currentEdge.type == "door")
       {
           if (visible)
           {
               visible = false;
               Vector3 newPosition = model.GetCurrentViewPosition();
               newPosition.z = 100000f;
               viewPosition = newPosition;
           }
       }
       else
       {
           if (!visible)
           {
               visible = true;
           }
           viewPosition = model.GetCurrentViewPosition();
       }
       transform.localPosition = viewPosition;
   }

   private void UpdateDirection()
   {
		MovableObjectNode movable = model.GetMovableNode ();
		UnitDirection movableDirection = movable.GetDirection ();
		/*
		Transform puppet = puppetNode.transform;

		Vector3 puppetScale = puppet.localScale;
		*/
		Vector3 scale = directionScaleFactor;

		if (movableDirection == UnitDirection.RIGHT)
		{
			if (scale.x < 0) {
				scale.x = -scale.x;
			}
		}
		else
		{
			if (scale.x > 0) {
				scale.x = -scale.x;
			}
		}
		directionScaleFactor = scale;

		return;
		/*
       MapEdge currentEdge = model.GetCurrentEdge();
       int edgeDirection = model.GetMovableNode().GetEdgeDirection();

       if (model.lookAtTarget != null)
       {
           Vector2 myPosition = model.GetCurrentViewPosition();
           Vector2 targetPosition = model.lookAtTarget.GetCurrentViewPosition();
           Vector3 scale = directionScaleFactor;

           if (myPosition.x > targetPosition.x && scale.x > 0)
           {
               scale.x = -scale.x;
           }
           else if (myPosition.x < targetPosition.x && scale.x < 0)
           {
               scale.x = -scale.x;
           }

           directionScaleFactor = scale;
       }
       else
       {
           if (currentEdge != null)
           {
               MapNode node1 = currentEdge.node1;
               MapNode node2 = currentEdge.node2;
               Vector2 pos1 = node1.GetPosition();
               Vector2 pos2 = node2.GetPosition();

               if (edgeDirection == 1)
               {
                   Vector3 scale = directionScaleFactor;

                   if (pos2.x - pos1.x > 0 && scale.x < 0)
                   {
                       scale.x = -scale.x;
                   }
                   else if (pos2.x - pos1.x < 0 && scale.x > 0)
                   {
                       scale.x = -scale.x;
                   }
                   directionScaleFactor = scale;
               }
               else
               {
                   Vector3 scale = directionScaleFactor;

                   if (pos2.x - pos1.x > 0 && scale.x > 0)
                   {
                       scale.x = -scale.x;
                   }
                   else if (pos2.x - pos1.x < 0 && scale.x < 0)
                   {
                       scale.x = -scale.x;
                   }
                   directionScaleFactor = scale;
               }
           }
       }
       */
   }

   private void UpdateScale()
   {
       return;
       Vector3 mouseScale = new Vector3(1, 1, 1);
       if (mousePointEnter)
       {
           mouseScale = new Vector3(1.2f, 1.2f, 1.2f);
       }
       
       creatureAnimator.transform.localScale = new Vector3(
           directionScaleFactor.x * scaleFactor.x * mouseScale.x,
           directionScaleFactor.y * scaleFactor.y * mouseScale.y,
           directionScaleFactor.z * scaleFactor.z * mouseScale.z
           );

       Debug.Log(creatureAnimator.transform.localScale
           +" " +directionScaleFactor + " " + scaleFactor + " " + mouseScale
           
           );

		if (animTarget != null)
		{
			Vector3 scale = animTarget.transform.localScale;
			if (scale.x < 0 && directionScaleFactor.x > 0)
				scale.x = -scale.x;
			if (scale.x > 0 && directionScaleFactor.x < 0)
				scale.x = -scale.x;
			animTarget.transform.localScale = scale;
				
			/*
			animTarget.transform.localScale = new Vector3 (
				directionScaleFactor.x,
				directionScaleFactor.y,
				directionScaleFactor.z
			);
			*/
		}
   }
    void FixedUpdate()
	{
		UpdateViewPosition();
        UpdateDirection();

        if (Input.GetKeyDown(KeyCode.Q)) {
            if (this.model.metadataId == 100003)
            {
                model.SubFeeling(10);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1) && this.model.script is SingingMachine) {
            this.model.SubFeeling(200);
            
            //this.model.script.skill.SkillActivate();
        }
        /*
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            AnimatorManager.instance.ChangeAnimatorByName(SefiraManager.instance.getSefira("1").agentList[0].instanceId , AnimatorName.RedShoes,
                AgentLayer.currentLayer.GetAgent(SefiraManager.instance.getSefira("1").agentList[0].instanceId).puppetAnim, true, false);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) {
            AnimatorManager.instance.ChangeAnimatorByName(SefiraManager.instance.getSefira("1").agentList[0].instanceId, AnimatorName.RedShoes,
               AgentLayer.currentLayer.GetAgent(SefiraManager.instance.getSefira("1").agentList[0].instanceId).puppetAnim, true, true);
        }
         */
	}

    private CreatureState oldState = CreatureState.WAIT;
    void Update()
    {
        

        if (oldState != model.state)
        {
            OnChangeState();
            oldState = model.state;
        }
    }

    void LateUpdate()
    {
        UpdateScale();
    }

    void Start()
    {
        if (model.state == CreatureState.ESCAPE_RETURN)
        {
            spriteRenderer.gameObject.SetActive(false);
            returnSpriteRenderer.gameObject.SetActive(true);
        }
        else
        {
            spriteRenderer.gameObject.SetActive(true);
            returnSpriteRenderer.gameObject.SetActive(false);
        }
    }

    void OnChangeState()
    {
        if (model.state == CreatureState.ESCAPE_RETURN)
        {
            spriteRenderer.gameObject.SetActive(false);
            returnSpriteRenderer.gameObject.SetActive(true);
        }
        else if (model.state != CreatureState.ESCAPE_RETURN && oldState == CreatureState.ESCAPE_RETURN)
        {
            spriteRenderer.gameObject.SetActive(true);
            returnSpriteRenderer.gameObject.SetActive(false);
        }
    }

    public Vector3 GetScaleFactor()
    {
        return scaleFactor;
    }

    public void SetScaleFactor(float x, float y, float z)
    {
        scaleFactor = new Vector3(x, y, z);
    }

    /**
     * 환상체가 삭제되면 격리소도 삭제
     * 
     */
    void OnDestroy()
    {
        Destroy(room.gameObject);
    } 

    public void PlaySound(string soundKey)
    {
        string soundFilename;
        if (model.metaInfo.soundTable.TryGetValue(soundKey, out soundFilename))
        {
            SoundEffectPlayer.PlayOnce(soundFilename, transform.position);
        }
    }

	public void OnClicked()
	{
        room.OnClickedCreatureRoom();
	}

    public void OnClick() {
		Debug.Log ("onClick");
		if (model.state == CreatureState.ESCAPE || model.state == CreatureState.ESCAPE_ATTACK)
		{
			SelectWorkAgentWindow.CreateWindow(model, WorkType.ESACAPE);
            SuppressWindow.CreateWindow(model);
		}
		else
        //if (model.state == CreatureState.WAIT)
        {
            SelectWorkAgentWindow.CreateWindow(model, WorkType.NORMAL);
            //IsolateRoomStatus.CreateWindow(this);
        }

        //Temporary Call Suppress window for Debug. Should Remove This line.
        SuppressWindow.CreateWindow(model);

    }

    public void PointerEnter()
    {
        mousePointEnter = true;
    }

    public void PointerOut()
    {
        mousePointEnter = false;
    }

    
}
