using UnityEngine;
using System.Collections;

public enum StoryState
{
    NONE, // 현재 스토리 씬이 아님
    START_STORY,
    GAMEOVER_STORY
}

public class StoryScene : MonoBehaviour {

    private StoryScene _currentInstance;

    public StoryScene currentInstance
    {
        get
        {
            return _currentInstance;
        }
    }

    private StoryState state;

    public GameObject storyNodeObject;

    void Awake()
    {
        _currentInstance = this;
    }

    public void LoadStory(string storyName)
    {
        storyNodeObject.SetActive(true);
        if (storyName == "start")
        {
            state = StoryState.START_STORY;
        }
        else if (storyName == "gameover")
        {
            state = StoryState.GAMEOVER_STORY;
        }
        else
        {
            storyNodeObject.SetActive(false);
            Debug.Log("invalid story name");
        }
    }

    public void OnClickStartStage()
    {
        state = StoryState.NONE;

        storyNodeObject.SetActive(false);


        GameManager.currentGameManager.StartGame();
    }
}
