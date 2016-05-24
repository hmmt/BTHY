using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class TempUI : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Save()
    {
		GlobalGameManager.instance.SaveData();
        //Notice.instance.Send("AddSystemLog", "test");
    }

    public void Load()
    {
		GlobalGameManager.instance.LoadData();
    }

    public void Escape()
    {
        CreatureModel[] list = CreatureManager.instance.GetCreatureList();
        List<CreatureModel> waitCreatures = new List<CreatureModel>();
        foreach (CreatureModel c in list)
        {
            if (c.state == CreatureState.WAIT)
            {
                waitCreatures.Add(c);
            }
        }

        if (waitCreatures.Count > 0)
        {
            waitCreatures[Random.Range(0, waitCreatures.Count)].Escape();
        }
    }
}
