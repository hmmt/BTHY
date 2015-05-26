using UnityEngine;
using System.Collections;

public class StageTimeInfoUI : MonoBehaviour, IObserver {

    private bool init = false;
    private float goalTime;
    private GameManager gameManager;

    public UnityEngine.UI.Text timerText;
    public UnityEngine.UI.Text dayText;

	void Start ()
    {
	}

    void OnEnable()
    {
        OnUpdateDayUI();
        Notice.instance.Observe(NoticeName.UpdateDay, this);
    }
    void OnDisable()
    {
        Notice.instance.Remove(NoticeName.UpdateDay, this);
    }

    IEnumerator UpdateTimer()
    {
        while (true)
        {
            int remain = (int)(goalTime - Time.time);

            if (remain > 0)
            {
                timerText.text = "" + (int)(goalTime - Time.time);
            }
            else
            {
                timerText.text = "0";

                // 뷰어에서 timeover 하는 건 이상한 듯.
                gameManager.TimeOver();
                yield break;
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    public void StartTimer(float goal, GameManager gameManager)
    {
        init = true;
        goalTime = Time.time + goal;
        this.gameManager = gameManager;
        StartCoroutine(UpdateTimer());
    }

    public void OnUpdateDayUI()
    {
        dayText.text = "DAY : " + PlayerModel.instnace.GetDay();
    }

    public void OnNotice(string name, params object[] param)
    {
        OnUpdateDayUI();
    }
}
