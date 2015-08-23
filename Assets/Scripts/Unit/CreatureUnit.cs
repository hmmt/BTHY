using UnityEngine;
using System.Collections.Generic;

public class CreatureUnit : MonoBehaviour {
    

    public CreatureModel model;

    public IsolateRoom room;
    public SpriteRenderer spriteRenderer;

    // 아직 안 씀
    public Animator creatureAnimator;
    public CreatureAnimBase script;

    Vector2 oldScale;

    private Vector3 viewPosition;
    private bool visible = true;

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
       MapEdge currentEdge = model.GetCurrentEdge();
       int edgeDirection = model.GetMovableNode().GetEdgeDirection();

       if (currentEdge != null)
       {
           MapNode node1 = currentEdge.node1;
           MapNode node2 = currentEdge.node2;
           Vector2 pos1 = node1.GetPosition();
           Vector2 pos2 = node2.GetPosition();

           if (edgeDirection == 1)
           {
               Transform anim = creatureAnimator.transform;

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
               Transform anim = creatureAnimator.transform;

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

	void FixedUpdate()
	{
		UpdateViewPosition();
	}

    void Update()
    {
        if (script != null)
        {
            script.Update();
        }
    }

    void LateUpdate()
    {
        if (script != null)
        {
            script.LateUpdate();
        }
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
        Debug.Log("크리쳐 상태 "+model.state);
		if(model.state == CreatureState.WAIT)
		{
			SelectWorkAgentWindow.CreateWindow(model, WorkType.NORMAL);
			//IsolateRoomStatus.CreateWindow(this);
		}
        else if (model.state == CreatureState.ESCAPE || model.state == CreatureState.ESCAPE_ATTACK)
        {
            SelectWorkAgentWindow.CreateWindow(model, WorkType.ESACAPE);
        }
	}

    public void PointerEnter()
    {
        oldScale = spriteRenderer.gameObject.transform.localScale;
        spriteRenderer.gameObject.transform.localScale = new Vector2(oldScale.x*1.2f, oldScale.y*1.2f);
    }

    public void PointerOut()
    {
        spriteRenderer.gameObject.transform.localScale = oldScale;
    }
}
