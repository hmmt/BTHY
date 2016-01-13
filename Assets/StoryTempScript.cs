using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StoryTempScript : MonoBehaviour {

    public Text day;
    public Text time;
    public Text energy;

    private bool nextLoading = false;

    void Awake()
    {
        int daynum = PlayerModel.instance.GetDay();

        int timenum = StageTypeInfo.instnace.GetStageGoalTime(daynum);
        float energynum = StageTypeInfo.instnace.GetEnergyNeed(daynum);

        day.text = "day: " + daynum;
        time.text = "time: " + timenum;
        energy.text = "energy: " + (int)energynum;
    }

    public void OnButtonClick() {
        if (nextLoading) return;
        nextLoading = true;
        LoadStartScene();
    }

    IEnumerator LoadStartScene() {
        Debug.Log("스토리끝 메인으로 넘어간다");
        AsyncOperation async = Application.LoadLevelAsync("Main");

        return null;
    }
    
}
