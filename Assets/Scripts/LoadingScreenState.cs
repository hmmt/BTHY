using UnityEngine;
using System.Collections;

public class LoadingScreenState : MonoBehaviour {

	
    public void OnFinishLoading()
    {
        GameManager.currentGameManager.OpenStoryScene("start");
    }

    public void StartLoading()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("preloadsprites");

        Debug.Log(textAsset.text);
        string[] spriteNames = textAsset.text.Split('\n');
        OnFinishLoading();
        //StartCoroutine(ResourceCache.instance.LoadSprites(spriteNames, delegate() { OnFinishLoading(); }));
    }
}
