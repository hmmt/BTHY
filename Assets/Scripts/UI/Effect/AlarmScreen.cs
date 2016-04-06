using UnityEngine;
using System.Collections;

public class AlarmScreen : MonoBehaviour, IObserver {

    private bool alarmOn;
    private float elapsedTime;

    private float finishTime = 3f;

    public UnityEngine.UI.Image targetImage;

    void OnEnable()
    {
        targetImage.gameObject.SetActive(false);
        Notice.instance.Observe(NoticeName.EscapeCreature, this);
    }

    void OnDisable()
    {
        Notice.instance.Remove(NoticeName.EscapeCreature, this);
    }

    void Alarm()
    {
		if (!alarmOn)
		{
			alarmOn = true;
			elapsedTime = 0;
		}
    }

    void Update()
    {
        if (alarmOn)
        {
            float alpha;

            elapsedTime += Time.deltaTime * 2;

            if (elapsedTime >= finishTime)
            {
                alarmOn = false;
                elapsedTime = 0;
                targetImage.gameObject.SetActive(false);
            }
            else
            {
                targetImage.gameObject.SetActive(true);

                alpha = MathUtil.UnitStep(elapsedTime - 0) * MathUtil.UnitStep(0.5f - elapsedTime) * (elapsedTime) * 2
                    + MathUtil.UnitStep(elapsedTime - 0.5f) * MathUtil.UnitStep(1.0f - elapsedTime) * (1.0f - elapsedTime) * 2 // 1~2
                    + MathUtil.UnitStep(elapsedTime - 1.0f) * MathUtil.UnitStep(1.5f - elapsedTime) * (elapsedTime - 1.0f) * 2 // 1~2
                    + MathUtil.UnitStep(elapsedTime - 1.5f) * MathUtil.UnitStep(2.0f - elapsedTime) * (2.0f - elapsedTime) * 2 // 1~2
                    + MathUtil.UnitStep(elapsedTime - 2.0f) * MathUtil.UnitStep(2.5f - elapsedTime) * (elapsedTime - 2.0f) * 2 // 1~2
                    + MathUtil.UnitStep(elapsedTime - 2.5f) * MathUtil.UnitStep(3.0f - elapsedTime) * (3.0f - elapsedTime) * 2; // 1~2

                // 빠밤

                Color c = targetImage.color;
                c.a = alpha;
                targetImage.color = c;
            }
        }
		else
		{
			Color c = targetImage.color;
			c.a = 0;
			targetImage.color = c;
		}
    }

    public void OnNotice(string notice, params object[] param)
    {
        if (notice == NoticeName.EscapeCreature)
        {
            Alarm();
        }
    }
}
