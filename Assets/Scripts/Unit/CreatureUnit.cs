﻿using UnityEngine;
using System.Collections.Generic;

public class CreatureUnit : MonoBehaviour {

    public CreatureModel model;

    public IsolateRoom room;
    public SpriteRenderer spriteRenderer;

   Vector2 oldScale;

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
               transform.localPosition = newPosition;
           }
       }
       else
       {
           if (!visible)
           {
               visible = true;
           }
           transform.localPosition = model.GetCurrentViewPosition();
       }
   }

	void FixedUpdate()
	{
		UpdateViewPosition();
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
