using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StoryTempScript : MonoBehaviour {

    public Text day;
    public Text time;
    public Text energy;
    public AudioSource src;
    private bool nextLoading = false;

    void Awake()
    {
        
        int daynum = PlayerModel.instance.GetDay();

        int timenum = StageTypeInfo.instnace.GetStageGoalTime(daynum);
        float energynum = StageTypeInfo.instnace.GetEnergyNeed(daynum);

        day.text = "근무일 " + daynum;
        time.text = "제한시간 " + timenum;
        energy.text = "모아야 하는 에너지 " + (int)energynum;
    }

    public void OnButtonClick() {
        if (nextLoading) return;
        src.PlayOneShot(src.clip);
        nextLoading = true;
        LoadStartScene();
        
    }

    IEnumerator LoadStartScene() {
        Debug.Log("스토리끝 메인으로 넘어간다");
        
        if (ConversationUnit.instance.selected == false)
        {
            ConversationUnit.instance.selected = true;
            ConversationUnit.instance.InturreptEnd();
        }
        AsyncOperation async = Application.LoadLevelAsync("Main");

        return null;
    }
    
}
