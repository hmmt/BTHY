using UnityEngine;
using System.Collections.Generic;

public class CreatureUnit : MonoBehaviour {

    public CreatureModel model;

    public IsolateRoom room;
    public SpriteRenderer spriteRenderer;

   Vector2 oldScale;

	private void UpdateViewPosition()
	{
		//transform.localPosition = GetCurrentViewPosition();
        transform.localPosition = model.GetCurrentViewPosition();
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
			//SelectWorkAgentWindow.CreateWindow(this);
            SelectWorkAgentWindow.CreateWindow(room);
			//IsolateRoomStatus.CreateWindow(this);
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
