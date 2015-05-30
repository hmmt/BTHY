﻿using UnityEngine;
using System.Collections;

public class StageTypeInfo  {

    private static StageTypeInfo _instance;
    public int goal = 100;
    public static StageTypeInfo instnace
    {
        get
        {
            if (_instance == null)
                _instance = new StageTypeInfo();
            return _instance;
        }
    }

    public int GetStageGoalTime(int day)
    {
        return 120;
    }

    public float GetEnergyNeed(int day)
    {

        return 20 +20*day;
    }
}
