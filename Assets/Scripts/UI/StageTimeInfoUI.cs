using UnityEngine;
using System.Collections;

public class StageTimeInfoUI : MonoBehaviour, IObserver {

    private bool init = false;
    private float limitTime;
    private float goalTime;
    private GameManager gameManager;

    public UnityEngine.UI.Text timerText;
    public UnityEngine.UI.Text dayText;

    public GameObject clockMinute;
    public GameObject clockHour;

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

    void Update()
    {
        if (limitTime <= 0)
        {
            return;
        }
        float elapsedTime = limitTime - (goalTime - Time.time);

        float minuteRate = (elapsedTime - ((int)elapsedTime / 10 * 10)) / 10.0f;

        clockMinute.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, -minuteRate * 360));

        float hourRate = ((int)elapsedTime / 10) / (limitTime / 10.0f);
        clockHour.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, -hourRate * 360));
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
        limitTime = goal;
        this.gameManager = gameManager;
        StartCoroutine(UpdateTimer());
    }

    public void OnUpdateDayUI()
    {
        dayText.text = "DAY : " + PlayerModel.instance.GetDay();
    }

    public void OnNotice(string name, params object[] param)
    {
        OnUpdateDayUI();
    }
}
