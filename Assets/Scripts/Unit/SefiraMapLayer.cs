using UnityEngine;
using System.Collections.Generic;

public class SefiraMapLayer : MonoBehaviour, IObserver {

    public SefiraObject[] sefiras;

    void Start()
    {
        foreach (SefiraObject sefira in sefiras)
        {
            if (PlayerModel.instnace.openedAreaList.Contains(sefira.sefiraName))
                sefira.gameObject.SetActive(true);
            else
                sefira.gameObject.SetActive(false);
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

    public void OnNotice(string notice, params object[] param)
    {
        if (notice == NoticeName.AreaOpenUpdate)
        {
            SetSefiraActive((string)param[0], true);
        }
    }
}
