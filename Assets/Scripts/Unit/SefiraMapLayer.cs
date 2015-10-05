﻿using UnityEngine;
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
