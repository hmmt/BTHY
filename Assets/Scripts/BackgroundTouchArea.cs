using UnityEngine;
using System.Collections;

public class BackgroundTouchArea : MonoBehaviour {

	public void OnClick()
	{
        // notification으로 바꿔야 함
		if (AgentStatusWindow.currentWindow != null)
			AgentStatusWindow.currentWindow.CloseWindow ();

		if (SelectWorkAgentWindow.currentWindow != null)
			SelectWorkAgentWindow.currentWindow.CloseWindow ();

        if(CollectionWindow.currentWindow != null)
            CollectionWindow.currentWindow.CloseWindow();

        if (SelectSefiraAgentWindow.currentWindow != null)
             SelectSefiraAgentWindow.currentWindow.CloseWindow();
	}
}
