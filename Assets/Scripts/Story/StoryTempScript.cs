using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StoryTempScript : MonoBehaviour {
    private static StoryTempScript _instance = null;
    public static StoryTempScript instance {
        get {
            return _instance;
        }
    }
    public Text day;
    public Text time;
    public Text energy;
    public AudioSource src;
    public Button NextButton;
    private bool nextLoading = false;
    private static bool started = false;
    public GameObject manual;
    public GameObject Menu;
    public Button Continue;
    public Button startGame;
    public Button Option;
    public Button Exit;
    public GameObject StoryBoard;
    public GameObject GameMenuBoard;

    public Animator menuAnim;

    void Awake()
    {
        _instance = this;
        GameStaticDataLoader.LoadStaticData();
        NextButton.interactable = false;
        manual.SetActive(false);
    }

    void Init() {
        int daynum = PlayerModel.instance.GetDay();
        int timenum = StageTypeInfo.instnace.GetStageGoalTime(daynum);
        float energynum = StageTypeInfo.instnace.GetEnergyNeed(daynum);
        day.text = "근무일 " + daynum;
        time.text = "제한시간 " + timenum;
        energy.text = "모아야 하는 에너지 " + (int)energynum;
    }

    IEnumerator LoadAnimEnd() {
        yield return new WaitForSeconds(1.2f);
        //menuAnim.enabled = false;
    }

    public void Start() {
        if (!started)
        {
            //menuAnim.SetBool("GameStart", true);
            //DisplayMenu -> 메뉴에서 애니메이터 수정
            //displayed 
            menuAnim.enabled = true;
            manual.SetActive(false);
            Menu.SetActive(true);
        }
        else
        {
            menuAnim.enabled = false;
            //OnStartGame();
            Menu.SetActive(false);
            manual.SetActive(true);
        }

        Init();

    }

    public void Activate() {
        started = true;
        Debug.Log("Activated");
        menuAnim.enabled = false;

        manual.SetActive(true);
        this.NextButton.interactable = true;
        ConversationUnit.instance.StartCassette();
    }

    public void OnButtonClick() {
        if (nextLoading) return;
        src.PlayOneShot(src.clip);
        nextLoading = true;
        LoadStartScene();
    }

    public void OnStartGame() {
        menuAnim.SetBool("Start", true);
    }

    IEnumerator LoadStartScene() {
        Debug.Log("스토리끝 메인으로 넘어간다");
        
        if (ConversationUnit.instance.selected == false)
        {
            ConversationUnit.instance.selected = true;
            ConversationUnit.instance.InturreptEnd();
        }
        //AsyncOperation async = Application.LoadLevelAsync("Main");
        SceneManager.LoadScene("Main");
        
        return null;
    }

    public void OnExitButton() {
        Application.Quit();
    }

}
