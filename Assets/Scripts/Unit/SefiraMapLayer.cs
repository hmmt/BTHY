using UnityEngine;
using System.Collections.Generic;

public class SefiraMapLayer : MonoBehaviour, IObserver {

    public SefiraObject[] sefiras;

    void Start()
    {
        foreach (SefiraObject sefira in sefiras)
        {
            if (PlayerModel.instance.openedAreaList.Contains(sefira.sefiraName))
                sefira.gameObject.SetActive(true);
            else
                sefira.gameObject.SetActive(false);
        }
    }

    public void sefiraFog(int sefiraNum, float fogDensity)
    {
        Color color = sefiras[sefiraNum].sepfiraFog.GetComponent<SpriteRenderer>().color;
        color.a = fogDensity;
        sefiras[sefiraNum].sepfiraFog.GetComponent<SpriteRenderer>().color = color;

        color = sefiras[sefiraNum].way1Fog.GetComponent<SpriteRenderer>().color;
        color.a = fogDensity;
        sefiras[sefiraNum].way1Fog.GetComponent<SpriteRenderer>().color = color;

        color = sefiras[sefiraNum].way2Fog.GetComponent<SpriteRenderer>().color;
        color.a = fogDensity;
        sefiras[sefiraNum].way2Fog.GetComponent<SpriteRenderer>().color = color;

        color = sefiras[sefiraNum].way3Fog.GetComponent<SpriteRenderer>().color;
        color.a = fogDensity;
        sefiras[sefiraNum].way3Fog.GetComponent<SpriteRenderer>().color = color;

        color = sefiras[sefiraNum].way4Fog.GetComponent<SpriteRenderer>().color;
        color.a = fogDensity;
        sefiras[sefiraNum].way4Fog.GetComponent<SpriteRenderer>().color = color;

        color = sefiras[sefiraNum].elevatorFog1.GetComponent<SpriteRenderer>().color;
        color.a = fogDensity;
        sefiras[sefiraNum].elevatorFog1.GetComponent<SpriteRenderer>().color = color;

        if (sefiras[sefiraNum].elevatorFog2.GetComponent<SpriteRenderer>().sprite != null)
        {
            color = sefiras[sefiraNum].elevatorFog2.GetComponent<SpriteRenderer>().color;
            color.a = fogDensity;
            sefiras[sefiraNum].elevatorFog2.GetComponent<SpriteRenderer>().color = color;
        }

    }

    void Update()
    {
        if (CreatureManager.instance.malkuthState == SefiraState.NOT_ENOUGH_AGENT)
        {
            sefiraFog(0,0.9f);
        }

        else
        {
            sefiraFog(0, 0f);
        }

        if (CreatureManager.instance.nezzachState == SefiraState.NOT_ENOUGH_AGENT)
        {
            sefiraFog(1, 0.9f);
        }

        else
        {
            sefiraFog(1, 0f);
        }

        if (CreatureManager.instance.hodState == SefiraState.NOT_ENOUGH_AGENT)
        {
            sefiraFog(2, 0.9f);
        }

        else
        {
            sefiraFog(2, 0f);
        }

        if (CreatureManager.instance.yessodState == SefiraState.NOT_ENOUGH_AGENT)
        {
            sefiraFog(3, 0.9f);
        }

        else
        {
            sefiraFog(3, 0f);
        }


    }

    void OnEnable()
    {
        Notice.instance.Observe(NoticeName.AreaOpenUpdate, this);
    }

    void OnDisable()
    {
        Notice.instance.Remove(NoticeName.AreaOpenUpdate, this);
    }

    public void SetSefiraActive(string sefiraName, bool b)
    {
        foreach (SefiraObject sefira in sefiras)
        {
            if (sefira.sefiraName == sefiraName)
            {
                sefira.gameObject.SetActive(b);
                break;
            }
        }
    }

    private void UpdateSefiraStateView()
    {
        // 나중에
    }
    private void OnUpdateSefiraState()
    {
    }

    public void OnNotice(string notice, params object[] param)
    {
        if (notice == NoticeName.AreaOpenUpdate)
        {
            SetSefiraActive((string)param[0], true);
        }
        else if (notice == NoticeName.AreaUpdate)
        {
            SetSefiraActive((string)param[0], (bool)param[1]);
        }
    }
}
