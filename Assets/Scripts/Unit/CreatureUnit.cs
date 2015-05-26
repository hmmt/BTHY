using UnityEngine;
using System.Collections.Generic;

public class CreatureUnit : MonoBehaviour {

    public CreatureModel model;

    public IsolateRoom room;
    public SpriteRenderer spriteRenderer;

	private void UpdateViewPosition()
	{
		//transform.localPosition = GetCurrentViewPosition();
        transform.localPosition = model.GetCurrentViewPosition();
	}

	void FixedUpdate()
	{
		UpdateViewPosition();
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
		if(model.state == CreatureState.WAIT)
		{
			//SelectWorkAgentWindow.CreateWindow(this);
            SelectWorkAgentWindow.CreateWindow(room);
			//IsolateRoomStatus.CreateWindow(this);
		}
	}
}
